// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;

namespace Chiats
{
    /// <summary>
    /// �r�껲�U�{��,
    /// </summary>
    public static class StringExtensions
    {
        private static bool IsSplitChar(char c)
        {
            return (c == ' ' || c == '\r' || c == '\n' || c == '\n');
        }

        /// <summary>
        /// �B�z�r�ꦨ���r��}�C.. ���j�Ÿ��� �ť�/����r��  ���\�ϥ� "" �� '' �@�Ӧr��
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static string[] CommandSplit(this string Command)
        {
            List<string> args = new List<string>();
            if (Command != null)
            {
                StringBuilder sb = new StringBuilder();
                char splitChar = '\0';
                foreach (var c in Command)
                {
                    if (IsSplitChar(c) && splitChar == '\0')
                    {
                        if (sb.Length > 0)
                        {
                            args.Add(sb.ToString());
                            sb.Clear();
                        }
                    }
                    else if (c == '"' || c == '\'')
                    {
                        if (splitChar == '\0')
                        {
                            splitChar = c;
                        }
                        else
                        {
                            if (splitChar == c)
                            {
                                if (sb.Length > 0)
                                {
                                    args.Add(sb.ToString());
                                    sb.Clear();
                                }
                                splitChar = '\0';
                            }
                            else
                            {
                                sb.Append(c);
                            }
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                if (sb.Length > 0)
                {
                    args.Add(sb.ToString());
                    sb.Clear();
                }
            }
            return args.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="SQLCommand"></param>
        /// <returns></returns>
        public static string[] SQLSplit(this string SQLCommand)
        {
            List<string> tokens = new List<string>();
            SQLTokenScanner SQLTokenScanner = new SQLTokenScanner(SQLCommand);
            foreach (var token in SQLTokenScanner)
            {
                if (token.IsString)
                    tokens.Add($"N'{token.StringConstant}'");
                else
                    tokens.Add(token.String);
            }
            return tokens.ToArray();
        }

        /// <summary>
        /// ���զr�ꤤ���ƭȩM��r���� (��r+�ƭ�)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string NextNameByNumberParts(string name)
        {
            int num;
            string sname;
            SplitNameNumberParts(name, out sname, out num);
            return string.Format("{0}{1}", sname, num);
        }

        /// <summary>
        /// ���X�r�ꤤ���ƭȩM��r����. ( 0~9 10�i��).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sname">�^�Ǥ�r</param>
        /// <param name="number">�^�Ǽƭ�</param>
        public static void SplitNameNumberParts(string name, out string sname, out int number)
        {
            number = 0;
            int name_lenght = name.Length - 1;
            sname = name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                for (int i = name_lenght; i > 0; i--)
                {
                    if (char.IsDigit(name[i]))
                    {
                        int c = (int)name[i];
                        number = (number * 10) + c - 0x30;
                    }
                    else
                    {
                        sname = name.Substring(0, i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// �䴩���޸��r�ꪺ���j��k Ex: "joe, john","sun, moon"
        /// </summary>
        /// <param name="SplitString"></param>
        /// <param name="SplitSymbol"></param>
        /// <param name="Quotation"></param>
        /// <returns></returns>
        public static string[] AutoSplit(string SplitString, string SplitSymbol = ",", char Quotation = '\'')
        {
            List<string> SplitStringList = new List<string>();
            CommonTokenScanner TokenScanner = new CommonTokenScanner(SplitString, SplitSymbol, Quotation);
            foreach (var Token in TokenScanner)
            {
                if (Token.Type == TokenType.Keyword || Token.Type == TokenType.Number)
                    SplitStringList.Add(Token.String);

                if (Token.Type == TokenType.String) SplitStringList.Add(Token.StringConstant);
            }
            return SplitStringList.ToArray();
        }

        /// <summary>
        /// �䴩���޸��r�ꪺ���j��k Ex: "joe, john","sun, moon"
        /// </summary>
        /// <param name="SplitString"></param>
        /// <param name="SplitSymbol"></param>
        /// <param name="Quotation"></param>
        /// <returns></returns>
        public static string[] AutoSplit(string SplitString, char SplitSymbol, char Quotation = '\'')
        {
            List<string> SplitStringList = new List<string>();
            CommonTokenScanner TokenScanner = new CommonTokenScanner(SplitString, SplitSymbol.ToString(), Quotation);
            foreach (var Token in TokenScanner)
            {
                if (Token.Type == TokenType.Keyword || Token.Type == TokenType.Number)
                    SplitStringList.Add(Token.String);

                if (Token.Type == TokenType.String) SplitStringList.Add(Token.StringConstant);
            }
            return SplitStringList.ToArray();
        }
    }
}