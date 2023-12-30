// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections.Generic;

namespace Chiats.SQL.Expression
{
    internal abstract class SyntaxTreeNode
    {
        public abstract bool IsSingleToken { get; }
        public abstract Token Token { get; }
    }

    internal class TreeLeafToken : SyntaxTreeNode
    {
        public TreeLeafToken(Token token)
        { this.token = token; }

        public readonly Token token;

        public override bool IsSingleToken
        {
            get { return true; }
        }

        public override Token Token
        {
            get { return token; }
        }

        /// <summary>
        /// 傳回表示目前物件的字串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("LeafToken {0}  ({1})", token.String, token.Type);
        }
    }

    internal class TreeToken : SyntaxTreeNode
    {
        public void Add(SyntaxTreeNode token)
        {
            Nodes.Add(token);
        }

        public override bool IsSingleToken
        {
            get { return false; }
        }

        public override Token Token
        {
            get { return Token.Empty; }
        }

        public List<SyntaxTreeNode> Nodes = new List<SyntaxTreeNode>();
    }
}