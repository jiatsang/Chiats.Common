using Chiats.SQL;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chiats.Data
{
    public class TableRowConverter : JsonConverter<TableRow>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsSubclassOf(typeof(TableRow)))
            {
                return true;
            }
            return base.CanConvert(typeToConvert);
        }
        public override TableRow Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var packer = JsonSerializer.Deserialize<TableRowPacker>(ref reader, options);
            return packer.Unpack();
        }

        public override void Write(Utf8JsonWriter writer, TableRow rows, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new TableRowPacker(rows), options);
        }

    }

    internal class TableRowPacker
    {
        public class RowColumn
        {
            public string Name { get; set; }
            public string TableName { get; set; }
            public string ColumnType { get; set; }
        }
        public List<RowColumn> Columns { get; set; }
        public object[] Row { get; set; }
        public TableRowPacker() { }

        public TableRow Unpack()
        {

            List<TableRowColumn> TableColumns = new List<TableRowColumn>();
            foreach (var col in Columns)
            {
                TableColumns.Add(new TableRowColumn(col.Name, new ColumnTypeInfo(col.ColumnType), col.TableName));
            }

            // Convert 

            for (int i = 0; i < Row.Length; i++)
            {
                if (Row[i] is JsonElement element)
                {
                    //var column = TableColumns[i];
                    switch (element.ValueKind)
                    {
                        case JsonValueKind.String:
                            Row[i] = element.GetString();
                            break;
                        case JsonValueKind.Null:
                            Row[i] = null;
                            break;
                        case JsonValueKind.True:
                            Row[i] = true;
                            break;
                        case JsonValueKind.False:
                            Row[i] = false;
                            break;
                        case JsonValueKind.Number:
                            var column = TableColumns[i];
                            var numType = column.GetColumnType();

                            switch (Type.GetTypeCode(numType))
                            {
                                case TypeCode.Decimal: Row[i] = element.GetDecimal(); break;
                                case TypeCode.Int32: Row[i] = element.GetInt32(); break;
                                case TypeCode.UInt32: Row[i] = element.GetUInt32(); break;
                                case TypeCode.Double: Row[i] = element.GetDouble(); break;
                                case TypeCode.Single: Row[i] = element.GetSingle(); break;
                                case TypeCode.Int64: Row[i] = element.GetInt64(); break;
                                case TypeCode.UInt64: Row[i] = element.GetUInt64(); break;
                                case TypeCode.Int16: Row[i] = element.GetInt16(); break;
                                case TypeCode.UInt16: Row[i] = element.GetUInt16(); break;
                                case TypeCode.Byte: Row[i] = element.GetByte(); break;
                                case TypeCode.SByte: Row[i] = element.GetSByte(); break;
                                default:
                                    throw new NotImplementedException($"未實作轉換型別 {numType.Name}");
                            }
                            break;
                    }
                }
            }

            return new TableRow(new TableRowColumns(TableColumns.ToArray()), Row);
        }

        public TableRowPacker(TableRow tableRow)
        {
            Columns = new List<RowColumn>();
            foreach (var col in tableRow.Columns)
            {
                Columns.Add(new RowColumn
                {
                    Name = col.Name,
                    ColumnType = col.ColumnTypeInfo?.ToString(),
                    TableName = col.TableName
                });
            }
            Row = tableRow.GetValues();

        }
    }
}
