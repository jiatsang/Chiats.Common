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
    public sealed class IndexDescriptionCollection : CommonClause, IEnumerable<IndexDescription>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<IndexDescription> columns = new List<IndexDescription>();

        /// <summary>
        /// 變更欄位所引發的事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<IndexDescription>> ColumnChanged;

        public string WithIndexOption = null;

        internal IndexDescriptionCollection(SqlModel parent) : base(parent)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="IndexDescription"></param>
        public IndexDescription Add(string IndexDescription)
        {
            IndexDescription col = new IndexDescription(IndexDescription);
            columns.Add(col);
            return col;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Column"></param>
        public void Add(IndexDescription Column)
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
        public IndexDescription this[int index]
        {
            get { return columns[index]; }
        }

        public IndexDescription PrimaryKey
        {
            get
            {
                foreach (IndexDescription index in this)
                {
                    if (index.PrimaryKey)
                    {
                        return index;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 移除一個欄位運算式物件, 指定欄位物件
        /// </summary>
        /// <param name="column"></param>
        public void Remove(IndexDescription column)
        {
            columns.Remove(column);
            if (ColumnChanged != null)
                ColumnChanged(this, new ChangedEventArgs<IndexDescription>(ChangedEventType.Removed, column));
            Parent.Changed();
        }

        /// <summary>
        /// 移除一個欄位運算式物件, 指定索引值.
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            IndexDescription column = columns[index];
            columns.RemoveAt(index);

            if (ColumnChanged != null)
                ColumnChanged(this, new ChangedEventArgs<IndexDescription>(ChangedEventType.Removed, column));
            Parent.Changed();
        }

        #region IEnumerable<TableDescription> Members

        IEnumerator<IndexDescription> IEnumerable<IndexDescription>.GetEnumerator()
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