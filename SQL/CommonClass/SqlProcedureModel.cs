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
        /// 預設建構子
        /// </summary>
        public SqlProcModel() { }

        /// <summary>
        /// 依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="BuildType">指示建立 SQL 命令的文字敘述的方式</param>
        /// <param name="buildFlags"></param>
        /// <returns>SQL 命令的文字敘述</returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions buildFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            throw new NotImplementedException();
        }
    }
}