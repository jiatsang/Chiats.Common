// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 指示處理 Parameter 參數的方法
    /// </summary>
    public enum ParameterMode
    {
        /// <summary>
        /// 使用資料庫的 Parameter
        /// </summary>
        Parameter = 0,
        /// <summary>
        /// 自動展開所有 Parameter
        /// </summary>
        Expand = 1,
    }
}