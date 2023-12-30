// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Chiats.SQL.Expression
{

    public class RankAndRowNumberExp : NonTerminalExp
    {
        public string Expression { get; private set; }
        public RankAndRowNumberExp(string Expression) 
        {
            this.Expression = Expression;
        }
        public override void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter, bool ForceLowerName)
        {
            // throw new System.NotImplementedException();
        }
    }
    /// <summary>
    ///  算術邏輯運算式  ." Example :   A + B
    ///  包含非終端運算元
    ///       四則運算元    +-*/  (多元運算子)
    ///  邏輯運算式本身也同時可以包含算式運算式   Example :    A >= B+C
    ///  ArithmeticLogicalExp 負責解析 算式或邏輯運算之關係. 不包含可計算順序和方法.
    /// </summary>
    public class ArithmeticLogicalExp : NonTerminalExp, IEnumerable<ArithmeticLogicalExp.ArithmeticLogicalLinker>
    {
        private List<ArithmeticLogicalLinker> linker_list = new List<ArithmeticLogicalLinker>();

        #region class ArithmeticLogicalLinker

        /// <summary>
        ///  算術邏輯運算式的單一節點 ,含運算式和連結子 (LogicalLinkOperator,ArithmeticLinkOperator)
        ///  算術邏輯運算式的最後一個節點連結子固定為 ArithmeticLogicalLink.Empty
        /// </summary>
        public class ArithmeticLogicalLinker
        {
            /// <summary>
            /// 新增一個算術邏輯運算式
            /// </summary>
            /// <param name="Exp"></param>
            public ArithmeticLogicalLinker(BaseExp Exp) { this.exp = Exp; this.linker = ArithmeticLogicalLink.Empty; }

            /// <summary>
            ///
            /// </summary>
            /// <param name="Exp"></param>
            /// <param name="Linker"></param>
            public ArithmeticLogicalLinker(BaseExp Exp, ArithmeticLogicalLink Linker) { this.exp = Exp; this.linker = Linker; }

            private BaseExp exp;
            private ArithmeticLogicalLink linker;

            /// <summary>
            ///  是否為最後一個節點連結子
            /// </summary>
            public bool IsTerminalLinker { get { return linker == ArithmeticLogicalLink.Empty; } }

            /// <summary>
            ///  是否為邏輯運算子.  False 時為算術運算式子
            /// </summary>
            public bool IsLogicalLinker { get { return linker >= ArithmeticLogicalLink.LogicalLink; } }

            /// <summary>
            /// 回傳運算式物件.
            /// </summary>
            public BaseExp Expression { get { return exp; } }

            /// <summary>
            ///  回傳運算式物件.
            /// </summary>
            /// <typeparam name="T">運算式物件型別</typeparam>
            /// <returns></returns>
            public T Exp<T>() where T : BaseExp
            {
                return (T)(exp as T);
            }

            /// <summary>
            /// 回傳算式子.(算術或邏輯運算式)
            /// </summary>
            public ArithmeticLogicalLink Linker { get { return linker; } internal set { linker = value; } }
        }

        #endregion class ArithmeticLogicalLinker

        /// <summary>
        ///  算術邏輯運算式,
        /// </summary>
        public ArithmeticLogicalExp(BaseExp Exp)
        {
            linker_list.Add(new ArithmeticLogicalLinker(Exp, ArithmeticLogicalLink.Empty));
        }

        /// <summary>
        /// 回傳算式的個數
        /// </summary>
        public int Count { get { return linker_list.Count; } }

        /// <summary>
        ///  回傳指定 算式或邏輯運算式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ArithmeticLogicalLinker this[int index] { get { return linker_list[index]; } }

        /// <summary>
        /// 新增一個 算術邏輯運算式 .
        /// </summary>
        /// <param name="Exp"></param>
        /// <param name="ArithmeticLink"></param>
        internal void Add(ArithmeticLogicalLink ArithmeticLink, BaseExp Exp)
        {
            if (ArithmeticLink != ArithmeticLogicalLink.Empty &&
                ArithmeticLink != ArithmeticLogicalLink.LogicalLink &&
                ArithmeticLink != ArithmeticLogicalLink.ArithmeticLink)   // Empty,LogicalLink , ArithmeticLink 為無效的連結子
            {
                ArithmeticLogicalLinker LastLinker = linker_list[linker_list.Count - 1];
                LastLinker.Linker = ArithmeticLink;
                if (Exp is ArithmeticLogicalExp)
                {
                    // 合併 多個 ArithmeticLogicalExp 為單一算術邏輯運算式 ,
                    Debug.Print("Message : 合併 多個 ArithmeticLogicalExp 為單一算術邏輯運算式  ");
                    ArithmeticLogicalExp ALExp = (ArithmeticLogicalExp)Exp;
                    foreach (var linker in ALExp)
                    {
                        linker_list.Add(linker);
                    }
                }
                else
                {
                    linker_list.Add(new ArithmeticLogicalLinker(Exp, ArithmeticLogicalLink.Empty));
                }
            }
            else
            {
                Debug.Print("ERROR : Empty,LogicalLink , ArithmeticLink 為無效的連結子  ");
            }
        }

        /// <summary>
        /// 輸出運算式的字串表示式
        /// </summary>
        /// <param name="CommandBuilder">運算式的字串表示式產生器</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public override void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName)
        {
            foreach (ArithmeticLogicalExp.ArithmeticLogicalLinker linker in linker_list)
            {
                linker.Expression.Export(CommandBuilder, BuildType, ParameterMode, Exporter, ForceLowerName);
                if (!linker.IsTerminalLinker)
                {
                    CommandBuilder.AppendToken(linker.Linker.EnumConvert<ArithmeticLogicalLink>());
                }
            }
        }

        /// <summary>
        /// 傳回會逐一查看集合的列舉程式。
        /// </summary>
        /// <returns>型別：System.Collections.IEnumerator  物件，用於逐一查看集合。</returns>
        public IEnumerator<ArithmeticLogicalExp.ArithmeticLogicalLinker> GetEnumerator()
        {
            return linker_list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return linker_list.GetEnumerator();
        }
    }
}