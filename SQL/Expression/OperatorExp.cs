// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    ///// <summary>
    ///// 單元運算(unary operator); 二元運算(binary operator); 多元運算子(? operator)
    ///// </summary>
    //public abstract class UnaryOperatorExp : NonTerminalExp
    //{
    //    public BaseExp Exp;
    //}

    /// <summary>
    /// 二元運算(binary operator);
    /// </summary>
    public abstract class BinaryOperatorExp : NonTerminalExp
    {
        public BaseExp FirstExp;
        public BaseExp SecondExp;
    }
}