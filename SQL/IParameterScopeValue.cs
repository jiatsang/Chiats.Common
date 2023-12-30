// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 定義參數的範圍字串值 (From~TO)
    /// </summary>
    public interface IParameterScopeValue : IParameterValue
    {
        /// <summary>
        ///  開始字串值
        /// </summary>
        string FromStringValue { get; }

        /// <summary>
        ///  結束字串值
        /// </summary>
        string ToStringValue { get; }
    }
}