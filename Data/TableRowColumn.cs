using Chiats.SQL;
using System;

namespace Chiats.Data
{

    public sealed class TableRowColumn
    {
        public TableRowColumn() { }

        public TableRowColumn(TableRowColumn column)
        {
            this.Name = column.Name;
            this.TableName = column.TableName;
            this.ColumnTypeInfo = column.ColumnTypeInfo;
        }

        public TableRowColumn(string Name, string TableName = null)
        {
            this.Name = Name;
            this.TableName = TableName;
        }

        public TableRowColumn(string Name, Type Type, string TableName = null) : this(Name, TableName)
        {
            this.ColumnTypeInfo = new ColumnTypeInfo(ColumnTypeHelper.ConvertType(Type));
        }
        

        public TableRowColumn(string Name, ColumnTypeInfo columnTypeInfo, string TableName = null)
            : this(Name, TableName)
        {
            this.ColumnTypeInfo = columnTypeInfo;
        }
        public TableRowColumn(string Name, ColumnType columnType, int Size, string TableName = null)
            : this(Name, TableName)
        {
            this.ColumnTypeInfo = new ColumnTypeInfo(columnType, Size);
        }

        public ColumnTypeInfo ColumnTypeInfo { get; set; }
        public string TableName { get; set; }
        public string Name { get; set; }
        public ColumnType ColumnType { get { return ColumnTypeInfo.ColumnType; } }
        public int Size { get { return ColumnTypeInfo.Size; } }
        public short NumericPrecision { get { return ColumnTypeInfo.NumericPrecision; } }
        public short NumericScale { get { return ColumnTypeInfo.NumericScale; } }

        public Type GetColumnType()
        {
            return ColumnTypeHelper.ConvertType(ColumnType);
        }

        public object ConvertValue(object val)
        {
            Type type = ColumnTypeHelper.ConvertType(ColumnType);
            if (type != null)
            {
                return val.ChangeTypeEx(type);
            }
            return val;
        }
        /// <summary>
        /// 傳回表示目前物件的字串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"TableColumn {Name} {ColumnType}({ColumnType},{Size},{NumericPrecision})";
        }
    }
}
