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
    /// SqlModel 組成元件類別. 原則上是以支援標準 SQL Select 語法的基礎類別.<br/>
    /// SelectSqlModel 的設計是以標準 SQL Select 語法,為主要目的, 但限於現實的考量. 僅以
    /// SQL Server 2005/2008 的環境進行測試和運作. 支援的語法部份則以較常用的語法為限制.
    /// 但會考量未來的擴張性及不同資料庫產品的可能性, 但無法保證一定可以相容.
    /// </summary>
    /// <remarks>
    /// 以下的範例碼包含了對SQL參數, 和 SqlModel 的條件式的用法. SqlModel 的條件式可同時用於 Where 和 Having 子句
    /// <code>
    /// SelectSqlModel obj_model = new SelectSqlModel();
    /// obj_model.Table.PrimaryTableName = new TableName("MyTable");
    /// obj_model.Where.Add(new Condition("C1", "KeyA=@KeyA"));
    /// obj_model.Where.Add(new Condition("C2", "KeyB=@KeyC or KeyD&lt;&gt;'2005/01/01'"));
    /// string sql = obj_model.CommandText;
    /// Debug.Print(sql);
    ///
    /// Assert.AreEqual(sql, "SELECT * FROM MyTable WHERE KeyA=@KeyA AND (KeyB=@KeyC or KeyD&lt;&gt;'2005/01/01')");
    /// Assert.AreEqual(obj_model.Where.Conditions.Count,2);
    /// Assert.IsNotNull(obj_model.Where.Conditions[0]);
    /// Assert.AreSame(obj_model.Where.Conditions[0], obj_model.Where.Conditions["C1"]);
    ///
    /// obj_model.Where.Conditions[0].Enabled = false; // Maybe change to 'obj_model.Conditions["C1"].Enabled = false;'
    /// sql = obj_model.CommandText;
    /// Assert.AreEqual(sql, "SELECT * FROM MyTable WHERE KeyB=@KeyC or KeyD&lt;&gt;'2005/01/01'");
    /// </code>
    /// </remarks>
    public class SelectModel : SqlModel, ISqlConditions
    {
        /// <summary>
        /// SelectModel 建構子
        /// </summary>
        public SelectModel() { }

        /// <summary>
        /// SqlModel  建構前的初始化事件. 初始化必要的物件欄位初值
        /// </summary>
        protected internal override void OnConstructorInitiailize()
        {
            Where = new WhereClause(this);
            Having = new HavingClause(this);
            Columns = new ColumnsClause(this);
            Tables = new TableClause(this);
            OrderBy = new OrderByClause(this);
            GroupBy = new GroupByClause(this);

            Where.WhereConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(Condition_Changed);
            Having.WhereConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(Condition_Changed);
        }

        /// <summary>
        /// 回傳複製一份內容完全相同的 SelectModel 物件.
        /// </summary>
        /// <returns></returns>
        public SelectModel Clone(SqlModel Parent = null, EventHandler<ChangedEventArgs<Condition>> WhereConditionChanged = null)
        {
            SelectModel select = new SelectModel(Parent);

            if (WhereConditionChanged != null)
            {
                select.Where.WhereConditionChanged += WhereConditionChanged;
            }

            select.Top = Top;
            select.Columns.CopyFrom(Columns);

            select.Tables.PrimaryTable = Tables.PrimaryTable;
            select.Tables.PrimaryAliasName = Tables.PrimaryAliasName;

            foreach (var table in Tables) select.Tables.Add(table);
            foreach (var condition in Where) select.Where.Add(condition.ConditionSource);
            foreach (var condition in Having) select.Having.Add(condition.ConditionSource);

            foreach (GroupByColumn GroupColumn in GroupBy) select.GroupBy.Add(GroupColumn.Name);
            foreach (OrderByColumn Column in OrderBy) select.OrderBy.Add(Column.ColumnName, Column.OrderSorting);
            // TODO: Union 


            foreach (var Parm in Parameters) select.Parameters[Parm.Name].Value = Parm.Value;
            return select;
        }

        /// <summary>
        /// SqlModel  建構後前的事件. 作為物件建立後需要處理的工作.
        /// </summary>
        protected internal override void OnConstructorFinish()
        {
            if (Parent != null)
            {
                CoverParameters = Parent.CoverParameters;           // 和父階物件 union 共用 CoverParameters 物件.
                if (Parent is ISqlConditions)
                {
                    conditions = ((ISqlConditions)Parent).Conditions;  // 和父階物件 union 共用 Conditions 物件.
                }
            }
            else
            {
                conditions = new SelectModel.ModelConditions(this);
            }
        }

        /// <summary>
        /// SelectModel 建構子
        /// </summary>
        internal SelectModel(SqlModel Parent) { this.Parent = Parent; }
        /// <summary>
        /// SelectModel 建構子, 由 SQL statement 或 含有 CTLSQL statement 建立 物件. 但語法錯誤或非 Select SQL 則會丟出例外
        /// </summary>
        /// <param name="ctlsql"></param>
        /// <param name="parameters"></param>
        public SelectModel(string ctlsql, object parameters = null) : this((SelectModel)null)
        {
            // ctlsql 不為單一表格名稱視為 SQL Command, 
            if (ctlsql.IndexOf('\x20') != -1)
            {
                SqlModel.Parse<SelectModel>(ctlsql, this);
                if (parameters != null)
                {
                    Parameters.Fill(parameters);
                    //foreach (var p in parameters.GetType().GetProperties())
                    //{
                    //    var p_name = $"@{p.Name}";
                    //    if (Parameters.Contains(p_name))
                    //        this.Parameters[p_name].Value = p.GetValue(parameters, null);
                    //    else
                    //        throw new CommonException($"未定義的 Parameter Name {p_name}");
                    //}
                }
            }
            else
                Tables.PrimaryTable = ctlsql;    // 不含空白的 CTLSQL 可視為表格名稱
        }

        private void Condition_Changed(object sender, ChangedEventArgs<Condition> e)
        {
            switch (e.ChangedEventType)
            {
                case ChangedEventType.Add:
                    this.CoverParameters.AddLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Removed:
                    this.CoverParameters.RemoveLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Replace:
                    this.CoverParameters.RemoveLinker(e.ReplaceObject.Parameters);
                    this.CoverParameters.AddLinker(e.ChangedObject.Parameters);
                    break;
            }

            if (this.Parent != null && Parent is SelectModel)  // 如為 union Select 時通知最上層的 ConditionChanged
                ((SelectModel)Parent).RaiseConditionChanged(sender, e);
            else if (ConditionChanged != null) ConditionChanged(sender, e);

            this.Changed();
        }
        /// <summary>
        /// 只有最上層的 ConditionChanged 會被觸發 , union Select 子階則不會被觸發
        /// </summary>
        internal event EventHandler<ChangedEventArgs<Condition>> ConditionChanged;

        /// <summary>
        /// 發出條件變更通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void RaiseConditionChanged(object sender, ChangedEventArgs<Condition> e)
        {
            // 觸發最上層的 ConditionChanged 事件.
            if (this.Parent != null && Parent is SelectModel)  // 表示為 unionSelect
            {
                ((SelectModel)Parent).RaiseConditionChanged(sender, e);
            }
            else if (ConditionChanged != null)
                ConditionChanged(sender, e);
        }

        /// <summary>
        /// 通知父階物件. 內容己變更
        /// </summary>
        protected override void OnCommandChanged()
        {
            if (Parent != null) Parent.Changed();  // 通知父階物件. 內容己變更
        }
        /// <summary>
        /// 條件(Condition)管理物件集合的管理類別.
        /// </summary>
        public class ModelConditions : CommonList<IConditionModel>, IConditionModels
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private SelectModel select;

            //記錄所有未命名的條件式.
            private List<Condition> AnonymousConditions = new List<Condition>();

            /// <summary>
            /// 取得功能型的條件, 預設不含匿名者
            /// </summary>
            /// <param name="Flags">是否包含匿名條件</param>
            /// <returns></returns>
            public FunctionCondition[] GetFunctionConditions(ConditionFlags Flags)
            {
                List<FunctionCondition> list = new List<FunctionCondition>();
                // SqlModel 的條件式是允許使用多個條件式共用同一名稱.
                foreach (ModelCondition CC in this)
                {
                    if (CC.Enabled) // 忽略未啟用的修件式.
                    {
                        if (Flags.HasFlag(ConditionFlags.Named))
                        {
                            foreach (var Condition in CC.Conditions)
                            {
                                if (Condition is FunctionCondition)
                                {
                                    list.Add((FunctionCondition)Condition);
                                }
                            }
                        }
                    }
                }
                if (Flags.HasFlag(ConditionFlags.Anonymous))
                {
                    foreach (var Condition in AnonymousConditions)
                    {
                        if (Condition is FunctionCondition && Condition.Enabled)
                        {
                            list.Add((FunctionCondition)Condition);
                        }
                    }
                }
                return list.ToArray();
            }

            /// <summary>
            /// 條件式變更事件
            /// </summary>
            public event EventHandler<ChangedEventArgs<IConditionModel>> ConditionChanged;

            internal ModelConditions(SelectModel select)
            {
                this.select = select;
                // ModelConditions 只攔下最上層的 ConditionChanged 事件.
                // 其包含之 UnionSlelect 需要由引發最上層的 ConditionChanged 事件
                select.ConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(ConditionChangedEx);
            }

            private void ConditionChangedEx(object sender, ChangedEventArgs<Condition> e)
            {
                select.Changed();

                if (e.ChangedObject.Name == null)
                {
                    AnonymousConditions.Add(e.ChangedObject);
                    return; // 忽略匿名(Anonymous) Condition
                }

                ModelCondition current = (ModelCondition)this[e.ChangedObject.Name];
                switch (e.ChangedEventType)
                {
                    case ChangedEventType.Add:
                        if (current == null)
                        {
                            current = new ModelCondition(e.ChangedObject.Name);
                            this.BaseAdd(current);
                            current.Conditions.Add(e.ChangedObject);

                            if (ConditionChanged != null)
                                ConditionChanged(sender, new ChangedEventArgs<IConditionModel>(ChangedEventType.Add, current));
                        }
                        else
                        {
                            if (current.Conditions.Contains(e.ChangedObject)) break; /* 如果加入 Condition 是相同時則忽略*/
                            current.Conditions.Add(e.ChangedObject);
                            if (ConditionChanged != null)
                                ConditionChanged(sender, new ChangedEventArgs<IConditionModel>(ChangedEventType.Add, current));
                        }
                        break;

                    case ChangedEventType.Removed:
                        if (current != null)
                        {
                            if (current.Conditions.Contains(e.ChangedObject))
                            {
                                current.Conditions.Remove(e.ChangedObject);
                                if (ConditionChanged != null)
                                    ConditionChanged(sender, new ChangedEventArgs<IConditionModel>(ChangedEventType.Removed, current));
                            }
                        }
                        break;

                    case ChangedEventType.Replace:

                        BaseReplace(current);
                        if (ConditionChanged != null)
                            ConditionChanged(sender, new ChangedEventArgs<IConditionModel>(ChangedEventType.Replace, current));

                        break;

                    case ChangedEventType.Changed:
                        if (ConditionChanged != null)
                            ConditionChanged(sender, new ChangedEventArgs<IConditionModel>(ChangedEventType.Changed, current));

                        break;
                }
            }

            /// <summary>
            /// 查詢指定參數所屬的條件式 (Condition) 名稱 , 同時屬於多個條件式以 ',' 區隔
            /// </summary>
            /// <param name="name">參數名稱</param>
            /// <returns>條件名稱字串集合, 有二個或以上符合時會以 ',' 區隔</returns>
            public string QueyBelongCondition(string name)
            {
                string condition_names = null;
                foreach (var Condition in select.Conditions)
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
            /// <param name="Parameter">參數</param>
            /// <returns>條件名稱字串集合, 有二個或以上符合時會以 ',' 區隔</returns>
            public string QueyBelongCondition(Parameter Parameter)
            {
                string condition_names = null;
                foreach (var Condition in select.Conditions)
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

            /// <summary>
            /// 取得指定位置之條件式物件.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public new SelectModel.ModelCondition this[int index]
            {
                get { return (SelectModel.ModelCondition)base[index]; }
            }

            /// <summary>
            /// 取得指定名稱之條件式物件.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public new SelectModel.ModelCondition this[string name]
            {
                get
                {
                    return (SelectModel.ModelCondition)base[name];
                }
            }

            /// <summary>
            /// 回傳是否包含該物件
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(SelectModel.ModelCondition item)
            {
                return this.BaseContains(item);
            }

            /// <summary>
            /// String，表示目前的 Object。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("SQLConditions (Count={0})", this.Count);
            }
        }

        /// <summary>
        /// Select Model Condition 管理類別 , 它整合 Where and Having 的 Condition 類別.
        /// </summary>
        public class ModelCondition : IConditionModel
        {
            /// <summary>
            /// SelectModel Condition 管理類別建構子
            /// </summary>
            /// <param name="name"></param>
            public ModelCondition(string name) { this.name = name; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string name = null;

            /// <summary>
            /// Conditions 包含Select 的 where and having , 及 union 內的 where and having
            /// </summary>
            internal List<Condition> Conditions = new List<Condition>();

            /// <summary>
            /// 指定參數是否所屬於的目前的條件式 (Condition)
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool BelongParameter(string name)
            {
                foreach (var Condition in Conditions)
                {
                    if (Condition.BelongParameter(name)) return true;
                }
                return false;
            }

            /// <summary>
            /// 指定參數是否所屬於的目前的條件式 (Condition)
            /// </summary>
            /// <param name="Parameter"></param>
            /// <returns></returns>
            public bool BelongParameter(Parameter Parameter)
            {
                foreach (var Condition in Conditions)
                {
                    if (Condition.BelongParameter(Parameter)) return true;
                }
                return false;
            }

            /// <summary>
            ///  回傳條件物件是否啟用, 原則上只有在最後輸出時( Export 方法) 會依据是否啟用條件來決定輸出與否
            /// </summary>
            public bool Enabled
            {
                get
                {
                    foreach (var Condition in Conditions)
                    {
                        if (!Condition.Enabled)
                            return false;
                    }
                    return true;
                }
                set
                {
                    foreach (var Condition in Conditions)
                    {
                        Condition.Enabled = value;
                    }
                }
            }

            /// <summary>
            /// 條件(Condition)名稱
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            /// <summary>
            /// String，表示目前的 Object。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Conditions.Count == 0)
                    return "Condition : {EMPTY}";
                else
                    return string.Format("Condition : {0}", Name, Conditions[0].Name);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IConditionModels conditions = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SelectModel unionSelect = null;

        /// <summary>
        /// 支援標準 Select 語法中的欄位.
        /// </summary>
        public ColumnsClause Columns { get; private set; }

        /// <summary>
        /// 支援標準 Select 語法 [ FROM { table_source } [ ,...n ] ]
        /// </summary>
        public TableClause Tables { get; private set; }       /*[ FROM { < table_source > } [ ,...n ] ]*/

        /// <summary>
        /// 支援標準 Select 語法 [ WHERE search_condition ]
        /// </summary>
        public WhereClause Where { get; private set; }       /*[ WHERE < search_condition > ] */

        /// <summary>
        /// 支援標準 Select 語法 [ ORDER BY { order_by_expression | column_position [ ASC | DESC ] }
        /// </summary>
        public OrderByClause OrderBy { get; private set; } /*[ ORDER BY { order_by_expression | column_position [ ASC | DESC ] } */

        /// <summary>
        /// 支援標準 Select 語法 [ GROUP BY [ ALL ] group_by_expression [ ,...n ]  [ WITH { CUBE | ROLLUP } ]
        /// </summary>
        public GroupByClause GroupBy { get; private set; } /*[ GROUP BY [ ALL ] group_by_expression [ ,...n ] */

        /// <summary>
        /// 支援標準 Select 語法 [ HAVING search_condition ]
        /// </summary>
        public HavingClause Having { get; private set; }    /*[ HAVING < search_condition > ] */

        // 移至 TABLESOURCE
        //private string hints;

        ///// <summary>
        ///// 只找 with () 內的字串存至 Select.Tables.Hints ,(目前以處理 SQL Server 為主)
        ///// 不理會其內容 這部份可能需對不同資料庫的作法進行分析後, 才能進行細部設計
        ///// </summary>
        //public string Hints
        //{
        //    get { return hints; }
        //    set { hints = value; }
        //}

        /// <summary>
        /// 支援標準 Select 語法 [ ALL | DISTINCT ]  PS :為 ALL 時不輸出 
        /// </summary>
        public bool Distinct { get; set; }  /*  false : ALL  true : DISTINCT */

        /// <summary>
        ///  支援 SQL Server Select 語法 TOP N
        /// </summary>
        public int Top { get; set; } = 0;  /* TOP N  PS: N 必須大於 0  <=0 時不輸出 TOP N */

        public bool Percent4Top { get; set; } = false;  /* TOP N  PS: N 必須大於 0  <=0 時不輸出 TOP N */

        /// <summary>
        /// 是否為 Select-Union SQL
        /// </summary>
        public bool IsUnion { get { return unionSelect != null && UnionType != UnionType.None; } }

        /// <summary>
        /// Select-Union 的 SelectModel , SelectModel 允許以 UnionSelect 來描述多重的 Select-Union 模型.
        /// 包含 Union/Union All/Minus/Intersect 由 UnionType 指定.
        /// </summary>
        public SelectModel UnionSelect
        {
            get
            {
                return unionSelect;
            }
        }

        /// <summary>
        /// 表示和 UnionSelect 的連結關係 , 包含 Union/Union All/Minus/Intersect
        /// </summary>
        public UnionType UnionType
        {
            get;
            set;
        }

        /// <summary>
        /// 建立 Select-Union 模型
        /// </summary>
        public void CreateUnionModel(UnionType UnionType)
        {
            this.UnionType = UnionType;
            this.unionSelect = new SelectModel(this);
        }

        /// <summary>
        /// 建立 Select-Union 模型
        /// </summary>
        /// <param name="UnionType"></param>
        /// <param name="model"></param>
        public void CreateUnionModel(UnionType UnionType, SelectModel model)
        {
            this.UnionType = UnionType;
            this.unionSelect = model;
        }

        /// <summary>
        /// 回傳 SelectModel 條件物件集合
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public IConditionModels Conditions
        {
            get { return conditions; }
        }

        IConditionModels ISqlConditions.Conditions
        {
            get { return conditions; }
        }

        /// <summary>
        /// 依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="BuildType">指示建立 SQL 命令的文字敘述的方式</param>
        /// <param name="formatFlags">指示建立 SQL 命令的文字格式化選項</param>
        /// <returns>SQL 命令的文字敘述</returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions formatFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            // 取得建立SQL 命令的文字敘述所需的 SqlBuildExport  物件實體
            ISqlBuildExport BuildExport = GetBuildExport(BuildType, formatFlags, buildExport);

            CommandBuilder CommandBuilder = new CommandBuilder(formatFlags);

            BuildSqlCommand(this, BuildExport, CommandBuilder, false);

            return CommandBuilder.ToString();
        }

        private void BuildSqlCommand(SelectModel currentModel, ISqlBuildExport BuildExport, CommandBuilder CommandBuilder, Boolean NoDataSQLCommand)
        {
            CommandBuilder.AppendKeywordToken("SELECT");
            CommandBuilder.Indent();
            if (Distinct) CommandBuilder.AppendKeywordToken("DISTINCT");
            // for SQL Server ONLY
            if (Top > 0)
            {
                CommandBuilder.AppendKeywordToken($"TOP {Top}");
                if (Percent4Top)
                {
                    CommandBuilder.AppendKeywordToken("PERCENT");
                }
            }
            BuildExport.ExportForColumnsClause(currentModel.Columns, CommandBuilder, this);

            CommandBuilder.Unindent();

            CommandBuilder.Unindent();
            CommandBuilder.NewLine();
            CommandBuilder.Indent();

            CommandBuilder.AppendKeywordToken("FROM");

            // TODO: 不同資料的輸出有分別的, 目前只支援 SQL Server , 注意 輸出 的位置.
            BuildExport.ExportForTableClause(currentModel.Tables, CommandBuilder, this);
            //if (!string.IsNullOrWhiteSpace(Hints))
            //{
            //    CommandBuilder.AppendToken("WITH ({0})", Hints);
            //}
            if (!NoDataSQLCommand)
                BuildExport.ExportForWhereClause(currentModel.Where, CommandBuilder, this);
            else
            {
                CommandBuilder.Unindent();
                CommandBuilder.NewLine();
                CommandBuilder.Indent();
                CommandBuilder.AppendKeywordToken("WHERE");
                CommandBuilder.AppendToken(" 1=0");  //  for CommandTextForNoResultData
            }

            if (BuildExport.ExportForGroupByClause(currentModel.GroupBy, CommandBuilder, this) != 0)
            {
                BuildExport.ExportForWhereClause(currentModel.Having, CommandBuilder, this);
            }
            BuildExport.ExportForOrderByClause(currentModel.OrderBy, CommandBuilder, this);
            if (currentModel.IsUnion)
            {
                CommandBuilder.Unindent();
                CommandBuilder.NewLine();
                CommandBuilder.Indent();
                switch (currentModel.UnionType)
                {
                    case UnionType.Union: CommandBuilder.AppendKeywordToken("UNION"); break;
                    case UnionType.UnionAll: CommandBuilder.AppendKeywordToken("UNION ALL"); break;
                    case UnionType.Minus: CommandBuilder.AppendKeywordToken("MINUS"); break;
                    case UnionType.Intersect: CommandBuilder.AppendKeywordToken("INTERSECT"); break;
                }
                BuildSqlCommand(currentModel.UnionSelect, BuildExport, CommandBuilder, NoDataSQLCommand);
            }
        }

        /// <summary>
        /// 回傳取得無資料的 SelectModel 的字串敘述. 即以 WHERE 1=0 取代原先 Where 物件.
        /// </summary>
        /// <returns>SQL 命令的文字敘述</returns>
        public string CommandTextForNoResultData(CommandFormatOptions formatFlags)
        {
            ISqlBuildExport BuildExport = GetBuildExport(this.BuildType, formatFlags, null);
            CommandBuilder CommandBuilder = new CommandBuilder(formatFlags);

            BuildSqlCommand(this, BuildExport, CommandBuilder, true);

            return CommandBuilder.ToString();
        }

        public SelectModel CounterModel()
        {
            if (IsUnion)
            {
                SelectModel select = new SelectModel();
                select.Tables.PrimaryTable = new TableSource(this);
                select.Columns.Add("count(*)");
                return select;
            }
            else
            {
                SelectModel select = this.Clone();
                select.Columns.RemoveAll();
                select.Columns.Add("count(*)");
                return select;
            }
        }
    }
}