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
    /// ��Ʈw�����Ψ䫬�A��T�y�z���X�޲z����.
    /// </summary>
    public sealed class ColumnDescriptionCollection : CommonClause, IEnumerable<ColumnDescription>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<ColumnDescription> columns = new List<ColumnDescription>();

        /// <summary>
        /// �ܧ����Ҥ޵o���ƥ�
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
        /// �l����Ӽ�
        /// </summary>
        public int Count
        {
            get { return columns.Count; }
        }

        /// <summary>
        /// �Ǧ^���w���B�⦡����.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ColumnDescription this[int index]
        {
            get { return columns[index]; }
        }

        /// <summary>
        /// �����@�����B�⦡����, ���w��쪫��
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
        /// �����@�����B�⦡����, ���w���ޭ�.
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