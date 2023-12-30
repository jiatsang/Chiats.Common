// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using Chiats.SQL;
using System;
using System.Collections.Generic;

namespace Chiats
{

    /// <summary>
    /// 文字串符記號掃描解析基礎類別.
    /// </summary>
    public abstract class TokenScanner : IEnumerable<Token>, IDisposable// changeto internal
    {
        protected List<Token> token_list = new List<Token>();
        protected string statement;

        /// <summary>
        /// 傳回是否為分隔符號.  空白/換行符號 '\n''\r' /TAB '\t'/結束符號 '\0
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected bool is_space_char(char ch)
        {
            return (ch == '\x20' || ch == '\r' || ch == '\n' || ch == '\t' || ch == '\0');
        }

        /// <summary>
        /// 傳回行結束符號
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected bool is_end_char(char ch)
        {
            return (ch == '\r' || ch == '\n' || ch == '\0');
        }

        public TokenScanner(string statement, bool Scan = true)
        {
            if (statement != null)
            {
                this.statement = statement;
                if (Scan) ScanNow();
            }
        }

        public TokenScanner(TokenScanner scanner, int start, int end)
            : this(null)
        {
            Token start_token = scanner[start];
            Token end_token = scanner[end];
            int start_at = start_token.StartAt;

            statement = scanner.Statement.Substring(start_at, (end_token.StartAt + end_token.String.Length) - start_at);
            // statement = scanner.Statement.Substring(start_at, end_token.EndAt - start_at + 1);

            for (int i = start; i <= end; i++)
            {
                Token _token = scanner[i];
                _token.StartAt -= start_at;
                token_list.Add(_token);
            }
        }

        protected virtual void ScanNow() { }

        public Token this[int index]
        {
            get
            {
                if (index < token_list.Count)
                    return (Token)token_list[index];
                return Token.Empty;
            }
        }

        /// <summary>
        /// 子物件個數
        /// </summary>
        public int Count
        {
            get { return token_list.Count; }
        }

        protected void add_token(Token key)
        {
            token_list.Add(key);
        }

        public string Statement
        {
            get
            {
                return this.statement;
            }
        }

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.statement;
        }

        public int Find(string key)
        {
            return Find(key, 0, true);
        }

        public int Find(string key, int start, bool ignoreCase)
        {
            for (int i = start; i < Count; i++)
            {
                if (string.Compare(this[i].String, key, ignoreCase) == 0)
                    return i;
            }
            return -1;
        }

        public string RebuildToken(int start_index = 0, int end_index = -1, bool ForceLowerName = false) 
        {

            if (end_index == -1 || end_index > token_list.Count) end_index = token_list.Count - 1;

            using (CommandBuilder sb = new CommandBuilder())
            {
                for (int i = start_index; i <= end_index; i++)
                {
                   sb.AppendToken(this[i] , ForceLowerName);
                }
                return sb.ToString();
            }
        }

        #region IEnumerable<Token> Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Token> GetEnumerator()
        {
            return token_list.GetEnumerator();
        }

        #endregion IEnumerable<Token> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return token_list.GetEnumerator();
        }

        #endregion IEnumerable Members

        /// <summary>
        /// 釋放物件及其所擁有的資源.
        /// </summary>
        public void Dispose()
        {
            token_list = null;
            statement = null;
        }
    }
}