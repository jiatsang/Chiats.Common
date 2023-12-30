// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// 常數值運算式  Example :
    /// </summary>
    public class ConstantExp : TerminalExp
    {
        private string value;
        // private bool minusSymbol = false;
        public ConstantExp(string value, bool IsConstantString) { this.value = value; this.IsConstantString = IsConstantString; }

        public string ConstantValue
        {
            get { return value; }
        }

        public bool IsConstantString
        {
            //get { return value != null && value.StartsWith("'") && value.EndsWith("'"); }
            get;
            private set;
        }

        public bool HasMinusSymbol
        {
            get;
            internal set;
        }

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ConstantExp:{0}", value);
        }

        /// <summary>
        /// 輸出運算式的字串表示式
        /// </summary>
        /// <param name="CommandBuilder">運算式的字串表示式產生器</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public override void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter, bool ForceLowerName)
        {
            if (IsConstantString)
                CommandBuilder.AppendToken($"N'{ConstantValue}'");
            else
                CommandBuilder.AppendToken(ConstantValue);
        }
    }
}