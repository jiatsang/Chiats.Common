// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// InsertModel 組成元件類別. 支援標準(共用) Sql Delete 述?語法. 詳見 <see cref="SqlModel"/> 說明
    /// </summary>
    /// <remarks>
    /// SelectModel/InsertModel/UpdateModel/DeleteModel 是以支援標準(共用) Sql Delete 述?語法的物件模型. 並且為抽象類別(Abstract Class))<br/>
    /// 它們基本上不處理各資料庫特有的部份. 例如 SQL Server "TOP" 語法. 同時所有的特有語法上的模型是由各資料庫相依的物件.
    /// 例如 SQL Server 為 SelectModel/SqlInsert/UpdateModel/DeleteModel 類別實作.
    /// INSERT [INTO] table_or_view [(column_list)] VALUES (data_values)
    /// </remarks>
    public class InsertModel : SqlModel, ISqlConditions
    {
        private TableName table;

        /// <summary>
        /// 回傳一個欄位定義和其值 /加入一個新欄位
        /// </summary>
        public ColumnValues Columns { get; private set; }

        /// <summary>
        /// 建構子
        /// </summary>
        public InsertModel()
        {
            Columns = new ColumnValues(this);
            conditions = new ModelConditions(this);
            Columns.ColumnValueChanged += new EventHandler<ChangedEventArgs<ColumnValue>>(Columns_ColumnValueChanged);
        }

        /// <summary>
        /// InsertModel 建構子
        /// </summary>
        /// <param name="CTLSQL">不包含空白字元的名稱視為表格名稱</param>
        /// <param name="parameters">CTLSQL 為表格名稱時 , parameters 則視為要新增的欄位名稱及定義. 否則為參數</param>
        public InsertModel(string CTLSQL, object parameters = null)
            : this()
        {
            // ctlsql 不為單一表格名稱視為 SQL Command, 
            if (CTLSQL.Trim().IndexOf('\x20') != -1)
            {
                SqlModel.Parse<InsertModel>(CTLSQL, this);
                if (parameters != null)
                {
                    Parameters.Fill(parameters);
                    //foreach (var p in parameters.GetType().GetProperties())
                    //{
                    //    var p_name = $"@{p.Name}";
                    //    if (Parameters.Contains(p_name))
                    //        this.Parameters[p_name].Value = p.GetValue(parameters,null);
                    //    else
                    //        throw new CommonException($"未定義的 Parameter Name {p_name} in {nameof(parameters)}");
                    //}
                }
            }
            else
            {
                Table = CTLSQL.Trim();  // 不含空白的 CTLSQL 可視為表格名稱
                this.Columns.AddColumnParameter(parameters);  /* CTLSQL 為表格名稱時 , parameters 則視為要新增的欄位名稱及定義 */
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ModelConditions conditions;

        /// <summary>
        /// 回傳 DeleteModel 條件物件集合
        /// </summary>
        public ModelConditions Conditions
        {
            get { return conditions; }
        }

        IConditionModels ISqlConditions.Conditions
        {
            get { return conditions; }
        }

        /// <summary>
        /// DeleteModel Condition 集合之管理類別
        /// </summary>
        public class ModelConditions : CommonList<IConditionModel>, IConditionModels
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private InsertModel insert;

            /// <summary>
            /// 條件式變更事件
            /// </summary>
            public event EventHandler<ChangedEventArgs<IConditionModel>> ConditionChanged;

            internal ModelConditions(InsertModel insert)
            {
                this.insert = insert;
                // this.insert.Where.WhereConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(WhereConditionChanged);
            }

            /// <summary>
            /// 查詢指定參數所屬的條件式 (Condition) 名稱 , 同時屬於多個條件式以 ',' 區隔
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public string QueyBelongCondition(string name)
            {
                string condition_names = null;
                foreach (var Condition in insert.Conditions)
                {
                    if (Condition.BelongParameter(name))
                    {
                        if (condition_names != null)
                            condition_names += "," + Condition.Name;
                        else
                            condition_names += Condition.Name;
                    }
                }
                return condition_names;
            }

            /// <summary>
            /// 查詢指定參數所屬的條件式 (Condition) 名稱 , 同時屬於多個條件式以 ',' 區隔
            /// </summary>
            /// <param name="Parameter"></param>
            /// <returns></returns>
            public string QueyBelongCondition(Parameter Parameter)
            {
                string condition_names = null;
                foreach (var Condition in insert.Conditions)
                {
                    if (Condition.BelongParameter(Parameter))
                    {
                        if (condition_names != null)
                            condition_names += "," + Condition.Name;
                        else
                            condition_names += Condition.Name;
                    }
                }
                return condition_names;
            }

            public void RaiseWhereConditionChanged(ChangedEventArgs<Condition> e)
            {
                WhereConditionChanged(this, e);
            }

            private void WhereConditionChanged(object sender, ChangedEventArgs<Condition> e)
            {
                insert.Changed();

                if (e.ChangedObject.Name == null) return; // 忽略匿名(Anonymous) Condition
                ModelCondition current = (ModelCondition)this[e.ChangedObject.Name];

                switch (e.ChangedEventType)
                {
                    case ChangedEventType.Add:
                        if (current == null)
                        {
                            current = new ModelCondition(e.ChangedObject.Name);
                            this.BaseAdd(current);
                            current.Where = e.ChangedObject;
                        }
                        else
                        {
                            if (current.Where == e.ChangedObject) break; /* 如果加入 Condition 是相同時則忽略*/
                            if (!current.Where.IsEmpty)
                                throw new SqlModelSyntaxException("重覆加入 Condition - {0}", e.ChangedObject.Name);
                            current.Where = e.ChangedObject;
                        }
                        break;

                    case ChangedEventType.Removed: BaseRemove(current); break;
                    case ChangedEventType.Replace: BaseReplace(current); break;
                }
                if (ConditionChanged != null)
                    ConditionChanged(sender, new ChangedEventArgs<IConditionModel>(e.ChangedEventType, current));
            }

            /// <summary>
            /// 回傳是否包含該物件
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool Contains(string name)
            {
                return this.BaseContains(name);
            }

            /// <summary>
            /// 回傳是否包含該物件
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(ModelCondition item)
            {
                return this.BaseContains(item);
            }

            /// <summary>
            /// 取得指定位置之條件式物件.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public new InsertModel.ModelCondition this[int index]
            {
                get { return (InsertModel.ModelCondition)base[index]; }
            }

            /// <summary>
            /// 取得指定名稱之條件式物件.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public new InsertModel.ModelCondition this[string name]
            {
                get { return (InsertModel.ModelCondition)base[name]; }
            }

            /// <summary>
            /// String，表示目前的 Object。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("Conditions(Count={0})", this.Count);
            }

            public FunctionCondition[] GetFunctionConditions(ConditionFlags Flags)
            {
                List<FunctionCondition> list = new List<FunctionCondition>();
                // TODO:
                return list.ToArray();
            }
        }

        /// <summary>
        ///  DeleteModel Condition 管理類別
        /// </summary>
        public class ModelCondition : IConditionModel
        {
            /// <summary>
            /// DeleteModel Condition 管理類別建構子
            /// </summary>
            /// <param name="name"></param>
            public ModelCondition(string name) { this.name = name; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string name = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Condition Condition = null;

            /// <summary>
            /// 指定參數是否所屬於的目前的條件式 (Condition)
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool BelongParameter(string name)
            {
                return Condition.BelongParameter(name);
            }

            /// <summary>
            /// 指定參數是否所屬於的目前的條件式 (Condition)
            /// </summary>
            /// <param name="Parameter"></param>
            /// <returns></returns>
            public bool BelongParameter(Parameter Parameter)
            {
                return Condition.BelongParameter(Parameter);
            }

            /// <summary>
            /// 是否啟用條件物件 , True:啟用 Flase:不使用 Null:自動
            /// </summary>
            public bool Enabled
            {
                get
                {
                    return Condition.Enabled;
                }
                set
                {
                    Condition.Enabled = value;
                }
            }

            #region INamed Members

            /// <summary>
            /// Condition 物件名稱
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            /// <summary>
            /// Where Condition 物件內容
            /// </summary>
            public Condition Where
            {
                get { return Condition; }
                internal set
                {
                    Condition = value;
                }
            }

            #endregion INamed Members

            /// <summary>
            /// String，表示目前的 Object。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Condition == null)
                    return "Condition : {EMPTY}";
                else
                    return string.Format("Condition : {0}", Name);
            }
        }

        /// <summary>
        /// 清除所有欄位內容
        /// </summary>
        public void ColumnValueClearAll()
        {
            Changed();
            foreach (var col in Columns)
            {
                col.Value = null;
            }
        }

        /// <summary>
        /// 表格名稱
        /// </summary>
        public TableName Table
        {
            get { return table; }
            set
            {
                this.Changed();
                table = value;
            }
        }

        private SelectModel select = null;

        /// <summary>
        /// 增加支援 Insert into .... select * from ....
        /// </summary>
        public SelectModel Select
        {
            get { return select; }
            set
            {
                select = value.Clone(this, new EventHandler<ChangedEventArgs<Condition>>(Where_WhereConditionChanged));
            }
        }

        private void Where_WhereConditionChanged(object sender, ChangedEventArgs<Condition> e)
        {
            switch (e.ChangedEventType)
            {
                case ChangedEventType.Add:
                    this.CoverParameters.AddLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Replace:
                    this.CoverParameters.RemoveLinker(e.ReplaceObject.Parameters);
                    this.CoverParameters.AddLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Removed:
                    this.CoverParameters.RemoveLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Changed:
                    break;
            }
            this.Changed();
        }

        private void Columns_ColumnValueChanged(object sender, ChangedEventArgs<ColumnValue> e)
        {
            this.Changed();
        }

        /// <summary>
        /// 依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="BuildType">指示建立 SQL 命令的文字敘述的方式</param>
        /// <param name="formatFlags"></param>
        /// <returns>SQL 命令的文字敘述</returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions formatFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            ISqlBuildExport BuildExport = GetBuildExport(BuildType, formatFlags, buildExport);

            CommandBuilder sb = new CommandBuilder(formatFlags);
            sb.AppendKeywordToken("INSERT INTO");
            sb.AppendToken(table.FullName);
            sb.Append(' ');
            sb.Append('(');
            int col_count = 0;

            foreach (ColumnValue cv in Columns)
            {
                if (col_count++ != 0)
                    sb.Append(',');
                sb.AppendToken(cv.Name);
            }
            if (this.Select == null)
            {
                sb.AppendToken(")");
                if (formatFlags.HasFlag(CommandFormatOptions.AutoFormat)) sb.Append("\r\n");
                sb.AppendKeywordToken("VALUES");
                if (formatFlags.HasFlag(CommandFormatOptions.AutoFormat)) sb.Append("\r\n");
                sb.AppendToken("(");
                col_count = 0;
                foreach (ColumnValue cv in Columns)
                {
                    if (col_count++ != 0)
                        sb.Append(',');

                    BuildExport.ExportForInsertValues(cv, sb, this);
                }
                sb.Append(')');
            }
            else
            {
                sb.Append(") ");
                if (formatFlags.HasFlag(CommandFormatOptions.AutoFormat)) sb.Append("\r\n");

                sb.Append(Select.BuildCommand(BuildType, formatFlags));
            }
            return sb.ToString();
        }
    }
}