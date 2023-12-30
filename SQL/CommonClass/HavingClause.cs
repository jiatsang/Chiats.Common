// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// SQL Model for Having
    /// </summary>
    public class HavingClause : WhereClause
    {
        /// <summary>
        /// Having SQL Model �غc�l
        /// </summary>
        /// <param name="parent"></param>
        public HavingClause(SqlModel parent) : base(parent) { }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ISqlBuildExport BuildExport = DefaultBuildExport.SQLBuildExport;
            CommandBuilder CommandBuilder = new CommandBuilder();
            BuildExport.ExportForWhereClause(this, CommandBuilder, this.Parent, BuildExportOptions.None);
            return CommandBuilder.ToString();
        }

        /// <summary>
        /// �^�� "HAVING"
        /// </summary>
        public override string ClauseKey
        {
            get
            {
                return "HAVING";
            }
        }
    }
}