// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chiats.SQL
{

    /// <summary>
    /// SqlModel 的基礎元件 CommonModel 元件類別. 原則上是以支援標準 SQL 語法的基礎類別.<br/>
    /// SelectSqlModel 的設計是以標準 SQL 語法,為主要目的, 但限於現實的考量. 僅以
    /// SQL Server 2005/2008 的環境進行測試和運作. 支援的語法部份則以較常用的語法為限制.
    /// 但會考量未來的擴張性及不同資料庫產品的可能性, 但無法保證一定可以相容.
    /// </summary>
    /// <remarks>
    /// SQLModlel 支援以程式語言的處理方法, 來產生合法的 SQL 語法 , 如 Select/Insert/Update/Delete<br/>
    /// 如下以範例所示.
    /// <code>
    /// SelectSqlModel obj_model = new SelectSqlModel();
    ///
    /// obj_model.Column.Add("*");
    /// obj_model.Table.PrimaryTableName = "MyTable";
    /// Assert.AreEqual(obj_model.CommandText, "SELECT * FROM MyTable");
    /// </code>
    /// SqlModel 基礎上是將程式中以字串的組成 SQL 語法方式轉換成另一種方法.
    /// <code>
    /// sql = string.Format("Select * From {0}",MyTableName);
    /// </code>
    /// 而此種方式能提供比字串的組成更多的優點. <br/>
    /// 1. 程式的可讀性, 並且不容易對 SQL 語法出錯. 例如打錯關鍵字. <br/>
    /// 2. SQL 命令的可驗證性. 例如我們可以輕易的找出 'SELECT * FROM MyTable' 中的表格名稱, 別名等等<br/>
    /// 有了表格名稱, 那我們也就可以取得資料庫該表格的所有欄位資訊, 甚至索引資訊等等.那也就表示我們其實可以作到
    /// SQL 語法的驗證.
    /// <code>
    /// TableName = obj_model.Table.PrimaryTableName;
    /// AlisaTableName = obj_model.Table.AlisaTableName;
    /// </code>
    /// 目前計劃中支援的有 Select/Insert/Update/Delete 及 StoreProcedure 的呼叫介面 中的常用語法.<br/>
    /// 明確不支援項目 Select Into 語法. <br/>
    /// StoreProcedure 的呼叫介面也會有其限制. 其中對 StoreProcedure 傳入參數是比較沒有問題.
    /// 但對輸出的結果. 如果在無法驗證下, 則可能會造成一些系統的不穩定的來源.
    /// </remarks>
    public abstract class SqlModel :
        ISqlBuilder,
        IPartSqlModel,
        ICommandInitialize,
        ICommandBuilder,
        IParameterCommandBuilder,
        IBuildExportSupport,
        IDisposable
    {

        internal SqlModel Parent = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _init = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _changed = 0;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ParameterMode parameterMode = ParameterMode.Parameter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ISqlBuildExport currentBuildExport = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ISqlBuildExport defaultExport = DefaultBuildExport.SQLBuildExport;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ISqlBuildExport defaultCTLExport = DefaultBuildExport.SQLCTLBuildExport;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SqlOptions options;

        /// <summary>
        /// 指示建立 SQL 命令的文字敘述的輸出格式方式,
        /// </summary>
        public CommandBuildType BuildType { get; private set; } = CommandBuildType.SQL;

        /// <summary>
        ///  強制轉換成小寫名稱. 欄位名稱/表格名稱
        /// </summary>
        public SqlOptions Options
        {
            get
            {
                var TopModel = GetTopModel();
                if (TopModel == this)
                    return options;
                return TopModel.Options;
            }

            set
            {
                var TopModel = GetTopModel();
                if (TopModel == this)
                    options = value;
                else
                    TopModel.Options = value;
            }
        }

        /// <summary>
        /// 取得目前的 BuildExport 物件.
        /// </summary>
        protected ISqlBuildExport GetBuildExport(CommandBuildType buildType, CommandFormatOptions formatFlags, ISqlBuildExport buildExport)
        {
            ISqlBuildExport BuildExport = null;
            if (buildType == CommandBuildType.SQLCTL)
                BuildExport = defaultCTLExport;
            else if (buildExport != null)
                // 指定 BuildExport 物件是由 IBuildExportSupport.BuildBegin 到 BuildEnd 範圍間.
                BuildExport = currentBuildExport = buildExport;
            else if (currentBuildExport != null)
                BuildExport = currentBuildExport;
            else
                BuildExport = defaultExport; // 未指定則共用同一個 BuildExport 物件.

            BuildExport.FormatOptions = formatFlags;
            BuildExport.Options = Options;
            return BuildExport;
        }


        /// <summary>
        /// 解析 SQL 字串命令共用介面. 依字串命令產生相對應的 SelectModel/UpdateModel/InsertModel/DeleteModel 物件
        /// </summary>
        /// <typeparam name="T">CommonModel 如 SelectModel/UpdateModel/InsertModel/DeleteModel </typeparam>
        /// <param name="CTLSQL">SQL 字串命令</param>
        /// <returns></returns>
        public static T Parse<T>(string CTLSQL, object parameters = null) where T : SqlModel
        {

            if (typeof(T) == typeof(CommandModel))
            {
                SqlModel rs = new CommandModel(CTLSQL, parameters);
                return (T)rs;
            }

            T result = null;
            SQLTokenScanner list = new SQLTokenScanner(CTLSQL);
            ISqlParserObject ParserObject = null;

            if (list.Count > 0)
            {
                string first_token = list[0].String;
                if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "select"))
                    ParserObject = SelectParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "update"))
                    ParserObject = UpdateParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "delete"))
                    ParserObject = DeleteParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "insert"))
                    ParserObject = InsertParser.Default;

                if (ParserObject != null &&
                    (ParserObject.ModelType == typeof(T) ||
                    ParserObject.ModelType.IsSubclassOf(typeof(T))))
                {
                    result = (T)ParserObject.PaserCommand(list, null, parameters);
                }
            }
            return result; // 無法解析或解析錯誤時回傳 NULL
        }

        /// <summary>
        /// 解析 SQL 字串命令共用介面. 依字串命令產生相對應的 SelectModel/UpdateModel/InsertModel/DeleteModel 物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CTLSQL"></param>
        /// <param name="SqlModel"></param>
        /// <returns></returns>
        internal static T Parse<T>(string CTLSQL, T SqlModel, object parameters = null) where T : SqlModel
        {
            SQLTokenScanner list = new SQLTokenScanner(CTLSQL);
            ISqlParserObject ParserObject = null;
            T result = null;

            if (list.Count > 0)
            {
                string first_token = list[0].String;
                string second_token = (list.Count > 1) ? list[1].String : null;

                // 以字串字首決定使用那一個 ParserObject
                if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "select"))
                    ParserObject = SelectParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "update"))
                    ParserObject = UpdateParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "delete"))
                    ParserObject = DeleteParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "insert"))
                    ParserObject = InsertParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "create") && StringComparer.OrdinalIgnoreCase.Equals(second_token, "table"))
                    ParserObject = CreateTableParser.Default;

                if (ParserObject != null &&
                    (ParserObject.ModelType == typeof(T) || ParserObject.ModelType.IsSubclassOf(typeof(T)))
                )
                {
                    result = (T)ParserObject.PaserCommand(list, SqlModel, parameters);
                }
            }
            return result; // 解析無法解析或解析時回傳 NULL
        }

        /// <summary>
        /// 解析 SQL 字串命令共用介面. 依字串命令產生相對應的 SelectModel/UpdateModel/InsertModel/DeleteModel 物件
        /// </summary>
        /// <param name="commandText">SQL 字串命令</param>
        /// <returns></returns>
        public static SqlModel Parse(string commandText, object parameters = null)
        {
            return Parse<SqlModel>(commandText, parameters);
        }

        /// <summary>
        ///  SqlModel 基本建構子
        /// </summary>
        public SqlModel()
        {
            OnConstructorInitiailize();
            OnConstructorFinish();
        }

        /// <summary>
        ///  SqlModel  建構前的初始化事件. 初始化必要的物件欄位初值.
        /// </summary>
        protected internal virtual void OnConstructorInitiailize()
        { }

        /// <summary>
        /// SqlModel  建構後前的事件. 作為物件建立後需要處理的工作.
        /// </summary>
        protected internal virtual void OnConstructorFinish()
        { }

        /// <summary>
        ///  ISupportInitialize.BeginInit
        /// </summary>
        protected void BeginInit()
        {
            lock (this) { _init++; }
        }

        void ICommandInitialize.BeginInit()
        {
            BeginInit();
        }

        /// <summary>
        ///  ISupportInitialize.EndInit
        /// </summary>
        protected void EndInit()
        {
            lock (this) { _init--; }
        }

        void ICommandInitialize.EndInit()
        {
            EndInit();
        }

        /// <summary>
        /// 依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="BuildType">指示建立 SQL 命令的文字敘述的方式</param>
        /// <param name="buildFlags"></param>
        /// <returns>SQL 命令的文字敘述</returns>
        protected abstract string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions buildFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null);

        /// <summary>
        /// 初始化完成會執行.
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// SQL Command 資料內容變更的事件通知
        /// </summary>
        public event EventHandler CommandChanged;

        /// <summary>
        ///
        /// </summary>
        protected virtual void OnCommandChanged() { }

        /// <summary>
        /// 通知物件內容己變更 , SqlModel
        /// </summary>
        protected internal void Changed()
        {
            lock (this)
            {
                _changed++; // 通知 COMMAND 內容有異動或產生SQL COMMAND 條件變更
                OnCommandChanged();
                if (CommandChanged != null) CommandChanged(this, EventArgs.Empty);
            }
        }
        protected internal int ChangeCount()
        {
            if (this.Parent != null)
                return _changed + Parent.ChangeCount();  // 變更同時包含 Parent SqlModel 
            return _changed;
        }
        /// <summary>
        /// 傳回最上一層的 CommonModel 父階物件
        /// </summary>
        /// <returns>最上一層的 CommonModel 父階物件</returns>
        public SqlModel GetTopModel()
        {
            if (Parent == null)
                return this;
            return Parent.GetTopModel();
        }

        /// <summary>
        /// 指示處理 Parameter 參數的方法.  此一參數為共享 TopModel 的值.
        /// </summary>
        public ParameterMode ParameterMode
        {
            get
            {
                if (GetTopModel() == this)
                    return parameterMode;
                return GetTopModel().ParameterMode;
            }

            set
            {
                if (GetTopModel() == this)
                    parameterMode = value;
                else
                    GetTopModel().ParameterMode = value;
                this.Changed();
            }
        }

        #region ISQLCommandBuilder 成員

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string command_text = null;

        /// <summary>
        /// 重新建立 SQL 命令的文字敘述.
        /// </summary>
        public string CommandText
        {
            get
            {
                lock (this)
                {
                    // _change <> 0 表示 COMMAND 內容有異動或產生SQL COMMAND 條件變更
                    if (command_text == null || ChangeCount() != 0)
                        command_text = RebuildCommand(BuildType);
                    _changed = 0;
                    return command_text;
                }
            }
        }

        /// <summary>
        /// 依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="buildType">指示建立 SQL 命令的文字敘述的方式</param>
        /// <returns>SQL 命令的文字敘述</returns>
        public string BuildCommand(CommandBuildType buildType)
        {
            return RebuildCommand(buildType, CommandFormatOptions.None);
        }

        public string BuildCommand(ISqlBuildExport buildExport, CommandFormatOptions buildFlags = CommandFormatOptions.None)
        {
            return RebuildCommand(buildExport.BuildType, buildFlags, buildExport);
        }

        /// <summary>
        ///  BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="buildFlags"></param>
        /// <returns></returns>
        public string BuildCommand(CommandBuildType buildType, CommandFormatOptions buildFlags = CommandFormatOptions.None)
        {
            return RebuildCommand(buildType, buildFlags);
        }

        #endregion ISQLCommandBuilder 成員

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RebuildCommand(CommandBuildType.SQLCTL);
        }

        /// <summary>
        /// 取得所有相關連的 Parameter 物件. 運作於 CoverLinker 將不同集合中但名稱相同的物連結起來.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal CoverLinker<Parameter> CoverParameters = new CoverLinker<Parameter>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommonModelParameters list = null;

        /// <summary>
        /// 傳回內含的所有參數物件.
        /// </summary>
        public CommonModelParameters Parameters
        {
            get
            {
                if (list == null)
                {
                    list = new CommonModelParameters(CoverParameters);
                    list.Changed += Parameters_Changed;
                }

                return list;
            }
        }
        /// <summary>
        /// 設定指定參數, 如參數不存在, 則回傳 false
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool SetParameterValue(string Name, object Value)
        {

            foreach(var cp in CoverParameters)
            {
                Debug.Print($"CoverParameter [{Name}] {cp.Name}/{cp.Object.Name}");
                //if(cp.Name == Name)
                //{

                //}
            }
            var __params = CoverParameters.FirstOrDefault(f => f.Object.Name == Name);
            if (__params != null)
            {
                __params.Object.Value = Value;
                return true;
            }
            return false;
        }
        public bool ParameterExist(string Name)
        {
            return CoverParameters.Count(f => f.Name == Name) != 0;
        }

        private void Parameters_Changed(object sender, EventArgs e)
        {
            this.Changed();
        }

        #region ICommandBuilder Members

        string ICommandBuilder.CommandText
        {
            get { return CommandText; }
        }

        /// <summary>
        ///
        /// </summary>
        public System.Data.CommandType CommandType
        {
            get { return System.Data.CommandType.Text; }
        }

        #endregion ICommandBuilder Members

        #region IParameterCommandBuilder Members

        /// <summary>
        /// 取得參數模式是否啟用
        /// </summary>
        public bool ParameterEnabled
        {
            get { return ParameterMode == ParameterMode.Parameter; }
        }

        Parameter[] IParameterCommandBuilder.Parameters
        {
            get
            {
                List<Parameter> list = new List<Parameter>();
                foreach (Parameter CoverObject in Parameters)
                {
                    if (CoverObject != null)
                    {
                        list.Add(CoverObject);
                    }
                }
                return list.ToArray();
            }
        }

        #endregion IParameterCommandBuilder Members

        #region IBuildExportSupport Members

        void IBuildExportSupport.BeginBuild(ISqlBuildExport buildExport, BuildExportSupportEventArgs e)
        {
            if (this.currentBuildExport != buildExport)
            {
                this.currentBuildExport = buildExport;
                this.currentBuildExport.DbInformation = e.DbInformation;
                this.currentBuildExport.BuildOptions = e.BuildOptions;
                _changed++; // 通知 COMMAND 內容有異動或產生SQL COMMAND 條件變更
            }
        }

        void IBuildExportSupport.EndBuild()
        {
            if (this.currentBuildExport != null)
            {
                this.currentBuildExport.DbInformation = null;
                this.currentBuildExport.BuildOptions = CommandBuildOptions.None;
                this.currentBuildExport = null;
                _changed++;// 通知 COMMAND 內容有異動或產生SQL COMMAND 條件變更
            }
        }

        #endregion IBuildExportSupport Members

        /// <summary>
        ///
        /// </summary>
        public virtual void Dispose() { }
    }
}