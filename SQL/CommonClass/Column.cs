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
    /// ���B�⦡����. �]�t���W�٩ΧO�W, ���B�⦡�M���ʺ�
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
        /// ���B�⦡�����W�٪��ܧ�ƥ�.
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
                    // �h�� As Name
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
        /// �Ǧ^ (���t���ʺ٪����) �B�⦡�r���
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
        /// �Ǧ^���B�⦡������.
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
        /// ���ʺ�(Hypocorism), �ΥH���N���W�٩����B�⦡
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
        /// String�A��ܥثe�� Object�C
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