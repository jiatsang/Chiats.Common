// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 定義參數的單一字串值
    /// </summary>
    public interface IParameterSingleValue : IParameterValue
    {
        /// <summary>
        /// 參數內容字串值.
        /// </summary>
        string StringValue { get; }
    }
}