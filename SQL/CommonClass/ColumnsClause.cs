// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Chiats.SQL
{
    /// <summary>
    /// 欄位運算式或欄位名稱的集合並且包含各欄位運算式或欄位名稱的暱稱, 它通常是指 Select 敘述的欄位運算式定義.
    /// </summary>
    public sealed class ColumnsClause : CommonClause, IEnumerable<Column>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Column> columnList = new List<Column>();

        internal ColumnsClause(SqlModel parent) : base(parent) { }

        /// <summary>
        /// 變更欄位所引發的事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<Column>> ColumnChanged;

        /// <summary>
        /// 加入欄位運算式(Column Expression) , 包含 Count() , Sum() 等等之複合之運算式
        /// </summary>
        /// <param name="Column">指定欄位名稱或 SQL 運算式</param>
        public void Add(string Column)
        {
            Add(new Column(this, Column));
        }
        /// <summary>
        ///  GetColumnByIndex 配合 SchemaTable 的順序, 可以取得 Select 原始欄位敘述. 前題是不可以用 * 的欄位
        ///  若有 * 的欄位, 表示無法正確取得正確的順序
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Column GetColumnByIndex(int index)
        {
            if (index >= 0 && index < columnList.Count)
                return columnList[index];
            return null;
        }
        internal void RaiseColumnChanged(ChangedEventArgs<Column> columnChangedEventArgs)
        {
            if (ColumnChanged != null)
            {
                ColumnChanged(this, columnChangedEventArgs);
            }
            Parent.Changed();
        }

        /// <summary>
        /// 加入多個欄位運算式(Column Expression) , 包含 Count() , Sum() 等等之複合之運算式
        /// </summary>
        /// <param name="ColumnList">指定多個欄位運算式 , 欄位運算式之間以 ','  區隔</param>
        public void AddList(string ColumnList)
        {
            SQLTokenScanner[] t_list = SplitColumns(new SQLTokenScanner(ColumnList));
            foreach (SQLTokenScanner tokens in t_list)
            {
                Add(new Column(this, tokens));
            }
        }

        /// <summary>
        /// 加入一個 Column 物件.
        /// </summary>
        /// <param name="column"></param>
        public void Add(Column column)
        {
            if (column.parent != this)
            {
                // 不屬於本物件之 Column , 則 COPY 一份.
                column = new Column(this, column.ColumnExpression, column.AsName);
            }

            columnList.Add(column);

            if (ColumnChanged != null)
                ColumnChanged(this, new ChangedEventArgs<Column>(ChangedEventType.Add, column));
            Parent.Changed();
        }

        private SQLTokenScanner[] SplitColumns(SQLTokenScanner TokenList)
        {
            List<SQLTokenScanner> t_list = new List<SQLTokenScanner>();
            int start = 0, end = 0;
            foreach (Token t in TokenList)
            {
                if (t.Type == TokenType.Symbol && t.String == ",")
                {
                    if (start == end)
                        throw new SqlModelSyntaxException("欄位運算式錯誤");

                    SQLTokenScanner new_list = TokenList.Copy(start, end - 1);
                    t_list.Add(new_list);
                    start = ++end;
                    continue;
                }
                end++;
            }
            if (start != end)
            {
                SQLTokenScanner new_list = TokenList.Copy(start, end - 1);
                t_list.Add(new_list);
            }
            return t_list.ToArray();
        }

        /// <summary>
        /// 加入欄位運算式(Column Expression) , 包含 Count() , Sum() 等等之複合之運算式. 指定欄位所屬之表格名稱或別名
        /// </summary>
        /// <param name="Column">欄位運算式</param>
        /// <param name="ShortName">欄位暱稱</param>
        public void Add(string Column, string ShortName)
        {
            Add(new Column(this, Column, ShortName));
        }

        /// <summary>
        /// 加入欄位運算式(Column Expression) , 包含 Count() , Sum() 等等之複合之運算式. 指定欄位所屬之表格名稱或別名
        /// </summary>
        /// <param name="ColumnExpression"></param>
        internal void Add(SQLTokenScanner ColumnExpression)
        {
            Add(new Column(this, ColumnExpression));
        }

        /// <summary>
        /// 加入欄位運算式(Column Expression) , 包含 Count() , Sum() 等等之複合之運算式. 指定欄位所屬之表格名稱或別名
        /// </summary>
        /// <param name="ColumnExpression"></param>
        /// <param name="ShortName"></param>
        internal void Add(SQLTokenScanner ColumnExpression, string ShortName)
        {
            Add(new Column(this, ColumnExpression, ShortName));
        }

        /// <summary>
        /// 傳回欄位運算式之個數. 當無任何欄位運算式時, 傳回 0 表示 '*' 全部欄位均以原始形式傳回.
        /// </summary>
        public int Count
        {
            get
            {
                return columnList.Count;
            }
        }

        /// <summary>
        /// 傳回指定欄位運算式物件.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Column this[int index]
        {
            get { return columnList[index]; }
        }

        /// <summary>
        /// 移除指定欄位物件名稱之欄位運算式物件,
        /// </summary>
        /// <param name="Column">欄位物件名稱</param>
        public void Remove(Column Column)
        {
            if (columnList.Remove(Column))
            {
                if (ColumnChanged != null)
                    ColumnChanged(this, new ChangedEventArgs<Column>(ChangedEventType.Removed, Column));
                Parent.Changed();
            }
        }

        /// <summary>
        /// 除所有欄位運算式物件.
        /// </summary>
        public void RemoveAll()
        {
            if (ColumnChanged != null)
            {
                foreach (var Column in columnList)
                    ColumnChanged(this, new ChangedEventArgs<Column>(ChangedEventType.Removed, Column));
            }
            columnList.Clear();
            Parent.Changed();
        }

        /// <summary>
        /// 移除一個欄位運算式物件, 指定索引值.
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            Column column = columnList[index];
            if (column != null)
            {
                columnList.RemoveAt(index);

                if (ColumnChanged != null)
                    ColumnChanged(this, new ChangedEventArgs<Column>(ChangedEventType.Removed, column));
                Parent.Changed();
            }
        }

        #region IEnumerable<Column> Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Column> GetEnumerator()
        {
            return columnList.GetEnumerator();
        }

        #endregion IEnumerable<Column> Members

        #region IEnumerable Members

        /// <summary>
        /// 支援 IEnumerator 介面的程式語言需要這個方法，才能逐一查看集合的成員
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return columnList.GetEnumerator();
        }

        #endregion IEnumerable Members

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ISqlBuildExport BuildExport = DefaultBuildExport.SQLBuildExport;
            CommandBuilder CommandBuilder = new CommandBuilder();
            BuildExport.ExportForColumnsClause(this, CommandBuilder, this.Parent, BuildExportOptions.None);
            return CommandBuilder.ToString();
        }

        internal void CopyFrom(ColumnsClause Columns)
        {
            columnList.AddRange(from c in Columns.columnList select c.Clone(this));
        }
    }
}