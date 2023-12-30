// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Diagnostics;

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// �ѼƩw�q. �B�⦡  @KeyName
    /// </summary>
    public class ParameterExp : TerminalExp
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        public ParameterExp(string name)
        {
            Debug.Assert(name != null && name.StartsWith("@"));
            this.name = name;
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ParameterExp Name='{0}'", name);
        }

        /// <summary>
        /// ��X�B�⦡���r���ܦ�
        /// </summary>
        /// <param name="sb">�B�⦡���r���ܦ����;�</param>
        /// <param name="BuildType"></param>
        /// <param name="pMode"></param>
        /// <param name="Exporter"></param>
        public override void Export(CommandBuilder sb, CommandBuildType BuildType, ParameterMode pMode, ExportParameter Exporter , bool ForceLowerName)
        {
            sb.AppendToken(this.name);
        }
    }
}