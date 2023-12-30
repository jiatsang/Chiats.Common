// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL 解析字串命令共用介面. 依字串命令產生 SelectModel 物件
    /// </summary>
    internal class SelectParser : BaseParser, ISqlParserObject
    {
        public static SelectParser Default = new SelectParser();

        public SqlModel PaserCommand(SQLTokenScanner tokenScanner, SqlModel SqlModel, object parameters = null)
        {
            SelectModel Model = ConvertCommonModel<SelectModel>(SqlModel);
            ((ICommandInitialize)Model).BeginInit();
            SQLTokenScanner.SQLStatement[] TokenScannerList = tokenScanner.Split4Union();

            SelectModel currentModel = null;

            foreach (var currentTokenScanner in TokenScannerList)
            {
                int CurrentIndex = 1; // 直接忽略第一個關鍵字 Select/Insert/Update/Delete

                if (currentModel == null)
                    currentModel = Model;
                else
                {
                    currentModel.CreateUnionModel(currentTokenScanner.UnionType);
                    currentModel = currentModel.UnionSelect;
                }

                ParserDistinct(currentTokenScanner.Scanner, currentModel, ref CurrentIndex);
                ParserTopN(currentTokenScanner.Scanner, currentModel, ref CurrentIndex);

                // ProcessSelectColumns 處理至 "From" 為止
                ParserSelectColumns(currentTokenScanner.Scanner, currentModel, ref CurrentIndex);

                int start_index = CurrentIndex;
                string find_key = "from";
                int inside_count = 0;

                for (; CurrentIndex < currentTokenScanner.Scanner.Count; CurrentIndex++)
                {

                    var Token = currentTokenScanner.Scanner[CurrentIndex];

                    string key = Token.String;

                    if (key == "(")
                        inside_count++;
                    else if (key == ")") inside_count--;
                    else if (inside_count == 0)
                    {
                        if (Token.IsKeyword) // 只有  Keyword 才需要檢查是否為 order by/where/having/group by
                        {
                            key = key?.ToLower();
                            switch (key)
                            {
                                case "order":   // order by
                                case "where":
                                case "having":
                                case "group": // group by

                                    set_list(currentTokenScanner.Scanner, currentModel, find_key, start_index, CurrentIndex - 1);

                                    find_key = key;
                                    //  _fix_end : 修正結束位置.
                                    start_index = CurrentIndex + 1 + currentTokenScanner.Scanner._fix_end;
                                    currentTokenScanner.Scanner._fix_end = 0;

                                    break;
                            }
                        }
                    }
                }
                set_list(currentTokenScanner.Scanner, currentModel, find_key, start_index, CurrentIndex - 1);
            }

            if (parameters != null)
            {
                currentModel.Parameters.Fill(parameters);
            }

            ((ICommandInitialize)Model).EndInit();

            return Model;
        }

        public Type ModelType
        {
            get { return typeof(SelectModel); }
        }

        #region Parser SQL Statement

        private void ParserSelectColumns(SQLTokenScanner TokenScanner, SelectModel Select, ref int CurrentIndex)
        {
            // Debug.Print("ProcessFromStatement : {0}", RebuildToken(TokenScanner, start_index, end_index));
            Token ColumnAliasNameToken = Token.Empty;

            for (; ; ) // 處理 From 前的欄位運算式
            {
                // MODEL: Expression  
                // MODEL: Expression COLUMNNAME
                // MODEL: Expression as COLUMNNAME
                // MODEL: COLUMNNAME = Expression

                int from_index = -1, comma_index = -1, as_index = -1, eqal_index = -1;

                if (CurrentIndex > TokenScanner.Count) break;
                Token t = TokenScanner[CurrentIndex];

                if (TokenKeys.FROM.Compare(t.String))
                    break;

                int left_brackets = 0; /* 記錄目前左括號 */
                Token find_t = Token.Empty;

                for (int find_index = CurrentIndex; find_index < TokenScanner.Count; find_index++) // 找尋下一個關鍵字  "From" , "," ,"as"
                {
                    find_t = TokenScanner[find_index];
                    // 在括號內的 from 視為運算式的一部份.
                    if (left_brackets == 0 && TokenKeys.FROM.Compare(find_t.String))
                    {
                        if (eqal_index != -1)
                        {
                            // 最後一組 MODEL: COLUMNNAME = Expression
                            ColumnAliasNameToken = TokenScanner[CurrentIndex];
                            CurrentIndex += 2;
                        }
                        from_index = find_index;
                        break;
                    }

                    switch (find_t.Type)
                    {
                        case TokenType.Keyword:
                            if (left_brackets == 0 && TokenKeys.AS.Compare(find_t.String))
                            {
                                if (as_index != -1)
                                    throw new SqlModelSyntaxException($"at {find_index} Token={find_t}");
                                as_index = find_index;
                            }
                            break;

                        case TokenType.Symbol:
                            if (left_brackets == 0 && find_t.String == ",")
                            {
                                if (comma_index != -1)
                                    throw new SqlModelSyntaxException($"at {find_index} Token={find_t}");
                                comma_index = find_index;
                                break;
                            }
                            if (left_brackets == 0 && find_t.String == "=")
                            {
                                eqal_index = find_index;
                                break;
                            }
                            if (find_t.String == "(") { left_brackets++; continue; }
                            if (find_t.String == ")") { left_brackets--; continue; }
                            break;
                    }


                    if (comma_index != -1)
                    {
                        if (eqal_index != -1)
                        {
                            // MODEL: COLUMNNAME = Expression
                            ColumnAliasNameToken = TokenScanner[CurrentIndex];
                            CurrentIndex += 2;
                            break;
                        }
                        // MODEL: Expression  
                        break;
                    }
                    else if (as_index != -1)
                    {
                        // MODEL: Expression as COLUMNNAME
                        ColumnAliasNameToken = TokenScanner[as_index + 1];
                        if (ColumnAliasNameToken.Type == TokenType.Keyword)
                        {
                            Token t3 = TokenScanner[as_index + 2];
                            if (t3.String == ",") { comma_index = as_index + 2; break; }
                            if (TokenKeys.FROM.Compare(t3.String)) { from_index = as_index + 2; break; }
                        }
                        else
                        {
                            throw new SqlModelSyntaxException($"無效的欄位別名 {ColumnAliasNameToken} at {find_index} {find_t}");
                        }
                    }

                }
                if (from_index != -1 || comma_index != -1)  // 有找到結束符號
                {
                    int end_index = (from_index != -1) ? from_index : comma_index;

                    if (as_index != -1) // Expression As Name
                    {
                        SQLTokenScanner Expression = new SQLTokenScanner(TokenScanner, CurrentIndex, as_index - 1);
                        Select.Columns.Add(Expression, ColumnAliasNameToken.String);
                        CurrentIndex = as_index + 3;
                    }
                    else // Expression
                    {

                        SQLTokenScanner Expression = new SQLTokenScanner(TokenScanner, CurrentIndex, end_index - 1);

                        if (eqal_index != -1)
                            // MODEL: COLUMNNAME = Expression  
                            Select.Columns.Add(Expression, ColumnAliasNameToken.Source);
                        else
                            // MODEL: Expression  
                            Select.Columns.Add(Expression);

                        CurrentIndex = end_index + 1;
                    }
                    if (from_index != -1)
                        break;
                }
                else
                    throw new SqlModelSyntaxException("找不到結束符號. {0}", find_t);
            }
        }

        private void ParserDistinct(SQLTokenScanner TokenScanner, SelectModel Select, ref int CurrentIndex)
        {
            Token t = TokenScanner[CurrentIndex];

            // SELECT Distinct a,b as n,c from
            if (t.Type == TokenType.Keyword && StringComparer.OrdinalIgnoreCase.Equals(t.String, "distinct"))
            {
                Select.Distinct = true;
                CurrentIndex++;
            }
            // SELECT all a,b as n,c from
            if (t.Type == TokenType.Keyword && StringComparer.OrdinalIgnoreCase.Equals(t.String, "all"))
            {
                CurrentIndex++;
            }
        }

        private void ParserTopN(SQLTokenScanner TokenScanner, SelectModel Select, ref int CurrentIndex)
        {
            Token t = TokenScanner[CurrentIndex];
            // SELECT top n [percent] a,b as n,c from
            if (t.Type == TokenType.Keyword && StringComparer.OrdinalIgnoreCase.Equals(t.String, "top"))
            {
                Token next = TokenScanner[CurrentIndex + 1];
                Token next2 = TokenScanner[CurrentIndex + 2];
                if (next.IsNumber)
                {
                    if (next2.Type == TokenType.Keyword && StringComparer.OrdinalIgnoreCase.Equals(next2.String, "percent"))
                    {
                        Select.Top = int.Parse(next.String);
                        Select.Percent4Top = true;
                        CurrentIndex += 3;
                    }
                    else
                    {
                        Select.Top = int.Parse(next.String);
                        Select.Percent4Top = false;
                        CurrentIndex += 2;
                    }
                }
                else
                {
                    throw new SqlModelSyntaxException("找不到 Top N. {0}", next);
                }
            }
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
        private static string[] apply_keywords = { "cross", "outer" };

        private int find_join_end(SQLTokenScanner tokenScanner, int start_index, int end_index)
        {
            for (int i = start_index; i < end_index; i++)
            {
                int bracket_count = 0;
                var CurrentToken = tokenScanner[i];
                if (CurrentToken.String == "(") bracket_count++;
                if (CurrentToken.String == ")") bracket_count--;
                if (bracket_count == 0)
                {
                    if (CurrentToken.IsKeywordAndMatch(join_keywords) != null)
                    {
                        if (i != end_index)
                        {
                            // ex: left ( mf 不是運算式的結尾)
                            var NextToken = tokenScanner[i + 1];
                            if (NextToken.String == "(")
                            {
                                continue;
                            }
                        }
                        return i - 1;
                    }
                }
            }
            return end_index;
        }

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
                if (StringComparer.OrdinalIgnoreCase.Equals(NextToken2.String, "apply"))
                {
                    // cross/outer apply
                    start_index += 2;
                    switch (CurrentToken.IsKeywordAndMatch(apply_keywords))
                    {
                        case "cross":
                            type = JoinType.CrossApply;
                            return true;

                        case "outer":
                            type = JoinType.OuterApply;
                            return true;

                        default:
                            throw new SqlModelSyntaxException("cross/outer not found.");
                    }
                }
                Token NextToken3 = tokenScanner[start_index + 2];
                if (StringComparer.OrdinalIgnoreCase.Equals(NextToken3.String, "join"))
                {
                    // left/reght  inner/outer join
                    start_index += 3;
                    string key = CurrentToken.IsKeywordAndMatch(new string[] { "left", "right" });
                    string key2 = NextToken2.IsKeywordAndMatch(new string[] { "inner", "outer" });
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

        private void ParserFromStatement(SQLTokenScanner tokenScanner, SelectModel Select, int start_index, int end_index)
        {
            // (tablename) (a) [left/right] join (tablename) (a) on (expression) [left/right] join ...
            // string table_1,join_key , table_2 , on_expression
            int current_index = start_index;
            StringObjectName table1, table2;
            string table2_shotname = null;
            JoinType joinType = JoinType.Inner;

            table1 = StringObjectName.NameTest(tokenScanner, ref current_index, ref end_index);
            if (table1.IsTableSource)
            {
                Select.Tables.PrimaryTable = table1.CreateNewTableSource;
                // 檢查是否己解析至結束位置
                if (current_index > end_index) return;

                Token CurrentToken = tokenScanner[current_index];
                Token NextToken = tokenScanner[current_index + 1];
                // ... <PrimaryAliasName>
                if (CurrentToken.Type == TokenType.Keyword && !BaseParser.IsKeyword(CurrentToken.String))
                {
                    Select.Tables.PrimaryAliasName = CurrentToken.String;
                    if (++current_index >= end_index) return;
                }
                // ...  AS <PrimaryAliasName>
                if (CurrentToken.IsKeywordAndMatch("as"))
                {
                    Select.Tables.PrimaryAliasName = NextToken.String;
                    if ((current_index += 2) >= end_index) return;
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
                            //  AS <table2_shotname>
                            if (NextToken.IsKeywordAndMatch("as"))
                            {
                                NextToken = tokenScanner[++current_index];
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
                                    for (int cc = current_index; cc <= end_index; cc++)
                                    {
                                        if (tokenScanner[cc].String == "(") bracket_count++;  // 括號內容直接視為 Expression
                                        if (tokenScanner[cc].String == ")") bracket_count--;
                                        if (bracket_count == 0)
                                        {
                                            if (cc == 0 && BaseParser.IsNotExpressionKeyword(tokenScanner[cc].String))
                                            {
                                                current_end = cc - 1; break;
                                            }
                                        }
                                        else
                                        {
                                            //Debug.Print($"{tokenScanner[cc].String}");
                                        }
                                    }
                                    int join_end = find_join_end(tokenScanner, current_index, current_end);

                                    Select.Tables.Add(
                                        new JoinTable(Select.Tables,
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
                                Select.Tables.Add(new JoinTable(Select.Tables, _TableSource, table2_shotname, joinType, null));

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

                                        Select.Tables.PrimaryTable.Hints = (tokenScanner.Statement.Substring(start, end - start));

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

        private void ParserOrderByStatement(SQLTokenScanner tokenScanner, SelectModel Select, int start_index, int end_index)
        {
            //Debug.Print("Parser Order By: {0}", RebuildToken(tokenScanner, start_index, end_index));
            for (int index = start_index; index <= end_index; index++)
            {
                Token t = tokenScanner[index];
                OrderSorting OrderSorting = OrderSorting.Ascending;

                if (t.Type == TokenType.Keyword && !IsKeyword(t.String))
                {
                    index++;

                    if (index <= end_index)
                    {
                        Token next_token = tokenScanner[index];
                        string sorting = null;
                        if ((sorting = next_token.IsKeywordAndMatch(new string[] { "desc", "asc" })) != null)
                        {
                            if (sorting == "desc")
                                OrderSorting = OrderSorting.Descending;

                            if (++index <= end_index)
                                next_token = tokenScanner[index];
                        }
                        if (index > end_index || next_token.Type == TokenType.Symbol && next_token.String == ",")
                        {
                            Select.OrderBy.Add(t.String, OrderSorting);
                            continue;
                        }
                        throw new SqlModelSyntaxException("ORDER BY 附近文法錯誤");
                    }
                    else
                    {
                        Select.OrderBy.Add(t.String);
                        break;
                    }
                }
                throw new SqlModelSyntaxException("ORDER BY 附近文法錯誤");
            }
        }

        private void ParserGroupByStatement(SQLTokenScanner tokenScanner, SelectModel Select, int start_index, int end_index)
        {
            for (int index = start_index; index <= end_index; index++)
            {
                Token t = tokenScanner[index];
                if (t.Type == TokenType.Keyword && !IsKeyword(t.String))
                {
                    index++;
                    if (index <= end_index)
                    {
                        Token next_token = tokenScanner[index];
                        if (index > end_index || next_token.Type == TokenType.Symbol && next_token.String == ",")
                        {
                            Select.GroupBy.Add(t.String);
                            // Debug.Print("Group By => {0}", k123.ToString());
                            continue;
                        }
                        throw new SqlModelSyntaxException("Group By 附近文法錯誤");
                    }
                    else
                    {
                        Select.GroupBy.Add(t.String);
                        // Debug.Print("Group By => {0}", k123.ToString());
                        break;
                    }
                }
                throw new SqlModelSyntaxException("Group By 附近文法錯誤");
            }
        }

        #endregion Parser SQL Statement

        protected void set_list(SQLTokenScanner token, SelectModel select_model, string key, int start_index, int end_index)
        {
            if (key == null && start_index > end_index)
                throw new SqlModelSyntaxException($"SQL Parser '{key}' clause failed");

            switch (key.ToLower())
            {
                case "from": ParserFromStatement(token, select_model, start_index, end_index); break;
                case "where": ParserWhereStatement(token, select_model.Where, start_index, end_index); break;
                case "having": ParserWhereStatement(token, select_model.Having, start_index, end_index); break;
                case "group":
                case "order":

                    if (string.Compare(token[start_index].String, "by", true) == 0)
                    {
                        start_index++;
                        if (key == "order")
                            ParserOrderByStatement(token, select_model, start_index, end_index);
                        else
                            ParserGroupByStatement(token, select_model, start_index, end_index);
                    }
                    else
                        throw new SqlModelSyntaxException($"SQL Parser cann't found 'By' at '{key}'");

                    break;

                default:
                    throw new SqlModelSyntaxException($"SQL Parser no support '{key}' clause ");
            }
        }
    }
}