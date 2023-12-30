// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// �������ѪR�B�⦡ - �L����B�⦡�@�����ѪR �u�O�|��ª����X�Ѽ��ܼƨä��\���]�᭫�չB�⦡.
    /// </summary>
    public class NonCompleteExpression
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SQLTokenScanner list;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NamedCollection<Parameter> Params = new NamedCollection<Parameter>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IPartSqlModel parent = null;

        /// <summary>
        /// �������ѪR�B�⦡�غc�l
        /// </summary>
        /// <param name="parent">���ݪ� SqlModel</param>
        /// <param name="expression">�ѪR�B�⦡�r��</param>
        public NonCompleteExpression(IPartSqlModel parent, string expression)
        {
            this.parent = parent;
            list = new SQLTokenScanner(expression);
            foreach (Token token in list)
            {
                if (token.Type == TokenType.Keyword && token.String.StartsWith("@"))
                {
                    // �ۦP�Ѽƥu�[�J�@��.
                    if (Params.GetIndexByName(token.String) == -1)
                        Params.Add(new Parameter(token.String));
                }
            }
        }

        internal void Attach(string expression)
        {
            SQLTokenScanner newList = new SQLTokenScanner(expression);
            foreach (Token token in newList)
            {
                list.Attach(token);
                if (token.Type == TokenType.Keyword && token.String.StartsWith("@"))
                {
                    // �ۦP�Ѽƥu�[�J�@��.
                    if (Params.GetIndexByName(token.String) == -1)
                        Params.Add(new Parameter(token.String));
                }
            }
        }

        internal void Attach(ConditionLink link, string conditionExpression)
        {
            list.Attach(new Token(TokenType.Keyword, (link == ConditionLink.And) ? "AND" : "OR", -1, -1));
            Attach(conditionExpression);
        }

        /// <summary>
        /// �^�ǸѪR�᪺�Ѽ��ܼƦC.
        /// </summary>
        public NamedCollection<Parameter> Parameters
        {
            get { return Params; }
        }

        /// <summary>
        /// ���^���ի᪺�B�⦡ , CommandBuildType/ParameterMode �O�̩��ݪ� SqlModel �M�w. �p�G�L����SqlModel �h�H CommandBuildType.SQL, ParameterMode.Expand.
        /// </summary>
        /// <returns></returns>
        public string StringExpression()
        {
            SqlModel TopModel = parent?.GetTopModel();
            if (TopModel != null)
                return RebuildExpression(TopModel.BuildType, TopModel.ParameterMode, DefaultExporter , TopModel.Options);
            return RebuildExpression(CommandBuildType.SQL, ParameterMode.Expand, DefaultExporter, TopModel.Options);
        }

        public SqlModel TopModel
        {
            get
            {
                    return  parent?.GetTopModel();
            }
        }

        /// <summary>
        /// �t�ιw�]����X��k
        /// </summary>
        /// <param name="ExpressionBuilder"></param>
        /// <param name="e"></param>
        public static void DefaultExporter(CommandBuilder ExpressionBuilder, ExportParameterEventArgs e)
        {
            // TODO : �\�૬������ (Condition)�W��
            if (e.pMode == ParameterMode.Expand)
            {
                if (e.Parameter != null)
                {
                    switch (e.Parameter.ColumnType)
                    {
                        case ColumnType.Double:
                        case ColumnType.Single:
                        case ColumnType.Int32:
                        case ColumnType.Int64:
                        case ColumnType.Decimal:
                            ExpressionBuilder.AppendToken($"{e.Parameter.Value}");
                            break;

                        default:
                            ExpressionBuilder.AppendToken($"N'{ e.Parameter.Value}'");
                            break;
                    }
                }
            }
            else
                ExpressionBuilder.AppendToken(e.Name);
        }

        /// <summary>
        /// �̫��w����(BuildType,ParameterMode)���չB�⦡�r��.
        /// </summary>
        /// <param name="BuildType">���� CommandBuildType ��k.</param>
        /// <param name="pMode">���� ParameterMode ��k.</param>
        /// <param name="Exporter"></param>
        /// <returns></returns>
        public string RebuildExpression(CommandBuildType BuildType, ParameterMode pMode, ExportParameter Exporter , SqlOptions options)
        {
            CommandBuilder ExpressionBuilder = new CommandBuilder();
            foreach (Token token in list)
            {
                if (BuildType != CommandBuildType.SQLCTL)
                {
                    if (token.Type == TokenType.Keyword && token.String.StartsWith("@"))
                    {
                        Parameter parameter = null;
                        int index = Params.GetIndexByName(token.String);
                        if (index != -1) parameter = Params[index];
                        if (Exporter == null) Exporter = DefaultExporter;
                        Exporter(ExpressionBuilder, new ExportParameterEventArgs(token.String, BuildType, pMode, parameter));
                        continue;
                    }
                }
                // CommandBuildType.CTLSQL �ĥΪ�����X�W��. �]���n�ŦX CTLSQL �W�d.            
                ExpressionBuilder.AppendToken(token);
            }
            return ExpressionBuilder.ToString();
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StringExpression();
        }
    }
}