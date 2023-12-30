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
    /// SQL Command ���r�겣�;� ( CommandBuilder �ʸ� StringBuilder ����. �å[�H�X�R )
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
        /// �Y��
        /// </summary>
        public void Indent()
        {
            IndentLevel++;
        }

        /// <summary>
        /// �Y��
        /// </summary>
        public void Unindent()
        {
            IndentLevel--;
        }

        /// <summary>
        /// ����
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
        /// �N���w���󪺦r����ܪ��[�ܳo�Ӱ�����骺���ݡC���|���ݭn�Ӧۦ�M�w�O�_�n�[�ťզr���b Token �e
        /// </summary>
        /// <param name="token_char"></param>
        public void AppendToken(char token_char)
        {
            if (sb.Length > 0)
            {
                // �u�n�e��r���]�t ' ()<>=' �Ÿ��h���[�ť�, �Ϥ��h�[
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
        /// �ثe���r��Ӽ�
        /// </summary>
        public int Length
        {
            get { return sb.Length; }
        }

        /// <summary>
        ///  �^�ǭӧO�r��
        /// </summary>
        /// <param name="index">�r����m</param>
        /// <returns></returns>
        public char this[int index]
        {
            get { return sb[index]; }
        }

        /// <summary>
        /// �N�o�Ӱ�����餤�Ҧ��X�{�����w�r����N���t�@�ӫ��w�r��C
        /// </summary>
        /// <param name="oldString">�n���N���r��C</param>
        /// <param name="newString">���N oldValue ���r��� Null �ѷ� (�Y Visual Basic ���� Nothing)�C</param>
        public void Replace(string oldString, string newString)
        {
            sb.Replace(oldString, newString);
        }

        /// <summary>
        /// �H�t�@�ӫ��w���r���A���N�o�Ӱ�����餤���w���r���Φr�ꪺ�Ҧ����ءC
        /// </summary>
        /// <param name="oldChar">�n���N���r���C</param>
        /// <param name="newChar">���N oldValue ���r��� Null �ѷ� (�Y Visual Basic ���� Nothing)�C</param>
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

                if (last_index != -1) // ��ܤ��t�ť�
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
                    // �u�n�e��r���]�t ' ()<>=' �Ÿ��h���[�ť�, �Ϥ��h�[
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
        /// �N���w���󪺦r���ܪ��[�ܳo�Ӱ�����骺���ݡC���|���ݭn�Ӧۦ�M�w�O�_�n�[�ťզr���b Token �e
        /// </summary>
        /// <param name="token"></param>
        public void AppendKeywordToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                bool IsKeyword = true;// KeywordTesting(token, true);
                if (sb.Length > 0)
                {
                    // �u�n�e��r���]�t ' ()<>=' �Ÿ��h���[�ť�, �Ϥ��h�[
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
        /// �N���w���󪺦r���ܪ��[�ܳo�Ӱ�����骺���ݡC���|���ݭn�Ӧۦ�M�w�O�_�n�[�ťզr���b Token �e
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
                        AppendToken(token.String); // �v�]�t N' �}�Y�r��, ,���ݭn�A�[�J
                    else
                        AppendToken($"N'{token.String.Replace("'", "''")}'"); // �p�G����޸��h�^�_�����G�ӳ�޸�
                }
                else if (ForceLowerName)
                    AppendToken(token.String?.ToLower());
                else 
                    AppendToken(token.String);
            }
        }

        /// <summary>
        /// �N���w���󪺦r���ܪ��[�ܳo�Ӱ�����骺���ݡC
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
        /// �N���w���󪺦r���ܪ��[�ܳo�Ӱ�����骺���ݡC
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
        /// �N���w�� Unicode �r����r���ܪ��[�ܳo�Ӱ�����骺���ݡC
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
        /// �N CommandBuilder �����ഫ�� String�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return sb.ToString();
        }

        /// <summary>
        /// ���񪫥�Ψ�Ҿ֦����귽.
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