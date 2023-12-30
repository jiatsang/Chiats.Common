using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Chiats.Data
{
    /// <summary>
    /// 表格資料列
    /// </summary>
    [JsonConverter(typeof(TableRowConverter))]
    public sealed class TableRow
    {
        public TableRowColumns Columns { get; private set; }

        private object[] data = null;
        private object[] modify_data = null;

        public object[] GetValues() { return data; }
        public object[] GetModifyValues() { return modify_data; }

        /// <summary>
        /// 表格資料列建構子
        /// </summary>
        /// <param name="PageTable"></param>
        public TableRow(TableRowColumns Columns, object[] vals)
        {
            this.Columns = Columns;
            if (vals != null)
            {
                Debug.Assert(vals.Length == Columns.Count, "資料的欄位數必須相同");
                data = vals;
            }
            else
            {
                data = new object[Columns.Count];
            }

            State = TableRowState.Newer;
        }

        /// <summary>
        /// 支援匿名類或 class 產生 TableRow
        /// </summary>
        /// <param name="obj"></param>
        public TableRow(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
            var type = obj.GetType();

            List<TableRowColumn> columns = new List<TableRowColumn>();
            List<object> values = new List<object>();
         
            //    Columns = new TableColumns();
            foreach (var p in type.GetProperties())
            {
                columns.Add(new TableRowColumn(p.Name, p.PropertyType));
                values.Add(p.GetValue(obj));
            }
            this.Columns = new TableRowColumns(columns.ToArray());
            this.data = values.ToArray();

            State = TableRowState.Newer;
        }

        public void CopyFrom(TableRow Row)
        {
            this.data = Row.data;
        }
        ///// <summary>
        ///// 表格資料列建構子
        ///// </summary>
        ///// <param name="PageTable"></param>
        ///// <param name="row"></param>
        //public TableRow(PageTable PageTable, TableRow row)
        //{
        //    this.PageTable = PageTable;
        //    Initiailize();

        //    for (int i = 0; i < PageTable.Columns.Count; i++) data[i] = row.data[i];

        //    RowState = RowState.Newer;
        //}


        /// <summary>
        /// 資料列的資料狀態
        /// </summary>
        public TableRowState State { get; private set; }

        /// <summary>
        /// 回傳是否有包含指定的欄位名稱
        /// </summary>
        /// <param name="name">指定的欄位名稱</param>
        /// <returns></returns>
        public bool ContainKey(string name)
        {
            return Columns.GetIndexByName(name) != -1;
        }

        /// <summary>
        /// 啟用編輯的資料緩衝區
        /// </summary>
        public void EnableUpdateBuffer()
        {
            if (State == TableRowState.Normal)
            {
                modify_data = new object[data.Length];

                for (int i = 0; i < data.Length; i++)
                {
                    modify_data[i] = data[i];
                }

                State = TableRowState.EnableUpdateBuffer;
                //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.Row));
            }
        }

        /// <summary>
        /// 取消所有的異動資料
        /// </summary>
        public void CancelChanged()
        {
            if (State == TableRowState.Newer)
            {
                State = TableRowState.Empty;
                //PageTable.Rows.Remove(this);
                //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.RowDeleted));
            }
            else if (State == TableRowState.EnableUpdateBuffer)
            {
                modify_data = null;  // Clear Buffer
                State = TableRowState.Normal;
                //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.Row));
            }
        }

        /// <summary>
        /// 確認所有的異動資料
        /// </summary>
        public void ApplyChanged()
        {
            if (State == TableRowState.EnableUpdateBuffer)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = modify_data[i];
                }
                modify_data = null;
                State = TableRowState.Normal;
                //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.Row));
            }
            else if (State == TableRowState.Newer)
            {
                State = TableRowState.Normal;
                //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.Row));
            }
        }
        /// <summary>
        /// 回傳指定欄位名稱的索引位置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIndexByName(string name)
        {
            return Columns.GetIndexByName(name);
        }

        /// <summary>
        /// 傳回 欄位值是否有變更(異動)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ValueChanged(string name)
        {
            if (State == TableRowState.EnableUpdateBuffer)
            {
                int index = GetIndexByName(name);
                if (index != -1 && modify_data[index] != null && data[index] != null)
                {
                    // 相同的型別 才需要進行資料內容比對
                    if (modify_data[index].GetType() == data[index].GetType())
                        return !(modify_data[index].Equals(data[index]));
                }
                if (index != -1 && modify_data[index] == null && data[index] == null)
                {
                    return false; // 欄位不存在, 一律傳回 false
                }
                return true; // 非相同的型別或有其中一個 null 時.
            }
            else if (State == TableRowState.Newer)
            {
                return true;
            }
            return false; // 非 EnableModifyBuffer 模式下, 一律傳回 false
        }


        /// <summary>
        /// 設定欄位內容值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValue(int index, object value)
        {
            if (State == TableRowState.Newer)
                data[index] = Columns[index].ConvertValue(value);
            else if (State == TableRowState.EnableUpdateBuffer)
            {
                modify_data[index] = Columns[index].ConvertValue(value);

            }
            //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.ColumnValue, PageTable.Columns[index].Name));
        }

        public void SetValue(string name, object value)
        {
            int index = Columns.GetIndexByName(name);
            if (index != -1)
            {
                if (State == TableRowState.Newer)
                    data[index] = Columns[index].ConvertValue(value);
                else if (State == TableRowState.EnableUpdateBuffer)
                {
                    modify_data[index] = Columns[index].ConvertValue(value);

                }
            }
            //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.ColumnValue, PageTable.Columns[index].Name));
        }
        /// <summary>
        /// 設定欄位內容值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValue<T>(int index, T value)
        {
            if (State == TableRowState.Newer)
                data[index] = Columns[index].ConvertValue(value);
            else if (State == TableRowState.EnableUpdateBuffer)
                modify_data[index] = Columns[index].ConvertValue(value);

            //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.ColumnValue, PageTable.Columns[index].Name));
        }
        /// <summary>
        /// 設定欄位內容值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue<T>(string name, T value)
        {
            int index = Columns.GetIndexByName(name);
            if (index != -1)
            {
                if (State == TableRowState.Newer)
                    data[index] = Columns[index].ConvertValue(value);
                else if (State == TableRowState.EnableUpdateBuffer)
                    modify_data[index] = Columns[index].ConvertValue(value);

                //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.ColumnValue, name));
            }
        }
        /// <summary>
        /// 傳回欄位內容值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(string name)
        {

            int index = Columns.GetIndexByName(name);
            if (index != -1)
            {
                if (State == TableRowState.EnableUpdateBuffer)
                    return modify_data[index];
                //if (RowState == RowState.Newer )
                //    return modify_data[index];
                return data[index];
            }
            return null;
        }

        /// <summary>
        /// 取得未編輯前的資料 , 僅 EnableModifyBuffer 有作用. 其他狀況下和 GetValue 相同.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetOriginalValue(string name)
        {
            int index = Columns.GetIndexByName(name);
            if (index != -1)
            {
                return data[index];
            }
            return null;
        }

        /// <summary>
        /// 取得未編輯前的資料, 僅 EnableModifyBuffer 有作用. 其他狀況下和 GetValue 相同.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetOriginalValue(int index)
        {
            return data[index];
        }

        public object GetValue(int index)
        {
            if (State == TableRowState.EnableUpdateBuffer)
                return modify_data[index];
            return data[index];
        }

        /// <summary>
        /// 取得未編輯前的資料, 僅 EnableModifyBuffer 有作用. 其他狀況下和 GetValue 相同.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetOriginalValue<T>(int index)
        {
            return CommonExtensions.ChangeType<T>(data[index]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetValue<T>(int index)
        {
            if (State == TableRowState.EnableUpdateBuffer)
                return CommonExtensions.ChangeType<T>(modify_data[index]);
            return CommonExtensions.ChangeType<T>(data[index]);
        }

        /// <summary>
        /// 取得未編輯前的資料, 僅 EnableModifyBuffer 有作用. 其他狀況下和 GetValue 相同.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetOriginalValue<T>(string name)
        {
            int index = Columns.GetIndexByName(name);
            if (index != -1)
            {
                return CommonExtensions.ChangeType<T>(data[index]);
            }
            return default(T);
        }

        public T GetValue<T>(string name)
        {
            int index = Columns.GetIndexByName(name);
            if (index != -1)
            {
                if (State == TableRowState.EnableUpdateBuffer)
                    return CommonExtensions.ChangeType<T>(modify_data[index]);
                return CommonExtensions.ChangeType<T>(data[index]);
            }
            return default(T);
        }

        public object this[int index]
        {
            get
            {
                return GetValue(index);
            }
            set
            {
                SetValue(index, value);
            }
        }

        public int Count
        {
            get
            {
                if (data != null)
                    return data.Length;
                return -1;
            }
        }

        /// <summary>
        /// 傳回表示目前物件的字串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (data != null)
                return string.Format("object[{0}]", data.Length);
            return "null";
        }


    }
}
