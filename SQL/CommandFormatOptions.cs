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
    /// 指示建立 SQL 命令的文字敘述的輸出格式選項,
    /// </summary>
    [Flags]
    public enum CommandFormatOptions
    {
        /// <summary>
        /// (預設值) 無格式化
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 自動加入換行字元
        /// </summary>
        AutoFormat = 0x01,
        /// <summary>
        /// 自動分隔欄位
        /// </summary>
        ColumnSplit = 0x02,
        /// <summary>
        /// 格式化成為 Html 
        /// </summary>
        Html = 0x10,
    }
}