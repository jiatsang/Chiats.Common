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
    /// ²�ƪ�����r��ŰO�����y�ѪR���O. ���j�Ÿ��B�z��
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
        /// <param name="Quotation">���ܦr�ꪺ���j�r��</param>
        /// <param name="QuotationEnd">���ܦr�ꪺ���j�����r��</param>
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

                // ���ӤU�� (offset) �r��. �Y�v�쵲��. �h�^�� null ����]���v�ܦr�굲���ӵo�Ϳ��~.
                Func<int, int, char> GetStatementCharAtOffset =
                    (current, offset) => (current < statementLength - offset) ? statement[current + offset] : '\0';

                currentKeyword = "";
                for (int current = 0; current < statementLength; current++)
                {
                    char scode = GetStatementCharAtOffset(current, 0);
                    char scode_next = GetStatementCharAtOffset(current, 1); // ���ӤU�Ĥ@�Ӧr��. �Y�v�쵲��. �h�^�� null

                    if (is_space_char(scode)) continue;
                    if (_is_first_number(scode))
                    {
                        char scode_next_next = GetStatementCharAtOffset(current, 2);// ���ӤU�ĤG�Ӧr��.�Y�v�쵲��. �h�^�� null
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
                    // CHECKING String   'abc ... ' �� N'12344'
                    // ���ݦb check Keyword A ~ Z 0 ~ 9 _ @ # �e,�_�h N'12344' �|�䤣��
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
                        currentKeyword = ""; // ���_��l��.
                        // �����Ĥ@�ӳ�޸��Ѧr�ꤤ�Ĥ@�Ӧr�������_.
                        for (; current < statementLength; current++)
                        {
                            scode = statement[current];
                            if (scode == QuotationEnd)
                            {
                                // �����r�����@�� �G�ӳ�޸�/���޸�, �����r�ꪺ�@����.
                                // ��  QuotationStartChar and QuotationEndChar ���ۦP��, ���������r�����@��
                                if ((current < statementLength - 1 && QuotationStart == QuotationEnd && statement[current + 1] == QuotationStart))
                                {
                                    current++;
                                    currentKeyword += scode; // �u�O�d�@�Ӧ��Ī���޸�
                                    continue;
                                }
                                break;
                            }
                            currentKeyword += scode; // ���A�O�d�e���޸�.
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
                     tag_end_at < statementLength ? tag_end_at : statementLength - 1  // �����m���V�r��~��.
                    ));
                // clear
                tag_type = TokenType.Null;
                tag_keyword = null;
                tag_start_at = -1;
            }
        }

        /// <summary>
        ///  �ˬd�r���O�_���X�k���W�ٶ}�Y�r��
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual bool _is_first_word(char ch)
        {
            //�䴩 $ -> SQL CTL ���ܼƦW��
            return char.IsLetter(ch) || ch == '_' || ch == '@' || ch == '$' || ch == '#';
        }

        /// <summary>
        /// �ˬd�r���O�_���X�k���ƭȶ}�Y�r��
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual bool _is_first_number(char ch)
        {
            return (ch >= '0' && ch <= '9') || ch == '.' || ch == '-';
        }

        /// <summary>
        /// �ˬd�r���O�_���X�k���ƭȦr��
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
            // Oracle �䴩 '$' ����檫��W��. �Ҧp : v$process v$session
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