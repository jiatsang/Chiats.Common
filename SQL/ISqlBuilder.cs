// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// �� SQL CommandText �����e���;�
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// �̹w�]�� BuildType ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// �j��� BuildType ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="buildFlags"></param>
        /// <returns></returns>
        string BuildCommand(CommandBuildType BuildType, CommandFormatOptions buildFlags = CommandFormatOptions.None);
    }
}