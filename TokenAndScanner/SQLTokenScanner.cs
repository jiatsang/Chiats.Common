// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using Chiats.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chiats
{
    /// <summary>
    /// SQL Command 文字串符記號掃描解析類別.
    /// </summary>
    public class SQLTokenScanner : TokenScanner /*, IMessageLog */
    {
        public struct SQLStatement
        {
            public SQLStatement(SQLTokenScanner Scanner, UnionType UnionType)
            {
                this.Scanner = Scanner;
                this.UnionType = UnionType;
            }

            public SQLTokenScanner Scanner;
            public UnionType UnionType;
        };

        /// <summary>
        /// 當 Tokens因為某些理由必須修正個數时.可經由 _fix_end 進行修正.
        /// </summary>
        public int _fix_end; // 修正結束位置.

        public SQLStatement[] Split4Union()
        {
            List<SQLStatement> list = new List<SQLStatement>();
            int start = 0, level = 0;
            UnionType union_type = UnionType.None;
            for (int i = 0; i < token_list.Count; i++)
            {
                Token token = token_list[i];
                if (token.Type == TokenType.Symbol)
                {
                    if (token.String == "(") level++;
                    if (token.String == ")") level--;
                }
                else if (level == 0)
                {
                    switch (token.FindKeywordAndMatch("union", "minus", "intersect"))
                    {
                        case 0: // union and union all
                            list.Add(new SQLStatement(Copy(start, i - 1), union_type));
                            if (i + 1 < token_list.Count)// support union all
                            {
                                Token next_token = token_list[i + 1];
                                if (next_token.IsKeywordAndMatch("all"))
                                {
                                    i++;
                                    union_type = UnionType.UnionAll;
                                }
                                else
                                    union_type = UnionType.Union;
                            }
                            start = i + 1;

                            break;

                        case 1: // minus
                            // UNION[ALL]/MINUS/INTERSECT 語法支援.
                            list.Add(new SQLStatement(Copy(start, i - 1), union_type));
                            union_type = UnionType.Minus;
                            start = i + 1;
                            break;

                        case 2: // intersect
                            list.Add(new SQLStatement(Copy(start, i - 1), union_type));
                            union_type = UnionType.Intersect;
                            start = i + 1;
                            break;
                    }
                }
            }
            list.Add(new SQLStatement(Copy(start, token_list.Count - 1), union_type));
            return list.ToArray();
        }

        public SQLTokenScanner(string statement) : base(statement)
        {
        }

        public SQLTokenScanner(SQLTokenScanner scanner, int start, int end)
            : base(scanner, start, end)
        {
        }

        public void Attach(Token key)
        {
            key.StartAt = -1;
            token_list.Add(key);
        }

        public void RemoveAt(int index)
        {
            if (index < token_list.Count) token_list.RemoveAt(index);
        }

        public SQLTokenScanner Copy(int start, int end)
        {
            return new SQLTokenScanner(this, start, end);
        }
        protected override void ScanNow()
        {
            int statementLength = statement.Length;
            if (statementLength > 0)
            {
                StringBuilder currentKeyword = new StringBuilder();
                TokenType currentType = TokenType.Null;
                int currentTokenStartAt = -1;
                currentKeyword.Clear();
                // 取個下第 (offset) 字元. 若己到結尾. 則回傳 null 防止因為己至字串結尾而發生錯誤.
                Func<int, int, bool, char> GetStatementCharAtOffset = (Current, offset, skipspace) =>
                {
                    char ch = '\0';
                    for (; ; )
                    {
                        ch = (Current < statementLength - offset) ? statement[Current + offset] : '\0';
                        if (skipspace && ch == ' ') // 忽略空白符號
                            offset++;
                        else
                            break;
                    }
                    return ch;
                };

                Action<int> TryAddToken = (Current) =>
                    {
                        if (currentType != TokenType.Null)
                        {
                            add_token(new Token(currentType, currentKeyword.ToString(),
                                currentTokenStartAt,
                                Current < statementLength ? Current : statementLength - 1  // 防止位置指向字串外面.
                                ));
                            // clear current status
                            currentType = TokenType.Null;
                            currentKeyword.Clear(); ;
                            currentTokenStartAt = -1;
                        }
                    };
                for (int current = 0; current < statementLength; current++)
                {
                    char scode = GetStatementCharAtOffset(current, 0, false);
                    if (is_space_char(scode)) continue;
                    char scode_next = GetStatementCharAtOffset(current, 1, false); // 取個下第一個字元. 若己到結尾. 則回傳 null


                    if (scode == '-' && scode_next == '-')  // Comment Line
                    {
                        int CommentStarting = current;
                        // Skip Comment Line
                        while (++current < statementLength)
                        {
                            scode = GetStatementCharAtOffset(current, 0, false);
                            if (is_end_char(scode)) break;
                        }
                        //Debug.Print("Skip Comment : {0}", statement.Substring(CommentStarting, current - CommentStarting));
                        continue;
                    }

                    if (_is_first_number(scode))
                    {
                        char scode_next_next = GetStatementCharAtOffset(current, 2, false);// 取個下第二個字元.若己到結尾. 則回傳 null
                        if (_is_number(scode) || (scode == '.' && !_is_digat(scode_next_next)) || (scode == '-' && _is_number(scode_next_next)))
                        //if (!(scode == '.' && !_is_digat(scode_next)) && !(scode == '-' && _is_number(scode_next)))
                        {
                            // .001
                            // -.001
                            // if ( scode == '-' &&  !( scode2 >= '0' &&  scode2 <= '9') )
                            currentType = TokenType.Number;
                            currentKeyword.Append(scode);
                            currentTokenStartAt = current++;

                            for (; current < statementLength; current++)
                            {
                                scode = GetStatementCharAtOffset(current, 0, false);

                                if (_is_number(scode))
                                {
                                    currentKeyword.Append(scode);
                                }
                                else if (_is_symbol(scode))
                                {
                                    current--;
                                    break;
                                }
                                else if (_is_end(scode))
                                {
                                    break;
                                }
                                else
                                {
                                    currentType = TokenType.ParserError;
                                    currentKeyword.Append(scode);
                                }
                            }
                            TryAddToken(current);// _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);
                            continue;
                        }
                    }
                    // CHECKING String   'abc ... ' 及 N'12344'
                    // 必需在 check Keyword A ~ Z 0 ~ 9 _ @ # 前,否則 N'12344' 會找不到
                    if (scode == '\'' || (scode == 'N' && scode_next == '\''))
                    {
                        currentType = TokenType.String;
                        currentKeyword.Clear(); // 給于初始值.
                        currentTokenStartAt = current;  // currentTokenStartAt 開始位置要包含 ' 和 N 字元
                        current++;
                        if (scode == 'N')
                        {
                            current++;  /* currentKeyword += '\'';  */
                        }
                        for (; current < statementLength; current++)
                        {
                            scode = GetStatementCharAtOffset(current, 0, false);
                            if (scode == '\'')
                            {
                                // 發現二個單引號, 視為字串的一個單引符號
                                if ((current < statementLength - 1 && statement[current + 1] == '\''))
                                {
                                    current++;
                                    currentKeyword.Append(scode); // 只保留一個有效的單引號
                                    continue;
                                }
                                break;
                            }
                            currentKeyword.Append(scode); // 不再保留前後單引號.
                        }
                        if (scode != '\'')
                            currentType = TokenType.ParserError;
                        TryAddToken(current);// _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);
                        continue;
                    }

                    // check Keyword A ~ Z 0 ~ 9 _ @ #
                    if (_is_first_word(scode) || scode == '[')
                    {
                        int dot_count = 0;
                        int word_count = 0;
                        int bracket_count = 0;  // open bracket
                        // support a.b.c
                        if (scode == '[')
                        {
                            bracket_count++;
                        }

                        currentType = TokenType.Keyword;
                        currentKeyword.Append(scode);
                        currentTokenStartAt = current++;

                        for (; current < statementLength; current++)
                        {
                            scode = GetStatementCharAtOffset(current, 0, false);

                            if (scode == ']')
                            {
                                bracket_count--;
                                currentKeyword.Append(scode);
                                // 下一個非 Keyword 的連結符號時. 則表示 Keyword 的結束符號
                                if (GetStatementCharAtOffset(current, 1, true) != '.') break; // 己發現結束符號
                            }
                            else if (scode == '[')
                            {
                                if (word_count == 0)
                                {
                                    bracket_count++;
                                    currentKeyword.Append(scode);
                                }
                                else
                                    throw new SyntaxException("Parser Error: 無效的 Keyword 字元");
                            }
                            else if (bracket_count > 0) // in open bracket
                            {
                                // 在中括號內中的所有符號均視為 keyword 的一部份, 包含中括號.
                                word_count++;
                                currentKeyword.Append(scode);
                            }
                            else if (scode == '.')
                            {
                                // Keyword 的連結符號. 視為 keyword 的一部份
                                dot_count++;
                                word_count = 0;
                                currentKeyword.Append(scode);
                            }
                            else if (_is_word(scode) || (dot_count > 0 && scode == '*'))  // alias.* 視為 keyword
                            {
                                // alias.* 視為 keyword 並且 * 之後就直接立即結束.
                                word_count++;
                                currentKeyword.Append(scode);
                            }
                            else if (_is_symbol(scode))
                            {
                                current--;
                                break;
                            }
                            else if (_is_end(scode))
                            {
                                break;
                            }
                            else
                            {
                                currentType = TokenType.ParserError;
                                currentKeyword.Append(scode);
                            }
                        }
                        TryAddToken(current);// _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);
                        continue;
                    }

                    scode_next = GetStatementCharAtOffset(current, 1, false);
                    switch (scode)
                    {
                        case '!':
                            currentType = TokenType.Symbol;
                            currentTokenStartAt = current;
                            currentKeyword.Clear();
                            switch (scode_next)
                            {
                                case '=': currentKeyword.Append("!="); current++; break;
                                default: currentKeyword.Append("!"); break;
                            }
                            TryAddToken(current);// _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);
                            break;

                        case '>':
                            currentType = TokenType.Symbol;
                            currentTokenStartAt = current;
                            currentKeyword.Clear();
                            switch (scode_next)
                            {
                                case '=': currentKeyword.Append(">="); current++; break;
                                default: currentKeyword.Append(">"); break;
                            }
                            TryAddToken(current);// _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);
                            break;

                        case '<':
                            currentType = TokenType.Symbol;
                            currentTokenStartAt = current;
                            currentKeyword.Clear();
                            switch (scode_next)
                            {
                                case '=': currentKeyword.Append("<="); current++; break;
                                case '>': currentKeyword.Append("<>"); current++; break;
                                default: currentKeyword.Append("<"); break;
                            }
                            TryAddToken(current);//  _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);

                            break;

                        case '[':

                            currentType = TokenType.Keyword;
                            currentTokenStartAt = current;
                            for (; current < statementLength; current++)
                            {
                                // 在中括號內中的所有符號均視為 keyword 的一部份, 包含中括號.
                                scode = GetStatementCharAtOffset(current, 0, false);
                                currentKeyword.Append(scode);
                                if (scode == ']') break;
                            }

                            if (scode != ']')
                                currentType = TokenType.ParserError;

                            TryAddToken(current);//  _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);

                            break;

                        default:
                            currentKeyword.Append(scode);
                            currentTokenStartAt = current;
                            currentType = (_is_symbol(scode)) ? TokenType.Symbol : TokenType.ParserError;

                            TryAddToken(current);// _try_add_token(ref currentKeyword, ref currentType, ref  currentTokenStartAt);
                            break;
                    }
                }
            }
        }

        #region Internal Private Method

        /// <summary>
        ///  檢查字元是否為合法的名稱開頭字元
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual bool _is_first_word(char ch)
        {
            //支援 $ -> SQL CTL 的變數名稱
            return char.IsLetter(ch) || ch == '_' || ch == '@' || ch == '$' || ch == '#';
        }

        /// <summary>
        /// 檢查字元是否為合法的數值開頭字元
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual bool _is_first_number(char ch)
        {
            return (ch >= '0' && ch <= '9') || ch == '.' || ch == '-';
        }

        /// <summary>
        /// 檢查字元是否為合法的數值字元
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual bool _is_number(char ch)
        {
            return (_is_digat(ch) || ch == '.');
        }

        protected virtual bool _is_digat(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }

        protected virtual bool _is_word(char ch)
        {
            // Oracle 支援 '$' 為表格物件名稱. 例如 : v$process v$session
            return char.IsLetterOrDigit(ch) || ch == '_' || ch == '$' || ch == '#';
            // return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || ch == '_' || ch == '$';
        }

        private static string symbol_table = "()<>!@#$%^&*-=+{}:;,?/\\~|.";

        protected virtual bool _is_symbol(char ch)
        {
            return !(symbol_table.IndexOf(ch) < 0);
        }

        protected virtual bool _is_end(char ch)
        {
            return (ch == ' ' || ch == '\x0a' || ch == '\x0d');
        }

        #endregion Internal Private Method
    }
}