// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// �w�q�Ѽƪ���@�r���
    /// </summary>
    public interface IParameterSingleValue : IParameterValue
    {
        /// <summary>
        /// �ѼƤ��e�r���.
        /// </summary>
        string StringValue { get; }
    }
}