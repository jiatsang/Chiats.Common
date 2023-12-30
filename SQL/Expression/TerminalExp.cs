// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    ///  �׺ݹB��l
    ///      �`�ƭ�. �ƭ�. �r��
    ///      �ܧ�W��/���W��
    /// </summary>
    public abstract class TerminalExp : BaseExp
    {
        /// <summary>
        /// �Ǧ^�O�_���׺ݹB��l
        /// </summary>
        public override bool IsTerminalExp
        {
            get { return true; }
        }
    }

    /// <summary>
    /// �D�׺ݹB��l
    /// </summary>
    public abstract class NonTerminalExp : BaseExp
    {
        /// <summary>
        /// �Ǧ^�O�_���׺ݹB��l
        /// </summary>
        public override bool IsTerminalExp
        {
            get { return false; }
        }
    }
}