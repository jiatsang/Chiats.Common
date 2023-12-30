// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// �ҥΰѼƸ�Ʈw�s�����󤶭�.
    /// </summary>
    public interface IParameterCommandBuilder
    {
        /// <summary>
        /// �q�� DataAccess Object �ҥΰѼ�
        /// </summary>
        bool ParameterEnabled { get; }

        /// <summary>
        /// ���o�ѼƤ��e.
        /// </summary>
        Parameter[] Parameters { get; }
    }
}