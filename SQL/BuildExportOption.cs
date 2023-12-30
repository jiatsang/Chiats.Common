// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL Command 輸出選項.  決定　SQL Command 　各子句是否輸出主要 Keyword ( ORDER BY/Where/Having .. )
    /// </summary>
    [Flags]
    public enum BuildExportOptions
    {
        /// <summary>
        ///  指示 BuildExport 不輸出 Keyword (ORDER BY/Where/Having ..)
        /// </summary>
        None = 0x0,

        /// <summary>
        ///  指示 BuildExport 輸出 Keyword (ORDER BY/Where/Having ..)
        /// </summary>
        ExportKeyword = 0x01,

        /// <summary>
        /// 預設選項 ExportKeyword
        /// </summary>
        Default = ExportKeyword,
    }
}