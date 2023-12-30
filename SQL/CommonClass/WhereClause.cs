// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Chiats.SQL
{
    /// <summary>
    /// SqlModel WhereClause 類別, 用以管理及產生 SQL WhereClause
    /// </summary>
    public class WhereClause : CommonClause, IEnumerable<Condition>
    {
        /// <summary>
        /// 內含的所有條件運算式
        /// </summary>
        public readonly Conditions Conditions;

        /// <summary>
        /// 條件運算式 變更事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<Condition>> WhereConditionChanged;

        /// <summary>
        /// WhereClause 建構子
        /// </summary>
        public WhereClause(SqlModel parent)
            : base(parent)
        {
            this.Conditions = new Conditions(this);
        }

        /// <summary>
        /// 回傳條件運算式個數
        /// </summary>
        public int Count
        {
            get
            {
                return (Conditions == null) ? 0 : Conditions.Count;
            }
        }

        /// <summary>
        /// 加入一個功能條件運算式(Function Condition)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ArgmentType"></param>
        /// <param name="ColumnnExpress"></param>
        /// <param name="ParamName"></param>
        /// <param name="link"></param>
        /// <param name="export"></param>
        public void AddFunction(string Name, ArgumentType ArgmentType,
           string ColumnnExpress,
           string ParamName, ConditionLink link, bool export)
        {
            Conditions.AddFunction(Name, ArgmentType, ColumnnExpress, ParamName, link, export);
        }

        /// <summary>
        /// 加入一個功能條件運算式(Function Condition)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ArgmentType"></param>
        /// <param name="ColumnnExpress"></param>
        /// <param name="ParamName"></param>
        /// <param name="link"></param>
        public void AddFunction(string Name, ArgumentType ArgmentType,
          string ColumnnExpress,
          string ParamName, ConditionLink link)
        {
            Conditions.AddFunction(Name, ArgmentType, ColumnnExpress, ParamName, link, true);
        }

        /// <summary>
        /// 加入一個功能條件運算式(Function Condition)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ArgmentType"></param>
        /// <param name="ColumnnExpress"></param>
        /// <param name="ParamName"></param>
        public void AddFunction(string Name, ArgumentType ArgmentType,
               string ColumnnExpress,
               string ParamName)
        {
            Conditions.AddFunction(Name, ArgmentType, ColumnnExpress, ParamName, ConditionLink.And, true);
        }

        /// <summary>
        /// 加入一個條件運算式(Expression Condition)
        /// </summary>
        /// <param name="conditionExpression">條件運算式 Example: ColumnA=100</param>
        public void Add(string conditionExpression)
        {
            Conditions.Add(conditionExpression);
        }

        /// <summary>
        /// 加入一個條件運算式(Expression Condition)
        /// </summary>
        /// <param name="name">條件式名稱</param>
        /// <param name="conditionExpression">條件運算式 Example: ColumnA=100</param>
        /// <param name="Enabled"></param>
        public void Add(string name, string conditionExpression, bool Enabled)
        {
            Conditions.Add(name, conditionExpression, Enabled);
        }

        /// <summary>
        /// 加入一個條件運算式(Expression Condition)
        /// </summary>
        /// <param name="name">條件式名稱</param>
        /// <param name="conditionExpression">條件運算式 Example: ColumnA=100</param>
        public void Add(string name, string conditionExpression)
        {
            Conditions.Add(name, conditionExpression);
        }

        /// <summary>
        /// 加入一個條件運算式(Expression Condition)
        /// </summary>
        /// <param name="name">條件式名稱</param>
        /// <param name="conditionExpression">條件運算式 Example: ColumnA=100</param>
        /// <param name="link">ConditionLink</param>
        /// <param name="export"></param>
        public void Add(string name, string conditionExpression, ConditionLink link, bool export)
        {
            Conditions.Add(name, conditionExpression, link, export);
        }

        /// <summary>
        /// 加入一個條件運算式(Expression Condition)
        /// </summary>
        /// <param name="name">條件式名稱</param>
        /// <param name="conditionExpression">條件運算式 Example: ColumnA=100</param>
        /// <param name="link">ConditionLink</param>
        public void Add(string name, string conditionExpression, ConditionLink link)
        {
            Conditions.Add(name, conditionExpression, link);
        }

        /// <summary>
        /// 加入一個條件運算式(Expression Condition)
        /// </summary>
        /// <param name="conditionExpression">條件運算式 Example: ColumnA=100</param>
        /// <param name="link">ConditionLink</param>
        public void Add(string conditionExpression, ConditionLink link)
        {
            Conditions.Add(conditionExpression, link);
        }

        #region RaisePrivateEvent

        internal void RaiseWhereConditionChanged(ChangedEventArgs<Condition> e)
        {
            if (WhereConditionChanged != null)
            {
                WhereConditionChanged(this, e);
            }
        }

        #endregion RaisePrivateEvent

        /// <summary>
        /// 建立一個條件運算式產生器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConditionBulider CreateBulider(string name)
        {
            return new ConditionBulider(Conditions, new ExpressionCondition(name, Conditions));
        }

        /// <summary>
        /// 建立一個條件運算式產生器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public ConditionBulider CreateBulider(string name, ConditionLink link)
        {
            return new ConditionBulider(Conditions, new ExpressionCondition(this.Conditions, name, null, link));
        }

        /// <summary>
        /// 建立一個條件運算式產生器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="link"></param>
        /// <param name="export"></param>
        /// <returns></returns>
        public ConditionBulider CreateBulider(string name, ConditionLink link, bool export)
        {
            return new ConditionBulider(Conditions, new ExpressionCondition(this.Conditions, name, null, link, export));
        }

        #region IEnumerable<WhereConditionStatement> Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Condition> GetEnumerator()
        {
            return Conditions.GetEnumerator();
        }

        #endregion IEnumerable<WhereConditionStatement> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Conditions.GetEnumerator();
        }

        #endregion IEnumerable Members

        /// <summary>
        /// 傳回表示目前物件的字串。，表示目前的 Where 條件運算式內容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ISqlBuildExport BuildExport = DefaultBuildExport.SQLBuildExport;
            CommandBuilder CommandBuilder = new CommandBuilder();
            BuildExport.ExportForWhereClause(this, CommandBuilder, this.Parent, BuildExportOptions.None);
            return CommandBuilder.ToString();
        }

        /// <summary>
        /// 回傳 SQL Command 關建字("WHERE/HAVING")
        /// </summary>
        public virtual string ClauseKey { get { return "WHERE"; } }
    }
}