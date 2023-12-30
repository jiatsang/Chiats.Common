using System;
using System.Diagnostics;

namespace Chiats.Data
{
    /// <summary>
    /// 表格資料列的迭代器 (Row Iterator)
    /// </summary>
    public sealed class RowIterator
    {
        // RowIndex == -1 -> BOF
        // RowIndex == PageTable.Rows.Count -> (NewRow or EOF)

        public static RowIterator operator ++(RowIterator Iterator)
        {
            Iterator.GoNext();
            return Iterator;
        }


        public static RowIterator operator --(RowIterator Iterator)
        {
            Iterator.GoPrevious();
            return Iterator;
        }

        public TableRows Rows { get; private set; }

        /// <summary>
        /// 回傳資料表格的欄位資訊列. 
        /// </summary>
        public TableRowColumns Columns
        {
            get
            {
                Debug.Assert(Rows != null, "Rows 不允許為空值");
                return Rows.Columns;
            }
        }

        /// <summary>
        /// 表格資料列 建構子.
        /// </summary>
        /// <param name="PageTable"></param>
        public RowIterator(TableRows Rows)
        {
            this.Rows = Rows;
        }

        /// <summary>
        /// 回傳目前表格資料列的索引指標 (0 ~ RowCount -1 )
        /// </summary>
        public int RowIndex { get; private set; }

        ///// <summary>
        ///// 修正 RowIndex 因為刪除後會保持在最後一筆.
        ///// </summary>
        //public void FixRowIndex()
        //{
        //    if (RowIndex >= Rows.Count)
        //    {
        //        RowIndex = Rows.Count - 1;
        //        //if (SelectChanged != null) SelectChanged(this, new SelectChangedEventArgs<T>(this, RowIndex));
        //    }
        //}

