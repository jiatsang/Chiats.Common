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
    /// 資料庫的索引定義.
    /// </summary>
    public class IndexDescription : IVariantName, IEnumerable<IndexDescription.Column>
    {
        /// <summary>
        /// 索引欄位定義.
        /// </summary>
        public struct Column
        {
            /// <summary>
            /// 索引欄位定義建構子
            /// </summary>
            /// <param name="Name">欄位名稱</param>
            /// <param name="Sorting">排序</param>
            public Column(string Name, OrderSorting Sorting) { this.Name = Name; this.Sorting = Sorting; }

            /// <summary>
            /// 欄位名稱
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// 排序
            /// </summary>
            public OrderSorting Sorting;

            /// <summary>
            /// 傳回表示目前物件的字串。
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
        /// 資料庫的索引定義建構子
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
        /// 回傳指定位置的索引欄位定義
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Column this[int index]
        {
            get { return columnkeys[index]; }
        }

        /// <summary>
        /// 索引欄位定義個數
        /// </summary>
        public int Count
        {
            get { return columnkeys.Count; }
        }

        /// <summary>
        /// 名稱.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 加入一個索引欄位定義
        /// </summary>
        /// <param name="Column"></param>
        public void Add(Column Column) { columnkeys.Add(Column); }

        /// <summary>
        /// 移除一個索引欄位定義
        /// </summary>
        /// <param name="Column"></param>
        public void Remove(Column Column) { columnkeys.Remove(Column); }

        /// <summary>
        /// 清除所有的索引欄位定義
        /// </summary>
        public void Clear() { columnkeys.Clear(); }

        /// <summary>
        /// 回傳或設定 是否為 Unique Index
        /// </summary>
        public bool Unique
        {
            get { return unique; }
            set { unique = value; }
        }

        /// <summary>
        /// 回傳或設定 是否為 Primary Key
        /// </summary>
        public bool PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }

        /// <summary>
        /// 回傳或設定 是否為 Clustered
        /// </summary>
        public bool Clustered
        {
            get { return clustered; }
            set { clustered = value; }
        }

        /// <summary>
        /// 回傳索引欄位名稱. 如有多個欄位時, 以 , 區隔
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
        /// 傳回表示目前物件的字串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Keys;
        }

        #region IEnumerable<KeyColumn> Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別子的物件。
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