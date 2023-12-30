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
    /// 指定加入 JOIN 表格名稱 , JOIN 表格必須在 指定 主表格名稱後加入 允許有多個 JOIN 表格名稱.(實際上限請查閱 資料庫文件)
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
        /// TableJoin 變更事件通知
        /// </summary>
        public event EventHandler DataChanged;

        #region 建構子(Constructor)

        /// <summary>
        /// JOIN 建構子
        /// </summary>
        /// <param name="tcase">SqlModel Table</param>
        /// <param name="tableName">JOIN 表格名稱</param>
        /// <param name="JoinType">JOIN 方式</param>
        /// <param name="expression">關係運算式  Ex a.name=b.name</param>
        public JoinTable(TableClause tcase, string tableName, JoinType JoinType, string expression, string hints = null) :
            this(tcase, tableName, null, JoinType, expression)
        {
        }

        /// <summary>
        /// JOIN 建構子
        /// </summary>
        /// <param name="tcase">SqlModel Table</param>
        /// <param name="tableName">JOIN 表格名稱</param>
        /// <param name="tableAliasName">JOIN 表格別名</param>
        /// <param name="joinType">JOIN 方式</param>
        /// <param name="expression">關係運算式  Ex a.name=b.name</param>
        public JoinTable(TableClause tcase, TableSource tableName, string tableAliasName, JoinType joinType, string expression)
        {
            this.parent = tcase;
            this.tableName = tableName;
            this.tableAliasName = tableAliasName;
            this.joinType = joinType;
            this.expression = expression;
            // 處理 所有 和 IChangedVariable 相關的事務
            // this.Variables = new IChangedVariable[] { this.tableName, this.tableAliasName, this.joinType, this.expression1, this.expression2 };
        }

        #endregion 建構子(Constructor)

        /// <summary>
        /// 回傳或設定 Join 的表格名稱(Table Name).
        /// </summary>
        public TableSource Table
        {
            get
            {
                return tableName;
            }
        }

        /// <summary>
        /// 回傳或設定 Join 的表格別名(Alias Name).
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
        /// 回傳或設定 Join 的表格第一個運算式. (??)
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
        /// 回傳或設定 Join 的型態. Example: left/right/inner join
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