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

namespace Chiats.SQL
{
    /// <summary>
    /// 資料庫的欄位及其型態資訊描述集合管理物件.
    /// </summary>
    public sealed class ColumnDescriptionCollection : CommonClause, IEnumerable<ColumnDescription>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<ColumnDescription> columns = new List<ColumnDescription>();

        /// <summary>
        /// 變更欄位所引發的事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<ColumnDescription>> ColumnChanged;

        internal ColumnDescriptionCollection(SqlModel parent) : base(parent)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="ColumnType"></param>
        /// <param name="size"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public ColumnDescription Add(string ColumnName, ColumnType ColumnType, int size, short precision, short scale)
        {
            ColumnDescription col = new ColumnDescription(ColumnName, ColumnType, size, precision, scale);
            columns.Add(col);
            return col;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ColumnDescription"></param>
        public ColumnDescription Add(string ColumnDescription)
        {
            ColumnDescription col = new ColumnDescription(ColumnDescription);
            columns.Add(col);
            return col;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Column"></param>
        public void Add(ColumnDescription Column)
        {
            columns.Add(Column);
        }

        /// <summary>
        /// 子物件個數
        /// </summary>
        public int Count
        {
            get { return columns.Count; }
        }

        /// <summary>
        /// 傳回指定欄位運算式物件.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ColumnDescription this[int index]
        {
            get { return columns[index]; }
        }

        /// <summary>
        /// 移除一個欄位運算式物件, 指定欄位物件
        /// </summary>
        /// <param name="column"></param>
        public void Remove(ColumnDescription column)
        {
            columns.Remove(column);
            if (ColumnChanged != null)
                ColumnChanged(this, new ChangedEventArgs<ColumnDescription>(ChangedEventType.Removed, column));
            Parent.Changed();
        }

        /// <summary>
        /// 移除一個欄位運算式物件, 指定索引值.
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            ColumnDescription column = columns[index];
            columns.RemoveAt(index);

            if (ColumnChanged != null)
                ColumnChanged(this, new ChangedEventArgs<ColumnDescription>(ChangedEventType.Removed, column));
            Parent.Changed();
        }

        #region IEnumerable<TableDescription> Members

        IEnumerator<ColumnDescription> IEnumerable<ColumnDescription>.GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        #endregion IEnumerable<TableDescription> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}