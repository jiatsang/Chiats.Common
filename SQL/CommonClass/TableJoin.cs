// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using Chiats.SQL.Expression;
using System;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// ���w�[�J JOIN ���W�� , JOIN ��楲���b ���w �D���W�٫�[�J ���\���h�� JOIN ���W��.(��ڤW���Ьd�\ ��Ʈw���)
    /// </summary>
    public sealed class JoinTable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TableSource tableName = new TableSource();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string tableAliasName = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JoinType joinType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string expression = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TableClause parent;

        /// <summary>
        /// TableJoin �ܧ�ƥ�q��
        /// </summary>
        public event EventHandler DataChanged;

        #region �غc�l(Constructor)

        /// <summary>
        /// JOIN �غc�l
        /// </summary>
        /// <param name="tcase">SqlModel Table</param>
        /// <param name="tableName">JOIN ���W��</param>
        /// <param name="JoinType">JOIN �覡</param>
        /// <param name="expression">���Y�B�⦡  Ex a.name=b.name</param>
        public JoinTable(TableClause tcase, string tableName, JoinType JoinType, string expression, string hints = null) :
            this(tcase, tableName, null, JoinType, expression)
        {
        }

        /// <summary>
        /// JOIN �غc�l
        /// </summary>
        /// <param name="tcase">SqlModel Table</param>
        /// <param name="tableName">JOIN ���W��</param>
        /// <param name="tableAliasName">JOIN ���O�W</param>
        /// <param name="joinType">JOIN �覡</param>
        /// <param name="expression">���Y�B�⦡  Ex a.name=b.name</param>
        public JoinTable(TableClause tcase, TableSource tableName, string tableAliasName, JoinType joinType, string expression)
        {
            this.parent = tcase;
            this.tableName = tableName;
            this.tableAliasName = tableAliasName;
            this.joinType = joinType;
            this.expression = expression;
            // �B�z �Ҧ� �M IChangedVariable �������ư�
            // this.Variables = new IChangedVariable[] { this.tableName, this.tableAliasName, this.joinType, this.expression1, this.expression2 };
        }

        #endregion �غc�l(Constructor)

        /// <summary>
        /// �^�ǩγ]�w Join �����W��(Table Name).
        /// </summary>
        public TableSource Table
        {
            get
            {
                return tableName;
            }
        }

        /// <summary>
        /// �^�ǩγ]�w Join �����O�W(Alias Name).
        /// </summary>
        public string Alias
        {
            get { return tableAliasName; }
            set
            {
                if (tableAliasName != value)
                {
                    tableAliasName = value;
                    DataChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// �^�ǩγ]�w Join �����Ĥ@�ӹB�⦡. (??)
        /// </summary>
        public string Expression
        {
            get
            {
                var sqlModel = parent.GetTopModel();
                if (sqlModel != null && sqlModel.Options.HasFlag(SqlOptions.LowerName))
                {
                    ConditionExpression expr = new ConditionExpression(expression);
                    return expr.RebuildExpression(CommandBuildType.SQL, sqlModel.ParameterMode, null,true); 
                }
                return expression;
            }
            set
            {
                if (expression != value)
                {
                    expression = value;
                    DataChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// �^�ǩγ]�w Join �����A. Example: left/right/inner join
        /// </summary>
        public JoinType JoinType
        {
            get { return joinType; }
            set
            {
                if (joinType != value)
                {
                    joinType = value;
                    if (DataChanged != null)
                    {
                        DataChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}