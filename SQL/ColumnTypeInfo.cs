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
    /// 資料欄位資訊. 包含型別(ColumnType)和欄位大小資訊(Length and Scale)
    /// 和資料庫廠商版本無關欄位資訊(型態,大小), 資料欄位資訊
    /// 和資料版本相關的型態轉換則由 IDBInformation 介面處理.
    /// </summary>
    public class ColumnTypeInfo
    {
   
        /// <summary>
        /// 資料欄位資訊建構子
        /// </summary>
        public ColumnTypeInfo() : this(ColumnType.Auto, 0, 0, 0, true) { }

        /// <summary>
        /// 資料欄位資訊建構子
        /// </summary>
        /// <param name="ColumnType">資料欄位型別</param>
        public ColumnTypeInfo(ColumnType ColumnType) : this(ColumnType, 0, 0, 0, true) { }

        /// <summary>
        /// 資料欄位資訊建構子
        /// </summary>
        /// <param name="ColumnType">資料欄位型別</param>
        /// <param name="Size">資料欄位長度</param>
        public ColumnTypeInfo(ColumnType ColumnType, int Size) : this(ColumnType, Size, 0, 0, true) { }

        /// <summary>
        /// 資料欄位資訊建構子
        /// </summary>
        /// <param name="ColumnType">資料欄位型別</param>
        /// <param name="Size">資料欄位長度</param>
        /// <param name="NumericPrecision">欄位的數字整數位數</param>
        /// <param name="NumericScale">欄位小數部份的位數</param>
        public ColumnTypeInfo(ColumnType ColumnType, int Size, short NumericPrecision, short NumericScale) : this(ColumnType, Size, NumericPrecision, NumericScale, true) { }

        /// <summary>
        /// 資料欄位資訊建構子
        /// </summary>
        /// <param name="ColumnType">資料欄位型別</param>
        /// <param name="Size">資料欄位長度</param>
        /// <param name="NumericPrecision">欄位的數字整數位數</param>
        /// <param name="NumericScale">欄位小數部份的位數</param>
        /// <param name="Nullable">欄位是否允許為 Nullable</param>
        public ColumnTypeInfo(ColumnType ColumnType, int Size, short NumericPrecision, short NumericScale, bool Nullable)
        {
            this.ColumnType = ColumnType;
            this.Size = Size;
            this.NumericPrecision = NumericPrecision;
            this.NumericScale = NumericScale;
            this.Nullable = Nullable;
        }

        /// <summary>
        /// 資料欄位資訊建構子
        /// </summary>
        /// <param name="ColumnDescrtption">資料欄位資訊描述字串 Example : columnTypeName(m,n)</param>
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
        /// 資料欄位型別
        /// </summary>
        public ColumnType ColumnType { get; protected set; }

        /// <summary>
        /// 資料欄位長度
        /// </summary>
        public int Size { get; protected set; }

        /// <summary>
        /// 欄位小數部份的位數(當資料型別為 Decimal 時)
        /// </summary>
        public short NumericScale { get; protected set; }

        /// <summary>
        /// 欄位的數字整數位數(當資料型別為 Decimal 時) NumericPrecision
        /// </summary>
        public short NumericPrecision { get; protected set; }

        /// <summary>
        /// 欄位是否允許為 Nullable
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
        /// 取得資料顯示的最佳字元個數.
        /// </summary>
        /// <returns></returns>
        public int GetDisplayWordCount(int DisplayCharMaxCount = 0)
        {
            int DisplayCount = DisplayCharMaxCount;
            if (DisplayCount == 0)
            {
                // 自動依欄位型別給
                switch (this.ColumnType)
                {
                    case ColumnType.Char:
                    case ColumnType.NChar:
                    case ColumnType.Varchar:
                    case ColumnType.Nvarchar:
                        if (Size > 255) return 255;  // UI 可編輯的最大字元數.
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
        /// String，表示目前的 Object。
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
        /////  屬性處理物件轉換屬性字串
        ///// </summary>
        ///// <returns>屬性字串</returns>
        //public PropertyValue GetPropertyValue()
        //{
        //    return new PropertyValue(ToString());
        //}

        /// <summary>
        /// 比較運算 Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// 比較運算 ==
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
        /// 比較運算 !=
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
        /// 預設雜湊函式
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}