// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// 函數運算式 Example : Count(*)
    /// </summary>
    public class FunctionExp : ParameterListExp
    {
        /// <summary>
        /// 函數名稱
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 函數運算式建構子
        /// </summary>
        /// <param name="Name">函數名稱</param>
        /// <param name="Parameters">單一參數</param>
        public FunctionExp(string Name, BaseExp Parameters) : base(Parameters) { this.Name = Name; }

        /// <summary>
        /// 函數運算式建構子
        /// </summary>
        /// <param name="Name">函數名稱</param>
        /// <param name="Parameters">參數列</param>
        public FunctionExp(string Name, BaseExp[] Parameters) : base(Parameters) { this.Name = Name; }

        /// <summary>
        /// 函數運算式建構子
        /// </summary>
        /// <param name="Name">函數名稱</param>
        public FunctionExp(string Name) { this.Name = Name; }

        /// <summary>
        /// 輸出運算式的字串表示式
        /// </summary>
        /// <param name="CommandBuilder">運算式的字串表示式產生器</param>
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