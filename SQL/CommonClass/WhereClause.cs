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
    /// SqlModel WhereClause ���O, �ΥH�޲z�β��� SQL WhereClause
    /// </summary>
    public class WhereClause : CommonClause, IEnumerable<Condition>
    {
        /// <summary>
        /// ���t���Ҧ�����B�⦡
        /// </summary>
        public readonly Conditions Conditions;

        /// <summary>
        /// ����B�⦡ �ܧ�ƥ�
        /// </summary>
        public event EventHandler<ChangedEventArgs<Condition>> WhereConditionChanged;

        /// <summary>
        /// WhereClause �غc�l
        /// </summary>
        public WhereClause(SqlModel parent)
            : base(parent)
        {
            this.Conditions = new Conditions(this);
        }

        /// <summary>
        /// �^�Ǳ���B�⦡�Ӽ�
        /// </summary>
        public int Count
        {
            get
            {
                return (Conditions == null) ? 0 : Conditions.Count;
            }
        }

        /// <summary>
        /// �[�J�@�ӥ\�����B�⦡(Function Condition)
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
        /// �[�J�@�ӥ\�����B�⦡(Function Condition)
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
        /// �[�J�@�ӥ\�����B�⦡(Function Condition)
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
        /// �[�J�@�ӱ���B�⦡(Expression Condition)
        /// </summary>
        /// <param name="conditionExpression">����B�⦡ Example: ColumnA=100</param>
        public void Add(string conditionExpression)
        {
            Conditions.Add(conditionExpression);
        }

        /// <summary>
        /// �[�J�@�ӱ���B�⦡(Expression Condition)
        /// </summary>
        /// <param name="name">���󦡦W��</param>
        /// <param name="conditionExpression">����B�⦡ Example: ColumnA=100</param>
        /// <param name="Enabled"></param>
        public void Add(string name, string conditionExpression, bool Enabled)
        {
            Conditions.Add(name, conditionExpression, Enabled);
        }

        /// <summary>
        /// �[�J�@�ӱ���B�⦡(Expression Condition)
        /// </summary>
        /// <param name="name">���󦡦W��</param>
        /// <param name="conditionExpression">����B�⦡ Example: ColumnA=100</param>
        public void Add(string name, string conditionExpression)
        {
            Conditions.Add(name, conditionExpression);
        }

        /// <summary>
        /// �[�J�@�ӱ���B�⦡(Expression Condition)
        /// </summary>
        /// <param name="name">���󦡦W��</param>
        /// <param name="conditionExpression">����B�⦡ Example: ColumnA=100</param>
        /// <param name="link">ConditionLink</param>
        /// <param name="export"></param>
        public void Add(string name, string conditionExpression, ConditionLink link, bool export)
        {
            Conditions.Add(name, conditionExpression, link, export);
        }

        /// <summary>
        /// �[�J�@�ӱ���B�⦡(Expression Condition)
        /// </summary>
        /// <param name="name">���󦡦W��</param>
        /// <param name="conditionExpression">����B�⦡ Example: ColumnA=100</param>
        /// <param name="link">ConditionLink</param>
        public void Add(string name, string conditionExpression, ConditionLink link)
        {
            Conditions.Add(name, conditionExpression, link);
        }

        /// <summary>
        /// �[�J�@�ӱ���B�⦡(Expression Condition)
        /// </summary>
        /// <param name="conditionExpression">����B�⦡ Example: ColumnA=100</param>
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
        /// �إߤ@�ӱ���B�⦡���;�
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConditionBulider CreateBulider(string name)
        {
            return new ConditionBulider(Conditions, new ExpressionCondition(name, Conditions));
        }

        /// <summary>
        /// �إߤ@�ӱ���B�⦡���;�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public ConditionBulider CreateBulider(string name, ConditionLink link)
        {
            return new ConditionBulider(Conditions, new ExpressionCondition(this.Conditions, name, null, link));
        }

        /// <summary>
        /// �إߤ@�ӱ���B�⦡���;�
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
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
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
        /// �Ǧ^��ܥثe���󪺦r��C�A��ܥثe�� Where ����B�⦡���e
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
        /// �^�� SQL Command ���ئr("WHERE/HAVING")
        /// </summary>
        public virtual string ClauseKey { get { return "WHERE"; } }
    }
}