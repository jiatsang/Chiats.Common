// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chiats.SQL
{
    /// <summary>
    /// ��Ʈw�����ީw�q.
    /// </summary>
    public class IndexDescription : IVariantName, IEnumerable<IndexDescription.Column>
    {
        /// <summary>
        /// �������w�q.
        /// </summary>
        public struct Column
        {
            /// <summary>
            /// �������w�q�غc�l
            /// </summary>
            /// <param name="Name">���W��</param>
            /// <param name="Sorting">�Ƨ�</param>
            public Column(string Name, OrderSorting Sorting) { this.Name = Name; this.Sorting = Sorting; }

            /// <summary>
            /// ���W��
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// �Ƨ�
            /// </summary>
            public OrderSorting Sorting;

            /// <summary>
            /// �Ǧ^��ܥثe���󪺦r��C
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Sorting == OrderSorting.Descending)
                    return Name;

                return string.Format("{0} asc", Name);
            }
        }

        /// <summary>
        /// ��Ʈw�����ީw�q�غc�l
        /// </summary>
        /// <param name="name"></param>
        public IndexDescription(string name)
        {
            this.name = name;
            this.unique = false;
            this.primaryKey = false;
            this.clustered = false;
        }

        private List<Column> columnkeys = new List<Column>();
        private string name;
        private bool unique;
        private bool primaryKey;
        private bool clustered;

        /// <summary>
        /// �^�ǫ��w��m���������w�q
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Column this[int index]
        {
            get { return columnkeys[index]; }
        }

        /// <summary>
        /// �������w�q�Ӽ�
        /// </summary>
        public int Count
        {
            get { return columnkeys.Count; }
        }

        /// <summary>
        /// �W��.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// �[�J�@�ӯ������w�q
        /// </summary>
        /// <param name="Column"></param>
        public void Add(Column Column) { columnkeys.Add(Column); }

        /// <summary>
        /// �����@�ӯ������w�q
        /// </summary>
        /// <param name="Column"></param>
        public void Remove(Column Column) { columnkeys.Remove(Column); }

        /// <summary>
        /// �M���Ҧ����������w�q
        /// </summary>
        public void Clear() { columnkeys.Clear(); }

        /// <summary>
        /// �^�ǩγ]�w �O�_�� Unique Index
        /// </summary>
        public bool Unique
        {
            get { return unique; }
            set { unique = value; }
        }

        /// <summary>
        /// �^�ǩγ]�w �O�_�� Primary Key
        /// </summary>
        public bool PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }

        /// <summary>
        /// �^�ǩγ]�w �O�_�� Clustered
        /// </summary>
        public bool Clustered
        {
            get { return clustered; }
            set { clustered = value; }
        }

        /// <summary>
        /// �^�ǯ������W��. �p���h������, �H , �Ϲj
        /// </summary>
        public string Keys
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Column key in columnkeys)
                {
                    if (sb.Length != 0)
                        sb.Append(',');
                    sb.AppendFormat(key.ToString());
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// �Ǧ^��ܥثe���󪺦r��C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Keys;
        }

        #region IEnumerable<KeyColumn> Members

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO�l������C
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IndexDescription.Column> GetEnumerator()
        {
            return columnkeys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return columnkeys.GetEnumerator();
        }

        #endregion IEnumerable<KeyColumn> Members
    }
}