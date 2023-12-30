// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// ���o�L��ƪ� SelectModel , �Y�H GetNoDataCommandText ���N GetCommandText ��k.
    /// </summary>
    internal sealed class SelectNoDataModel : ICommandBuilder
    {
        private SelectModel select = null;

        /// <summary>
        /// ���o�L��ƪ� SelectModel �غc�l
        /// </summary>
        /// <param name="select"></param>
        public SelectNoDataModel(SelectModel select) { this.select = select; }

        #region ICommandBuilder Members

        /// <summary>
        /// �Ǧ^ Database SQL Command ������R�O.
        /// </summary>
        public string CommandText
        {
            get { return select.CommandTextForNoResultData(CommandFormatOptions.None); }
        }

        /// <summary>
        /// �Ǧ^ Database SQL Command �����A. �p�з� SQL Command , Table , StoredProcedure
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