// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats
{
    public struct Token  // changeto internal
    {
        public TokenType Type;
        public string String;
        public int StartAt;
        public int EndAt;

        public Token(TokenType Type, string String, int StartAt, int EndAt)
        {
            this.Type = Type;
            this.String = String;
            this.StartAt = StartAt;
            this.EndAt = EndAt;
        }

        public static Token Empty = new Token { Type = TokenType.Null, String = "", StartAt = 0, EndAt = 0 };

        public static bool operator ==(Token token1, Token token2)
        {
            return (token1.Type == token2.Type && token1.String == token2.String);
        }

        // 取得字串常數的內容值. 資料為非字串時則回傳 null
        public string StringConstant
        {
            get
            {
                if (IsString)
                {
                    return String;    // 解析後不再保留前後單引號.
                }
                return null;
            }
        }

        public string Source
        {
            get
            {
                if (IsString)
                {
                    return $"'{String}'";    // 解析後不再保留前後單引號.
                }
                return String;
            }
        }

        #region Token Compare Function

        public bool IsSymbol()
        {
            return (Type == TokenType.Symbol);
        }

        public bool IsSymbol(string Symbol)
        {
            return (Type == TokenType.Symbol && String == Symbol);
        }

        public bool IsSymbol(Char Symbol)
        {
            return (Type == TokenType.Symbol && String.Length == 1 && String[0] == Symbol);
        }

        public bool IsKeywordAndMatch(string Keyword)
        {
            return (Type == TokenType.Keyword && StringComparer.OrdinalIgnoreCase.Equals(String, Keyword));
        }

        public int FindKeywordAndMatch(params string[] Keywords)
        {
            if (Type == TokenType.Keyword)
            {
                for (int index = 0; index < Keywords.Length; index++)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(String, Keywords[index])) return index;
                }
            }
            return -1;
            //return (Type == TokenType.Keyword && Comparer.String.Equals(String, Keyword));
        }

        public bool IsKeyword
        {
            get
            {
                return (Type == TokenType.Keyword);
            }
        }

        public bool IsString
        {
            get
            {
                return (Type == TokenType.String);
            }
        }

        public bool IsNumber
        {
            get
            {
                return (Type == TokenType.Number);
            }
        }

        public string IsKeywordAndMatch(string[] Keywords)
        {
            if (Type == TokenType.Keyword)
            {
                foreach (string key in Keywords)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(String, key)) return key;
                }
            }
            return null;
        }

        #endregion Token Compare Function

        public static bool operator !=(Token token1, Token token2)
        {
            return (token1.Type != token2.Type || token1.String != token2.String);
        }

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this == Token.Empty)
                return "(token.empty)";
            return string.Format(" '{0}' (Type={1} Location={2})", String, Type, StartAt);
        }

        public override int GetHashCode()
        {
            if (String != null)
                return Type.GetHashCode() + String.GetHashCode();
            return this.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool IsEmpty()
        {
            return Type == TokenType.Null || String == null;
        }
    }
}