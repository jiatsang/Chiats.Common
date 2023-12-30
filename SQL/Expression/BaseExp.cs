// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    ///  表示一個可解析的的欄位運算式 Example : A + B
    ///  文法樹的基礎型別
    ///  非終端運算元
    ///       四則運算符號    +-*/  (二元運算子)
    ///       Function 運算元   F(p1,...)
    ///       邏輯運算元 . Logical:   and or  not
    ///  終端運算元
    ///      常數值. 數值. 字串
    ///      變更名稱/欄位名稱
    /// </summary>
    public abstract class BaseExp : IExpression
    {
        #region IExpression Members

        /// <summary>
        /// 上層之運算元
        /// </summary>
        /// <returns></returns>
        public BaseExp Top
        {
            get;
            protected set;
        }

        /// <summary>
        /// 傳回是否為終端運算子
        /// </summary>
        public abstract bool IsTerminalExp { get; }

        /// <summary>
        /// 輸出運算式的字串表示式
        /// </summary>
        /// <param name="CommandBuilder">運算式的字串表示式產生器</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public abstract void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName);

        #endregion IExpression Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            CommandBuilder cb = new CommandBuilder();
            cb.AppendToken(GetType().Name);
            cb.AppendToken("->");
            Export(cb, CommandBuildType.SQLCTL, ParameterMode.Parameter, null , false);
            return cb.ToString();
        }
    }
}