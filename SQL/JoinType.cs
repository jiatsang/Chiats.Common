// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// JOIN: Return rows when there is at least one match in both tables <br/>
    /// LEFT JOIN: Return all rows from the left table, even if there are no matches in the right table<br/>
    /// RIGHT JOIN: Return all rows from the right table, even if there are no matches in the left table<br/>
    /// Outer(FULL) JOIN: Return rows when there is a match in one of the tables<br/>
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// Outer(FULL) JOIN/OUTER JOIN: Return rows when there is a match in one of the tables
        /// </summary>
        Outer,

        /// <summary>
        /// INNER JOIN: Return rows when there is at least one match in both tables
        /// </summary>
        Inner,

        /// <summary>
        /// LEFT JOIN/LEFT OUTER JOIN: Return all rows from the left table, even if there are no matches in the right table
        /// </summary>
        LeftOuter,

        /// <summary>
        /// LEFT JOIN: Return all rows from the left table, even if there are no matches in the right table
        /// </summary>
        Left,

        /// <summary>
        /// RIGHT JOIN/RIGHT OUTER JOIN: Return all rows from the right table, even if there are no matches in the left table
        /// </summary>
        RightOuter,

        /// <summary>
        /// RIGHT INNER JOIN: Return all rows from the right table, even if there are no matches in the left table
        /// </summary>
        Right,

        /// <summary>
        /// 表示非使用  JOIN Keyword 的表示法.
        /// </summary>
        None,

        /// <summary>
        /// CROSS APPLY
        /// </summary>
        CrossApply,

        /// <summary>
        /// OUTER APPLY
        /// </summary>
        OuterApply
    }
}