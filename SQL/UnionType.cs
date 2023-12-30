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
        /// 表示無聯集 UNION 的 SQL Command
        /// </summary>
        None,

        /// <summary>
        /// 表示聯集 UNION 的 SQL Command
        /// </summary>
        Union,

        /// <summary>
        /// 表示聯集 UNION ALL 的 SQL Command
        /// </summary>
        UnionAll,

        /// <summary>
        /// 表示差集 Minus 的 SQL Command
        /// </summary>
        Minus,

        /// <summary>
        /// 表示交集 Intersect 的 SQL Command
        /// </summary>
        Intersect
    };
}