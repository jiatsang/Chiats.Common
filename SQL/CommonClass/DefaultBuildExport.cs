// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using Chiats.SQL.Expression;
using System;

namespace Chiats.SQL
{
    /// <summary>
    /// 為 SqlModel 產生 SQL Server 2005/2008 敘述的實作. DefaultBuildExport 是以 SQL Server 2005 為基礎制作的.
    /// </summary>
    public class DefaultBuildExport : ISqlBuildExport
    {
        public static DefaultBuildExport SQLBuildExport = new DefaultBuildExport(CommandBuildType.SQL);
        public static DefaultBuildExport SQLCTLBuildExport = new DefaultBuildExport(CommandBuildType.SQLCTL);

        private CommandBuildType buildType = CommandBuildType.SQL;

        /// <summary>
        /// 指示建立 SQL 命令的文字敘述的方式
        /// </summary>
        public CommandBuildType BuildType { get { return buildType; } }

        /// <summary>
        /// 指示依 SQL Commnad 的文字敘述選項參數處理 SQL Commnad 的文字輸出
        /// </summary>
        public CommandBuildOptions BuildOptions { get; set; }

        /// <summary>
        /// 指示依 SQL Commnad 的文字敘述格式參數處理 SQL Commnad 的文字輸出. 如自動加入換行字元
        /// </summary>
        public CommandFormatOptions FormatOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SqlOptions Options { get; set; }

        /// <summary>
        /// 取得資料庫相關參數. 當 DbInformation 為 NULL 時, 則表示因無法識別資料庫廠商或版本時而取不到資料庫相關參數. 因為資料庫相關參數是和資料庫系統不同而有所不同
        /// </summary>
        public IDbInformation DbInformation { get; set; }

        /// <summary>
        /// SQL Commnad Export 建構子
        /// </summary>
        /// <param name="BuildType"></param>
        private DefaultBuildExport(CommandBuildType BuildType)
        {
            this.buildType = BuildType;
            //this.BuildOptions = CommandBuildOptions.StringNational;
        }

        /// <summary>
        /// 產生 ColumnValue SQL 敘述的實作介面. Example Column1=Vaule1,Column2=Vaule2
        /// </summary>
        /// <param name="column"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForSetValues(ColumnValue column, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            CommandBuilder.AppendToken($"{column.Name}={ExportValue(column, sql_model, BuildType, ExportParameter)}");
            return 1;
        }

