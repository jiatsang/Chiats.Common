// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats
{
    /// <summary>
    /// 簡化版本文字串符記號掃描解析類別. 分隔符號處理器
    /// </summary>
    internal class CommonTokenScanner : TokenScanner
    {
        static string symbol_table = "()<>!@#$%^&*-=+{}[]:;,?/\\~|.";
        private string SymbolTable = null;

        private char QuotationStart;
        private char QuotationEnd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="SymbolTable"></param>
        /// <param name="Quotation">指示字串的分隔字元</param>
        /// <param name="QuotationEnd">指示字串的分隔結束字元</param>
        public CommonTokenScanner(string statement, string SymbolTable = null, char Quotation = '\'', char QuotationEnd = '\0')
            : base(statement, false)
        {
            this.SymbolTable = SymbolTable ?? symbol_table;
            this.QuotationStart = Quotation;
            this.QuotationEnd = (QuotationEnd == '\x0') ? Quotation : QuotationEnd;
            this.ScanNow();
        }

        protected override void ScanNow()
        {
            if (statement != null && statement.Length > 0)
            {
                int statementLength = statement.Length;
                string currentKeyword = null;
                TokenType currentType = TokenType.Null;
                int currentTokenStartAt = -1;

                // 取個下第 (offset) 字元. 若己到結尾. 則回傳 null 防止因為己至字串結尾而發生錯誤.
                Func<int, int, char> GetStatementCharAtOffset =
                    (current, offset) => (current < statementLength - offset) ? statement[current + offset] : '\0';

                currentKeyword = "";
                for (int current = 0; current < statementLength; current++)
                {
                    char scode = GetStatementCharAtOffset(current, 0);
                    char scode_next = GetStatementCharAtOffset(current, 1); // 取個下第一個字元. 若己到結尾. 則回傳 null

                    if (is_space_char(scode)) continue;
                    if (_is_first_number(scode))
                    {
                        char scode_next_next = GetStatementCharAtOffset(current, 2);// 取個下第二個字元.若己到結尾. 則回傳 null
                        if (_is_number(scode) || (scode == '.' && !_is_digat(scode_next)) || (scode == '-' && _is_number(scode_next)))
                        {
                            // .001
                            // -.001
                            // if ( scode == '-' &&  !( scode2 >= '0' &&  scode2 <= '9') )
                            currentType = TokenType.Number;
                            currentKeyword += scode;
                            currentTokenStartAt = current++;

                            for (; current < statementLength; current++)
                            {
                                scode = statement[current];

                                if (_is_number(scode))
                                {
                                    currentKeyword += scode;
                                }
                                else if (_is_end(scode))
                                {
                                    current--;
                                    break;
                                }
                                else
                                {
                                    currentType = TokenType.ParserError;
                                    currentKeyword += scode;
                                }
                            }
                            _try_add_token(statementLength, ref currentKeyword, ref currentType, ref currentTokenStartAt, current);
                            continue;
                        }
                    }
                    // CHECKING String   'abc ... ' 及 N'12344'
                    // 必需在 check Keyword A ~ Z 0 ~ 9 _ @ # 前,否則 N'12344' 會找不到
                    if (scode == QuotationStart || (scode == 'N' && scode_next == QuotationStart))
                    {
                        currentType = TokenType.String;
                        current++;
                        if (scode == 'N')
                        {
                            //currentKeyword += QuotationChar;
                            current++;
                        }
                        currentTokenStartAt = current;
                        currentKeyword = ""; // 給于初始值.
                        // 忽略第一個單引號由字串中第一個字元收集起.
                        for (; current < statementLength; current++)
                        {
                            scode = statement[current];
                            if (scode == QuotationEnd)
                            {
                                // 雙重字元的作用 二個單引號/雙引號, 視為字串的一部份.
                                // 當  QuotationStartChar and QuotationEndChar 不相同時, 忽略雙重字元的作用
                                if ((current < statementLength - 1 && QuotationStart == QuotationEnd && statement[current + 1] == QuotationStart))
                                {
                                    current++;
                                    currentKeyword += scode; // 只保留一個有效的單引號
                                    continue;
                                }
                                break;
                            }
                            currentKeyword += scode; // 不再保留前後單引號.
                        }

                        if (scode != QuotationEnd)
                            currentType = TokenType.ParserError;

                        _try_add_token(statementLength, ref currentKeyword, ref currentType, ref currentTokenStartAt, current);
                        continue;
                    }

                    // check Keyword A ~ Z 0 ~ 9 _ @ #
                    if (_is_first_word(scode))
                    {
                        int dot_count = 0;
                        int word_count = 0;
                        int bracket_count = 0;  // open bracket

                        currentType = TokenType.Keyword;
                        currentKeyword += scode;
                        currentTokenStartAt = current++;

                        for (; current < statementLength; current++)
                        {
                            scode = statement[current];
                            if (bracket_count > 0) // in open bracket
                            {
                                word_count++;
                                currentKeyword += scode;
                            }
                            else if (scode == '.')
                            {
                                dot_count++;
                                word_count = 0;
                                currentKeyword += scode;
                            }
                            else if (_is_word(scode))
                            {
                                word_count++;
                                currentKeyword += scode;
                            }
                            else if (_is_end(scode))
                            {
                                current--;
                                break;
                            }
                            else
                            {
                                currentType = TokenType.ParserError;
                                currentKeyword += scode;
                            }
                        }
                        _try_add_token(statementLength, ref currentKeyword, ref currentType, ref currentTokenStartAt, current);
                        continue;
                    }

                    switch (scode)
                    {
                        case '!':
                            currentType = TokenType.Symbol;
                            currentTokenStartAt = current;
                            if (current < statementLength - 1)
                            {
                                switch (statement[current + 1])
                                {
                                    case '=': currentKeyword = "!="; current++; break;
                                    default: currentKeyword = "!"; break;
                                }
                            }
                            else
                                currentKeyword = "!";
                            _try_add_token(statementLength, ref currentKeyword, ref currentType, ref currentTokenStartAt, current);
                            break;

                        case '>':
                            currentType = TokenType.Symbol;
                            currentTokenStartAt = current;

                            if (current < statementLength - 1)
                            {
                                switch (statement[current + 1])
                                {
                                    case '=': currentKeyword = ">="; current++; break;
                                    default: currentKeyword = ">"; break;
                                }
                            }
                            else
                                currentKeyword = ">";

                            _try_add_token(statementLength, ref currentKeyword, ref currentType, ref currentTokenStartAt, current);
                            break;

                        case '<':
                            currentType = TokenType.Symbol;
                            currentTokenStartAt = current;
                            if (current < statementLength - 1)
                            {
                                switch (statement[current + 1])
                                {
                                    case '=': currentKeyword = "<="; current++; break;
                                    case '>': currentKeyword = "<>"; current++; break;
                                    default: currentKeyword = "<"; break;
                                }
                            }
                            else
                                currentKeyword = "<";
                            _try_add_token(statementLength, ref currentKeyword, ref currentType, ref currentTokenStartAt, current);
                            break;

                        default:
                            currentKeyword += scode;
                            currentTokenStartAt = current;
                            if (_is_symbol(scode))
                            {
                                currentType = TokenType.Symbol;
                            }
                            else
                            {
                                currentType = TokenType.ParserError;
                            }
                            _try_add_token(statementLength, ref currentKeyword, ref currentType, ref currentTokenStartAt, current);
                            break;
                    }
                }
            }
        }

        #region Internal Private Method

        private void _try_add_token(int statementLength, ref string tag_keyword, ref TokenType tag_type, ref int tag_start_at, int tag_end_at)
        {
            if (tag_type != TokenType.Null)
            {
                add_token(new Token(tag_type, tag_keyword,
                     tag_start_at,
                     tag_end_at < statementLength ? tag_end_at : statementLength - 1  // 防止位置指向字串外面.
                    ));
                // clear
                tag_type = TokenType.Null;
                tag_keyword = null;
                tag_start_at = -1;
            }
        }

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

        protected virtual bool _is_symbol(char ch)
        {
            return !(SymbolTable.IndexOf(ch) < 0);
        }

        protected virtual bool _is_end(char ch)
        {
            return _is_symbol(ch) || is_space_char(ch);
        }

        #endregion Internal Private Method
    }
}