// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Chiats.SQL
{
    /// <summary>
    /// 和資料庫廠商版本無關欄位資訊(名稱,型能,大小)
    /// </summary>
    public class ColumnInfo : ColumnTypeInfo
    {

        public ColumnInfo() { }
        /// <summary>
        /// 欄位資訊(名稱,型能,大小) 建構子
        /// </summary>
        /// <param name="name">欄位名稱</param>
        public ColumnInfo(string name) : this(name, ColumnType.Auto, 10, 0, 0, true) { }

        /// <summary>
        /// 欄位資訊(名稱,型能,大小) 建構子
        /// </summary>
        /// <param name="name">欄位名稱</param>
        /// <param name="ColumnType">資料欄位型別</param>
        /// <param name="Size">資料欄位長度</param>
        public ColumnInfo(string name, ColumnType ColumnType, int Size) : this(name, ColumnType, Size, 0, 0, true) { }

        /// <summary>
        /// 欄位資訊(名稱,型能,大小) 建構子
        /// </summary>
        /// <param name="name">欄位名稱</param>
        /// <param name="ColumnType">資料欄位型別</param>
        /// <param name="Size">資料欄位長度</param>
        /// <param name="NumericPrecision">欄位的數字整數位數</param>
        /// <param name="NumericScale">欄位小數部份的位數</param>
        /// <param name="Nullable">欄位是否允許為 Nullable</param>
        public ColumnInfo(string name, ColumnType ColumnType, int Size, short NumericPrecision, short NumericScale, bool Nullable)
            : base(ColumnType, Size, NumericPrecision, NumericScale, Nullable)
        {
            this.Name = name;
        }

        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Name { get; set; }

        //
        // ^[@#_A-Z][A-Z0-9#_]*$  未考量 Tablename
        // ^[@#_A-Z][A-Z0-9#_]*.[@#_A-Z][A-Z0-9#_]*$  考量 Tablename 但有問題.
        //
        private static Regex ColumnNameRegEx = new Regex("^[@#_A-Z][A-Z0-9#_]*([.][#_A-Z][A-Z0-9#_]*|$)?$",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        /// 驗證有效的欄位名稱
        /// </summary>
        /// <returns></returns>
        public static bool ColumnNameVaildate(string name)
        {
            // 只允許 0-9 A-Z _ @ #
            return ColumnNameRegEx.IsMatch(name);
        }

        /// <summary>
        ///  String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ColumnInfo {0} ColumnType={1} Size={2} Precision={3} Scale={4}", Name, ColumnType, Size, NumericPrecision, NumericScale);
        }
    }
}