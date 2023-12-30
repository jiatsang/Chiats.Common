// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL 解析字串命令共用介面. 依字串命令產生 InsertModel 物件
    /// </summary>
    internal class InsertParser : BaseParser, ISqlParserObject
    {
        // delete from table where ...
        public static InsertParser Default = new InsertParser();

        #region ISqlParserObject Members

        public SqlModel PaserCommand(SQLTokenScanner tokenScanner, SqlModel SqlModel, object parameters = null)
        {
            InsertModel currentModel = ConvertCommonModel<InsertModel>(SqlModel);
            int currrntIndex = 1; // 直接忽略第一個關鍵字 Select/Insert/Update/Delete

            ((ICommandInitialize)currentModel).BeginInit();

            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "into"))
            {
                currrntIndex++; // 忽略 into 關鍵字(允許可有可無)
            }
            StringObjectName k123 = StringObjectName.NameTest(tokenScanner, ref currrntIndex);
            if (k123.IsTableName)
                currentModel.Table = k123.Table;
            else
                throw new SqlModelSyntaxException("Not Table Name Found.");

            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "("))
            {
                currrntIndex++;
                int end = tokenScanner.Find(")", currrntIndex, true);
                if (end == -1)
                    throw new SqlModelSyntaxException("未找到指定的關鍵符號 ')'");

                ParserColumns(tokenScanner, currentModel, currrntIndex, end);
                currrntIndex = ++end;
            }
            else
            {
                throw new SqlModelSyntaxException("未找到指定的關鍵符號 '(',SqlModel 尚未支援未指定欄位名稱之 INSETR 敘述.");
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "values"))
            {
                currrntIndex++;
                if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "("))
                {
                    currrntIndex++;
                    int end = tokenScanner.Find(")", currrntIndex, true);
                    if (end == -1)
                        throw new SqlModelSyntaxException("未找到指定的關鍵符號 ')'");
                    if (end != tokenScanner.Count - 1)
                        throw new SqlModelSyntaxException("無效的 INSETR 敘述 " + tokenScanner.RebuildToken(end, tokenScanner.Count));

                    ParserValues(tokenScanner, currentModel, currrntIndex, end);
                }
                else
                {
                    throw new SqlModelSyntaxException("未找到指定的關鍵符號 '('");
                }
            }
            else if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "select"))
            {
                PaserSelectModel(currentModel, tokenScanner, currrntIndex, tokenScanner.Count - 1);
            }
            else if (tokenScanner[currrntIndex].String == "(")
            {
                if ((tokenScanner[tokenScanner.Count - 1].String == ")") && (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex + 1].String, "select")))
                {
                    PaserSelectModel(currentModel, tokenScanner, currrntIndex + 1, tokenScanner.Count - 2);
                }
                else
                    throw new SqlModelSyntaxException("未找到指定的關鍵字 'values/select'.");
                // (Select * .... )
            }
            else
            {
                throw new SqlModelSyntaxException("未找到指定的關鍵字 'values/select'.");
            }


            ((ICommandInitialize)currentModel).EndInit();

            return currentModel;
        }

        private void PaserSelectModel(InsertModel currentModel, SQLTokenScanner tokenScanner, int StartIndex, int EndIndex)
        {
            // Select
            SQLTokenScanner newList = tokenScanner.Copy(StartIndex, EndIndex);
            currentModel.Select = (SelectModel)SelectParser.Default.PaserCommand(newList, currentModel);
            foreach (var columnName in columnNames)
            {
                currentModel.Columns.Add(columnName);
            }
            columnNames.Clear();
        }

        /// <summary>
        /// 指示本物件, 所產生之對應  SqlModel 相對應物件物件類別 如 SelectModel/UpdateModel/InsertModel/DeleteModel
        /// </summary>
        public Type ModelType
        {
            get { return typeof(InsertModel); }
        }

        #endregion ISqlParserObject Members

        private Queue<string> columnNames = new Queue<string>();

        private void ParserColumns(SQLTokenScanner TokenScanner, InsertModel currentModel, int CurrentIndex, int EndIndex)
        {
            int find_index = CurrentIndex;
            for (;;)
            {
                if (find_index > EndIndex) break;
                StringObjectName k123 = StringObjectName.NameTest(TokenScanner, ref find_index, ref EndIndex);

                if (k123.IsColumnName)
                    columnNames.Enqueue(k123.ColumnName);
                else
                    throw new SqlModelSyntaxException("無效的欄位名稱. {0}}", TokenScanner[find_index]);

                if (find_index < EndIndex && TokenScanner[find_index].String != ",")
                    throw new SqlModelSyntaxException("未找到指定的關鍵符號 ','");

                find_index++;
            }
        }

        private void ParserValues(SQLTokenScanner TokenScanner, InsertModel currentModel, int CurrentIndex, int EndIndex)
        {
            for (;;) // Process From
            {
                if (CurrentIndex > TokenScanner.Count) break;
                Token t = TokenScanner[CurrentIndex];

                if (StringComparer.OrdinalIgnoreCase.Equals(t.String, ")")) break;
                int end_symbol_index = -1, comma_index = -1;

                int left_brackets = 0; /* 記錄目前左括號數 */

                for (int find_index = CurrentIndex; find_index < TokenScanner.Count; find_index++) // Find ")" , ","
                {
                    int find2_index = TokenScanner.Find(",", find_index, false);
                    if (find2_index == -1)
                    {
                        if (left_brackets != 0)
                            throw new SqlModelSyntaxException("at " + find_index.ToString());

                        find_index = TokenScanner.Find(")", find_index, false);
                        if (find_index == -1)
                            throw new SqlModelSyntaxException("at " + find_index.ToString());

                        end_symbol_index = find_index;
                        break;
                    }
                    else
                        find_index = find2_index;

                    switch (TokenScanner[find_index].Type)
                    {
                        case TokenType.Symbol:
                            if (left_brackets == 0 && TokenScanner[find_index].String == ",")
                            {
                                if (comma_index != -1)
                                    throw new SqlModelSyntaxException(" at " + find_index.ToString());
                                comma_index = find_index;
                            }
                            else if (TokenScanner[find_index].String == "(")
                            {
                                left_brackets++; continue;
                            }
                            else if (TokenScanner[find_index].String == ")")
                            {
                                left_brackets--; continue;
                            }
                            break;
                    }
                    if (comma_index != -1) break;
                }

                if (end_symbol_index != -1 || comma_index != -1)
                {
                    int compare_index = (end_symbol_index != -1) ? end_symbol_index : comma_index;

                    if (columnNames.Count == 0)
                        throw new SqlModelSyntaxException("欄位個數不符合.");

                    if (CurrentIndex == compare_index - 1)
                    {
                        if (TokenScanner[CurrentIndex].Type == TokenType.String)
                            currentModel.Columns.Add(columnNames.Dequeue(), TokenScanner[CurrentIndex].String);
                        else
                            currentModel.Columns.AddExpression(columnNames.Dequeue(), TokenScanner[CurrentIndex].String);
                    }
                    else
                        currentModel.Columns.AddExpression(columnNames.Dequeue(), TokenScanner.RebuildToken(CurrentIndex, compare_index - 1));

                    //Debug.Print("Column => {0}", RebuildToken(TokenScanner, CurrentIndex, compare_index - 1));
                    CurrentIndex = compare_index + 1;
                    if (end_symbol_index != -1) break;
                }
            }
            if (columnNames.Count != 0)
                throw new SqlModelSyntaxException("欄位個數不符合.");
        }
    }
}