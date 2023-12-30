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
    /// SQL 解析字串命令共用介面. 依字串命令產生 UpdateModel 物件
    /// </summary>
    internal class UpdateParser : BaseParser, ISqlParserObject
    {
        // delete from table where ...
        public static UpdateParser Default = new UpdateParser();

        public SqlModel PaserCommand(SQLTokenScanner tokenScanner, SqlModel SqlModel, object parameters = null)
        {
            UpdateModel currentModel = ConvertCommonModel<UpdateModel>(SqlModel);

            int currrntIndex = 1; // 直接忽略第一個關鍵字 Select/Insert/Update/Delete

            ((ICommandInitialize)currentModel).BeginInit();

            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "from"))
            {
                currrntIndex++; // 忽略 from 關鍵字(允許可有可無)
            }

            int _end = tokenScanner.Count;
            StringObjectName k123 = StringObjectName.NameTest(tokenScanner, ref currrntIndex, ref _end);

            if (k123.IsTableName)
                currentModel.Table = k123.Table;
            else
                throw new SqlModelSyntaxException("Table Name Not Found.");

            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "set"))
            {
                currrntIndex++;
                ParserSetColumns(tokenScanner, currentModel, ref currrntIndex);
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "from"))
            {
                // found from
                int end_index = currrntIndex + 1;
                int level = 0;
                for (; ; )
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[end_index].String, "(")) level++;
                    if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[end_index].String, ")")) level--;
                    if (level == 0)
                    {
                        if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[end_index].String, "where"))
                        {
                            ParserFromStatement(tokenScanner, currentModel, currrntIndex + 1, end_index - 1);
                            break;
                        }
                    }
                    if (end_index++ == tokenScanner.Count - 1) break;
                }
                currrntIndex = end_index;
            }

            if (currrntIndex < tokenScanner.Count && StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "where"))
            {
                currrntIndex++;
                ParserWhereStatement(tokenScanner, currentModel.Where, currrntIndex, tokenScanner.Count - 1);
            }

            if (parameters != null)
            {
                currentModel.Parameters.Fill(parameters);
            }

            ((ICommandInitialize)currentModel).EndInit();


            return currentModel;
        }

        private void ParserFromStatement(SQLTokenScanner tokenScanner, UpdateModel updateModel, int start_index, int end_index)
        {
            // (tablename) (a)  [left/right] join (tablename) (a) on (expression) [left/right] join ...
            // string table_1,join_key , table_2 , on_expression
            int current_index = start_index;
            StringObjectName table1, table2;
            string table2_shotname = null;
            JoinType joinType = JoinType.Inner;

            updateModel.Tables = new TableClause(updateModel);

            table1 = StringObjectName.NameTest(tokenScanner, ref current_index, ref end_index);
            if (table1.IsTableSource)
            {
                updateModel.Tables.PrimaryTable = table1.CreateNewTableSource;
                // 檢查是否己解析至結束位置
                if (current_index > end_index) return;

                Token CurrentToken = tokenScanner[current_index];
                Token NextToken = tokenScanner[current_index + 1];

                if (CurrentToken.Type == TokenType.Keyword && !BaseParser.IsKeyword(CurrentToken.String))
                {
                    updateModel.Tables.PrimaryAliasName = CurrentToken.String;
                    if (++current_index >= end_index) return;
                }
                for (; ; )
                {
                    if (current_index >= end_index) break;
                    if (joinType == JoinType.None || IsJoinType(tokenScanner, ref current_index, out joinType, end_index))
                    {
                        table2 = StringObjectName.NameTest(tokenScanner, ref current_index, ref end_index);
                        if (table2.IsTableSource)
                        {
                            TableSource _TableSource = table2.CreateNewTableSource;

                            NextToken = tokenScanner[current_index];
                            if (NextToken.Type == TokenType.Keyword && !BaseParser.IsKeyword(NextToken.String))
                            {
                                table2_shotname = NextToken.String;
                                if (++current_index >= end_index && joinType != JoinType.None)
                                    throw new SqlModelSyntaxException("無效的關鍵字");
                            }
                            // 檢查是否有  WITH ( < table_hint > )    WITH (TABLOCK, INDEX(myindex))
                            if (IsTablehints(tokenScanner, ref current_index, end_index))
                            {
                                bool FoundFinish = false;
                                int level = 0;
                                // 只找 () 內的字串存至 Select.Tables.Hints , 不理會其內容這部份可能需對不同資料庫的作法進行分析後, 才能進行細部設計
                                for (int find_index = current_index; find_index <= end_index; find_index++)
                                {
                                    CurrentToken = tokenScanner[find_index];
                                    if (CurrentToken.String == "(") level++;
                                    else if (CurrentToken.String == ")")
                                    {
                                        if (level == 0)
                                        {
                                            int start = tokenScanner[current_index].StartAt;
                                            int end = tokenScanner[find_index].StartAt;
                                            _TableSource.Hints = tokenScanner.Statement.Substring(start, end - start);
                                            FoundFinish = true;
                                            current_index = find_index + 1;
                                            break;
                                        }
                                        level--;
                                    }
                                }
                                if (!FoundFinish)
                                    throw new SqlModelSyntaxException("ERROR :  WITH ( < table_hint > ) 找不到結束字元");
                            }
                            Token TestIsOnToken = tokenScanner[current_index];
                            if (joinType != JoinType.None)
                            {
                                if (TestIsOnToken.IsKeywordAndMatch("on"))
                                {
                                    current_index++;
                                    int current_end = end_index;  // 取得 Expression 的結束點.
                                    int bracket_count = 0;
                                    for (int cc = current_index; cc < end_index; cc++)
                                    {
                                        if (tokenScanner[cc].String == "(") bracket_count++;  // 括號內容直接視為 Expression
                                        if (tokenScanner[cc].String == ")") bracket_count--;

                                        if (cc == 0 && BaseParser.IsNotExpressionKeyword(tokenScanner[cc].String))
                                        {
                                            current_end = cc - 1; break;
                                        }
                                    }
                                    int join_end = find_join_end(tokenScanner, current_index, current_end);

                                    updateModel.Tables.Add(
                                        new JoinTable(updateModel.Tables,
                                                       _TableSource,
                                                       table2_shotname,
                                                       joinType,
                                                       tokenScanner.RebuildToken(current_index, join_end))
                                         );
                                    current_index = join_end + 1;
                                }
                                else
                                    throw new SqlModelSyntaxException("Cann't Found 'on' ");
                            }
                            else
                            {
                                // joinType == joinType.None
                                updateModel.Tables.Add(new JoinTable(updateModel.Tables, _TableSource, table2_shotname, joinType, null));

                                if (tokenScanner[current_index].String == ",")
                                {
                                    joinType = JoinType.None;
                                    current_index++;
                                }
                            }
                        }
                        else
                            throw new SqlModelSyntaxException("Cann't Found join TableName");
                    }
                    else
                    {
                        // 檢查是否有  WITH ( < table_hint > )    WITH (TABLOCK, INDEX(myindex))
                        if (IsTablehints(tokenScanner, ref current_index, end_index))
                        {
                            bool FoundFinish = false;
                            int level = 0;
                            // 只找 () 內的字串存至 Select.Tables.Hints , 不理會其內容  這部份可能需對不同資料庫的作法進行分析後, 才能進行細部設計
                            for (int find_index = current_index; find_index <= end_index; find_index++)
                            {
                                CurrentToken = tokenScanner[find_index];
                                if (CurrentToken.String == "(") level++;
                                else if (CurrentToken.String == ")")
                                {
                                    if (level == 0)
                                    {
                                        int start = tokenScanner[current_index].StartAt;
                                        int end = tokenScanner[find_index].StartAt;

                                        updateModel.Tables.PrimaryTable.Hints = (tokenScanner.Statement.Substring(start, end - start));

                                        FoundFinish = true;
                                        current_index = find_index + 1;
                                        break;
                                    }
                                    level--;
                                }
                            }
                            if (!FoundFinish)
                                throw new SqlModelSyntaxException("ERROR :  WITH ( < table_hint > ) 找不到結束字元");
                        }
                        else if (tokenScanner[current_index].String == ",")
                        {
                            joinType = JoinType.None;
                            current_index++;
                        }
                        else
                            throw new SqlModelSyntaxException("Cann't Found left/right/inner join with(...)");
                    }
                }
            }
            else
            {
                throw new SqlModelSyntaxException("FROM 附近文法錯誤");
            }
        }

        private int find_join_end(SQLTokenScanner tokenScanner, int start_index, int end_index)
        {
            for (int i = start_index; i < end_index; i++)
            {
                int cc = 0;
                Token CurrentToken = tokenScanner[i];
                if (CurrentToken.String == "(") cc++;
                if (CurrentToken.String == ")") cc--;
                if (cc == 0)
                {
                    if (CurrentToken.IsKeywordAndMatch(join_keywords) != null)
                    {
                        return i - 1;
                    }
                }
            }
            return end_index;
        }

        private bool IsTablehints(SQLTokenScanner tokenScanner, ref int start_index, int end_index)
        {
            Token CurrentToken = tokenScanner[start_index];
            Token TokenNext = tokenScanner[start_index + 1];

            // 支援解析含 with 或不含 with  的 Tablehints 的子句.
            if (CurrentToken.IsKeywordAndMatch("with") && TokenNext.String == "(")
            {
                start_index += 2;
                return true;
            }
            else if (CurrentToken.String == "(")
            {
                start_index++;
                return true;
            }
            return false;
        }

        private static string[] join_keywords = { "left", "right", "inner", "outer" };

        private bool IsJoinType(SQLTokenScanner tokenScanner, ref int start_index, out JoinType type, int end_index)
        {
            // LEFT OUTER JOIN
            type = JoinType.Inner;
            // left/reght inner/outer join
            Token CurrentToken = tokenScanner[start_index];
            if (CurrentToken.Type == TokenType.Keyword)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(CurrentToken.String, "join"))
                {
                    start_index += 1;
                    type = JoinType.Outer;
                    return true;
                }
                Token NextToken2 = tokenScanner[start_index + 1];
                if (StringComparer.OrdinalIgnoreCase.Equals(NextToken2.String, "join"))
                {
                    // left/reght/inner/outer join
                    start_index += 2;
                    switch (CurrentToken.IsKeywordAndMatch(join_keywords))
                    {
                        case "left":
                            type = JoinType.LeftOuter;
                            return true;

                        case "right":
                            type = JoinType.RightOuter;
                            return true;

                        case "inner":
                            type = JoinType.Inner;
                            return true;

                        case "outer":
                            type = JoinType.Outer;
                            return true;

                        default:
                            throw new SqlModelSyntaxException("left/right/inner/outer not found.");
                    }
                }

                Token NextToken3 = tokenScanner[start_index + 1];
                if (StringComparer.OrdinalIgnoreCase.Equals(NextToken3.String, "join"))
                {
                    // left/reght  inner/outer join
                    start_index += 3;
                    string key = CurrentToken.IsKeywordAndMatch(new string[] { "left", "right" });
                    string key2 = CurrentToken.IsKeywordAndMatch(new string[] { "inner", "outer" });
                    if (key != null && key2 != null)
                    {
                        switch (key)
                        {
                            case "left":
                                type = (key2 == "inner") ? JoinType.Left : JoinType.LeftOuter;
                                return true;

                            case "right":
                                type = (key2 == "inner") ? JoinType.Right : JoinType.RightOuter;
                                return true;
                        }
                    }
                    throw new SqlModelSyntaxException("left/right/inner/outer not found.");
                }
            }
            return false;
        }

        private void ParserSetColumns(SQLTokenScanner tokenList, UpdateModel currentModel, ref int currentIndex)
        {

            var END_INDEX = tokenList.Count - 1;

            for (; ; ) // Process Set
            {
                if (currentIndex > tokenList.Count) break;
                Token t = tokenList[currentIndex];
                if (StringComparer.OrdinalIgnoreCase.Equals(t.String, "from") ||
                    StringComparer.OrdinalIgnoreCase.Equals(t.String, "where"))
                    break;


                // column_express : a=b
                // column_express : a=abc * cdf,
                // column_express : a=(abc * cdf) / 12,

                int set_end_index = -1, comma_index = -1;
                int left_brackets = 0; /* 記錄目前左括號 */
                int _end = tokenList.Count;
                // 取左值
                StringObjectName k123 = StringObjectName.NameTest(tokenList, ref currentIndex, ref _end);

                Token find_t = tokenList[currentIndex];

                if (StringComparer.OrdinalIgnoreCase.Equals(find_t.String, "from") ||
                    StringComparer.OrdinalIgnoreCase.Equals(find_t.String, "where"))
                {
                    set_end_index = currentIndex;
                    break;
                }
                if (!StringComparer.OrdinalIgnoreCase.Equals(find_t.String, "="))
                {
                    throw new SqlModelSyntaxException("必須是 '=' ({0})", find_t.String);
                }
                currentIndex++;
                // 取右值
                for (int find_index = currentIndex; find_index < tokenList.Count; find_index++) // Find "where"
                {
                    if (find_index != END_INDEX)
                    {
                        find_t = tokenList[find_index];

                        switch (find_t.Type)
                        {
                            case TokenType.Keyword:
                                if (StringComparer.OrdinalIgnoreCase.Equals(find_t.String, "from") ||
                                    StringComparer.OrdinalIgnoreCase.Equals(find_t.String, "where"))
                                {
                                    set_end_index = find_index;
                                }
                                break;

                            case TokenType.Symbol:
                                if (left_brackets == 0 && find_t.String == ",")
                                {
                                    if (comma_index != -1)
                                        throw new SqlModelSyntaxException("at " + find_index.ToString());
                                    comma_index = find_index;
                                }
                                else if (find_t.String == "(")
                                {
                                    left_brackets++; continue;
                                }
                                else if (find_t.String == ")")
                                {
                                    left_brackets--; continue;
                                }
                                break;
                        }
                        if (set_end_index != -1 || comma_index != -1) break;
                    }
                    else
                    {
                        set_end_index = END_INDEX + 1;
                        break;
                    }
                }

                if (set_end_index != -1 || comma_index != -1)
                {
                    int end_index = (set_end_index != -1) ? set_end_index : comma_index;
                    if (currentIndex == end_index - 1 && tokenList[currentIndex].Type == TokenType.String)
                    {
                        // Add Constant Value
                        currentModel.Columns.Add(k123.ToString(), tokenList[currentIndex].String);
                    }
                    else
                    {
                        // Add Expression
                        currentModel.Columns.Add(k123.ToString(), null, tokenList.RebuildToken(currentIndex, end_index - 1));
                    }

                    // Debug.Print("Column => {0}={1}", k123, RebuildToken(tokenList, currentIndex, end_index - 1));

                    if (set_end_index != -1)
                    {
                        currentIndex = end_index;
                        break;
                    }
                    else
                    {
                        currentIndex = end_index + 1;
                    }
                }
                else
                {
                    throw new SqlModelSyntaxException("找不到結束符號");
                }
            }
        }

        /// <summary>
        /// 指示本物件, 所產生之對應  SqlModel 相對應物件物件類別 如 SelectModel/UpdateModel/InsertModel/DeleteModel
        /// </summary>
        public Type ModelType
        {
            get { return typeof(UpdateModel); }
        }
    }
}