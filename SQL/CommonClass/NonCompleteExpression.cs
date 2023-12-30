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
    /// 不完全解析運算式 - 他不對運算式作正式解析 只是會單純的取出參數變數並允許重設後重組運算式.
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
        /// 不完全解析運算式建構子
        /// </summary>
        /// <param name="parent">所屬的 SqlModel</param>
        /// <param name="expression">解析運算式字串</param>
        public NonCompleteExpression(IPartSqlModel parent, string expression)
        {
            this.parent = parent;
            list = new SQLTokenScanner(expression);
            foreach (Token token in list)
            {
                if (token.Type == TokenType.Keyword && token.String.StartsWith("@"))
                {
                    // 相同參數只加入一個.
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
                    // 相同參數只加入一個.
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
        /// 回傳解析後的參數變數列.
        /// </summary>
        public NamedCollection<Parameter> Parameters
        {
            get { return Params; }
        }

        /// <summary>
        /// 取回重組後的運算式 , CommandBuildType/ParameterMode 是依所屬的 SqlModel 決定. 如果無所屬SqlModel 則以 CommandBuildType.SQL, ParameterMode.Expand.
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
        /// 系統預設的輸出方法
        /// </summary>
        /// <param name="ExpressionBuilder"></param>
        /// <param name="e"></param>
        public static void DefaultExporter(CommandBuilder ExpressionBuilder, ExportParameterEventArgs e)
        {
            // TODO : 功能型的條件式 (Condition)名稱
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
        /// 依指定條件(BuildType,ParameterMode)重組運算式字串.
        /// </summary>
        /// <param name="BuildType">重組 CommandBuildType 方法.</param>
        /// <param name="pMode">重組 ParameterMode 方法.</param>
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
                // CommandBuildType.CTLSQL 採用直接輸出名稱. 因為要符合 CTLSQL 規範.            
                ExpressionBuilder.AppendToken(token);
            }
            return ExpressionBuilder.ToString();
        }

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StringExpression();
        }
    }
}