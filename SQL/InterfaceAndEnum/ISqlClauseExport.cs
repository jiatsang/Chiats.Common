// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Text;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL Command Clause 的產生介面
    /// </summary>
    public interface ISqlClauseExport
    {
        /// <summary>
        /// 產出 SQL Command Clause 字串,
        /// </summary>
        /// <param name="sb">輸出的 Clause 字串 </param>
        /// <param name="sql_model"></param>
        /// <returns>回傳是實際輸出個數.</returns>
        int Export(StringBuilder sb, SqlModel sql_model);
    }
}