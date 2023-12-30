using Chiats.SQL;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Chiats.Data
{
    internal class TableRowsPacker
    {
        public class RowColumn
        {
            public string Name { get; set; }
            public string TableName { get; set; }
            public string ColumnType { get; set; }
        }
        public List<RowColumn> Columns { get; set; }
        public List<object[]> Rows { get; set; }
        public TableRowsPacker() { }

        public TableRows Unpack()
        {

            List<TableRowColumn> TableColumns = new List<TableRowColumn>();
            foreach (var col in Columns)
            {
                TableColumns.Add(new TableRowColumn(col.Name, new ColumnTypeInfo(col.ColumnType) , col.TableName) );
            }

            // Convert 
            foreach (object[] row in Rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] is JsonElement element)
                    {
                        //var column = TableColumns[i];
                        switch (element.ValueKind)
                        {
                            case JsonValueKind.String:
                                row[i] = element.GetString();
                                break;
                            case JsonValueKind.Null:
                                row[i] = null;
                                break;
                            case JsonValueKind.True:
                                row[i] = true;
                                break;
                            case JsonValueKind.False:
                                row[i] = false;
                                break;
                            case JsonValueKind.Number:
                                var column = TableColumns[i];
                                var numType = column.GetColumnType();

                                switch (Type.GetTypeCode(numType))
                                {
                                    case TypeCode.Decimal: row[i] = element.GetDecimal(); break;
                                    case TypeCode.Int32: row[i] = element.GetInt32(); break;
                                    case TypeCode.UInt32: row[i] = element.GetUInt32(); break;
                                    case TypeCode.Double: row[i] = element.GetDouble(); break;
                                    case TypeCode.Single: row[i] = element.GetSingle(); break;
                                    case TypeCode.Int64: row[i] = element.GetInt64(); break;
                                    case TypeCode.UInt64: row[i] = element.GetUInt64(); break;
                                    case TypeCode.Int16: row[i] = element.GetInt16(); break;
                                    case TypeCode.UInt16: row[i] = element.GetUInt16(); break;
                                    case TypeCode.Byte: row[i] = element.GetByte(); break;
                                    case TypeCode.SByte: row[i] = element.GetSByte(); break;
                                    default:
                                        throw new NotImplementedException($"未實作轉換型別 {numType.Name}");
                                }
                                break;
                        }
                    }
                }
            }
            TableRows rows = new TableRows(new TableRowColumns(TableColumns.ToArray()), Rows);
            return rows;
        }

        public TableRowsPacker(TableRows tableRows)
        {
            Columns = new List<RowColumn>();
            Rows = new List<object[]>();
            foreach (var col in tableRows.Columns)
            {
                Columns.Add(new RowColumn { 
                    Name = col.Name, 
                    ColumnType = col.ColumnTypeInfo?.ToString(),
                    TableName = col.TableName
                });
            }
            foreach (var row in tableRows)
            {
                Rows.Add(row.GetValues());
            }
        }
    }

}
