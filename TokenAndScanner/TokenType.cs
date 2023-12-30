// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats
{
    [Flags]
    public enum TokenType  // changeto internal
    {
        Null = 0,
        ParserError = 0x401,
        Keyword = 0x101,
        String = 0x102,
        Symbol = 0x103,
        Number = 0x104
    }
}