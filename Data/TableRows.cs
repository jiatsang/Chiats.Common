using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chiats.Data
{

    /// <summary>
    /// 表格資料列集合
    /// </summary>
    [JsonConverter(typeof(TableRowsConverter))]
    public sealed class TableRows : IEnumerable<TableRow>
    {
        public TableRowColumns Columns { get; private set; }

        public List<TableRow> Rows { get; private set; } = new List<TableRow>();

        // 存放己過濾的資料. 
        private List<TableRow> FilterRows = new List<TableRow>();

        /// <summary>
        /// 表格資料列集合建構子
        /// </summary>
        /// <param name="Columns"></param>
        public TableRows(TableRowColumns Columns, List<object[]> rows = null)
        {
            this.Columns = Columns;
            if (rows != null)
            {
                foreach (var r in rows)
                {
                    // TODO: Check Columns and r 的一致性
                    if (r.Length == Columns.Count)
                    {
                        Rows.Add(new TableRow(Columns, r));
                    }
                    else
                        throw new CommonException("TableRows 資料列數對不上");
                }
            }
        }

        private ColumnFilter[] ColumnFilters = null;

        public void Filter(ColumnFilter[] ColumnFilters)
        {
            this.ColumnFilters = ColumnFilters;

            if (FilterRows == null)
                FilterRows = new List<TableRow>();
            else
                FilterRows.Clear();

            Debug.Write("Begin Column Filter ");
            foreach (var filter in ColumnFilters)
                Debug.Write($" filter({filter})");

            Debug.Write("\r\n");
            foreach (var tableRow in Rows)
            {

                bool FilterOk = true;
                foreach (var filter in ColumnFilters)
                {
                    if (filter.ColumnIndex != -1)
                    {
                        if (filter.Value != null)
                        {
                            if (!filter.Value.Equals(tableRow[filter.ColumnIndex]))
                            {
                                FilterOk = false;
                                break;
                            }
                        }
                        else if (tableRow[filter.ColumnIndex] != null)
                        {
                            FilterOk = false;
                            break;
                        }
                    }
                }
                if (FilterOk)
                {

                    // Dump Row
                    Debug.Write("FilterOk - ");
                    for (int i = 0; i < tableRow.Count; i++)
                    {
                        Debug.Write($"{tableRow.Columns[i].Name}({tableRow[i]}) ");
                    }
                    Debug.Write("\r\n");

                    FilterRows.Add(tableRow);
                }
            }
            Debug.Print("End Column Filter ");
        }

        public void Remove(int Index)
        {
            Rows.RemoveAt(Index);
        }

        public void Remove(TableRow Row)
        {
            if (ColumnFilters != null)
                FilterRows.Remove(Row);

            Rows.Remove(Row);
        }

        public void Add(TableRow Row)
        {
            // 由自己產生的資料列, 才能正確的加入. 產生的資料列的方法為 CreateNew().
            if (Row != null && Row.Columns == Columns)
            {
                Row.ApplyChanged();
                Rows.Add(Row);

                if (ColumnFilters != null)
                    FilterRows.Add(Row);

                //PageTable.RaiseRowChanged(new TableRowChangedEventArgs(TableRowType.Row));
            }
            else
            {
                Rows.Add(Row);
            }
        }

        /// <summary>
        /// 傳回指定列數的資料列. 索引值由 0 開始.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TableRow this[int index]
        {
            get
            {
                if (ColumnFilters != null)
                {
                    if (FilterRows.Count != 0 && index < FilterRows.Count)
                        return FilterRows[index];
                }
                else
                {
                    if (Rows.Count != 0 && index < Rows.Count)
                        return Rows[index];
                }
                return null;
            }
        }

        /// <summary>
        /// 建立一筆的空的資料列. 
        /// </summary>
        /// <returns></returns>
        public TableRow Create(object[] values = null)
        {
            TableRow NewRow = new TableRow(this.Columns, values);
            if (ColumnFilters != null)
                FilterRows.Add(NewRow);
            Rows.Add(NewRow);
            return NewRow;
        }

        /// <summary>
        /// 資料列筆數
        /// </summary>
        public int Count
        {
            get
            {
                if (ColumnFilters != null)
                    return FilterRows.Count;

                return Rows.Count;
            }
        }

        /// <summary>
        /// 清除所有資料列.
        /// </summary>
        public void Clear()
        {
            if (ColumnFilters != null)
            {
                ColumnFilters = null;
                FilterRows = null;
            }

            Rows.Clear();
        }

        /// <summary>
        /// 傳回會逐一查看集合的列舉程式。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TableRow> GetEnumerator()
        {
            if (ColumnFilters != null)
                return FilterRows.GetEnumerator();

            return Rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (ColumnFilters != null)
                return FilterRows.GetEnumerator();

            return Rows.GetEnumerator();
        }
    }
    public class TableRowsConverter : JsonConverter<TableRows>
    {
        public override TableRows Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {

            var packer = JsonSerializer.Deserialize<TableRowsPacker>(ref reader, options);

            return packer.Unpack();
            //  DateTimeOffset.ParseExact(reader.GetString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, TableRows rows, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new TableRowsPacker(rows), options);
            // writer.WriteStringValue(dateTimeValue.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
        }

    }

}
