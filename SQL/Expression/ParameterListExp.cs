// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// 運算式參數列  Example: a,b,c ....
    /// </summary>
    /// <summary>
    /// SQL 函數運算式 Example : Count(*)
    /// </summary>
    public class ParameterListExp : NonTerminalExp
    {
        /// <summary>
        /// 運算式參數列, 運算式子物件陣列
        /// </summary>
        public BaseExp[] Parameters { get; protected set; }

        /// <summary>
        /// 運算式參數列物件建構子
        /// </summary>
        public ParameterListExp()
        {
            this.Parameters = new BaseExp[0];
        }

        /// <summary>
        /// 運算式參數列物件建構子 , 單一運算式子物件
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
        /// 運算式參數列物件建構子 , 多個運算式子物件
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
        /// 輸出運算式的字串表示式
        /// </summary>
        /// <param name="CommandBuilder">運算式的字串表示式產生器</param>
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