// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// ���B�⦡����, ���|�۰ʥѦr��B�⦡�ѪR�����B�⦡����.
    /// </summary>
    public class ColumnExpression : IExpression
    {
        private BaseExp baseExp;

        /// <summary>
        /// ���B�⦡���󪫥�غc�l
        /// </summary>
        /// <param name="expression">���B�⦡�r��</param>
        public ColumnExpression(string expression)
        {
           
            baseExp = (new SyntaxAnalysis(new SQLTokenScanner(expression))).Analysis();
        }

        /// <summary>
        ///  ���B�⦡���󪫥�غc�l�غc�l
        /// </summary>
        /// <param name="Column">���B�⦡����</param>
        /// <returns></returns>
        public static explicit operator BaseExp(ColumnExpression Column)
        {
            return Column.Expression;
        }

        /// <summary>
        /// ���B�⦡����
        /// </summary>
        public BaseExp Expression { get { return baseExp; } }

        /// <summary>
        ///  �^�ǹB�⦡����.
        /// </summary>
        /// <typeparam name="T">�B�⦡���󫬧O</typeparam>
        /// <returns></returns>
        public T Exp<T>() where T : BaseExp
        {
            return (T)(baseExp as T);
        }

        /// <summary>
        /// ��X�B�⦡���r���ܦ�
        /// </summary>
        /// <param name="CommandBuilder">�B�⦡���r���ܦ����;�</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName)
        {
            baseExp.Export(CommandBuilder, BuildType, ParameterMode, Exporter, ForceLowerName);
        }

        /// <summary>
        /// ��X�B�⦡����r��
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            CommandBuilder cb = new CommandBuilder();
            Export(cb, CommandBuildType.SQLCTL, ParameterMode.Parameter, null , false);
            return cb.ToString();
        }

        /// <summary>
        /// ���ժ��B�⦡
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="pMode">���� ParameterMode ��k.</param>
        /// <param name="Exporter"></param>
        /// <returns></returns>
        public string RebuildExpression(CommandBuildType BuildType, ParameterMode pMode, ExportParameter Exporter = null , bool ForceLowerName = false)
        {
            CommandBuilder cb = new CommandBuilder();
            Export(cb, BuildType , pMode, Exporter, ForceLowerName);
            return cb.ToString();
        }

        /// <summary>
        /// �Ǧ^�O�_���׺ݹB��l
        /// </summary>
        public bool IsTerminalExp
        {
            get { return baseExp.IsTerminalExp; }
        }
    }
}