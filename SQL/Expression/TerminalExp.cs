// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    ///  終端運算子
    ///      常數值. 數值. 字串
    ///      變更名稱/欄位名稱
    /// </summary>
    public abstract class TerminalExp : BaseExp
    {
        /// <summary>
        /// 傳回是否為終端運算子
        /// </summary>
        public override bool IsTerminalExp
        {
            get { return true; }
        }
    }

    /// <summary>
    /// 非終端運算子
    /// </summary>
    public abstract class NonTerminalExp : BaseExp
    {
        /// <summary>
        /// 傳回是否為終端運算子
        /// </summary>
        public override bool IsTerminalExp
        {
            get { return false; }
        }
    }
}