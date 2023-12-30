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
    internal abstract class BaseParser
    {
        private static string[] KeywordSortedList = {
          "on", "in",
          "insert", "update",  "delete","select",
          "from", "where","asc", "desc", "order", "group", "by","join", "inner", "outer","left", "right",
          "as", "and", "or", "not" ,"with","over"
        };

        private static string[] KeywordExpressList = { "and", "or", "not" };

        public BaseParser()
        {
            Array.Sort<string>(KeywordSortedList, StringComparer.OrdinalIgnoreCase);
        }

        protected enum CTLGroupMode
        {
            /// <summary>
            /// ��ܸѪR�b Group �d��~
            /// </summary>
            Outside,

            /// <summary>
            /// ��ܸѪR�b Group �d��
            /// </summary>
            Inside
        }

        protected class CTLGroup
        {
            private SQLTokenScanner tokenList;
            private string groupName = null;
            private ArgumentType funcName = ArgumentType.None;
            public readonly int StartLocation = 0, EndLocation = 0, CommaLocation = 0;

            /*Support Syntax : {$ID ConditionExpression} | { ConditionExpression}*/

            public CTLGroup(SQLTokenScanner tokenList, CTLGroupMode mode, int start, int end)
            {
                this.tokenList = tokenList;
                GroupMode = mode;

                if (GroupMode == CTLGroupMode.Inside)
                {
                    // �ѪR Group Name {$GroupName ... } or �ΦW(Anonymous)�g�k {$ ... }
                    StartLocation = start + 1;  // Skip {
                    EndLocation = end - 1;      // Skip }
                    if (StartLocation != EndLocation)
                    {
                        Token token = tokenList[StartLocation];
                        if (token.Type == TokenType.Keyword && token.String.StartsWith("$"))
                        {
                            // �䴩          {$GroupName:Function ColumnOrExpression,@T1}
                            //      �ΦW(Anonymous)�g�k  {$:MutilSocpe T,@T1} �ѪR.
                            if (token.String.Length > 1)
                                groupName = token.String.Substring(1); // �h�� @ �r��

                            if (StartLocation + 2 <= end)
                            {
                                Token next_token = tokenList[StartLocation + 1];
                                Token funcNameToken = tokenList[StartLocation + 2];
                                if (next_token.String == ":" && funcNameToken.Type == TokenType.Keyword)
                                {
                                    funcName = funcNameToken.String.EnumConvert<ArgumentType>(ArgumentType.None);
                                    if (ArgmentType == ArgumentType.None)
                                        throw new SqlModelSyntaxException($"CTLGroup: Invalid FunctionName :{funcNameToken.String}");

                                    StartLocation += 2;     // Skip ':' and 'FunctionName'
                                    int level = 0;
                                    for (int i = start; i <= end; i++)
                                    {
                                        if (tokenList[i].IsSymbol("(")) level++;
                                        else if (tokenList[i].IsSymbol(")")) level--;
                                        else if (level == 0 && tokenList[i].IsSymbol(","))
                                        {
                                            // Find Comma(,) location
                                            CommaLocation = i;
                                            break;
                                        }
                                    }
                                    if (CommaLocation == 0)
                                        throw new SqlModelSyntaxException("CTLGroup: Invalid FunctionName Parameter : {0}", tokenList.RebuildToken(start, end));
                                }
                            }
                            StartLocation++;   // Skip 'GroupName'
                        }
                    }
                }
                else
                {
                    StartLocation = start;
                    EndLocation = end;
                }
            }

            public string GroupName
            {
                get { return groupName; }
            }

            public ArgumentType ArgmentType
            {
                get { return funcName; }
            }

            public readonly CTLGroupMode GroupMode = CTLGroupMode.Outside;
        }

        // Split CTL Syntax  Example: A { b c } c { D } {E F G}
        protected CTLGroup[] SplitGroupByCTL(SQLTokenScanner tokenList, int start_index, int end_index)
        {
            List<CTLGroup> list = new List<CTLGroup>();
            int currentStart = 0;
            int current_left = start_index;
            bool groupFound = false; /* ���ܥثe�O�_�b Group �d�� */

            for (int i = start_index; i <= end_index; i++)
            {
                if (tokenList[i].String == "{")
                {
                    if (groupFound)
                        throw new SqlModelSyntaxException("CTLGroup: Syntax Error");

                    groupFound = true; /* �i�J Group �d�� */
                    currentStart = i;
                    if (current_left < i)
                    {
                        list.Add(new CTLGroup(tokenList, CTLGroupMode.Outside, current_left, i - 1));
                    }
                    current_left = -1;
                }
                else if (tokenList[i].String == "}")
                {
                    if (currentStart == 0 || !groupFound)
                        throw new SqlModelSyntaxException("CTLGroup: Syntax Error");

                    groupFound = false;  /* ���} Group �d�� */

                    list.Add(new CTLGroup(tokenList, CTLGroupMode.Inside, currentStart, i));
                    current_left = i + 1;
                }
            }
            if (current_left != -1 && current_left < end_index)
            {
                list.Add(new CTLGroup(tokenList, CTLGroupMode.Outside, current_left, end_index));
            }
            return list.ToArray();
        }

        public static bool IsNotExpressionKeyword(string key)
        {
            foreach (string t in KeywordExpressList)
            {
                if (string.Compare(t, key, true) == 0) { return true; }
            }
            return Array.BinarySearch<string>(KeywordSortedList, key, StringComparer.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsKeyword(string key)
        {
            return Array.BinarySearch<string>(KeywordSortedList, key, StringComparer.OrdinalIgnoreCase) >= 0;
        }

        protected void ParserWhereStatement(SQLTokenScanner tokenList, WhereClause where, int start_index, int end_index)
        {
            // Debug.Print("Parser Where : {0}", RebuildToken(tokenScanner, start_index, end_index));
            // Support SQL CTL : {$T1 T1=@T1} => {@T1}

            CTLGroup[] CTLGroups = SplitGroupByCTL(tokenList, start_index, end_index);
            if (CTLGroups.Length > 0)
            {
                ConditionLink link = ConditionLink.And; //  �O���W�@�ӳs���F  and/or

                foreach (CTLGroup group in CTLGroups)
                {
                    if (group.GroupMode == CTLGroupMode.Outside && group.StartLocation == group.EndLocation)
                    {
                        // �ѪR��@�s���B�⤸(ConditionLink)
                        string TokenString = tokenList[group.StartLocation].String;

                        if (!TokenString.EnumConvert<ConditionLink>(out link))
                            throw new SqlModelSyntaxException("CTLGroup : �s���B�⤸������ And/Or ,���i�H�O {0}", TokenString);
                    }
                    else if (group.StartLocation != group.EndLocation)
                    {
                        if (group.ArgmentType != ArgumentType.None)
                        {
                            // �[�J�@�ӥ\�૬������B�⦡
                            string ParamName;
                            string ColumnExpression = tokenList.RebuildToken(group.StartLocation, group.CommaLocation - 1);
                            if (string.IsNullOrEmpty(ColumnExpression))
                                throw new SqlModelSyntaxException("CTLGroup : Function Paramerter ColumnExpression ���o���Ŧr��");

                            if (group.CommaLocation + 1 == group.EndLocation)
                            {
                                // �Ѽƥu�঳�@�� token �åB�O @�}�Y���ѼƦW��
                                int ParameterLocation = group.EndLocation;
                                ParamName = tokenList[ParameterLocation].String;
                                if (ParamName == null || !ParamName.StartsWith("@"))
                                    throw new SqlModelSyntaxException("CTLGroup : Function Paramerter ������@�}�Y���X�k�ѼƦW��");
                            }
                            else
                                throw new SqlModelSyntaxException("Function Paramerter Error!");

                            where.AddFunction(group.GroupName, group.ArgmentType, ColumnExpression, ParamName);
                        }
                        else
                        {
                            ConditionLink nextLink = ConditionLink.And, firstLink = ConditionLink.And;

                            // �Ĥ@�s����(and/or) �����󦡪��u�����e�m�s����.
                            Token FirstToken = tokenList[group.StartLocation];
                            bool FirstTokenIsLink = (FirstToken.IsKeyword && FirstToken.String.EnumConvert<ConditionLink>(out firstLink));

                            Token LastToken = tokenList[group.EndLocation];
                            bool LastTokenIsLink = (LastToken.IsKeyword && LastToken.String.EnumConvert<ConditionLink>(out nextLink));

                            where.Add(group.GroupName,
                                tokenList.RebuildToken(group.StartLocation + ((FirstTokenIsLink) ? 1 : 0)
                                , group.EndLocation - ((LastTokenIsLink) ? 1 : 0)),
                                (FirstTokenIsLink) ? firstLink : link);
                            // �̫�@�ӵ����s����(and/or) ���U�@�ӱ��󦡪��e�m�s����.
                            link = nextLink;
                        }
                    }

                    // Debug.Print("CTLGroupItem({0},{1},{2}) => {3}", GroupItem.GroupMode, GroupItem.StartLocation, GroupItem.EndLocation, RebuildToken(tokenScanner, GroupItem.StartLocation, GroupItem.EndLocation));
                }
            }
        }

        protected T ConvertCommonModel<T>(SqlModel SqlModel) where T : SqlModel
        {
            if (SqlModel != null)
            {
                if (typeof(T) == SqlModel.GetType())
                    return (T)SqlModel;
            }
            return Activator.CreateInstance<T>();
        }
    }
}