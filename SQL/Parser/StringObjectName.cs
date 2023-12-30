// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Chiats.SQL
{
    /// <summary>
    /// K123 ->  A.B.C  解析標準 SQL 的文法 如 [database].[owner].[object_name]
    /// 或
    /// 欄位名稱定義(不含運算式) [alisa].[column_name]
    /// </summary>
    internal class StringObjectName
    {
        public enum KeyIndexType
        {
            /// <summary>
            /// 內含值必須為 0 ~ 3 因為他表示識別碼個數.
            /// </summary>
            K0 = 0, K1 = 1, K2 = 2, K3 = 3
        }

        private string[] names = new string[3];

        public KeyIndexType KeyIndex;

        public TokenType Type;

        // 表示為非名稱之 表格名稱 Ex (select ..... ) a
        public SelectModel SelectModel;

        public StringObjectName()
        {
        }

        public StringObjectName(string name)
        {
            string[] KeyList = split(name);
            if (names.Length < KeyList.Length)
                throw new SqlModelSyntaxException("無效的表格稱或欄位名稱");

            KeyIndex = (StringObjectName.KeyIndexType)KeyList.Length;
            for (int i = 0; i < KeyList.Length; i++) names[i] = KeyList[i];

            SelectModel = null;
            Type = TokenType.Keyword;
        }

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < (int)KeyIndex; i++)
            {
                if (i != 0) sb.Append('.');
                sb.Append(names[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 第一個名稱值
        /// </summary>
        public string Name1
        {
            get { return names[0]; }
            set { names[0] = value; }
        }

        /// <summary>
        /// 第二個名稱值
        /// </summary>
        public string Name2
        {
            get { return names[1]; }
            set { names[1] = value; }
        }

        /// <summary>
        /// 第三個名稱值 , 當 K123 為 欄位名稱定義 此時為無效值
        /// </summary>
        public string Name3
        {
            get { return names[2]; }
            set { names[2] = value; }
        }

        /// <summary>
        /// 切割字串以 '.' 並支援 中括號
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string[] split(string name)
        {
            List<string> list = new List<string>();
            string current = null;
            int brackets = 0;

            foreach (char c in name)
            {
                if (c == '[') brackets++;
                else if (c == ']') brackets--;
                else if (c == '.')
                {
                    if (brackets == 0) { list.Add(current); current = null; }
                    else
                        current += c;
                }
                else
                    current += c;
            }
            list.Add(current);
            return list.ToArray();
        }

        public static StringObjectName NameTest(SQLTokenScanner list, ref int index)
        {
            int _fix = 0;
            return (new StringObjectName()).Test(list, ref index, ref _fix);
        }

        public static StringObjectName NameTest(SQLTokenScanner list, ref int index, ref int _fix)
        {
            return (new StringObjectName()).Test(list, ref index, ref _fix);
        }

        public static StringObjectName NameTest(SQLTokenScanner list, int index)
        {
            int _fix = 0;
            return (new StringObjectName()).Test(list, ref index, ref _fix);
        }

        public static StringObjectName NameTest(SQLTokenScanner list)
        {
            int _index = 0, _fix = 0;
            return (new StringObjectName()).Test(list, ref _index, ref _fix);
        }

        /// <summary>
        /// suport ( select ... ) 語法
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="_fix"></param>
        /// <returns></returns>
        public StringObjectName Test(SQLTokenScanner list, ref int index, ref int _fix)
        {
            Token TestToken = list[index];
            Token NextToken = Token.Empty;
            KeyIndex = 0;

            if (TestToken.Type == TokenType.Keyword)
            {
                string[] KeyList = split(TestToken.String);

                if (names.Length < KeyList.Length)
                    throw new SqlModelSyntaxException("無效的表格稱或欄位名稱");

                KeyIndex = (KeyIndexType)KeyList.Length;
                for (int i = 0; i < KeyList.Length; i++) names[i] = KeyList[i];

                // 移動指標位置. 並取得下一個位置 Token object
                index++;
                if (index < list.Count)
                    NextToken = list[index];
            }
            else if (TestToken.Type == TokenType.Symbol && TestToken.String == "(")
            {
                //  找尋下一個配對的 ")" 符號.

                int end_index = index + 1;
                bool EndSymbolFound = false;
                int SymbolFound = 0;
                for (; end_index < list.Count; end_index++)
                {
                    Token TestEndToken = list[end_index];
                    if (TestEndToken.Type == TokenType.Symbol)
                    {
                        if (TestEndToken.String == "(") SymbolFound++;
                        if (TestEndToken.String == ")")
                        {
                            if (SymbolFound == 0)
                            {
                                EndSymbolFound = true;
                                break;
                            }
                            else
                                SymbolFound--;
                        }
                    }
                }
                if (EndSymbolFound)
                {
                    Token FirstToken = list[index + 1];
                    if (FirstToken.IsKeyword && StringComparer.OrdinalIgnoreCase.Equals(FirstToken.String, "Select"))
                    {
                        // Select
                        SQLTokenScanner newList = list.Copy(index + 1, end_index - 1);
                        SelectModel = (SelectModel)SelectParser.Default.PaserCommand(newList, null);
                        index = ++end_index;
                    }
                    else
                    {
                        // Example : Select ... From (table a left join ....)
                        // 忽略 () 再重測一次.
                        list.RemoveAt(end_index);
                        list.RemoveAt(index);

                        // 修正結束位置
                        list._fix_end -= 2;
                        _fix -= 2;
                        return Test(list, ref index, ref _fix);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// 表示是否為表格物件名稱定義 [database].[owner].[object_name]
        /// </summary>
        public bool IsTableName
        {
            get { return KeyIndex != 0; }
        }

        /// <summary>
        /// table_source 表示是否為表格物件定義. 包含 表格物件名稱定義 或另一個 Select
        /// </summary>
        public bool IsTableSource
        {
            get { return KeyIndex != 0 || SelectModel != null; }
        }

        /// <summary>
        /// 表示是否為欄位物件名稱定義 [table_name].[column_name]
        /// </summary>
        public bool IsColumnName
        {
            get { return KeyIndex != 0 && (int)KeyIndex < 3; }
        }

        /// <summary>
        /// 回傳 Table Source Object
        /// </summary>
        public TableSource CreateNewTableSource
        {
            get
            {
                switch (KeyIndex)
                {
                    case KeyIndexType.K0: return new TableSource(SelectModel);
                    case KeyIndexType.K1: return new TableSource(null, null, Name1);
                    case KeyIndexType.K2: return new TableSource(null, Name1, Name2);
                    case KeyIndexType.K3: return new TableSource(Name1, Name2, Name3);
                }
                return new TableName(this.ToString());
            }
        }

        /// <summary>
        /// 回傳 TableName Object
        /// </summary>
        public TableName Table
        {
            get
            {
                switch (KeyIndex)
                {
                    case KeyIndexType.K1: return new TableName(null, null, Name1);
                    case KeyIndexType.K2: return new TableName(null, Name1, Name2);
                    case KeyIndexType.K3: return new TableName(Name1, Name2, Name3);
                }
                return new TableName(this.ToString());
            }
        }

        /// <summary>
        /// 回傳 ColumnName or Alias.ColumnName
        /// </summary>
        public string ColumnName
        {
            get
            {
                if (KeyIndex == KeyIndexType.K1)
                    return Name1; // ColumnName
                if (KeyIndex == KeyIndexType.K2)
                    return string.Format("{0}.{1}", SQLHelper.PackObjectName(Name1), SQLHelper.PackObjectName(Name2));

                throw new SqlModelSyntaxException("無效的欄位名稱.", string.Format("{0}.{1}.{2}", Name1, Name2, Name3));
            }
        }
    }
}