// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL
{
    /// <summary>
    /// Support Store Procedure SQL Command Model
    /// </summary>
    public class SqlProcModel : SqlModel
    {
        /// <summary>
        /// �w�]�غc�l
        /// </summary>
        public SqlProcModel() { }

        /// <summary>
        /// �� BuildType ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        /// <param name="BuildType">���ܫإ� SQL �R�O����r�ԭz���覡</param>
        /// <param name="buildFlags"></param>
        /// <returns>SQL �R�O����r�ԭz</returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions buildFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            throw new NotImplementedException();
        }
    }
}