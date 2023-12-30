// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// 欄位運算式物件, 它會自動由字串運算式解析成欄位運算式物件.
    /// </summary>
    public class ColumnExpression : IExpression
    {
        private BaseExp baseExp;

        /// <summary>
        /// 欄位運算式物件物件建構子
        /// </summary>
        /// <param name="expression">欄位運算式字串</param>
        public ColumnExpression(string expression)
        {
           
            baseExp = (new SyntaxAnalysis(new SQLTokenScanner(expression))).Analysis();
        }

        /// <summary>
        ///  欄位運算式物件物件建構子建構子
        /// </summary>
        /// <param name="Column">欄位運算式物件</param>
        /// <returns></returns>
        public static explicit operator BaseExp(ColumnExpression Column)
        {
            return Column.Expression;
        }

        /// <summary>
        /// 欄位運算式物件
        /// </summary>
        public BaseExp Expression { get { return baseExp; } }

        /// <summary>
        ///  回傳運算式物件.
        /// </summary>
        /// <typeparam name="T">運算式物件型別</typeparam>
        /// <returns></returns>
        public T Exp<T>() where T : BaseExp
        {
            return (T)(baseExp as T);
        }

        /// <summary>
        /// 輸出運算式的字串表示式
        /// </summary>
        /// <param name="CommandBuilder">運算式的字串表示式產生器</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName)
        {
            baseExp.Export(CommandBuilder, BuildType, ParameterMode, Exporter, ForceLowerName);
        }

        /// <summary>
        /// 輸出運算式物件字串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            CommandBuilder cb = new CommandBuilder();
            Export(cb, CommandBuildType.SQLCTL, ParameterMode.Parameter, null , false);
            return cb.ToString();
        }

        /// <summary>
        /// 重組的運算式
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="pMode">重組 ParameterMode 方法.</param>
        /// <param name="Exporter"></param>
        /// <returns></returns>
        public string RebuildExpression(CommandBuildType BuildType, ParameterMode pMode, ExportParameter Exporter = null , bool ForceLowerName = false)
        {
            CommandBuilder cb = new CommandBuilder();
            Export(cb, BuildType , pMode, Exporter, ForceLowerName);
            return cb.ToString();
        }

        /// <summary>
        /// 傳回是否為終端運算子
        /// </summary>
        public bool IsTerminalExp
        {
            get { return baseExp.IsTerminalExp; }
        }
    }
}