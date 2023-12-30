// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// ��ƹB�⦡ Example : Count(*)
    /// </summary>
    public class FunctionExp : ParameterListExp
    {
        /// <summary>
        /// ��ƦW��
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// ��ƹB�⦡�غc�l
        /// </summary>
        /// <param name="Name">��ƦW��</param>
        /// <param name="Parameters">��@�Ѽ�</param>
        public FunctionExp(string Name, BaseExp Parameters) : base(Parameters) { this.Name = Name; }

        /// <summary>
        /// ��ƹB�⦡�غc�l
        /// </summary>
        /// <param name="Name">��ƦW��</param>
        /// <param name="Parameters">�ѼƦC</param>
        public FunctionExp(string Name, BaseExp[] Parameters) : base(Parameters) { this.Name = Name; }

        /// <summary>
        /// ��ƹB�⦡�غc�l
        /// </summary>
        /// <param name="Name">��ƦW��</param>
        public FunctionExp(string Name) { this.Name = Name; }

        /// <summary>
        /// ��X�B�⦡���r���ܦ�
        /// </summary>
        /// <param name="CommandBuilder">�B�⦡���r���ܦ����;�</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public override void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName)
        {
            CommandBuilder.AppendToken(Name);
            CommandBuilder.Append('(');
            base.Export(CommandBuilder, BuildType, ParameterMode, Exporter, ForceLowerName);
            CommandBuilder.Append(')');
        }
    }
}