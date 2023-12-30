// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Text;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL Command 的字串產生器 ( CommandBuilder 封裝 StringBuilder 物件. 並加以擴充 )
    /// </summary>
    public class CommandBuilder : IDisposable
    {
        private static string last_test_key;
        private static bool last_result;
        private static object last_locker = new object();

        public CommandBuilder(CommandFormatOptions Options = CommandFormatOptions.None)
        {
            this.Options = Options;
        }
        public CommandFormatOptions Options { get; private set; }

        private const string include_string = "()<>=,+-*/";
        private bool IsKeywordOnPrevious = false;
        private StringBuilder sb = new StringBuilder();

        private int IndentLevel = 0;

        /// <summary>
        /// 縮排
        /// </summary>
        public void Indent()
        {
            IndentLevel++;
        }

        /// <summary>
        /// 縮排
        /// </summary>
        public void Unindent()
        {
            IndentLevel--;
        }

        /// <summary>
        /// 換行
        /// </summary>
        /// <param name="FormatOptions"></param>
        public void NewLine()
        {
            if (Options.HasFlag(CommandFormatOptions.AutoFormat))
            {
                Append("\r\n");
                for (int i = 0; i < IndentLevel; i++)
                {
                    sb.Append("    ");
                }
            }
            else if (Options.HasFlag(CommandFormatOptions.Html))
            {
                Append("<br/>\r\n");
            }
        }

        /// <summary>
        /// 將指定物件的字元表示附加至這個執行個體的尾端。它會視需要而自行決定是否要加空白字元在 Token 前
        /// </summary>
        /// <param name="token_char"></param>
        public void AppendToken(char token_char)
        {
            if (sb.Length > 0)
            {
                // 只要前後字元包含 ' ()<>=' 符號則不加空白, 反之則加
                char ch = sb[sb.Length - 1];
                if (ch != ' ' && (IsKeywordOnPrevious || !(include_string.IndexOf(ch) != -1 || include_string.IndexOf(token_char) != -1)))
                {
                    sb.Append(' ');
                }
            }
            sb.Append(token_char);
            IsKeywordOnPrevious = false;
        }

        /// <summary>
        /// 目前的字串個數
        /// </summary>
        public int Length
        {
            get { return sb.Length; }
        }

        /// <summary>
        ///  回傳個別字元
        /// </summary>
        /// <param name="index">字元位置</param>
        /// <returns></returns>
        public char this[int index]
        {
            get { return sb[index]; }
        }

        /// <summary>
        /// 將這個執行個體中所有出現的指定字串取代為另一個指定字串。
        /// </summary>
        /// <param name="oldString">要取代的字串。</param>
        /// <param name="newString">取代 oldValue 的字串或 Null 參照 (即 Visual Basic 中的 Nothing)。</param>
        public void Replace(string oldString, string newString)
        {
            sb.Replace(oldString, newString);
        }

        /// <summary>
        /// 以另一個指定的字元，取代這個執行個體中指定的字元或字串的所有項目。
        /// </summary>
        /// <param name="oldChar">要取代的字元。</param>
        /// <param name="newChar">取代 oldValue 的字串或 Null 參照 (即 Visual Basic 中的 Nothing)。</param>
        public void Replace(char oldChar, char newChar)
        {
            sb.Replace(oldChar, newChar);
        }

        private bool KeywordTesting(string key, bool firstKey)
        {
            lock (last_locker)
            {
                string test_key = key;
                int last_index = key.LastIndexOf(' ');

                if (last_index != -1) // 表示不含空白
                {
                    int first_index = key.IndexOf(' ');
                    if (firstKey)
                    {
                        test_key = key.Substring(0, first_index);
                    }
                    else
                    {
                        test_key = key.Substring(last_index + 1);
                    }
                }

                if (test_key != last_test_key)
                {
                    last_result = BaseParser.IsKeyword(test_key);
                    last_test_key = test_key;
                }
                return last_result;
            }
        }
        public void AppendToken(string token)
        {
            //token = (token != null) ? token.Trim() : null;
            if (!string.IsNullOrEmpty(token))
            {
                bool IsKeyword = KeywordTesting(token, true);
                if (sb.Length > 0)
                {
                    // 只要前後字元包含 ' ()<>=' 符號則不加空白, 反之則加
                    char ch = sb[sb.Length - 1];
                    if (ch != ' ' &&
                        (
                        IsKeyword || IsKeywordOnPrevious ||
                        !(include_string.IndexOf(ch) != -1 ||
                        include_string.IndexOf(token[0]) != -1)
                        ))
                    {
                        sb.Append(' ');
                    }
                }
                IsKeywordOnPrevious = KeywordTesting(token, false);
                sb.Append(token);
            }
        }
        /// <summary>
        /// 將指定物件的字串表示附加至這個執行個體的尾端。它會視需要而自行決定是否要加空白字元在 Token 前
        /// </summary>
        /// <param name="token"></param>
        public void AppendKeywordToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                bool IsKeyword = true;// KeywordTesting(token, true);
                if (sb.Length > 0)
                {
                    // 只要前後字元包含 ' ()<>=' 符號則不加空白, 反之則加
                    char last_char = sb[sb.Length - 1];
                    char token_first_char = token[0];
                    if (last_char != ' ' &&
                        (
                        IsKeyword || IsKeywordOnPrevious ||
                        !(include_string.IndexOf(last_char) != -1 ||
                        include_string.IndexOf(token_first_char) != -1)
                        ))
                    {
                        sb.Append(' ');
                    }
                }
                IsKeywordOnPrevious = true; // KeywordTesting(token, false);

                if (Options.HasFlag(CommandFormatOptions.Html))
                    Append($"<span style='color:#2929a3'>{token}</span> ");
                else
                    sb.Append(token);
            }
        }

        /// <summary>
        /// 將指定物件的字串表示附加至這個執行個體的尾端。它會視需要而自行決定是否要加空白字元在 Token 前
        /// </summary>
        /// <param name="token"></param>
        internal void AppendToken(Token token , bool ForceLowerName = false)
        {
            if (!token.IsEmpty())
            {
                if (token.Type == TokenType.String)
                {
                    if (token.String != null &&
                        token.String.StartsWith("N'", StringComparison.OrdinalIgnoreCase)
                         && token.String.EndsWith("'"))
                        AppendToken(token.String); // 己包含 N' 開頭字元, ,不需要再加入
                    else
                        AppendToken($"N'{token.String.Replace("'", "''")}'"); // 如果有單引號則回復成為二個單引號
                }
                else if (ForceLowerName)
                    AppendToken(token.String?.ToLower());
                else 
                    AppendToken(token.String);
            }
        }

        /// <summary>
        /// 將指定物件的字串表示附加至這個執行個體的尾端。
        /// </summary>
        /// <param name="str"></param>
        public void Append(string str)
        {
            if (str != null && str.Length != 0)
            {
                sb.Append(str);
                IsKeywordOnPrevious = false;
            }
        }

        /// <summary>
        /// 將指定物件的字串表示附加至這個執行個體的尾端。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        public void AppendFormat(string str, params object[] args)
        {
            if (str != null && str.Length != 0)
            {
                sb.AppendFormat(str, args);
                IsKeywordOnPrevious = false;
            }
        }

        /// <summary>
        /// 將指定的 Unicode 字元其字串表示附加至這個執行個體的尾端。
        /// </summary>
        /// <param name="ch"></param>
        public void Append(char ch)
        {
            sb.Append(ch);
            IsKeywordOnPrevious = false;
        }

        internal void Append(SQL.Expression.ComparisonOperator Operator)
        {
            sb.AppendFormat(" {0} ", Operator);
            IsKeywordOnPrevious = false;
        }

        /// <summary>
        /// 將 CommandBuilder 的值轉換為 String。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return sb.ToString();
        }

        /// <summary>
        /// 釋放物件及其所擁有的資源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer call
        /// </summary>
        ~CommandBuilder()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// The bulk of the clean-up code is implemented in Dispose(bool)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //// free managed resources
            }
            sb = null;
            // free native resources if there are any.
        }
    }
}