        /// <summary>)
        /// 輸出欄位運算式.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="sql_model"></param>
        /// <param name="BuildType"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public string ExportValue(ColumnValue column, SqlModel sql_model, CommandBuildType BuildType, ExportParameter Parameter)
        {
            var StringNational = !Options.HasFlag(SqlOptions.NonStringNational);
            //TODO: 增加一個 系統參數表  記載當資料為 NULL 或 空值時 要如何輸出 .
            if (column.Value != null)
            {
                if (ColumnTypeHelper.IsNumber(column.ColumnType))
                {
                    if (string.IsNullOrWhiteSpace(column.Value))
                        return "0";    // 數值型態 - 固定輸出 "0"
                    return $"{column.Value}";
                }
                if (StringNational)
                    return $"N'{Encoding(column.Value)}'";   //TODO: 支援 unicode 輸出
                return $"'{Encoding(column.Value)}'";   //TODO: 支援 unicode 輸出
            }
            else if (column.Expression != null)
                return column.RebuildExpression(BuildType, sql_model.ParameterMode, Parameter, Options);

            if (ColumnTypeHelper.IsNumber(column.ColumnType))
                return "0";  // 數值型態 - 固定輸出 "0"
            else
                return "''";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="column"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
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
        ///
        /// </summary>
        /// <param name="where"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForWhereClause(WhereClause where, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int case_count = 0;
            ParameterMode Mode = where.GetTopModel().ParameterMode;
            for (int i = 0; i < where.Conditions.Count; i++)
            {
                var Condition = where.Conditions[i];
                if (Condition.ExportEnabled(Mode) || BuildType == CommandBuildType.SQLCTL)
                {
                    if (case_count == 0)
                    {
                        if (Options.HasFlag(BuildExportOptions.ExportKeyword))
                        {
                            CommandBuilder.Unindent();
                            CommandBuilder.NewLine();
                            CommandBuilder.Indent();
                            CommandBuilder.AppendToken(where.ClauseKey); // Support Where/Having
                        }
                    }
                    else
                    {
                        switch (Condition.Link)
                        {
                            case ConditionLink.And:
                                CommandBuilder.AppendKeywordToken("AND");
                                break;

                            case ConditionLink.Or:
                                CommandBuilder.AppendKeywordToken("OR");
                                break;
                        }
                    }
                    CommandBuilder.Indent();
                    CommandBuilder.NewLine();
                    CommandBuilder.AppendToken(Condition.ConditionExpression(BuildType, Mode, ExportParameter));
                    CommandBuilder.Unindent();
                    case_count++;
                }
            }
            return case_count;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForOrderByClause(OrderByClause orderBy, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int case_count = 0;
            if (orderBy.Count > 0)
            {
                if (Options.HasFlag(BuildExportOptions.ExportKeyword))
                {
                    CommandBuilder.Unindent();
                    CommandBuilder.NewLine();
                    CommandBuilder.Indent();
                    CommandBuilder.AppendKeywordToken("ORDER BY");
                }
                foreach (OrderByColumn key in orderBy)
                {
                    if (case_count > 0) CommandBuilder.AppendToken(',');
                    if (sql_model.Options.HasFlag(SqlOptions.LowerName))
                        CommandBuilder.AppendToken(key.ColumnName?.ToLower());
                    else
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

        private string FindTableName(SelectModel select, Expression.ColumnExp ColumnExp)
        {
            // 搜尋欄位名稱的指定實際表格名稱. TableName
            // SelectModel select = sql_model as SelectModel;
            if (select != null)
            {
                if (ColumnExp.TableAlias == null)  // 未定義 TableAlias 則直接取得 PrimaryTableName
                    return select.Tables.PrimaryTable.Name;
                else
                {
                    if (select.Tables.PrimaryAliasName != null)
                    {
                        if (ColumnExp.TableAlias == select.Tables.PrimaryAliasName)
                            return select.Tables.PrimaryTable.Name;
                    }
                    else
                    {
                        if (ColumnExp.TableAlias == select.Tables.PrimaryAliasName)
                            return select.Tables.PrimaryAliasName;
                    }
                    foreach (var table in select.Tables)
                    {
                        if (table.Alias != null)
                        {
                            if (ColumnExp.TableAlias == table.Alias) return table.Table.Name;
                        }
                        else
                        {
                            if (ColumnExp.TableAlias == table.Table.Name) return table.Table.Name;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns>回傳輸出欄位個數</returns>
        public int ExportForColumnsClause(ColumnsClause columns, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            CommandBuildOptions CurrentBuildOptions = BuildOptions;
            int column_count = 0; /*  記錄輸出個數 */

            foreach (Column col in columns)
            {
                if (FormatOptions.HasFlag(CommandFormatOptions.ColumnSplit))
                {
                    CommandBuilder.NewLine();
                }
                if (column_count != 0)
                {
                    CommandBuilder.Append(','); // 加入分隔字元在欄位之間
                }
                // [ColumnName] as [AliasName]
                bool ExpandAliasColumnName = false;

                // 非 IsTerminalExp 不適用 ExpandAsteriskColumnName 展開作用
                if (col.Expression.IsTerminalExp)
                {
                    if (CurrentBuildOptions.HasFlag(CommandBuildOptions.ExpandAsteriskColumnName))
                    {
                        Expression.ColumnExp ColumnExp = col.Expression.Expression as Expression.ColumnExp;
                        if (ColumnExp != null && ColumnExp.ColumnName == "*")
                        {
                            string TableName = FindTableName(sql_model as SelectModel, ColumnExp);
                            if (TableName != null && DbInformation != null)
                            {
                                TableInfo tableInfo = DbInformation.QueryTableInfo(TableName);
                                // ExpandAsteriskColumnName
                                ExpandAliasColumnName = true;
                                int expand_count = 0; /*  記錄輸出個數 */
                                foreach (ColumnInfo ExpandColumn in tableInfo.Columns)
                                {
                                    CommandBuilder.Append((expand_count == 0) ? "" : ",");// 視需要加入分隔字元在欄位之間
                                    expand_count++;
                                    // TODO:  HOW TO Hidden Special Column ( 利用字典 )
                                    CommandBuilder.NewLine();
                                    if (ColumnExp.TableAlias == null)
                                        CommandBuilder.AppendToken(Name(ExpandColumn.Name));
                                    else
                                    {
                                        CommandBuilder.AppendToken($"{Name(ColumnExp.TableAlias)}.{Name(ExpandColumn.Name)}");
                                        if (CurrentBuildOptions.HasFlag(CommandBuildOptions.ExpandAliasColumnName))
                                        {
                                            CommandBuilder.AppendKeywordToken("AS");
                                            CommandBuilder.AppendToken($"[{Name(ColumnExp.TableAlias)}.{Name(ExpandColumn.Name)}]");
                                        }
                                    }
                                }
                                column_count += expand_count; // 累加 因為 ExpandAliasColumnName 所產生的欄位個數.
                            }
                        }
                    }
                }
                // 表示目前欄位己展開 (ExpandAliasColumnName == ture)
                if (!ExpandAliasColumnName)
                {
                    // CommandBuilder.NewLine();
                    CommandBuilder.AppendToken(col.RebuildExpression(sql_model.Options));
                    if (!string.IsNullOrEmpty(col.AsName))
                    {
                        CommandBuilder.AppendKeywordToken("AS");
                        CommandBuilder.AppendToken(Name(col.AsName));
                    }
                    else
                    {
                        // 無指定 AliasName 時，才加入 Alias Column Name [TableAliasName.ColumnName]
                        if (CurrentBuildOptions.HasFlag(CommandBuildOptions.ExpandAliasColumnName))
                        {
                            // Column 必須為 單一欄位定義. 運算式 [table alisaname].[column name] ,才啟用 ExpandAliasColumnName 作用
                            ColumnExp ColumnExp = col.Expression.Expression as ColumnExp;
                            if (ColumnExp != null && ColumnExp.ColumnName != "*")
                            {
                                CommandBuilder.AppendKeywordToken("AS");
                                CommandBuilder.AppendToken($"[{Name(ColumnExp.TableAlias)}.{Name(ColumnExp.ColumnName)}]");
                            }
                        }
                    }
                    column_count++; // 累加, 所產生的欄位個數
                }
            }
            if (column_count == 0)  // 表示無欄位輸出. 預設輸出 *
            {
                column_count++;
                CommandBuilder.Append(" *");  /* 在Select 和 * 間加入空白*/
            }
            CommandBuilder.Append(' '); // 加入結尾的空白符號
            return column_count;
        }
        private string Name(string name)
        {
            if (Options.HasFlag(SqlOptions.LowerName))
                return name?.ToLower();
            return name;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="sql_model"></param>
        /// <param name="BuildOptions"></param>
        /// <returns></returns>
        public int ExportForTableClause(TableClause table, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions BuildOptions)
        {
            int case_count = 0;
            if (!table.PrimaryTable.IsEmpty)
            {
                case_count++;
                CommandBuilder.AppendToken(table.PrimaryTable.GetFullName(this.Options));

                if (table.PrimaryTable.IsSelectModel)
                {
                    CommandBuilder.Append(' ');  // 子查詢時則加入一個空白作分隔.
                }

                if (!string.IsNullOrEmpty(table.PrimaryAliasName))
                {
                    CommandBuilder.AppendToken(Name(table.PrimaryAliasName));
                }

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
                    CommandBuilder.Unindent();
                    CommandBuilder.NewLine();
                    CommandBuilder.Indent();

                    switch (join.JoinType)
                    {
                        case JoinType.LeftOuter: CommandBuilder.AppendKeywordToken("LEFT JOIN"); break;
                        case JoinType.RightOuter: CommandBuilder.AppendKeywordToken("RIGHT JOIN"); break;
                        case JoinType.Inner: CommandBuilder.AppendKeywordToken("INNER JOIN"); break;
                        case JoinType.Left: CommandBuilder.AppendKeywordToken("LEFT JOIN"); break;
                        case JoinType.Right: CommandBuilder.AppendKeywordToken("RIGHT JOIN"); break;
                        case JoinType.Outer: CommandBuilder.AppendKeywordToken("JOIN"); break;
                    }

                    CommandBuilder.AppendToken(join.Table.GetFullName(this.Options));

                    if (!string.IsNullOrEmpty(join.Alias))
                        CommandBuilder.AppendToken(Name(join.Alias));

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
        ///
        /// </summary>
        /// <param name="groupBy"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="sql_model"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public int ExportForGroupByClause(GroupByClause groupBy, CommandBuilder CommandBuilder, SqlModel sql_model, BuildExportOptions Options)
        {
            int key_count = 0;
            if (groupBy.Count > 0)
            {
                if (Options.HasFlag(BuildExportOptions.ExportKeyword))
                {
                    CommandBuilder.Unindent();
                    CommandBuilder.NewLine();
                    CommandBuilder.Indent();
                    CommandBuilder.AppendKeywordToken("GROUP BY");
                }
                foreach (GroupByColumn key in groupBy)
                {
                    if (key_count++ != 0)
                        CommandBuilder.AppendToken(",");
                    if (sql_model.Options.HasFlag(SqlOptions.LowerName))
                        CommandBuilder.AppendToken(key.Name.ToLower());
                    else
                        CommandBuilder.AppendToken(key.Name);
                }
            }
            return key_count;
        }

        private void ExportParameter(CommandBuilder CommandBuilder, ExportParameterEventArgs e)
        {
            int ParamIndex = 0;  // 參數次序
            var StringNational = !Options.HasFlag(SqlOptions.NonStringNational);

            switch (e.Parameter.ArgmentType)
            {
                case ArgumentType.None:

                    if (e.pMode == ParameterMode.Expand)
                    {
                        if (e.Parameter != null)
                        {
                            if (e.Parameter.ColumnType == ColumnType.Auto)
                            {
                                if (e.Parameter.Value is DateTime)
                                {
                                    DateTime _val = (DateTime)e.Parameter.Value;
                                    if (_val != DateTime.MinValue)
                                        CommandBuilder.AppendToken($"CAST('{_val:yyyy-MM-dd HH:mm:ss}' as DATETIME)");
                                    else
                                        CommandBuilder.AppendToken("null");
                                }
                                else
                                {
                                    if (StringNational)
                                        CommandBuilder.AppendToken($"N'{Encoding(e.Parameter.Value)}'");
                                    else
                                        CommandBuilder.AppendToken($"'{Encoding(e.Parameter.Value)}'");
                                }
                            }
                            else
                            {
                                // TODO: 依據 Parameter 的資訊展開
                                switch (e.Parameter.ColumnType)
                                {
                                    case ColumnType.Double:
                                    case ColumnType.Single:
                                    case ColumnType.Int32:
                                    case ColumnType.Int64:
                                    case ColumnType.Decimal:
                                        CommandBuilder.AppendToken($"{e.Parameter.Value}");
                                        break;

                                    case ColumnType.DateTime:
                                        // CAST('01-JAN-2009' AS DATETIME) for SQL Server
                                        DateTime _val = (DateTime)e.Parameter.Value;
                                        if (_val != DateTime.MinValue)
                                            CommandBuilder.AppendToken($"CAST('{_val:yyyy-MM-dd HH:mm:ss}' as DATETIME)");
                                        else
                                            CommandBuilder.AppendToken("null");
                                        break;

                                    default:
                                        if (StringNational)
                                            CommandBuilder.AppendToken($"N'{Encoding(e.Parameter.Value)}'");
                                        else
                                            CommandBuilder.AppendToken($"'{Encoding(e.Parameter.Value)}'");
                                        break;
                                }
                            }
                        }
                        else
                            CommandBuilder.AppendToken("''");
                    }
                    else
                        CommandBuilder.AppendToken(e.Name);
                    break;

                case ArgumentType.Like:
                    ExportArgumentTypeLike(CommandBuilder, e);
                    break;

                case ArgumentType.Equal:
                    ExportArgumentTypeEqual(CommandBuilder, e);
                    break;

                case ArgumentType.Scope:
                case ArgumentType.Multi:
                case ArgumentType.MultiScope:
                    ParameterValues pvs = e.Parameter.Value as ParameterValues;
                    if (e.Parameter.Value is ParameterValues)
                    {
                        foreach (IParameterValue p_value in (ParameterValues)e.Parameter.Value)
                        {
                            ParamIndex++;
                            if (p_value is IParameterScopeValue && e.ArgmentType.HasFlag(ArgumentType.Scope))
                                ExportParamValue(CommandBuilder, e, p_value, ParamIndex);
                            else if (p_value is IParameterSingleValue && e.ArgmentType.HasFlag(ArgumentType.Multi))
                                ExportParamValue(CommandBuilder, e, p_value, ParamIndex);
                        }
                    }
                    else if (e.Parameter.Value is IParameterValue)
                    {
                        ParamIndex++;
                        IParameterValue p_value = (IParameterValue)e.Parameter.Value;

                        if (p_value is IParameterScopeValue && e.ArgmentType.HasFlag(ArgumentType.Scope))
                            ExportParamValue(CommandBuilder, e, p_value, ParamIndex);
                        else if (p_value is IParameterSingleValue && e.ArgmentType.HasFlag(ArgumentType.Multi))
                            ExportParamValue(CommandBuilder, e, p_value, ParamIndex);
                    }
                    else
                        throw new SqlModelParameterException("Invlaid Value" + e.Parameter.Value.GetType().Name);
                    break;

                default:
                    throw new SqlModelParameterException("Invlaid Value" + e.Parameter.Value.GetType().Name);
                    //break;
            }
        }

        private object Encoding(object val)
        {
            if (val != null && val is string)
            {
                string _val = (string)val;
                if (_val.IndexOf('\'') != -1)
                {
                    return _val.Replace("'", "''");
                }
            }
            return val;
        }

        private int ExportParamValue(CommandBuilder CommandBuilder, ExportParameterEventArgs e, IParameterValue p_value, int ParamIndex)
        {

            var StringNational = !Options.HasFlag(SqlOptions.NonStringNational);

            string Column = e.ColumnExpression.RebuildExpression(
                CommandBuildType.SQL,
                ParameterMode.Parameter, null); /* 功能型的 ColumnExpression 不會含有 Parameter 變數 */

            if (ParamIndex != 1) CommandBuilder.AppendToken("OR");
            if (p_value is IParameterScopeValue)
            {
                IParameterScopeValue s_value = (IParameterScopeValue)p_value;
                // {$Name:Socpe T,@T1} ==> {$ T>=@T1#MN#FROM and T<=@T1#MN#TO}
                CommandBuilder.AppendToken("(");
                if (e.pMode == ParameterMode.Expand)
                {
                    CommandBuilder.AppendToken(Column);
                    CommandBuilder.AppendToken(">=");
                    if (StringNational)
                        CommandBuilder.AppendToken($"N'{s_value.FromStringValue}'");
                    else
                        CommandBuilder.AppendToken($"'{s_value.FromStringValue}'");

                    CommandBuilder.AppendToken("AND");

                    CommandBuilder.AppendToken(Column);
                    CommandBuilder.AppendToken("<=");
                    if (StringNational)
                        CommandBuilder.AppendToken($"N'{s_value.ToStringValue}'");
                    else
                        CommandBuilder.AppendToken($"'{s_value.ToStringValue}'");
                }
                else
                {
                    CommandBuilder.AppendToken(Column);
                    CommandBuilder.AppendToken(">=");
                    CommandBuilder.AppendToken($"{e.Name}#M{ParamIndex}#FROM");
                    CommandBuilder.AppendToken("AND");

                    CommandBuilder.AppendToken(Column);
                    CommandBuilder.AppendToken("<=");
                    CommandBuilder.AppendToken($"{e.Name}#M{ParamIndex}#TO");
                }
                CommandBuilder.AppendToken(")");
            }
            else
            {
                // {$Name:Mutil T,@T1}  ==> {$ T=@T1#M1 or T=@T1#M2 or T=@T1#M3}
                if (e.pMode == ParameterMode.Expand)
                {
                    IParameterSingleValue s_value = (IParameterSingleValue)p_value;

                    CommandBuilder.AppendToken(Column);
                    CommandBuilder.AppendToken("=");
                    if (StringNational)
                        CommandBuilder.AppendToken($"N'{s_value.StringValue}'");
                    else
                        CommandBuilder.AppendToken($"'{s_value.StringValue}'");
                }
                else
                {
                    CommandBuilder.AppendToken(Column);
                    CommandBuilder.AppendToken("=");
                    CommandBuilder.AppendToken($"{e.Name}#M{ParamIndex}");
                }
            }
            return 0;
        }

        private void ExportArgumentTypeEqual(CommandBuilder CommandBuilder, ExportParameterEventArgs e)
        {
            var StringNational = !Options.HasFlag(SqlOptions.NonStringNational);

            string Column = e.ColumnExpression.RebuildExpression(
                         CommandBuildType.SQL,
                         ParameterMode.Parameter, null); /* 功能型的 ColumnExpression 不會含有 Parameter 變數 */

            if (e.pMode == ParameterMode.Expand)
            {
                CommandBuilder.AppendToken(Column);
                CommandBuilder.AppendToken("=");
                if (StringNational)
                    CommandBuilder.AppendToken($"N'{Encoding(e.Parameter.Value)}'");
                else
                    CommandBuilder.AppendToken($"'{Encoding(e.Parameter.Value)}'");
            }
            else
            {
                CommandBuilder.AppendToken(Column);
                CommandBuilder.AppendToken("=");
                CommandBuilder.AppendToken($"@{Column}");
            }
        }

        private static char[] Like_cmds = { '%', '_' };

        private void ExportArgumentTypeLike(CommandBuilder CommandBuilder, ExportParameterEventArgs e)
        {
            var StringNational = !Options.HasFlag(SqlOptions.NonStringNational);

            string Column = e.ColumnExpression.RebuildExpression(
                         CommandBuildType.SQL,
                         ParameterMode.Parameter, null); /* 功能型的 ColumnExpression 不會含有 Parameter 變數 */

            if (e.Parameter.Value is string)
            {
                bool UseLIke = false;
                if (e.Parameter.Value != null && e.Parameter.Value is string)
                {
                    string _val = (string)e.Parameter.Value;
                    UseLIke = _val.IndexOfAny(Like_cmds, 0) != -1;
                }
                if (UseLIke) // 是否啟用  LIKE 或用 =  決定於參數內容是否含有有效的查詢指令
                {
                    // % (百分比符號)：代表零個、一個、或數個字符。
                    // _ (底線)：代表一個字符。
                    if (e.pMode == ParameterMode.Expand)
                    {
                        CommandBuilder.AppendToken(Column);
                        CommandBuilder.AppendToken("LIKE");
                        if (StringNational)
                            CommandBuilder.AppendToken($"N'{Encoding(e.Parameter.Value)}'");
                        else
                            CommandBuilder.AppendToken($"'{Encoding(e.Parameter.Value)}'");
                    }
                    else
                    {
                        CommandBuilder.AppendToken(Column);
                        CommandBuilder.AppendToken("LIKE");
                        CommandBuilder.AppendToken($"@{Column}");
                    }
                }
                else
                {
                    if (e.pMode == ParameterMode.Expand)
                    {
                        CommandBuilder.AppendToken(Column);
                        CommandBuilder.AppendToken("=");
                        if (StringNational)
                            CommandBuilder.AppendToken($"N'{Encoding(e.Parameter.Value)}'");
                        else
                            CommandBuilder.AppendToken($"'{Encoding(e.Parameter.Value)}'");
                    }
                    else
                    {
                        CommandBuilder.AppendToken(Column);
                        CommandBuilder.AppendToken("=");
                        CommandBuilder.AppendToken($"@{Column}");
                    }
                }
            }
            else
                throw new SqlModelParameterException("Invlaid Value" + e.Parameter.Value.GetType().Name);
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
            // [uid] [numeric](28, 0) IDENTITY(1,1) NOT NULL,
            int key_count = 0;
            foreach (ColumnDescription key in Columns)
            {
                if (key_count++ != 0)
                {
                    CommandBuilder.AppendToken(",");
                    // if (BuildOptions.HasFlag(CommandFormatOptions.AutoFormat)) CommandBuilder.AppendToken("\r\n\t");
                }

                CommandBuilder.AppendToken($"{key.Name} {ColumnTypeHelper.ConvertColumnType(key.ColumnType)}");

                switch (key.ColumnType)
                {
                    case ColumnType.Varbinary:
                    case ColumnType.Binary:
                    case ColumnType.Text:
                    case ColumnType.Varchar:
                    case ColumnType.Char:
                    case ColumnType.NChar:
                    case ColumnType.Nvarchar:
                        CommandBuilder.AppendToken($"({key.Size})");
                        break;

                    case ColumnType.Decimal:
                        CommandBuilder.AppendToken($"({key.NumericPrecision},{key.NumericScale})");
                        break;
                }
                // IDENTITY [ (seed ,increment ) ]
                if (key.Identity != null) CommandBuilder.AppendToken($" IDENTITY({key.Identity.Seed},{key.Identity.Increment})");

                if (!key.Nullable) CommandBuilder.AppendKeywordToken("NOT NULL");
                if (key.Default != null)
                {
                    // CONSTRAINT DF_ieac03h DEFAULT ('')
                    string Definition = key.Default.Definition;
                    if (!string.IsNullOrEmpty(Definition))
                    {
                        if (!Definition.StartsWith("(")) Definition = $"({Definition})";
                        CommandBuilder.AppendToken($"CONSTRAINT {key.Default.Name} DEFAULT {Definition}");
                    }
                }
            }
            return key_count;
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
            // CONSTRAINT [PK_ieac07h] PRIMARY KEY NONCLUSTERED (uid ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON PRIMARY

            int key_count = 0;
            IndexDescription PrimaryKeyIndex = indexes.PrimaryKey;
            if (PrimaryKeyIndex != null)
            {
                CommandBuilder.AppendToken(",");
                if (FormatOptions.HasFlag(CommandFormatOptions.AutoFormat)) CommandBuilder.AppendToken("\r\n\t");
                //string Additions = null;
                //if (PrimaryKeyIndex.Unique)
                //    Additions += " UNIQUE ";
                // if (PrimaryKeyIndex.Clustered) Additions += " CLUSTERED "; else Additions += " NONCLUSTERED ";

                if (PrimaryKeyIndex.Clustered)
                    CommandBuilder.AppendToken($"CONSTRAINT {PrimaryKeyIndex.Name} PRIMARY KEY CLUSTERED ({PrimaryKeyIndex.Keys})");
                else
                    CommandBuilder.AppendToken($"CONSTRAINT {PrimaryKeyIndex.Name} PRIMARY KEY NONCLUSTERED ({PrimaryKeyIndex.Keys})");

                // With (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90)
                if (indexes.WithIndexOption != null)
                {
                    CommandBuilder.AppendToken(indexes.WithIndexOption);
                }
            }

            //foreach (IndexDescription index in indexes)
            //{
            //    //if (key_count++ != 0)
            //    //{
            //        CommandBuilder.AppendToken(",");
            //        if (Format == CommandBuildFormat.Auto) CommandBuilder.AppendToken("\r\n\t");
            //    //}
            //    string Additions = null;

            //    if (index.PrimaryKey)
            //        Additions += " PRIMARY KEY ";
            //    else if (index.Unique)
            //        Additions += " UNIQUE ";

            //    if (index.PrimaryKey || index.Unique)
            //    {
            //        if (index.Clustered) Additions += " CLUSTERED "; else Additions += " NONCLUSTERED ";
            //    }

            //    CommandBuilder.AppendToken("CONSTRAINT {0}{1}({2})", index.Name, Additions, index.Keys);

            //}
            return key_count;
        }

        #endregion ISqlBuildExport Members
    }
}