// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL
{
    public enum SQLProduct
    {
        MSSQL,
        Oracle
    }

    /// <summary>
    /// ColumnType ���U�{��
    /// </summary>
    public static class ColumnTypeHelper
    {
         // �إߤ@�M²���������W�h. �w�q ColumnType �����X.
        internal const int TypeVAR = 0x1000;

        internal const int TypeDoubleByte = 0x2000;
        internal const int TypeNumber = 0x0100;
        internal const int TypeDateTime = 0x0200;


        internal const int TypeChar = 0x0001;      
        internal const int TypeBinary = 0x0002;
        internal const int TypeText = 0x0003;
        internal const int TypeBool = 0x0004;
        internal const int TypeImage = 0x0005;
        internal const int TypeXml = 0x0006;
        internal const int TypeGuid = 0x0007;


        /// <summary>
        /// �ഫ�����쫬�O���{�����O
        /// </summary>
        /// <param name="ColumnType"></param>
        /// <returns></returns>
        public static Type ConvertType(ColumnType ColumnType)
        {
            switch (ColumnType)
            {
                case ColumnType.Decimal:
                    return typeof(decimal);

                case ColumnType.NChar:
                case ColumnType.Nvarchar:
                case ColumnType.Char:
                case ColumnType.Varchar:
                    return typeof(string);

                case ColumnType.Int:
                    return typeof(short);

                case ColumnType.Int32:
                    return typeof(int);

                case ColumnType.Int64:
                    return typeof(long);

                case ColumnType.Bool:
                    return typeof(bool);

                case ColumnType.Byte:
                    return typeof(Byte);

                case ColumnType.DateTime:
                    return typeof(DateTime);

                case ColumnType.Double:
                    return typeof(double);

                case ColumnType.Single:
                    return typeof(float);

                case ColumnType.Text:
                    return typeof(string);

                case ColumnType.Uniqueidentifier:
                    return typeof(Guid);

                default:
                    return typeof(object);
            }
        }


        public static ColumnType ConvertType(Type Type)
        {

            if (Type == typeof(decimal))
                return ColumnType.Decimal;

            if (Type == typeof(string))
                return ColumnType.Nvarchar;

            if (Type == typeof(int))
                return ColumnType.Int32;

            if (Type == typeof(short))
                return ColumnType.Int;

            if (Type == typeof(long))
                return ColumnType.Int64;

            if (Type == typeof(bool))
                return ColumnType.Bool;

            if (Type == typeof(byte))
                return ColumnType.Byte;


            if (Type == typeof(double))
                return ColumnType.Double;

            if (Type == typeof(float))
                return ColumnType.Single;

            if (Type == typeof(DateTime))
                return ColumnType.DateTime;

            if (Type == typeof(Guid))
                return ColumnType.Uniqueidentifier;

            return ColumnType.Auto;

            
        }

    /// <summary>
    /// �q�Ϊ� ColumnType �ഫ�{��, ��h�W�H SQL Server �����.
    /// </summary>
    /// <param name="ColumnTypeName"></param>
    /// <returns></returns>
    public static ColumnType ConvertColumnType(string ColumnTypeName, SQLProduct product = SQLProduct.MSSQL)
    {
        if (!string.IsNullOrEmpty(ColumnTypeName))
        {
            // TODO: �q�Ϊ� ColumnType �ഫ�{�ǡA�䴩 Oracle Database
            var _ColumnTypeName = ColumnTypeName.ToLower();
            switch (product)
            {
                case SQLProduct.MSSQL:
                    switch (_ColumnTypeName)
                    {
                        case "nvarchar":
                            return ColumnType.Nvarchar;

                        case "varchar":
                            return ColumnType.Varchar;

                        case "char":
                            return ColumnType.Char;

                        case "nchar":
                            return ColumnType.NChar;

                        case "money":
                        case "number":
                        case "numeric":
                        case "decimal":
                            return ColumnType.Decimal;

                        case "datetime":
                            return ColumnType.DateTime;

                        case "double":
                            return ColumnType.Double;

                        case "float":
                            return ColumnType.Single;

                        case "text":
                            return ColumnType.Text;

                        case "ntext":
                            return ColumnType.NText;

                        case "xml":
                            return ColumnType.XML;

                        case "smallint":
                        case "tinyint":
                            return ColumnType.Int;

                        case "int":
                            return ColumnType.Int32;

                        case "bigint":
                            return ColumnType.Int64;

                        case "bit":
                            return ColumnType.Bool;

                        case "image":
                            return ColumnType.Image;

                        case "binary":
                            return ColumnType.Binary;

                         case "uniqueidentifier":
                              return ColumnType.Uniqueidentifier;
                        }
                        break;
                case SQLProduct.Oracle:

                    // Oracle ������� SQL Server ������� ���N���
                    // BFILE	VARBINARY(MAX)	�O
                    // BLOB	VARBINARY(MAX)	�O
                    // CHAR([1-2000])	CHAR([1-2000])	�O
                    // CLOB	VARCHAR(MAX)	�O
                    // DATE	DATETIME	�O
                    // FLOAT	FLOAT	�_
                    // FLOAT([1-53])	FLOAT([1-53])	�_
                    // FLOAT([54-126])	FLOAT	�_
                    // INT	NUMERIC(38)	�O
                    // INTERVAL	DATETIME	�O
                    // LONG	VARCHAR(MAX)	�O
                    // LONG RAW	IMAGE	�O
                    // NCHAR([1-1000])	NCHAR([1-1000])	�_
                    // NCLOB	NVARCHAR(MAX)	�O
                    // NUMBER	FLOAT	�O
                    // NUMBER([1-38])	NUMERIC([1-38])	�_
                    // NUMBER([0-38],[1-38])	NUMERIC([0-38],[1-38])	�O
                    // NVARCHAR2([1-2000])	NVARCHAR([1-2000])	�_
                    // RAW([1-2000])	VARBINARY([1-2000])	�_
                    // real	FLOAT	�_
                    // ROWID	CHAR(18)	�_
                    // TIMESTAMP	DATETIME	�O
                    // TIMESTAMP(0-7)	DATETIME	�O
                    // TIMESTAMP(8-9)	DATETIME	�O
                    // TIMESTAMP(0-7) WITH TIME ZONE	VARCHAR(37)	�O
                    // TIMESTAMP(8-9) WITH TIME ZONE	VARCHAR(37)	�_
                    // TIMESTAMP(0-7) WITH LOCAL TIME ZONE	VARCHAR(37)	�O
                    // TIMESTAMP(8-9) WITH LOCAL TIME ZONE	VARCHAR(37)	�_
                    // UROWID	CHAR(18)	�_
                    // VARCHAR2([1-4000])	VARCHAR([1-4000])	�O

                    switch (_ColumnTypeName)
                    {
                        case "nvarchar":
                        case "nvarchar2":
                            return ColumnType.Nvarchar;

                        case "varchar":
                        case "varchar2":
                            return ColumnType.Varchar;

                        case "char":
                            return ColumnType.Char;

                        case "nchar":
                            return ColumnType.NChar;

                        case "decimal":
                        case "numeric":
                            return ColumnType.Decimal;

                        case "date":
                            return ColumnType.DateTime;

                        case "double":
                            return ColumnType.Double;

                        case "float":
                            return ColumnType.Single;

                        case "text":
                            return ColumnType.Text;

                        case "ntext":
                            return ColumnType.NText;

                        case "xml":
                            return ColumnType.XML;

                        case "smallint":
                        case "tinyint":
                            return ColumnType.Int;

                        case "int":
                            return ColumnType.Int32;

                        case "bigint":
                            return ColumnType.Int64;

                        case "bit":
                            return ColumnType.Bool;

                        case "long raw":
                            return ColumnType.Image;

                        case "binary":
                            return ColumnType.Binary;
                    }
                    break;
            }


        }
        return ColumnType.UnKnown;
    }

    public static string ConvertColumnType(ColumnType dbtype, SQLProduct product = SQLProduct.MSSQL)
    {
        switch (product)
        {
            case SQLProduct.MSSQL:
                switch (dbtype)
                {
                    case ColumnType.Varbinary: return "varbinary";
                    case ColumnType.Binary: return "binary";
                    case ColumnType.Text: return "text";
                    case ColumnType.Varchar: return "varchar";
                    case ColumnType.Char: return "char";
                    case ColumnType.NChar: return "nchar";
                    case ColumnType.Nvarchar: return "nvarchar";
                    case ColumnType.Decimal: return "decimal";
                    case ColumnType.Image: return "image";
                    case ColumnType.Int32: return "int";
                    case ColumnType.Int64: return "bigint";
                    case ColumnType.DateTime: return "datetime";
                    case ColumnType.Int: return "smallint";
                    case ColumnType.Byte: return "tinyint";
                    default:
                        throw new NotImplementedException($"{dbtype} Not NotImplemented");
                }
            case SQLProduct.Oracle:
                switch (dbtype)
                {
                    case ColumnType.Varbinary: return "varbinary";
                    case ColumnType.Binary: return "binary";
                    case ColumnType.Text: return "text";
                    case ColumnType.Varchar: return "varchar2";
                    case ColumnType.Char: return "char";
                    case ColumnType.NChar: return "nchar";
                    case ColumnType.Nvarchar: return "nvarchar2";
                    case ColumnType.Decimal: return "decimal";
                    case ColumnType.Image: return "long raw";
                    case ColumnType.Int32: return "int";
                    case ColumnType.Int64: return "long";
                    case ColumnType.DateTime: return "date";
                    case ColumnType.Int: return "smallint";
                    case ColumnType.Byte: return "tinyint";
                    default:
                        throw new NotImplementedException($"{dbtype} Not NotImplemented");
                }
            default:
                throw new NotImplementedException($"{product} Not NotImplemented");
        }
    }

    /// <summary>
    /// �O�_���ƭȪ���쫬�O Int64,Int32,Decimal.Single.Double ����.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNumber(ColumnType type)
    {
        return (((int)type) & TypeNumber) != 0;
    }

    /// <summary>
    /// �O�_���r������쫬�O , �]�tVarChar , Char , NVarChar , NChar ����.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsChar(ColumnType type)
    {
        return (((int)type) & TypeChar) != 0;
    }
}
}