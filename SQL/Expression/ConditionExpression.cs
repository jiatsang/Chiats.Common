// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// 條件運算式 .
    /// </summary>
    public class ConditionExpression : IExpression
    {
        private string stringExpression;
        private BaseExp baseExp;

        public ConditionExpression(string expression)
        {
            stringExpression = expression;
            baseExp = (new SyntaxAnalysis(new SQLTokenScanner(expression))).Analysis();
        }

        public bool IsTerminalExp
        {
            get { return baseExp.IsTerminalExp; }
        }

        /// <summary>
        /// 輸出條件運算式文孛內容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            CommandBuilder sb = new CommandBuilder();

            baseExp.Export(sb, CommandBuildType.SQLCTL, ParameterMode.Parameter, null , false);

            return sb.ToString();
        }

        /// <summary>
        /// 條件運算式轉換程序
        /// </summary>
        /// <param name="Condition">條件運算式</param>
        /// <returns></returns>
        public static explicit operator BaseExp(ConditionExpression Condition)
        {
            return Condition.Expression;
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
        /// 重組的運算式
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="pMode">重組 ParameterMode 方法.</param>
        /// <param name="Exporter"></param>
        /// <returns></returns>
        public string RebuildExpression(CommandBuildType BuildType, ParameterMode pMode, ExportParameter Exporter = null, bool ForceLowerName = false)
        {
            CommandBuilder cb = new CommandBuilder();
            Export(cb, BuildType, pMode, Exporter, ForceLowerName);
            return cb.ToString();
        }
    }
}