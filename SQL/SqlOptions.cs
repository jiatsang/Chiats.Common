// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsaienum SqlOptions
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL
{
    [Flags]
    public enum SqlOptions
    {
        None = 0,
        LowerName = 0x01,         /* 強制轉換成小寫名稱. 欄位名稱/表格名稱 */
        NonStringNational = 0x02,
    }
}