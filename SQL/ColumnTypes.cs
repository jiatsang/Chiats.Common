// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 資料的欄位型態,定義通用的資訊和資料庫廠商版本無關.
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// 自動資料型態 , 視系統自行決定其型別  Ex  NVarChar
        /// </summary>
        Auto = 0x0000,

        /// <summary>
        /// 未指定的資料型態
        /// </summary>
        UnKnown = 0xFFFF,

        /// <summary>
        ///
        /// </summary>
        Varbinary = ColumnTypeHelper.TypeVAR | ColumnTypeHelper.TypeBinary,

        /// <summary>
        ///
        /// </summary>
        Binary = ColumnTypeHelper.TypeBinary,

        /// <summary>
        ///
        /// </summary>
        Text = ColumnTypeHelper.TypeText,

        /// <summary>
        ///
        /// </summary>
        NText = ColumnTypeHelper.TypeDoubleByte | ColumnTypeHelper.TypeText,

        /// <summary>
        /// 變動長度字串
        /// </summary>
        Varchar = ColumnTypeHelper.TypeVAR | ColumnTypeHelper.TypeChar,

        /// <summary>
        /// 固定長度字串
        /// </summary>
        Char = ColumnTypeHelper.TypeChar,

        /// <summary>
        /// 變動長度字串, 會以 Unicode 編碼存在.
        /// </summary>
        Nvarchar = ColumnTypeHelper.TypeVAR | ColumnTypeHelper.TypeDoubleByte | ColumnTypeHelper.TypeChar,

        /// <summary>
        /// 固定長度字串, 會以 Unicode 編碼存在.
        /// </summary>
        NChar = ColumnTypeHelper.TypeDoubleByte | ColumnTypeHelper.TypeChar,

        /// <summary>
        /// 8 bit 整數值 , 含正負號
        /// </summary>
        Byte = ColumnTypeHelper.TypeNumber | 0x0001,

        /// <summary>
        /// 16 bit 整數值 , 含正負號
        /// </summary>
        Int = ColumnTypeHelper.TypeNumber | 0x0002,

        /// <summary>
        /// 32 bit 整數值 , 含正負號
        /// </summary>
        Int32 = ColumnTypeHelper.TypeNumber | 0x0003,

        /// <summary>
        /// 64 bit 整數值 , 含正負號
        /// </summary>
        Int64 = ColumnTypeHelper.TypeNumber | 0x0004,

        /// <summary>
        /// 布林值, 可以為 True/False
        /// </summary>
        Bool = ColumnTypeHelper.TypeBool,

        /// <summary>
        /// 數值型態 , 含正負號及小數位數
        /// </summary>
        Decimal = ColumnTypeHelper.TypeNumber | 0x0005,

        /// <summary>
        /// 倍精度 浮點數值
        /// </summary>
        Double = ColumnTypeHelper.TypeNumber | 0x0006,

        /// <summary>
        /// 單精度 浮點數值
        /// </summary>
        Single = ColumnTypeHelper.TypeNumber | 0x0007,

        /// <summary>
        /// 日期含時間
        /// </summary>
        DateTime = ColumnTypeHelper.TypeDateTime | 0x0001,

        /// <summary>
        /// XML 字串
        /// </summary>
        XML = ColumnTypeHelper.TypeXml,

        /// <summary>
        /// Image
        /// </summary>
        Image = ColumnTypeHelper.TypeImage,

        /// <summary>
        /// Uniqueidentifier 這是 16 位元組的 GUID。
        /// </summary>
        Uniqueidentifier = ColumnTypeHelper.TypeGuid
    }




}