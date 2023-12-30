using System;

namespace Chiats.Data
{
    /// <summary>
    /// Suppory DataTableReader.GetSchemaTable Method ()
    /// </summary>
    public class SchemaTableRow
    {
        /// <summary>
        /// The name of the column as it appears in the DataTable.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The ordinal of the column
        /// </summary>
        public int ColumnOrdinal { get; set; }

        /// <summary>
        /// -1 if the ColumnSize (or MaxLength) property of the DataColumn cannot be determined or is not relevant; otherwise, 0 or a positive integer that contains the MaxLength value.
        /// </summary>
        public int ColumnSize { get; set; }

        /// <summary>
        /// If the column type is a numeric type, this is the maximum precision of the column. If the column type is not a numeric data type, this is a null value.
        /// </summary>
        public int NumericPrecision { get; set; }

        /// <summary>
        /// If column data type has a scale component, return the number of digits to the right of the decimal point. Otherwise, return a null value.
        /// </summary>
        public int NumericScale { get; set; }

        /// <summary>
        /// The underlying type of the column.
        /// </summary>
        //public Type DataType { get; set; }

        /// <summary>
        /// The indicator of the column's data type. If the data type of the column varies from row to row, this value is Object. This column cannot contain a null value.
        /// </summary>
        public int ProviderType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DataTypeName { get; set; }

        /// <summary>
        /// 資料表或檢視中包含之資料行的資料存放區的名稱。 如果無法判斷基底資料表名稱，則為 null 值。 此資料行的預設值是 null 值。
        /// </summary>
        public string BaseTableName { get; set; }

        /// <summary>
        /// true if the data type of the column is String and its MaxLength property is -1. Otherwise, false.
        /// </summary>
        public bool IsLong { get; set; }

        public bool IsAliased { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsExpression { get; set; }

        /// <summary>
        /// true if the AllowDbNull constraint is set to true for the column; otherwise, false.
        /// </summary>
        public bool AllowDBNull { get; set; }

        /// <summary>
        /// true if the column cannot be modified; otherwise false.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// false, for every column.
        /// </summary>
        public bool IsRowVersion { get; set; }

        /// <summary>
        /// true: No two rows in the DataTable can have the same value in this column.IsUnique is guaranteed to be true if the column represents a key by itself or if there is a constraint of type UNIQUE that applies only to this column. false: The column can contain duplicate values in the DataTable. The default of this column is false.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// true: The column is one of a set of columns that, taken together, uniquely identify the row in the DataTable. The set of columns with IsKey set to true must uniquely identify a row in the DataTable. There is no requirement that this set of columns is a minimal set of columns.This set of columns may be generated from a DataTable primary key, a unique constraint or a unique index. false: The column is not required to uniquely identify the row.This value is true if the column participates in a single or composite primary key. Otherwise, its value is false.
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// true: The column assigns values to new rows in fixed increments. false: The column does not assign values to new rows in fixed increments.The default of this column is false.
        /// </summary>
        public bool IsAutoIncrement { get; set; }

        /// <summary>
        /// The name of the catalog in the data store that contains the column.Null if the base catalog name cannot be determined.The default value for this column is a null value.
        /// </summary>
        public string BaseCatalogName { get; set; }

        /// <summary>
        /// The name of the column in the DataTable.
        /// </summary>
        public string BaseColumnName { get; set; }

        /// <summary>
        /// The value of the DataTable's AutoIncrementSeed property.
        /// </summary>
        public int AutoIncrementSeed { get; set; }

        /// <summary>
        /// The value of the DataTable's AutoIncrementStep property.
        /// </summary>
        public int AutoIncrementStep { get; set; }

        /// <summary>
        /// The value of the DataColumn's DefaultValue property.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// The expression string, if the current column is an expression column and all columns used in the expression belong to the same T:System.Data.DataTable that contains the expression column; otherwise null.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The MappingType value associated with the DataColumn.The type can be one of Attribute, Element, Hidden, or SimpleContent. The default value is Element.
        /// </summary>
        public string ColumnMapping { get; set; }

        /// <summary>
        /// The value of the DataTable's Namespace property.
        /// </summary>
        public string BaseTableNamespace { get; set; }

        /// <summary>
        /// The value of the DataColumn's Namespace property.
        /// </summary>
        public string BaseColumnNamespace { get; set; }

        public override string ToString()
        {
            return $"{ColumnName} - {DataTypeName}";
        }
    }
}