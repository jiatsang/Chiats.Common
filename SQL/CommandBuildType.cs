// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 指示建立 SQL 命令的文字敘述的輸出格式的依循規則, SQLCTL 或 標準 SQL 規格的文字敘述.
    /// </summary>
    public enum CommandBuildType
    {
        /// <summary>
        /// (預設值) 符合標準 SQL 規格的文字敘述的方式 (測試平台 : Microsoft SQL Server 2005/2008)
        /// </summary>
        SQL,

        /// <summary>
        /// 符合 SQL Condition Template Language  規格的文字敘述的方式. CTL 是建立在一個符合通用 SQL 規格的文字之上.
        /// </summary>
        SQLCTL
    }
}