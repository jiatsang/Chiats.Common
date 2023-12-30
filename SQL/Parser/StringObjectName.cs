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
    /// K123 ->  A.B.C  �ѪR�з� SQL ����k �p [database].[owner].[object_name]
    /// ��
    /// ���W�٩w�q(���t�B�⦡) [alisa].[column_name]
    /// </summary>
    internal class StringObjectName
    {
        public enum KeyIndexType
        {
            /// <summary>
            /// ���t�ȥ����� 0 ~ 3 �]���L����ѧO�X�Ӽ�.
            /// </summary>
            K0 = 0, K1 = 1, K2 = 2, K3 = 3
        }

        private string[] names = new string[3];

        public KeyIndexType KeyIndex;

        public TokenType Type;

        // ��ܬ��D�W�٤� ���W�� Ex (select ..... ) a
        public SelectModel SelectModel;

        public StringObjectName()
        {
        }

        public StringObjectName(string name)
        {
            string[] KeyList = split(name);
            if (names.Length < KeyList.Length)
                throw new SqlModelSyntaxException("�L�Ī����٩����W��");

            KeyIndex = (StringObjectName.KeyIndexType)KeyList.Length;
            for (int i = 0; i < KeyList.Length; i++) names[i] = KeyList[i];

            SelectModel = null;
            Type = TokenType.Keyword;
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
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
        /// �Ĥ@�ӦW�٭�
        /// </summary>
        public string Name1
        {
            get { return names[0]; }
            set { names[0] = value; }
        }

        /// <summary>
        /// �ĤG�ӦW�٭�
        /// </summary>
        public string Name2
        {
            get { return names[1]; }
            set { names[1] = value; }
        }

        /// <summary>
        /// �ĤT�ӦW�٭� , �� K123 �� ���W�٩w�q ���ɬ��L�ĭ�
        /// </summary>
        public string Name3
        {
            get { return names[2]; }
            set { names[2] = value; }
        }

        /// <summary>
        /// ���Φr��H '.' �ä䴩 ���A��
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
        /// suport ( select ... ) �y�k
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
                    throw new SqlModelSyntaxException("�L�Ī����٩����W��");

                KeyIndex = (KeyIndexType)KeyList.Length;
                for (int i = 0; i < KeyList.Length; i++) names[i] = KeyList[i];

                // ���ʫ��Ц�m. �è��o�U�@�Ӧ�m Token object
                index++;
                if (index < list.Count)
                    NextToken = list[index];
            }
            else if (TestToken.Type == TokenType.Symbol && TestToken.String == "(")
            {
                //  ��M�U�@�Ӱt�諸 ")" �Ÿ�.

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
                        // ���� () �A�����@��.
                        list.RemoveAt(end_index);
                        list.RemoveAt(index);

                        // �ץ�������m
                        list._fix_end -= 2;
                        _fix -= 2;
                        return Test(list, ref index, ref _fix);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// ��ܬO�_����檫��W�٩w�q [database].[owner].[object_name]
        /// </summary>
        public bool IsTableName
        {
            get { return KeyIndex != 0; }
        }

        /// <summary>
        /// table_source ��ܬO�_����檫��w�q. �]�t ��檫��W�٩w�q �Υt�@�� Select
        /// </summary>
        public bool IsTableSource
        {
            get { return KeyIndex != 0 || SelectModel != null; }
        }

        /// <summary>
        /// ��ܬO�_����쪫��W�٩w�q [table_name].[column_name]
        /// </summary>
        public bool IsColumnName
        {
            get { return KeyIndex != 0 && (int)KeyIndex < 3; }
        }

        /// <summary>
        /// �^�� Table Source Object
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
        /// �^�� TableName Object
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
        /// �^�� ColumnName or Alias.ColumnName
        /// </summary>
        public string ColumnName
        {
            get
            {
                if (KeyIndex == KeyIndexType.K1)
                    return Name1; // ColumnName
                if (KeyIndex == KeyIndexType.K2)
                    return string.Format("{0}.{1}", SQLHelper.PackObjectName(Name1), SQLHelper.PackObjectName(Name2));

                throw new SqlModelSyntaxException("�L�Ī����W��.", string.Format("{0}.{1}.{2}", Name1, Name2, Name3));
            }
        }
    }
}