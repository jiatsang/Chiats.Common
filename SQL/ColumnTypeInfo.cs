// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using Chiats.Data;

namespace Chiats.SQL
{
    /// <summary>
    /// �������T. �]�t���O(ColumnType)�M���j�p��T(Length and Scale)
    /// �M��Ʈw�t�Ӫ����L������T(���A,�j�p), �������T
    /// �M��ƪ������������A�ഫ�h�� IDBInformation �����B�z.
    /// </summary>
    public class ColumnTypeInfo
    {
   
        /// <summary>
        /// �������T�غc�l
        /// </summary>
        public ColumnTypeInfo() : this(ColumnType.Auto, 0, 0, 0, true) { }

        /// <summary>
        /// �������T�غc�l
        /// </summary>
        /// <param name="ColumnType">�����쫬�O</param>
        public ColumnTypeInfo(ColumnType ColumnType) : this(ColumnType, 0, 0, 0, true) { }

        /// <summary>
        /// �������T�غc�l
        /// </summary>
        /// <param name="ColumnType">�����쫬�O</param>
        /// <param name="Size">���������</param>
        public ColumnTypeInfo(ColumnType ColumnType, int Size) : this(ColumnType, Size, 0, 0, true) { }

        /// <summary>
        /// �������T�غc�l
        /// </summary>
        /// <param name="ColumnType">�����쫬�O</param>
        /// <param name="Size">���������</param>
        /// <param name="NumericPrecision">��쪺�Ʀr��Ʀ��</param>
        /// <param name="NumericScale">���p�Ƴ��������</param>
        public ColumnTypeInfo(ColumnType ColumnType, int Size, short NumericPrecision, short NumericScale) : this(ColumnType, Size, NumericPrecision, NumericScale, true) { }

        /// <summary>
        /// �������T�غc�l
        /// </summary>
        /// <param name="ColumnType">�����쫬�O</param>
        /// <param name="Size">���������</param>
        /// <param name="NumericPrecision">��쪺�Ʀr��Ʀ��</param>
        /// <param name="NumericScale">���p�Ƴ��������</param>
        /// <param name="Nullable">���O�_���\�� Nullable</param>
        public ColumnTypeInfo(ColumnType ColumnType, int Size, short NumericPrecision, short NumericScale, bool Nullable)
        {
            this.ColumnType = ColumnType;
            this.Size = Size;
            this.NumericPrecision = NumericPrecision;
            this.NumericScale = NumericScale;
            this.Nullable = Nullable;
        }

        /// <summary>
        /// �������T�غc�l
        /// </summary>
        /// <param name="ColumnDescrtption">�������T�y�z�r�� Example : columnTypeName(m,n)</param>
        public ColumnTypeInfo(string ColumnDescrtption, bool ThrowException = true)
        {
            InitiailizeColumnDescrtption(ColumnDescrtption, ThrowException);
        }

        public ColumnTypeInfo(SchemaTableRow row)
        {

            this.ColumnType = row.DataTypeName.EnumConvert<ColumnType>(ColumnType.UnKnown);
            this.Size = row.ColumnSize;
            this.NumericPrecision = (short) row.NumericPrecision;
            this.NumericScale = (short) row.NumericScale;
            this.Nullable = row.AllowDBNull;
        }

        public static ColumnTypeInfo ColumnTypeInfoVaildate(string ColumnDescrtption, bool ThrowException)
        {
            ColumnTypeInfo NewInfo = new ColumnTypeInfo();
            SqlModelException ex = ColumnTypeInfoVaildate(ColumnDescrtption, NewInfo, ThrowException);
            if (ex == null)
                return NewInfo;
            return null;
        }

