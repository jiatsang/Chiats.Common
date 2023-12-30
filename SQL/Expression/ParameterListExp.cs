// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// �B�⦡�ѼƦC  Example: a,b,c ....
    /// </summary>
    /// <summary>
    /// SQL ��ƹB�⦡ Example : Count(*)
    /// </summary>
    public class ParameterListExp : NonTerminalExp
    {
        /// <summary>
        /// �B�⦡�ѼƦC, �B�⦡�l����}�C
        /// </summary>
        public BaseExp[] Parameters { get; protected set; }

        /// <summary>
        /// �B�⦡�ѼƦC����غc�l
        /// </summary>
        public ParameterListExp()
        {
            this.Parameters = new BaseExp[0];
        }

        /// <summary>
        /// �B�⦡�ѼƦC����غc�l , ��@�B�⦡�l����
        /// </summary>
        /// <param name="Parameters"></param>
        public ParameterListExp(BaseExp Parameters)
        {
            if (Parameters is ParameterListExp)
            {
                ParameterListExp ParameterList = (ParameterListExp)Parameters;
                this.Parameters = ParameterList.Parameters;
            }
            else
            {
                this.Parameters = new BaseExp[1];
                this.Parameters[0] = Parameters;
            }
        }

        /// <summary>
        /// �B�⦡�ѼƦC����غc�l , �h�ӹB�⦡�l����
        /// </summary>
        /// <param name="Parameters"></param>
        public ParameterListExp(BaseExp[] Parameters)
        {
            if (Parameters != null)
                this.Parameters = Parameters;
            else
                this.Parameters = new BaseExp[0];
        }

        /// <summary>
        /// ��X�B�⦡���r���ܦ�
        /// </summary>
        /// <param name="CommandBuilder">�B�⦡���r���ܦ����;�</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public override void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName)
        {
            bool IsFirstParameter = true;
            foreach (BaseExp ParamExp in Parameters)
            {
                if (!IsFirstParameter) CommandBuilder.Append(',');
                ParamExp.Export(CommandBuilder, BuildType, ParameterMode, Exporter , ForceLowerName);
                IsFirstParameter = false;
            }
        }
    }
}