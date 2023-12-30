// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 取得資料庫的系統資訊界面. 不同的資料庫或版本.必需實作本界面.
    /// </summary>
    public interface IDbInformation
    {
        /// <summary>
        /// 取得指定表格名稱之資料表格的詳細資訊.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        TableInfo QueryTableInfo(string name);
    }
}