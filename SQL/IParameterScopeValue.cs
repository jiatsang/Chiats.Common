// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// �w�q�Ѽƪ��d��r��� (From~TO)
    /// </summary>
    public interface IParameterScopeValue : IParameterValue
    {
        /// <summary>
        ///  �}�l�r���
        /// </summary>
        string FromStringValue { get; }

        /// <summary>
        ///  �����r���
        /// </summary>
        string ToStringValue { get; }
    }
}