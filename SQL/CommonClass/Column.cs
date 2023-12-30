// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// 欄位運算式物件. 包含表格名稱或別名, 欄位運算式和欄位暱稱
    /// </summary>
    public sealed class Column
    {
        internal ColumnsClause parent;

        private Expression.ColumnExpression expression = null;
        private string columnExpression = null;
        private string columnAsName = null;

        private SQLTokenScanner tokens = null;

        internal Column Clone(ColumnsClause parent = null)
        {
            var col = (Column)this.MemberwiseClone();
            if (parent != null) col.parent = parent;
            return col;
        }
        /// <summary>
        /// 欄位運算式或欄位名稱的變更事件.
        /// </summary>
        public event EventHandler<ChangedEventArgs<Column>> ColumnChanged;

        internal Column(ColumnsClause parent, SQLTokenScanner tokens, string columnAsName = null)
        {
            this.tokens = tokens;
            this.parent = parent;
            if (columnAsName == null)
            {
                var index = FindAsIndex(tokens);
                if (index == -1)
                {
                    this.columnExpression = tokens.Statement;
                    this.columnAsName = null;
                }
                else
                {
                    // 去除 As Name
                    this.columnExpression = tokens.RebuildToken(0, index);
                    this.columnAsName = tokens[tokens.Count - 1].String;
                    this.tokens = new SQLTokenScanner(tokens, 0, index);
                }
            }
            else
            {
                this.columnExpression = tokens.Statement;
                this.columnAsName = columnAsName;
            }
        }

        private static int FindAsIndex(SQLTokenScanner tokens)
        {
            if (tokens.Count >= 2)
            {
                var last_token = tokens[tokens.Count - 1];
                var last_as_token = tokens[tokens.Count - 2];
                if (last_token.IsKeyword && (last_as_token.IsKeyword || last_as_token.IsSymbol(')')))
                {
                    if (string.Compare(last_as_token.String, "as", true) == 0)
                    {
                        return tokens.Count - 3;
                    }
                    return tokens.Count - 2;
                    // expr as a1
                    // expr a1
                    // isnull(1,2,3) a1
                    // c1 + c2 a1
                }
            }
            return -1;
        }
        internal Column(ColumnsClause parent, string expression, string columnAliasName = null)
        {
            Debug.Assert(parent != null);
            this.parent = parent;
            this.columnExpression = expression;
            this.columnAsName = columnAliasName;
        }

        private void RaiseColumnChanged(ChangedEventType type)
        {
            ChangedEventArgs<Column> columnChangedEventArgs = new ChangedEventArgs<Column>(type, this);
            if (ColumnChanged != null)
                ColumnChanged(this, columnChangedEventArgs);
            parent.RaiseColumnChanged(columnChangedEventArgs);
        }

        /// <summary>
        /// 傳回 (不含欄位暱稱的欄位) 運算式字串值
        /// </summary>
        public string ColumnExpression
        {
            get { return columnExpression; }
            set
            {
                if (columnExpression != value)
                {
                    columnExpression = value;
                    expression = null; // reset ColumnExpression
                }
            }
        }
        public string RebuildExpression(SqlOptions options)
        {
            if (this.tokens != null)
                return tokens.RebuildToken(0, tokens.Count - 1, options.HasFlag(SqlOptions.LowerName));

            if (options.HasFlag(SqlOptions.LowerName))
                return columnExpression?.ToLower();
            else
                return columnExpression;
        }
        /// <summary>
        /// 傳回欄位運算式的物件.
        /// </summary>
        public Expression.ColumnExpression Expression
        {
            get
            {
                if (expression == null)
                    expression = new Expression.ColumnExpression(columnExpression);
                return expression;
            }
        }
        /// <summary>
        /// 欄位暱稱(Hypocorism), 用以取代欄位名稱或欄位運算式
        /// </summary>
        public string AsName
        {
            get { return columnAsName; }
            set
            {
                if (columnAsName != value)
                {
                    columnAsName = value;
                }
            }
        }

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(AsName))
                return ColumnExpression;
            return $"{ColumnExpression} as {AsName}";
        }
    }
}