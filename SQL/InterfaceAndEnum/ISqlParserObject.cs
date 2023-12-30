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
    /// SQL 解析字串命令共用介面. 依字串命令產生 SqlModel 相對應物件物件
    /// </summary>
    internal interface ISqlParserObject
    {
        /// <summary>
        /// 解析字串 (TokenList) 並依字串命令產生 SqlModel 相對應物件物件
        /// </summary>
        /// <param name="TokenScanner">由 CommandText 經 Token Scan 後產生之 TokenList.</param>
        /// <param name="SqlModel"></param>
        /// <returns>相對應物件物件類別</returns>
        SqlModel PaserCommand(SQLTokenScanner TokenScanner, SqlModel SqlModel, object parameters = null);

        /// <summary>
        /// 指示本物件, 所產生之對應  SqlModel 相對應物件物件類別 如 SelectModel/UpdateModel/InsertModel/DeleteModel
        /// </summary>
        Type ModelType { get; }
    }
}