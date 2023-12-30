// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// 使用不解析的 SQL Command 語法, 它會收集 @Parameter 並存至 Parameters
    /// </summary>
    public sealed class CommandModel : SqlModel
    {

        private string SQL;
        private NamedCollection<Parameter> __ps = new NamedCollection<Parameter>();
        /// <summary>
        /// SelectModel 建構子
        /// </summary>
        /// <param name="SQL"></param>
        public CommandModel(string SQL, object parameters = null)
        {

            this.SQL = SQL?.Trim();
            SQLTokenScanner list = new SQLTokenScanner(SQL);
            foreach (var token in list)
            {
                if (token.Type == TokenType.Keyword && token.String.StartsWith("@"))
                {
                    __ps.Add(new Parameter(token.String));
                }
            }
            if (__ps.Count > 0)
            {
                this.CoverParameters.AddLinker(__ps);
            }
        }


        /// <summary>
        /// 依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="BuildType">指示建立 SQL 命令的文字敘述的方式</param>
        /// <param name="formatFlags"></param>
        /// <returns>SQL 命令的文字敘述</returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions formatFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            return SQL;
        }
    }
}