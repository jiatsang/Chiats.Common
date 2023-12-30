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
    /// 為 SqlModel 產生 SQL 敘述的實作介面.  BuildExport for OleDb ,  未驗証的版本.
    /// </summary>
    public class OleDbBuildExport : ISqlBuildExport
    {
        private CommandBuildType buildType = CommandBuildType.SQL;

        /// <summary>
        /// 強制轉換成小寫名稱. 欄位名稱/表格名稱
        /// </summary>
        public SqlOptions Options { get; set; }
        /// <summary>
        ///
        /// </summary>
        public CommandBuildType BuildType
        {
            get { return buildType; }
            set { buildType = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public CommandBuildOptions BuildOptions { get; set; }

        public CommandFormatOptions FormatOptions { get; set; }
        public IDbInformation DbInformation { get; set; }

        /// <summary>
        /// 產生 ColumnValue SQL 敘述的實作介面. Example Column1=Vaule1,Column2=Vaule2
        /// </summary>
        /// <param name="column"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForSetValues(ColumnValue column, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            CommandBuilder.AppendToken($"{column.Name}={ExportValue(column, sql_model, CommandBuildType.SQL, ExportParameter)}");
            return 1;
        }

        /// <summary>
        /// 輸出欄位運算式.
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public string ExportValue(ColumnValue column, SqlModel sql_model, CommandBuildType BuildType, ExportParameter Parameter)
        {
            //TODO: 增加一個 系統參數表  記載當資料為 NULL 或 空值時 要如何輸出 .
            if (column.Value != null)
            {
                if (ColumnTypeHelper.IsNumber(column.ColumnType))
                {
                    if (string.IsNullOrWhiteSpace(column.Value))
                        return "0";    // 數值型態 - 固定輸出 "0"
                    return $"{column.Value}";
                }
                if (sql_model.Options.HasFlag(SqlOptions.NonStringNational))
                    return $"'{column.Value}'";
                return $"N'{column.Value}'";   //TODO: 支援 unicode 輸出
            }

            if (column.Expression != null)
                return column.RebuildExpression(BuildType, sql_model.ParameterMode, Parameter, sql_model.Options);

            if (ColumnTypeHelper.IsNumber(column.ColumnType))
                return "0";  // 數值型態 - 固定輸出 "0"
            else
                return "''";
        }

        /// <summary>
        /// 產生 ColumnValue SQL 敘述的實作介面. Example Vaule1,Vaule2
        /// </summary>
        /// <param name="column"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForInsertValues(ColumnValue column, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            CommandBuilder.AppendToken(
                ExportValue(column, sql_model, CommandBuildType.SQL, ExportParameter));
            return 1;
        }

        /// <summary>
        /// 產生 WhereClause/HavingClause 敘述的實作介面
        /// </summary>
        /// <param name="where"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForWhereClause(WhereClause where, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int case_count = 0;
            ParameterMode Mode = where.GetTopModel().ParameterMode;
            for (int i = 0; i < where.Conditions.Count; i++)
            {
                Condition Condition = where.Conditions[i];

                if (Condition.ExportEnabled(Mode) || BuildType == CommandBuildType.SQLCTL)
                {
                    if (case_count == 0)
                    {
                        if (Options.HasFlag(BuildExportOptions.ExportKeyword))
                            CommandBuilder.AppendToken(where.ClauseKey); // Support Where/Having
                    }
                    else
                        CommandBuilder.AppendKeywordToken((Condition.Link == ConditionLink.And) ? "AND" : "OR");

                    CommandBuilder.AppendToken(
                        Condition.ConditionExpression(CommandBuildType.SQL, Mode, ExportParameter)
                        );
                    case_count++;
                }
            }
            return case_count;
        }

        private void ExportParameter(CommandBuilder CommandBuilder, ExportParameterEventArgs e)
        {
            if (e.ArgmentType == ArgumentType.None)
            {
                if (e.pMode == ParameterMode.Expand)
                {
                    if (e.Parameter != null)
                    {
                        // TODO: 依據 Parameter 的資訊展開
                        switch (e.Parameter.ColumnType)
                        {
                            case ColumnType.Double:
                            case ColumnType.Single:
                            case ColumnType.Int32:
                            case ColumnType.Int64:
                            case ColumnType.Decimal:
                                CommandBuilder.AppendToken($"{ e.Parameter.Value}"); break;
                            case ColumnType.DateTime:
                                // CAST('01-JAN-2009' AS DATETIME) for SQL Server
                                CommandBuilder.AppendToken($"'{ e.Parameter.Value:yyyy-MM-dd HH:mm:ss}'");
                                break;

                            default:
                                CommandBuilder.AppendToken($"N'{e.Parameter.Value}'"); break;
                        }
                    }
                    else
                        CommandBuilder.AppendToken("''");
                }
                else
                {
                    // 將 @ 換成 :
                    CommandBuilder.AppendToken($":{e.Name.Substring(1)}");
                }
            }
            else
            {
                int ParamIndex = 0;
                Func<IParameterValue, int> ExportParamValue = (pv) =>
                {
                    /* 功能型的 ColumnExpression 不會含有 Parameter 變數 */
                    string Column = e.ColumnExpression.RebuildExpression(
                        CommandBuildType.SQL,
                        ParameterMode.Parameter, null);

                    if (ParamIndex != 1) CommandBuilder.AppendKeywordToken("OR");

                    IParameterScopeValue psv = pv as IParameterScopeValue;
                    if (psv != null)
                    {
                        // {$Name:Socpe T,@T1} ==> {$ T>=@T1#From and T<=@T1#To}
                        CommandBuilder.AppendToken("(");

                        if (e.pMode == ParameterMode.Expand)
                        {
                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendToken(">=");
                            CommandBuilder.AppendToken($"'{psv.FromStringValue}'");
                            CommandBuilder.AppendKeywordToken("AND");

                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendToken("<=");
                            CommandBuilder.AppendToken($"'{psv.ToStringValue}'");
                        }
                        else
                        {
                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendToken(">=");
                            // 將 @ 換成 :
                            CommandBuilder.AppendToken($":{e.Name.Substring(1)}#M{ParamIndex}From");
                            CommandBuilder.AppendToken("and");
                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendToken("<=");
                            // 將 @ 換成 :
                            CommandBuilder.AppendToken($":{e.Name.Substring(1)}#M{ParamIndex}To");
                        }
                        CommandBuilder.AppendToken(")");
                    }
                    else
                    {
                        // {$Name:Mutil T,@T1}  ==> {$ T=@T1#M1 or T=@T1#M2 or T=@T1#M3}
                        if (e.pMode == ParameterMode.Expand)
                        {
                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendToken("=");
                            CommandBuilder.AppendToken($"'{pv}'");
                        }
                        else
                        {
                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendToken("=");
                            // 將 @ 換成 :
                            CommandBuilder.AppendToken($":{e.Name.Substring(1)}#M{ParamIndex}");
                        }
                    }
                    return 0;
                };

                if (e.Parameter.ArgmentType == ArgumentType.Like)
                {
                    if (e.Parameter.Value is string)
                    {
                        string Column = e.ColumnExpression.RebuildExpression(
                                CommandBuildType.SQL,
                                ParameterMode.Parameter, null); /* 功能型的 ColumnExpression 不會含有 Parameter 變數 */

                        if (e.pMode == ParameterMode.Expand)
                        {
                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendKeywordToken("LIKE");
                            CommandBuilder.AppendToken($"'{ e.Parameter.Value}'");
                        }
                        else
                        {
                            CommandBuilder.AppendToken(Column);
                            CommandBuilder.AppendKeywordToken("LIKE");
                            CommandBuilder.AppendToken($"@{Column}");
                        }
                    }
                    else
                        throw new SqlModelParameterException("Invlaid Value" + e.Parameter.Value.GetType().Name);
                }
                else
                {
                    ParameterValues pvs = e.Parameter.Value as ParameterValues;
                    if (e.Parameter.Value is ParameterValues)
                    {
                        foreach (IParameterValue p_value in (ParameterValues)e.Parameter.Value)
                        {
                            ParamIndex++;

                            if (p_value is IParameterScopeValue && e.ArgmentType.HasFlag(ArgumentType.Scope))
                                ExportParamValue(p_value);
                            else if (p_value is IParameterSingleValue && e.ArgmentType.HasFlag(ArgumentType.Multi))
                                ExportParamValue(p_value);
                        }
                    }
                    else if (e.Parameter.Value is IParameterValue)
                    {
                        ParamIndex++;
                        IParameterValue p_value = (IParameterValue)e.Parameter.Value;

                        if (p_value is IParameterScopeValue && e.ArgmentType.HasFlag(ArgumentType.Scope))
                            ExportParamValue(p_value);
                        else if (p_value is IParameterSingleValue && e.ArgmentType.HasFlag(ArgumentType.Multi))
                            ExportParamValue(p_value);
                    }
                    else
                        throw new SqlModelParameterException("Invlaid Value" + e.Parameter.Value.GetType().Name);
                }
            }
        }

        /// <summary>
        /// 產生 OrderByClause 敘述的實作介面
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForOrderByClause(OrderByClause orderBy, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int case_count = 0;
            if (orderBy.Count > 0)
            {
                if (Options.HasFlag(BuildExportOptions.ExportKeyword)) CommandBuilder.AppendKeywordToken("ORDER BY");
                foreach (OrderByColumn key in orderBy)
                {
                    if (case_count > 0) CommandBuilder.AppendToken(',');
                    CommandBuilder.AppendToken(key.ColumnName);
                    if (key.OrderSorting == OrderSorting.Descending)
                    {
                        CommandBuilder.AppendKeywordToken("DESC");
                    }
                    case_count++;
                }
            }
            return case_count;
        }

        /// <summary>
        /// 產生 ColumnsClause 敘述的實作介面
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForColumnsClause(ColumnsClause columns, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int case_count = 0; /*  記錄輸出個數 */
            foreach (Column col in columns)
            {
                CommandBuilder.Append((case_count == 0) ? "" : ",");
                // [ColumnName] as [AliasName]
                CommandBuilder.AppendToken(col.ColumnExpression);
                if (!string.IsNullOrEmpty(col.AsName))
                {
                    CommandBuilder.AppendKeywordToken("AS");
                    CommandBuilder.AppendToken(col.AsName);
                }
                case_count++;
            }
            if (case_count == 0)
            {
                case_count++; CommandBuilder.AppendToken('*');
            }
            CommandBuilder.Append(' ');
            return case_count;
        }

        /// <summary>
        /// 產生 TableClause 敘述的實作介面
        /// </summary>
        /// <param name="table"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForTableClause(TableClause table, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int case_count = 0;
            if (!table.PrimaryTable.IsEmpty)
            {
                case_count++;
                CommandBuilder.AppendToken(table.PrimaryTable.FullName);

                if (table.PrimaryTable.IsSelectModel)
                {
                    CommandBuilder.Append(' ');  // 子查詢時則加入一個空白作分隔.
                }

                if (!string.IsNullOrEmpty(table.PrimaryAliasName))
                    CommandBuilder.AppendToken(table.PrimaryAliasName);

                if (!string.IsNullOrWhiteSpace(table.PrimaryTable.Hints))
                {
                    CommandBuilder.AppendToken($"WITH ({table.PrimaryTable.Hints})");
                }

                foreach (JoinTable join in table)
                {
                    if (join.JoinType == JoinType.None)
                    {
                        CommandBuilder.AppendToken(",");
                    }
                    switch (join.JoinType)
                    {
                        case JoinType.LeftOuter: CommandBuilder.AppendKeywordToken("LEFT JOIN"); break;
                        case JoinType.RightOuter: CommandBuilder.AppendKeywordToken("RIGHT JOIN"); break;
                        case JoinType.Inner: CommandBuilder.AppendKeywordToken("INNER JOIN"); break;
                        case JoinType.Left: CommandBuilder.AppendKeywordToken("LEFT JOIN"); break;
                        case JoinType.Right: CommandBuilder.AppendKeywordToken("RIGHT JOIN"); break;
                        case JoinType.Outer: CommandBuilder.AppendKeywordToken("JOIN"); break;
                        case JoinType.CrossApply: CommandBuilder.AppendKeywordToken("CROSS APPLY"); break;
                        case JoinType.OuterApply: CommandBuilder.AppendKeywordToken("OUTER APPLY"); break;
                    }

                    CommandBuilder.AppendToken(join.Table.FullName);

                    if (!string.IsNullOrEmpty(join.Alias))
                        CommandBuilder.AppendToken(join.Alias);

                    if (!string.IsNullOrWhiteSpace(join.Table.Hints))
                    {
                        CommandBuilder.AppendToken($"WITH ({join.Table.Hints})");
                    }

                    if (join.JoinType != JoinType.None)
                    {
                        CommandBuilder.AppendKeywordToken("ON");
                        CommandBuilder.AppendToken(join.Expression);
                    }
                }
            }
            return case_count;
        }

        /// <summary>
        /// 產生 GroupByClause 敘述的實作介面
        /// </summary>
        /// <param name="groupBy"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForGroupByClause(GroupByClause groupBy, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int key_count = 0;
            if (groupBy.Count > 0)
            {
                if (Options.HasFlag(BuildExportOptions.ExportKeyword)) CommandBuilder.AppendToken("GROUP BY");
                foreach (GroupByColumn key in groupBy)
                {
                    if (key_count++ != 0)
                        CommandBuilder.AppendToken(",");
                    CommandBuilder.AppendToken(key.Name);
                }
            }
            return key_count;
        }

        #region ISqlBuildExport Members

        /// <summary>
        /// 產生 ColumnDescriptionCollection 敘述的實作介面
        /// </summary>
        /// <param name="Columns"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForColumns(ColumnDescriptionCollection Columns, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="indexes"></param>
        /// <param name="CommandBuilder"></param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForIndexes(IndexDescriptionCollection indexes, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            throw new NotImplementedException();
        }

        #endregion ISqlBuildExport Members
    }
}