// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL �ѪR�r��R�O�@�Τ���. �̦r��R�O���� InsertModel ����
    /// </summary>
    internal class CreateTableParser : BaseParser, ISqlParserObject
    {
        // delete from table where ...
        public static CreateTableParser Default = new CreateTableParser();

        #region ISqlParserObject Members

        public SqlModel PaserCommand(SQLTokenScanner tokenScanner, SqlModel SqlModel, object parameters = null)
        {
            CreateTableModel currentModel = ConvertCommonModel<CreateTableModel>(SqlModel);
            int currrntIndex = 0;

            ((ICommandInitialize)currentModel).BeginInit();

            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "Create"))
            {
                currrntIndex++;
                if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "Table"))
                {
                    currrntIndex++;
                    StringObjectName k123 = StringObjectName.NameTest(tokenScanner, ref currrntIndex);

                    if (k123.IsTableName)
                        currentModel.Table = k123.Table;
                    else
                        throw new SqlModelSyntaxException("Not Table Name Found.");

                    if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "("))
                    {
                        currrntIndex++;
                        int level = 0;
                        int end = -1; // tokenScanner.Find(")", currrntIndex, true);
                        for (int i = currrntIndex; i < tokenScanner.Count; i++)
                        {
                            if (tokenScanner[i].String == "(") level++;

                            if (tokenScanner[i].String == ")")
                            {
                                if (level-- == 0)
                                {
                                    end = i;
                                    break;
                                }
                            }
                        }

                        if (end == -1)
                            throw new SqlModelSyntaxException("�������w������Ÿ� ')'");

                        ParserColumns(tokenScanner, currentModel, currrntIndex, end);
                        currrntIndex = ++end;
                    }
                    else
                    {
                        throw new SqlModelSyntaxException("�������w������Ÿ� '(',SqlModel �|���䴩�����w���W�٤� Create Table �ԭz.");
                    }
                }
                else
                {
                    throw new SqlModelSyntaxException("Syntax Error {0} Create {0}", tokenScanner[currrntIndex].String);
                }
            }
            else
            {
                ParserColumns(tokenScanner, currentModel, 0, tokenScanner.Count);
            }

            ((ICommandInitialize)currentModel).EndInit();

            return currentModel;
        }

        /// <summary>
        /// ���ܥ�����, �Ҳ��ͤ�����  SqlModel �۹������󪫥����O �p SelectModel/UpdateModel/InsertModel/DeleteModel
        /// </summary>
        public Type ModelType
        {
            get { return typeof(InsertModel); }
        }

        #endregion ISqlParserObject Members

        private Queue<string> columnNames = new Queue<string>();

        private void ParserColumns(SQLTokenScanner TokenScanner, CreateTableModel currentModel, int CurrentIndex, int EndIndex)
        {
            int find_index = CurrentIndex;
            for (; ; )
            {
                if (find_index > EndIndex) break;
                StringObjectName k123 = StringObjectName.NameTest(TokenScanner, ref find_index, ref EndIndex);
                if (k123.IsColumnName)
                    columnNames.Enqueue(k123.ColumnName);
                else
                    throw new SqlModelSyntaxException("�L�Ī����W��. {0}", TokenScanner[find_index]);

                string typeName = TokenScanner[find_index].String;
                string size1 = null, size2 = null;
                // find typeName(size1,size2)
                if (TokenScanner[find_index].Type == TokenType.Keyword)
                {
                    find_index++;
                    if (find_index < EndIndex && TokenScanner[find_index].String == "(")
                    {
                        find_index++;
                        if (find_index < EndIndex && TokenScanner[find_index].Type == TokenType.Number)
                        {
                            size1 = TokenScanner[find_index].String;
                            find_index++;
                            if (find_index < EndIndex && TokenScanner[find_index].String == ",")
                            {
                                find_index++;
                                if (find_index < EndIndex && TokenScanner[find_index].Type == TokenType.Number)
                                {
                                    size2 = TokenScanner[find_index].String;
                                    find_index++;
                                }
                                else
                                    throw new SqlModelSyntaxException("�������w������Ÿ� ','");
                            }
                        }
                        if (find_index < EndIndex && TokenScanner[find_index].String != ")")
                            throw new SqlModelSyntaxException("�������w������Ÿ� ')'");
                        find_index++;
                    }
                }
                if (find_index < EndIndex && TokenScanner[find_index].String != ",")
                    throw new SqlModelSyntaxException("�������w������Ÿ� ','");

                ColumnType t = ColumnTypeHelper.ConvertColumnType(typeName);
                switch (t)
                {
                    case ColumnType.Binary:
                    case ColumnType.Varchar:
                    case ColumnType.Nvarchar:
                        int size = Convert.ToInt32(size1);
                        currentModel.Columns.Add(k123.ColumnName, t, size, 0, 0);
                        break;

                    case ColumnType.Decimal:
                        short s1 = Convert.ToInt16(size1);
                        short s2 = Convert.ToInt16(size2);
                        if (s1 == 0)
                        {
                            s1 = 24; s2 = 4;
                        }
                        currentModel.Columns.Add(k123.ColumnName, t, 0, s1, s2);
                        break;

                    default:
                        currentModel.Columns.Add(k123.ColumnName, t, 0, 0, 0);
                        break;
                }
                find_index++;
            }
        }
    }
}