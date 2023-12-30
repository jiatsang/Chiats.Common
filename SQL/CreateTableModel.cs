// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// CreateTableModel �զ��������O. ��h�W�O�H�䴩�з� SQL Create Table �y�k����¦���O.
    /// </summary>
    public sealed class CreateTableModel : SqlModel
    {
        private TableName table;

        //IndexDescriptionCollection

        /// <summary>
        /// SelectModel �غc�l
        /// </summary>
        /// <param name="CTLSQL"></param>
        public CreateTableModel(string CTLSQL)
            : this()
        {
            SQLTokenScanner list = new SQLTokenScanner(CTLSQL);
            CreateTableParser.Default.PaserCommand(list, this);
        }

        /// <summary>
        /// ���W��
        /// </summary>
        public TableName Table
        {
            get { return table; }

            set
            {
                this.Changed();
                table = value;
            }
        }

        private readonly IndexDescriptionCollection indexes;
        private readonly ColumnDescriptionCollection columns;

        /// <summary>
        ///
        /// </summary>
        public CreateTableModel()
        {
            columns = new ColumnDescriptionCollection(this);
            indexes = new IndexDescriptionCollection(this);
        }

        /// <summary>
        ///
        /// </summary>
        public ColumnDescriptionCollection Columns
        {
            get { return columns; }
        }

        /// <summary>
        ///
        /// </summary>
        public IndexDescriptionCollection Indexes
        {
            get { return indexes; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="formatFlags"></param>
        /// <returns></returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions formatFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            ISqlBuildExport BuildExport = GetBuildExport(BuildType, formatFlags, buildExport);
            CommandBuilder CommandBuilder = new CommandBuilder(formatFlags);

            CommandBuilder.AppendToken($"CREATE TABLE {Table.FullName}");
            CommandBuilder.AppendToken(" (");

            if (formatFlags.HasFlag(CommandFormatOptions.AutoFormat)) CommandBuilder.Append("\r\n\t");

            BuildExport.ExportForColumns(Columns, CommandBuilder, this);

            // CONSTRAINT [PK_ieac07h] PRIMARY KEY NONCLUSTERED (uid ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON PRIMARY

            BuildExport.ExportForIndexes(Indexes, CommandBuilder, this);

            if (formatFlags.HasFlag(CommandFormatOptions.AutoFormat)) CommandBuilder.Append("\r\n");

            CommandBuilder.AppendToken(" )");
            return CommandBuilder.ToString();
        }
    }
}