        public void Reset()
        {
            GoFirst();
        }
        /// <summary>
        /// 移至第一筆資料列. 
        /// </summary>
        /// <returns>傳回是否已超過範圍</returns>
        public bool GoFirst()
        {
            if (Rows.Count > 0 && RowIndex != 0)
            {
                RowIndex = 0;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移至下一筆資料列..如超過範圍則不移位置.
        /// </summary>
        /// <param name="step">移動筆數</param>
        /// <returns>傳回是否已超過範圍.</returns>
        public bool GoNext(int step = 1)
        {
            if (Rows.Count > 0)
            {
                RowIndex = RowIndex + step;
                if (RowIndex >= 0 && RowIndex < Rows.Count)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 移至指定資料列.如超過範圍則不移位置.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>傳回是否已超過範圍.</returns>
        public bool GoTo(int index)
        {
            if (Rows.Count > 0)  // 必須有資料
            {
                if (index >= 0 && index < this.Rows.Count)
                {
                    RowIndex = index;
                    //if (SelectChanged != null) SelectChanged(this, new SelectChangedEventArgs<T>(this, RowIndex));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移至前一筆資料列..如超過範圍則不移位置.
        /// </summary>
        /// <param name="step">移動筆數</param>
        /// <returns>傳回是否已超過範圍</returns>
        public bool GoPrevious(int step = 1)
        {
            if (Rows.Count > 0)  // 必須有資料
            {
                int newIndex = RowIndex - step;
                if (newIndex >= 0 && newIndex < this.Rows.Count)
                {
                    RowIndex = newIndex;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移至最後一筆資料列..如超過範圍則不移位置.
        /// </summary>
        public void GoLast()
        {
            if (RowIndex != Rows.Count - 1)
            {
                RowIndex = Rows.Count - 1;
                //if (SelectChanged != null) SelectChanged(this, new SelectChangedEventArgs<T>(this, RowIndex));
            }
        }
        /// <summary>
        /// 建立一筆新的資料列, 並使迭代器(Row Iterator)指向新建的資料列
        /// </summary>
        /// <returns></returns>
        //public int CreateNewRow()
        //{
        //    this.RowIndex = Rows.Count;
        //    Rows.Create();

        //    //if (SelectChanged != null) SelectChanged(this, new SelectChangedEventArgs<T>(this, RowIndex));

        //    return RowIndex;
        //}

        /// <summary>
        /// 建立迭代器(Row Iterator)物件 指向 RowIndex 的資料列, 如果RowIndex 的資料列不存在則指向最一筆.
        /// </summary>
        /// <param name="PageTable"></param>
        /// <param name="RowIndex"></param>
        public RowIterator(TableRows Rows, int RowIndex/*, object Filter = null*/)
        {
            this.Rows = Rows;

            if (RowIndex < 0) RowIndex = -1;
            if (RowIndex > Rows.Count) RowIndex = Rows.Count;
            this.RowIndex = RowIndex;

            //this.PageTable.RowChanged += TableData_RowChanged;
            //if (SelectChanged != null) SelectChanged(this, new SelectChangedEventArgs<T>(this, RowIndex));

        }

        /// <summary>
        ///  建立迭代器(Row Iterator)物件 指向 RowIndex 的資料列, 如果RowIndex 的資料列不存在則指向最一筆.
        /// </summary>
        /// <param name="Iterator"></param>
        /// <param name="RowIndex"></param>
        public RowIterator(RowIterator Iterator, int RowIndex)
        {
            this.Rows = Iterator.Rows;

            if (RowIndex < 0) RowIndex = -1;
            if (RowIndex > Rows.Count) RowIndex = Rows.Count;

            this.RowIndex = RowIndex;
            //this.PageTable.RowChanged += TableData_RowChanged;
            //if (SelectChanged != null) SelectChanged(this, new SelectChangedEventArgs<T>(this, RowIndex));
        }

        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列.
        /// </summary>
        public TableRow CurretRow
        {
            get
            {
                if (Rows.Count > 0)  // 必須有資料
                {
                    if (RowIndex >= 0 && RowIndex < this.Rows.Count)
                    {
                        return Rows[RowIndex];
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列的狀態資訊.
        /// </summary>
        public TableRowState State
        {
            get
            {
                if (Rows.Count > 0)  // 必須有資料
                {
                    if (RowIndex >= 0 && RowIndex < this.Rows.Count)
                    {
                        //ITableRow Row = Rows[RowIndex];
                        return Rows[RowIndex]?.State ?? TableRowState.Empty;
                    }
                }
                return TableRowState.Empty;
            }
        }

        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列, 取得指定欄位名稱的內容值.
        /// </summary>
        /// <typeparam name="T">內容值型的資料型別</typeparam>
        /// <param name="ColumnName">欄位名稱</param>
        /// <returns></returns>
        public T GetValue<T>(string ColumnName)
        {
            int index = Columns.GetIndexByName(ColumnName);
            if (index != -1)
            {
                if (!EOF && !BOF)
                    return CurretRow.GetValue<T>(index);
            }
            return default(T);
        }

        public T GetValue<T>(int index)
        {
            if (index != -1)
            {
                if (!EOF && !BOF)
                    return CurretRow.GetValue<T>(index);
            }
            return default(T);
        }

        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列, 取得指定欄位名稱的未編輯前的原始內容值.
        /// </summary>
        /// <typeparam name="VT"></typeparam>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public T GetOriginalValue<T>(string ColumnName)
        {
            int index = Columns.GetIndexByName(ColumnName);
            if (index != -1)
            {
                if (!EOF && !BOF)
                    return CurretRow.GetOriginalValue<T>(index);
            }
            return default(T);
        }
        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列, 取得指定欄位名稱的內容值是否己變更.
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public bool ValueChanged(string ColumnName)
        {
            if (!EOF && !BOF)
                return CurretRow.ValueChanged(ColumnName);
            return false;
        }

        /// <summary>
        ///  目前迭代器(Row Iterator)所指向的有效的資料列, 取得指定欄位名稱的內容值.
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public object GetValue(string ColumnName)
        {
            int index = Columns.GetIndexByName(ColumnName);
            if (index != -1)
            {
                if (!EOF && !BOF)
                    return CurretRow.GetValue(index);
            }
            return null;
        }

        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列, 取得指定欄位名稱的未編輯前的原始內容值.
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public object GetOriginalValue(string ColumnName)
        {
            int index = Columns.GetIndexByName(ColumnName);
            if (index != -1)
            {
                if (!EOF && !BOF)
                    return CurretRow.GetOriginalValue(index);
            }
            return null;
        }
        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列, 設定指定欄位名稱的的內容值.
        /// </summary>
        /// <typeparam name="VT"></typeparam>
        /// <param name="ColumnName"></param>
        /// <param name="value"></param>
        public void SetValue<T>(string ColumnName, T value)
        {
            int index = Columns.GetIndexByName(ColumnName);
            if (index != -1)
            {
                if (!EOF && !BOF)
                    CurretRow.SetValue<T>(index, value);
            }
        }
        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列, 設定指定欄位名稱的的內容值.
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="value"></param>
        public void SetValue(string ColumnName, object value)
        {
            int index = Columns.GetIndexByName(ColumnName);
            if (index != -1)
            {
                if (!EOF && !BOF)
                    CurretRow.SetValue(index, value);

            }
        }

        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列,是可在 BOF (Begin Of File) 下
        /// </summary>
        public bool BOF { get { return Rows.Count == 0 || RowIndex == -1; } }

        /// <summary>
        /// 目前迭代器(Row Iterator)所指向的有效的資料列,是可在 EOF (End Of File) 下
        /// </summary>
        public bool EOF { get { return RowIndex >= Rows.Count; } }


        /// <summary>
        /// 復制目前迭代器(Row Iterator)物件.
        /// </summary>
        /// <returns></returns>
        public RowIterator Copy()
        {
            return new RowIterator(Rows, RowIndex);
        }

        /// <summary>
        /// 比對前迭代器(Row Iterator)物件是否指向同一個資料列.
        /// </summary>
        /// <param name="iter"></param>
        /// <returns></returns>
        public bool IsEquals(RowIterator iter)
        {
            if (iter == null) return false;
            return (Rows.Equals(iter.Rows)) && (RowIndex == iter.RowIndex);
        }
    }
}
