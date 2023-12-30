// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    ///  ��ܤ@�ӥi�ѪR�������B�⦡ Example : A + B
    ///  ��k�𪺰�¦���O
    ///  �D�׺ݹB�⤸
    ///       �|�h�B��Ÿ�    +-*/  (�G���B��l)
    ///       Function �B�⤸   F(p1,...)
    ///       �޿�B�⤸ . Logical:   and or  not
    ///  �׺ݹB�⤸
    ///      �`�ƭ�. �ƭ�. �r��
    ///      �ܧ�W��/���W��
    /// </summary>
    public abstract class BaseExp : IExpression
    {
        #region IExpression Members

        /// <summary>
        /// �W�h���B�⤸
        /// </summary>
        /// <returns></returns>
        public BaseExp Top
        {
            get;
            protected set;
        }

        /// <summary>
        /// �Ǧ^�O�_���׺ݹB��l
        /// </summary>
        public abstract bool IsTerminalExp { get; }

        /// <summary>
        /// ��X�B�⦡���r���ܦ�
        /// </summary>
        /// <param name="CommandBuilder">�B�⦡���r���ܦ����;�</param>
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