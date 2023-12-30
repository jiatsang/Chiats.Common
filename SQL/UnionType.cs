// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// SQL Union/Union All/Minus/Intersect Command
    /// </summary>
    public enum UnionType
    {
        /// <summary>
        /// ��ܵL�p�� UNION �� SQL Command
        /// </summary>
        None,

        /// <summary>
        /// ����p�� UNION �� SQL Command
        /// </summary>
        Union,

        /// <summary>
        /// ����p�� UNION ALL �� SQL Command
        /// </summary>
        UnionAll,

        /// <summary>
        /// ��ܮt�� Minus �� SQL Command
        /// </summary>
        Minus,

        /// <summary>
        /// ��ܥ涰 Intersect �� SQL Command
        /// </summary>
        Intersect
    };
}