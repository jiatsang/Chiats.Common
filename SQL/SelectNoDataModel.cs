// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 取得無資料的 SelectModel , 即以 GetNoDataCommandText 取代 GetCommandText 方法.
    /// </summary>
    internal sealed class SelectNoDataModel : ICommandBuilder
    {
        private SelectModel select = null;

        /// <summary>
        /// 取得無資料的 SelectModel 建構子
        /// </summary>
        /// <param name="select"></param>
        public SelectNoDataModel(SelectModel select) { this.select = select; }

        #region ICommandBuilder Members

        /// <summary>
        /// 傳回 Database SQL Command 的執行命令.
        /// </summary>
        public string CommandText
        {
            get { return select.CommandTextForNoResultData(CommandFormatOptions.None); }
        }

        /// <summary>
        /// 傳回 Database SQL Command 的型態. 如標準 SQL Command , Table , StoredProcedure
        /// </summary>
        public System.Data.CommandType CommandType
        {
            get { return ((ICommandBuilder)select).CommandType; }
        }

        #endregion ICommandBuilder Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="formatFlags"></param>
        /// <returns></returns>
        public string BuildCommand(CommandBuildType BuildType, CommandFormatOptions formatFlags)
        {
            return select.CommandTextForNoResultData(formatFlags);
        }
    }
}