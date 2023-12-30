// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 币ノ把计戈畐ンざ.
    /// </summary>
    public interface IParameterCommandBuilder
    {
        /// <summary>
        /// 硄 DataAccess Object 币ノ把计
        /// </summary>
        bool ParameterEnabled { get; }

        /// <summary>
        /// 眔把计ず甧.
        /// </summary>
        Parameter[] Parameters { get; }
    }
}