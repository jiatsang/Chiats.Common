// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// WhereKey 連結子
    /// </summary>
    public enum WhereKeyOperator
    {
        /// <summary>
        /// =
        /// </summary>
        Equals,             /* = */

        /// <summary>
        /// 不等於 , !=
        /// </summary>
        NotEquals,          /* <> */

        /// <summary>
        ///
        /// </summary>
        LessEquals,         /* <= */

        /// <summary>
        ///
        /// </summary>
        Less,               /* < */

        /// <summary>
        /// >=
        /// </summary>
        GreaterEquals,      /* >= */

        /// <summary>
        /// >
        /// </summary>
        Greater,            /* > */

        /// <summary>
        /// like
        /// </summary>
        Like,             /* like */

        /// <summary>
        /// not like
        /// </summary>
        NotLike                /* not like */
    }
}