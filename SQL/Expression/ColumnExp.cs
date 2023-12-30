// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// ��@���w�q. �B�⦡  [table alisaname].[column name]
    /// </summary>
    public class ColumnExp : TerminalExp
    {
        private string table_alias;
        private string name;

        internal ColumnExp(StringObjectName k123)
        {
            switch (k123.KeyIndex)
            {
                case StringObjectName.KeyIndexType.K1:
                    name = SQLHelper.UnpackObjectName(k123.Name1);
                    break;

                case StringObjectName.KeyIndexType.K2:
                    table_alias = SQLHelper.UnpackObjectName(k123.Name1);
                    name = SQLHelper.UnpackObjectName(k123.Name2);
                    break;
                    //default:
                    //    throw new SqlModelSyntaxException("ColumnExp Failed.");
            }
        }

        /// <summary>
        /// ���O�W
        /// </summary>
        public string TableAlias
        {
            get { return table_alias; }
            set { table_alias = value; }
        }

        /// <summary>
        /// �^�Ǫ����W [table alisaname].[column name]
        /// </summary>
        public string FullName
        {
            get
            {
                if (table_alias == null)
                    return SQLHelper.PackObjectName(ColumnName);
                return string.Format("{0}.{1}", SQLHelper.PackObjectName(TableAlias), SQLHelper.PackObjectName(ColumnName));
            }
        }

        /// <summary>
        /// ���W��
        /// </summary>
        public string ColumnName
        {
            get { return name; }
            set { name = value; }
        }

        public override void Export(CommandBuilder sb, CommandBuildType BuildType, ParameterMode pMode, ExportParameter Exporter, bool ForceLowerName)
        {
            if (ForceLowerName)
                sb.AppendToken(FullName?.ToLower());
            else
                sb.AppendToken(FullName);
        }

        /// <summary>
        /// �Ǧ^��ܥثe���󪺦r��C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ColumnExp:{0}", FullName);
        }
    }
}