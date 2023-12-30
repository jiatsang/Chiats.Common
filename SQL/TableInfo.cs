// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections.Generic;

namespace Chiats.SQL
{
    /// <summary>
    /// 資料表格的詳細資訊. 和資料庫廠商版本無關欄位資訊(名稱,型能,大小) 資料表格會由資料庫取得基於
    /// 1. DACTemplate.GetDBInformation 為取得 IDBInformation 介面
    /// 2. (Ext) 完整的 XML 的系統資訊.
    ///          作用在於資料庫的比對. 版本檢測. 資料庫回
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 資料庫資料表格資訊物件建構子
        /// </summary>
        public TableInfo() { Columns = new ColumnInfoCollection(this); }

        /// <summary>
        /// 資料庫資料欄位
        /// </summary>
        public ColumnInfoCollection Columns;
    }

    /// <summary>
    /// 資料表格欄位資訊集合
    /// </summary>
    public class ColumnInfoCollection : IEnumerable<ColumnInfo>
    {
        private TableInfo TableInfo = null;

        internal ColumnInfoCollection(TableInfo TableInfo)
        {
            this.TableInfo = TableInfo;
        }

        private List<ColumnInfo> columns = new List<ColumnInfo>();

        /// <summary>
        /// 資料庫資料表格欄位個數
        /// </summary>
        public int Count { get { return columns.Count; } }

        /// <summary>
        /// 新增一個料表格欄位物件
        /// </summary>
        /// <param name="ColumnInfo"></param>
        public void Add(ColumnInfo ColumnInfo)
        {
            lock (this)
            {
                columns.Add(ColumnInfo);
            }
        }

        /// <summary>
        /// 回傳指定位置的表格欄位物件
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ColumnInfo this[int index]
        {
            get { return columns[index]; }
        }

        /// <summary>
        /// 回傳指定名稱的表格欄位物件. 不存在時回傳 null
        /// </summary>
        /// <param name="name">表格欄位名稱</param>
        /// <returns></returns>
        public ColumnInfo Find(string name)
        {
            lock (this)
            {
                foreach (var column in columns)
                {
                    if (name == column.Name) { return column; }
                }
            }
            return null;
        }

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別子的物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ColumnInfo> GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return columns.GetEnumerator();
        }
    }
}