// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// 處理目前分析文法的狀態內容.
    /// </summary>
    internal class SyntaxAnalysis
    {
        #region SyntaxTree

        private class SyntaxTree
        {
            public List<SyntaxTreeNode> tree = new List<SyntaxTreeNode>();
            public Stack<TreeToken> stack = new Stack<TreeToken>();

            public void Add(Token token)
            {
                if (token.Type == TokenType.Symbol)
                {
                    switch (token.String)
                    {
                        case "(":
                            TreeToken TreeToken = new TreeToken();
                            addToken(TreeToken);
                            stack.Push(TreeToken);
                            addToken(new TreeLeafToken(token));  // 括號要放在文法節點內.
                            return;

                        case ")":
                            addToken(new TreeLeafToken(token));  // 括號要放在文法節點內.
                            stack.Pop();
                            return;
                    }
                }
                addToken(new TreeLeafToken(token));
            }

            private void addToken(SyntaxTreeNode node)
            {
                if (stack.Count > 0) stack.Peek().Add(node); else tree.Add(node);
            }
        }

        /// <summary>
        /// BaseExp or SyntaxTreeNode  的 Union . 它可以是己轉換的 BaseExp 物件或未轉換的 SyntaxTreeNode
        /// </summary>
        private struct UnionSyntaxNode
        {
            public BaseExp Exp;
            public SyntaxTreeNode Node;

            public ArithmeticLogicalLink Linker
            {
                get
                {
                    if (Exp == null && Node != null)
                    {
                        if (Node.IsSingleToken && (Node.Token.Type == TokenType.Symbol || Node.Token.Type == TokenType.Keyword))
                            return CommonExtensions.EnumConvert<ArithmeticLogicalLink>(Node.Token.String, ArithmeticLogicalLink.Empty);
                    }
                    return ArithmeticLogicalLink.Empty; // 非算式運算子
                }
            }

            public bool IsSymbol(string Symbol)
            {
                if (!IsExpression)
                    return Node.IsSingleToken && Node.Token.IsSymbol(Symbol);

                return false;
            }

            public int BracketsTokeCount()  // 是否為括號運算式 brackets parentheses
            {
                TreeToken tn = Node as TreeToken;
                if (tn != null && tn.Nodes.Count > 1) // 括號運算式 戈至少會有二個文法單元
                {
                    if (tn.Nodes[0].Token.String == "(" && tn.Nodes[tn.Nodes.Count - 1].Token.String == ")")
                        return tn.Nodes.Count - 2; // 括號運算式 的內含文法單 個數( 去前後括號 )
                }
                return -1; // 表示非括號運算式 brackets parentheses
            }

            /// <summary>
            /// 檢查是否為指定的關鍵字, 不區分大小寫字母
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool IsKey(string value)
            {
                if (!IsExpression)
                {
                    return Node.IsSingleToken &&
                    (
                        Node.Token.Type == TokenType.Keyword || string.Compare(Node.Token.String, value, true) == 0
                    );
                }
                return false;
            }

            public bool IsKeyOrValue
            {
                get
                {
                    if (!IsExpression)
                    {
                        if (Node.IsSingleToken)
                        {
                            return (Node.Token.Type == TokenType.Keyword || Node.Token.Type == TokenType.Number || Node.Token.Type == TokenType.String);
                        }
                        return true; // 非單一符號.. 均可視為  KeyOrValue
                    }
                    return true; // 己轉換之運算式. 均可視為  KeyOrValue
                }
            }

            public bool IsKeyword
            {
                get
                {
                    if (!IsExpression)
                    {
                        if (Node.IsSingleToken)
                        {
                            return (Node.Token.Type == TokenType.Keyword);
                        }
                        return false; // 非單一符號.. 均可視為  KeyOrValue
                    }
                    return false;
                }
            }

            // 配合  IsKeyword
            public string Keyword
            {
                get
                {
                    if (!IsExpression)
                    {
                        if (Node.IsSingleToken && Node.Token.Type == TokenType.Keyword)
                        {
                            return (Node.Token.String);
                        }
                        return null;
                    }
                    return null;
                }
            }

            public bool IsSymbol()
            {
                if (!IsExpression)
                    return Node.IsSingleToken && Node.Token.Type == TokenType.Symbol;

                return false;
            }

            public bool IsExpression { get { return (Exp != null); } }

            /// <summary>
            /// 傳回表示目前物件的字串。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Exp != null)
                    return string.Format("Exp ({0})", Exp);
                TreeToken tn = Node as TreeToken;

                if (tn != null)
                    return string.Format("TreeToken '{1}' at {0}", tn.Nodes.Count, tn.Nodes);

                return string.Format("SingleToken '{0}' at {1}", Node.Token.String, Node.Token.StartAt);
            }
        }

        #endregion SyntaxTree

        public SyntaxAnalysis(SQLTokenScanner tokens)
        {
            this.tokens = tokens;
        }

        public BaseExp Analysis()
        {
            SyntaxTree SyntaxTree = new SyntaxTree();

            if (tokens.Count == 1)
                return Convert2Exp(tokens[0]);



            foreach (var token in tokens) SyntaxTree.Add(token);

            // TODO: 檢查文法樹是否完整.

            return Convert2Exp(SyntaxTree.tree.ToArray<SyntaxTreeNode>());
        }

        private BaseExp Convert2Exp(SyntaxTreeNode[] Nodes)
        {
            if (Nodes[0].Token.IsSymbol("(") && Nodes[Nodes.Length - 1].Token.IsSymbol(")"))
            {
                // 去除最外層的  ()  符號
                SyntaxTreeNode[] NewNodes = new SyntaxTreeNode[Nodes.Length - 2];
                for (int i = 1; i < Nodes.Length - 1; i++) NewNodes[i - 1] = Nodes[i];
                return Convert2Exp(NewNodes);
            }
            int[] CommaNodeIndex = FoundCommaNode(Nodes);
            if (CommaNodeIndex.Length > 0)
            {
                int begin = 0;
                List<BaseExp> ParameterList = new List<BaseExp>();
                foreach (var index in CommaNodeIndex)
                {
                    ParameterList.Add(PaserExp(Nodes, begin, index - 1));
                    begin = index + 1;
                }
                return new ParameterListExp(ParameterList.ToArray<BaseExp>());
            }
            // Support 
            // ROW_NUMBER() OVER(ORDER BY SUM(SalesAmountQuota) DESC)
            // RANK/DENSE_RANK()  OVER(ORDER BY SUM(SalesAmountQuota) DESC)
            // NTILE( c ) OVER(ORDER BY SUM(SalesAmountQuota) DESC)
            if (Nodes[0].Token.FindKeywordAndMatch(new string[] { "ROW_NUMBER", "RANK", "DENSE_RANK" , "NTILE" }) != -1)
            {
                if (Nodes[2].Token.IsKeywordAndMatch("OVER"))
                {
                    if( Nodes[3] is TreeToken tree)
                    {
                        // tree.Nodes
                        // TODO : PARTITION BY value_expression , ... [ n ] ] order_by_clause
                        // string ORDER_BY 


                    }
                }
            }

            return PaserExp(Nodes, 0, Nodes.Length - 1);
        }

        private BaseExp Convert2Exp(Token Token)
        {
            if (Token.Type == TokenType.Keyword)
            {
                if (Token.String.StartsWith("@"))
                {
                    return new ParameterExp(Token.String);
                }
                return new ColumnExp(new StringObjectName(Token.String));
            }
            if (Token.String == "*")
            {
                return new ColumnExp(new StringObjectName(Token.String));
            }
            return new ConstantExp(Token.String, Token.IsString);
        }

        private BaseExp Convert2Exp(SyntaxTreeNode Node)
        {
            if (Node.IsSingleToken)
                return Convert2Exp(Node.Token);
            else
                return Convert2Exp(((TreeToken)Node).Nodes.ToArray<SyntaxTreeNode>());
        }

        private BaseExp Convert2Exp(UnionSyntaxNode UnionSyntaxNode)
        {
            if (UnionSyntaxNode.Exp != null)
                return UnionSyntaxNode.Exp;
            else
                return Convert2Exp(UnionSyntaxNode.Node);
        }

        private BaseExp Convert2Exp(TreeToken Tokens)
        {
            return Convert2Exp(Tokens.Nodes.ToArray<SyntaxTreeNode>());
        }

        private BaseExp PaserExp(SyntaxTreeNode[] Nodes, int beginIndex, int endIndex)
        {
            UnionSyntaxNode[] nods = new UnionSyntaxNode[endIndex - beginIndex + 1];
            for (int Index = 0; Index <= (endIndex - beginIndex); Index++)
            {
                nods[Index].Exp = null;
                nods[Index].Node = Nodes[beginIndex + Index];
            }
            return PaserExp(nods);
        }

        private BaseExp PaserExp(UnionSyntaxNode[] Nodes, int beginIndex, int endIndex)
        {
            UnionSyntaxNode[] nods = new UnionSyntaxNode[endIndex - beginIndex + 1];
            for (int Index = 0; Index <= (endIndex - beginIndex); Index++)
            {
                nods[Index] = Nodes[beginIndex + Index];
            }
            return PaserExp(nods);
        }

        private BaseExp PaserExp(BaseExp Exp, UnionSyntaxNode[] Nodes, int beginIndex, int endIndex)
        {
            UnionSyntaxNode[] nods = new UnionSyntaxNode[endIndex - beginIndex + 2];
            nods[0].Exp = Exp;
            nods[0].Node = null;

            for (int Index = 0; Index <= (endIndex - beginIndex); Index++)
            {
                nods[Index + 1] = Nodes[beginIndex + Index];
            }

            return PaserExp(nods);
        }

        private BaseExp PaserExp(UnionSyntaxNode[] Nodes)
        {
            if (Nodes.Length == 1)
                return Convert2Exp(Nodes[0]);

            int FirstIndex = 0;
            int LastIndex = Nodes.Length - 1;

            if (Nodes[FirstIndex].IsSymbol("(") && Nodes[LastIndex].IsSymbol(")"))
            {
                // 去除最外層的  ()  符號
                UnionSyntaxNode[] NewNodes = new UnionSyntaxNode[Nodes.Length - 2];
                for (int Index = 0; Index < (Nodes.Length - 2); Index++)
                {
                    NewNodes[Index].Exp = Nodes[Index + 1].Exp;
                    NewNodes[Index].Node = Nodes[Index + 1].Node;
                }
                return PaserExp(NewNodes);
            }
            ArithmeticLogicalLink Linker = Nodes[FirstIndex].Linker;

            if (Linker == ArithmeticLogicalLink.Empty && Nodes[FirstIndex].IsKeyOrValue)  // 第一個必須為 Key or Value
            {
                int BracketsTokeCount = Nodes[FirstIndex + 1].BracketsTokeCount();
                if (BracketsTokeCount != -1)
                {
                    if (Nodes[FirstIndex].IsKeyword)
                    {
                        FunctionExp funcExp = null;
                        if (BracketsTokeCount == 0)
                        {
                            funcExp = new FunctionExp(Nodes[FirstIndex].Keyword);// A()
                        }
                        else if (BracketsTokeCount > 0)
                        {
                            BaseExp p_exp = Convert2Exp(Nodes[FirstIndex + 1]);
                            funcExp = new FunctionExp(Nodes[FirstIndex].Keyword, p_exp);// A(B)
                        }
                        if (LastIndex == 1)
                            return funcExp;
                        else
                            return PaserExp(funcExp, Nodes, 2, LastIndex);
                    }
                    else
                    {
                        throw new SyntaxException("文法錯誤 B  A()/A(B) ");  // 文法錯誤
                    }
                }
                else if (Nodes[FirstIndex + 1].Linker != ArithmeticLogicalLink.Empty)
                {
                    if (Nodes[FirstIndex + 2].IsKeyOrValue)
                    {
                        ArithmeticLogicalExp LogicalExp = null;
                        LogicalExp = new ArithmeticLogicalExp(Convert2Exp(Nodes[FirstIndex]));
                        LogicalExp.Add(Nodes[FirstIndex + 1].Linker, PaserExp(Nodes, 2, LastIndex));
                        return LogicalExp;
                    }
                    else
                    {
                        throw new SyntaxException("文法錯誤 C");  // 文法錯誤
                    }
                }
            }
            else if (Linker == ArithmeticLogicalLink.Subtract)
            {
                BaseExp exp = PaserExp(Nodes, 1, LastIndex);
                if (exp is ConstantExp)
                {
                    ConstantExp ConstantExp = (ConstantExp)exp;
                    if (ConstantExp.IsConstantString)
                    {
                        // 字串值不支援 符號  (-) 開頭文法
                        throw new SyntaxException("文法錯誤: 字串值不支援的符號 '-'");
                    }
                    ConstantExp.HasMinusSymbol = true;
                    return ConstantExp;
                }
                throw new SyntaxException("尚未處理的符號 '-' ");
            }
            else if (Linker == ArithmeticLogicalLink.Not)
            {
                throw new SyntaxException("尚未處理的符號  'not'");  // 文法錯誤
            }
            //return new RankAndRowNumberExp();
            throw new SyntaxException(string.Format("文法錯誤 D : {0}", Nodes[FirstIndex]));  // 文法錯誤
        }

        private int[] FoundCommaNode(SyntaxTreeNode[] Nodes)
        {
            List<int> indexes = new List<int>();
            Func<TreeLeafToken, bool> NodeCheck = (n) => n != null && n.token.String == ",";
            for (int i = 0; i < Nodes.Length - 1; i++) if (NodeCheck(Nodes[i] as TreeLeafToken)) indexes.Add(i);
            if (indexes.Count > 0) indexes.Add(Nodes.Length);   // 補上最後一個位置
            return indexes.ToArray<int>();
        }

        private SQLTokenScanner tokens = null;

        /// <summary>
        /// 目前分析中的 Token 物件位置.
        /// </summary>
        public int currentIndex = 0;

        /// <summary>
        /// 取得目前分析中的 下一個位置 Token 物件.
        /// </summary>
        /// <returns></returns>
        public Token NextToken()
        {
            return NextToken(0);
        }

        /// <summary>
        /// 取得目前分析中 Token 物件.
        /// </summary>
        /// <returns></returns>
        public Token CurrentToken()
        {
            return NextToken(-1);
        }

        /// <summary>
        ///  取得相對位置的 Token 物件如果該位置不存在則回傳 Token.Empty.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Token NextToken(int index)
        {
            if (currentIndex + index >= 0 && currentIndex + index >= tokens.Count)
                return Token.Empty;
            return tokens[currentIndex + index];
        }
    }
}