// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 依 SQL CommandText 的內容產生器
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 依預設的 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// 強制依 BuildType 重新建立 SQL 命令的文字敘述.
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="buildFlags"></param>
        /// <returns></returns>
        string BuildCommand(CommandBuildType BuildType, CommandFormatOptions buildFlags = CommandFormatOptions.None);
    }
}