        public static SqlModelException ColumnTypeInfoVaildate(string ColumnDescrtption, ColumnTypeInfo NewInfo, bool ThrowException)
        {
            TokenScanner tc = new CommonTokenScanner(ColumnDescrtption, "()<>!@#$%^&*-=+{}:;,?/\\~|.");
            if (tc.Count == 1)
            {
                string ColumnTypeName = tc[0].String;

                if (ColumnTypeName == "bigint") ColumnTypeName = "Int64";

                NewInfo.ColumnType = ColumnTypeName.EnumConvert<ColumnType>(ColumnType.UnKnown);
                if (ThrowException && NewInfo.ColumnType == ColumnType.UnKnown)
                {
                    return new SqlModelException("ColumnType Not Found {0}", ColumnTypeName);
                }
                return null;
            }
            if (tc.Count == 4)  // name(10)
            {
                if (tc[1].String == "(" && tc[3].String == ")")
                {
                    string ColumnTypeName = tc[0].String;
                    if (tc[2].Type == TokenType.Number)
                    {
                        NewInfo.Size = int.Parse(tc[2].String);
                        NewInfo.ColumnType = ColumnTypeName.EnumConvert<ColumnType>(ColumnType.UnKnown);
                        if (ThrowException && NewInfo.ColumnType == ColumnType.UnKnown)
                        {
                            return new SqlModelException("ColumnType Not Found {0} in {1}", ColumnTypeName, ColumnDescrtption);
                        }
                        return null;
                    }
                }
            }
            if (tc.Count == 6)  // name(1,2)
            {
                if (tc[1].String == "(" && tc[5].String == ")" && tc[3].String == ",")
                {
                    string ColumnTypeName = tc[0].String;
                    if (tc[2].Type == TokenType.Number)
                    {
                        NewInfo.NumericPrecision = (short)int.Parse(tc[2].String);
                        NewInfo.NumericScale = (short)int.Parse(tc[4].String);
                        NewInfo.ColumnType = ColumnTypeName.EnumConvert<ColumnType>(ColumnType.UnKnown);
                        if (ThrowException && NewInfo.ColumnType == ColumnType.UnKnown)
                        {
                            return new SqlModelException("ColumnType Not Found {0} in {1}", ColumnTypeName, ColumnDescrtption);
                        }
                        return null;
                    }
                }
            }
            return new SqlModelException("ColumnType Description Format Error : {0}", ColumnDescrtption);
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        private void InitiailizeColumnDescrtption(string ColumnDescrtption, bool ThrowException)
        {
            this.Size = 0;
            this.NumericPrecision = 0;
            this.NumericScale = 0;

            CommonException ex = ColumnTypeInfoVaildate(ColumnDescrtption, this, ThrowException);
            if (ex != null) throw ex;
        }

        /// <summary>
        /// �����쫬�O
        /// </summary>
        public ColumnType ColumnType { get; protected set; }

        /// <summary>
        /// ���������
        /// </summary>
        public int Size { get; protected set; }

        /// <summary>
        /// ���p�Ƴ��������(���ƫ��O�� Decimal ��)
        /// </summary>
        public short NumericScale { get; protected set; }

        /// <summary>
        /// ��쪺�Ʀr��Ʀ��(���ƫ��O�� Decimal ��) NumericPrecision
        /// </summary>
        public short NumericPrecision { get; protected set; }

        /// <summary>
        /// ���O�_���\�� Nullable
        /// </summary>
        public bool Nullable { get; protected set; }

        internal void SetColumnTypeInfo(ColumnType ColumnType, int Size, short Precision, short Scale)
        {
            this.ColumnType = ColumnType;
            this.Size = Size;
            this.NumericPrecision = Precision;
            this.NumericScale = Scale;
        }

        internal void SetColumnType(ColumnType ColumnType)
        {
            this.ColumnType = ColumnType;
        }

        internal void SetColumnLength(int Length)
        {
            this.Size = Length;
        }

        internal void SetColumnScale(short Scale)
        {
            this.NumericScale = Scale;
        }

        internal void SetColumnPrecision(short Precision)
        {
            this.NumericPrecision = Precision;
        }

        internal void SetColumnNullable(bool Nullable)
        {
            this.Nullable = Nullable;
        }

        /// <summary>
        /// ���o�����ܪ��̨Φr���Ӽ�.
        /// </summary>
        /// <returns></returns>
        public int GetDisplayWordCount(int DisplayCharMaxCount = 0)
        {
            int DisplayCount = DisplayCharMaxCount;
            if (DisplayCount == 0)
            {
                // �۰ʨ���쫬�O��
                switch (this.ColumnType)
                {
                    case ColumnType.Char:
                    case ColumnType.NChar:
                    case ColumnType.Varchar:
                    case ColumnType.Nvarchar:
                        if (Size > 255) return 255;  // UI �i�s�誺�̤j�r����.
                        return Size;

                    case ColumnType.Decimal:
                        return 12;

                    case ColumnType.Int:
                    case ColumnType.Int32:
                    case ColumnType.Int64:
                        return 10;

                    default:
                        return 20;
                }
            }
            return DisplayCount;
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (ColumnType)
            {
                case ColumnType.Binary:
                case ColumnType.Varbinary:
                case ColumnType.Varchar:
                case ColumnType.Nvarchar:
                case ColumnType.Char:
                case ColumnType.NChar:
                case ColumnType.Byte:
                    return $"{ColumnType}({Size})".ToLower();

                case ColumnType.Decimal:
                    return $"{ColumnType}({NumericPrecision},{NumericScale})".ToLower();

                case ColumnType.Int64:
                    return "bigint";

                default:
                    return ColumnType.ToString().ToLower();
            }
        }

        ///// <summary>
        /////  �ݩʳB�z�����ഫ�ݩʦr��
        ///// </summary>
        ///// <returns>�ݩʦr��</returns>
        //public PropertyValue GetPropertyValue()
        //{
        //    return new PropertyValue(ToString());
        //}

        /// <summary>
        /// ����B�� Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// ����B�� ==
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        public static bool operator ==(ColumnTypeInfo col1, ColumnTypeInfo col2)
        {
            bool col1_isnull = object.ReferenceEquals(col1, null);
            bool col2_isnull = object.ReferenceEquals(col2, null);

            if (col1_isnull && col2_isnull) return true;
            if (col1_isnull || col2_isnull) return false;

            return (col1.ColumnType == col2.ColumnType &&
                col1.Size == col2.Size &&
                col1.NumericPrecision == col2.NumericPrecision &&
                col1.NumericScale == col2.NumericScale);
        }

        /// <summary>
        /// ����B�� !=
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        public static bool operator !=(ColumnTypeInfo col1, ColumnTypeInfo col2)
        {
            bool col1_isnull = object.ReferenceEquals(col1, null);
            bool col2_isnull = object.ReferenceEquals(col2, null);

            if (col1_isnull && col2_isnull) return false;
            if (col1_isnull || col2_isnull) return true;

            return (col1.ColumnType != col2.ColumnType ||
                col1.Size != col2.Size ||
                col1.NumericPrecision != col2.NumericPrecision ||
                col1.NumericScale != col2.NumericScale);
        }

        /// <summary>
        /// �w�]����禡
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}