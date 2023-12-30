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
    /// SqlModel 的條件  Update 語法物件.
    /// </summary>
    public class UpdateModel : SqlModel, ISqlConditions
    {
        /// <summary>
        /// 表格名稱 表格名稱格式如下 [database].[owner].[tablename]
        /// </summary>
        public TableName Table;

        /// <summary>
        /// 支援標準 Update 語法 [ FROM { table_source } [ ,...n ] ]
        /// </summary>
        public TableClause Tables { get; internal set; }       /*[ FROM { < table_source > } [ ,...n ] ]*/

        /// <summary>
        /// 支援標準 Update 語法 [ WHERE search_condition ]
        /// </summary>
        public WhereClause Where { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ModelConditions conditions;

        /// <summary>
        /// Update 語法物件 Set 的欄位定義及寫入值.
        /// </summary>
        public ColumnValues Columns { get; private set; }

        /// <summary>
        ///  UpdateModel 基礎建構子
        /// </summary>
        public UpdateModel()
        {
            Where = new WhereClause(this);
            Columns = new ColumnValues(this);
            conditions = new UpdateModel.ModelConditions(this);
            Where.WhereConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(Where_WhereConditionChanged);
            Columns.ColumnValueChanged += new EventHandler<ChangedEventArgs<ColumnValue>>(Columns_ColumnValueChanged);
        }

        /// <summary>
        /// UpdateModel 建構子
        /// </summary>
        /// <param name="CTLSQL">不包含空白字元的名稱視為表格名稱</param>
        /// <param name="parameters">CTLSQL 為表格名稱時 , parameters 則視為要更新的欄位名稱及定義. 否則為參數</param>
        /// <param name="condition">CTLSQL 為表格名稱時 , 附帶的 SQL 條件</param>
        /// <param name="conditionParameters">CTLSQL 為表格名稱時 ,且附帶有的 SQL 條件, 會自動帶入參數定義, 它和 parameters 相同但不會成為更新的欄位 </param>
        public UpdateModel(string CTLSQL, object parameters = null, string condition = null, object conditionParameters = null)
            : this()
        {
            // ctlsql 不為單一表格名稱視為 SQL Command, 
            // TODO : 以 [] 為表格名稱, 可能含有空白字元
            if (CTLSQL.Trim().IndexOf('\x20') != -1)
            {
                SqlModel.Parse<UpdateModel>(CTLSQL, this);
                if (parameters != null)
                {
                    Parameters.Fill(parameters);
                    //foreach (var p in parameters.GetType().GetProperties())
                    //{
                    //    var p_name = $"@{p.Name}";
                    //    if (Parameters.Contains(p_name))
                    //        this.Parameters[p_name].Value = p.GetValue(parameters, null);
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

            if (condition != null)
            {
                Where.Add(condition);
                if (conditionParameters != null)
                {
                    Parameters.Fill(conditionParameters);
                    //foreach (var p in conditionParameters.GetType().GetProperties())
                    //{
                    //    var p_name = $"@{p.Name}";
                    //    if (Parameters.Contains(p_name))
                    //        this.Parameters[p_name].Value = p.GetValue(conditionParameters, null);
                    //    else
                    //        throw new CommonException($"未定義的 Parameter Name {p_name} in {nameof(conditionParameters)}");
                    //}
                }
            }
        }

        /// <summary>
        /// 回傳 UpdateModel 條件物件集合
        /// </summary>
        public UpdateModel.ModelConditions Conditions
        {
            get { return conditions; }
        }

        IConditionModels ISqlConditions.Conditions
        {
            get { return conditions; }
        }

        private void Columns_ColumnValueChanged(object sender, ChangedEventArgs<ColumnValue> e)
        {
            this.Changed();
        }

        private void Where_WhereConditionChanged(object sender, ChangedEventArgs<Condition> e)
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

                case ChangedEventType.Changed:
                    break;
            }
            this.Changed();
        }

        /// <summary>
        /// UpdateModel Condition 集合之管理類別
        /// </summary>
        public class ModelConditions : CommonList<IConditionModel>, IConditionModels
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private UpdateModel update;

            /// <summary>
            /// 條件式變更事件
            /// </summary>
            public event EventHandler<ChangedEventArgs<IConditionModel>> ConditionChanged;

            internal ModelConditions(UpdateModel update)
            {
                this.update = update;
                this.update.Where.WhereConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(WhereConditionChanged);
            }

            /// <summary>
            /// 查詢指定參數所屬的條件式 (Condition) 名稱 , 同時屬於多個條件式以 ',' 區隔
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public string QueyBelongCondition(string name)
            {
                string condition_names = null;
                foreach (var Condition in update.Conditions)
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
                foreach (var Condition in update.Conditions)
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

            private void WhereConditionChanged(object sender, ChangedEventArgs<Condition> e)
            {
                update.Changed();

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
            public new UpdateModel.ModelCondition this[int index]
            {
                get { return (UpdateModel.ModelCondition)base[index]; }
            }

            /// <summary>
            /// 取得指定名稱之條件式物件.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public new UpdateModel.ModelCondition this[string name]
            {
                get { return (UpdateModel.ModelCondition)base[name]; }
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
        ///  UpdateModel Condition 管理類別
        /// </summary>
        public class ModelCondition : IConditionModel
        {
            /// <summary>
            /// UpdateModel Condition 管理類別建構子
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
        /// 依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="BuildType">指示建立 SQL 命令的文字敘述的方式</param>
        /// <param name="formatFlags"></param>
        /// <returns>SQL 命令的文字敘述</returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions formatFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            ISqlBuildExport BuildExport = GetBuildExport(BuildType, formatFlags, buildExport);

            CommandBuilder sb = new CommandBuilder(formatFlags);
            sb.AppendKeywordToken("UPDATE");
            sb.AppendToken(Table.FullName);
            sb.AppendKeywordToken("SET");

            int col_count = 0;
            foreach (ColumnValue cv in Columns)
            {
                if (col_count++ != 0)
                    sb.AppendToken(",");
                BuildExport.ExportForSetValues(cv, sb, this);
            }

            if (this.Tables != null)
            {
                sb.AppendToken("FROM");
                BuildExport.ExportForTableClause(Tables, sb, this);
            }

            BuildExport.ExportForWhereClause(Where, sb, this);
            return sb.ToString();
        }
    }
}