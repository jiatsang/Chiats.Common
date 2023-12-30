// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Chiats.SQL
{
    /// <summary>
    /// ��Ʈw�s���ѼƳ]�w���e , �]�t��ƫ��A,��Ƴ̤j����,��Ƥp�Ʀ�,��ƹw�]�� ����
    /// </summary>
    /// <remarks>
    /// Parameter �O���� SqlModel �ѼƩM��Ʈw�s���ѼƤ������sô�@��. <br/>
    /// 1. SqlModel �Ѽƥ����|���]�t �ѼƤ����O�j�p�������T��(���@�����ݭn�� �r��(Lexical) �ӥ[�H�ɥR)<br/>
    /// 2. SqlModel ���]�t�F�\�૬�O���ѼƱ���. �\�૬�O���ѼƱ���(Mutli/Scope/MutliScope) �]�t�U�C�S��<br/>
    ///     (1) �Ѽƥi�H�����ܦh�Ӹ�Ʈw�s���Ѽ�. �ѼƦW���X�i�̤@�w���W�h Example @Name#1,@Name#2From,@Name#2To  <br/>
    ///     (2) �Ѽƥu�঳�@���s�b �Y ILinkObject ���|�s���ܨ�L���ۦP�W�ٰѼ�. �_�h���Ӳ��ͤ@�� Execption �ƥ�.<br/>
    ///     (3) �t�X BehaviorArgument ���B�@���Ȥ����o.
    /// </remarks>
    public class Parameter : ILinkObject
    {
        /// <summary>
        /// ��Ʋ��ʳq�� . LinkObject ���ʮɷ|�H���ƥ�q��������. (���t���� Value ���ܧ�ɩһݤ޵o���ƥ�.)
        /// </summary>
        public event EventHandler Changed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ParameterData pd = new ParameterData();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string name = null;

        /// <summary>
        /// �^�ǰѼƪ�����ݪ����󪫥�, �Y���W�߰Ѽƪ���h�� null
        /// </summary>
        public Condition Condition { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ColumnType"></param>
        /// <param name="Size"></param>
        /// <param name="Precision"></param>
        /// <param name="Scale"></param>
        /// <param name="IsNullable"></param>
        /// <param name="Direction"></param>
        /// <param name="DefaultValue"></param>
        public void Copy(ColumnType ColumnType,
            int Size,
            short Precision,
            short Scale,
            bool IsNullable,
            ParameterDirection Direction,
            object DefaultValue)
        {
            pd.SetColumnTypeInfo(ColumnType, Size, Precision, Scale);
            pd.SetColumnNullable(IsNullable);
            pd.Direction = Direction;
            pd.DefaultValue = DefaultValue;

            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        void ILinkObject.Link(ILinkObject ChangedObject)
        {
            Parameter p = ChangedObject as Parameter;
            if (p != null && this != p)
            {
                if (this.pd.ArgmentType == ArgumentType.None)
                    this.pd = p.pd;  // �ϬۦP�W�٤��ѼƦ@�Τ@�� ParameterData
                else
                    throw new SqlModelSyntaxException(@"�\�૬�O���ѼƱ��� {0} �����\���Цs�b", name);
            }
        }

        /// <summary>
        /// ��Ʈw�s���Ѽƫغc�l
        /// </summary>
        /// <param name="name">�ѼƦW��</param>
        public Parameter(string name) : this(name, ArgumentType.None) { }

        /// <summary>
        /// ��Ʈw�s���Ѽƫغc�l
        /// </summary>
        /// <param name="name">�ѼƦW��</param>
        /// <param name="ArgmentType"></param>
        public Parameter(string name, ArgumentType ArgmentType)
        {
            if (name != null && name.StartsWith("@") && name.IndexOf(' ') == -1)
            {
                this.name = name;
                this.pd = new ParameterData();
                this.pd.ArgmentType = ArgmentType;
                return;
            }
            throw new SqlModelSyntaxException(@"�ѼƦW�ٿ��~. ���� @ �}�Y.�B�����\���ťթΨ�L�S��r��.");
        }

        /// <summary>
        /// ��Ʈw�s���Ѽƫغc�l
        /// </summary>
        /// <param name="param"></param>
        public Parameter(Parameter param)
        {
            this.name = param.Name;
            this.pd = param.pd;
        }

        /// <summary>
        /// �H��r�y�z�Ѽƪ��覡�غc�s���Ѽ� ,
        /// ColumnType,Size,Precision,Scale,Direction(In/Out/InOut),ArgmentType,'DefaultValue',Type'Value'
        /// </summary>
        /// <param name="ParameterDescription">�H��r�y�z�Ѽ�</param>
        public void CopyDescription(string ParameterDescription)
        {
            string[] descriptions = Split(ParameterDescription);
            if (descriptions.Length == 8)
            {
                ColumnType = CommonExtensions.EnumConvert<ColumnType>(descriptions[0], ColumnType.Auto);
                Size = descriptions[1].ChangeType<int>();
                NumericPrecision = descriptions[2].ChangeType<short>();
                NumericScale = CommonExtensions.ChangeType<short>(descriptions[3]);
                Direction = CommonExtensions.EnumConvert<ParameterDirection>(descriptions[4], ParameterDirection.Input);
                ArgmentType = CommonExtensions.EnumConvert<ArgumentType>(descriptions[5], ArgumentType.None);
                DefaultValue = GetStringDefaultValue(descriptions[6]);
                Value = GetStringValue(descriptions[7]);
            }
        }

        private string GetStringDefaultValue(string DefaultValue)
        {
            if (DefaultValue != null && DefaultValue.Length >= 2)
            {
                if (DefaultValue == "''")
                    return null;
                // ����нs�X, ����Ÿ���� %
                return PropertyExtensions.DecodingString(DefaultValue.Substring(1, DefaultValue.Length - 2), '%');
            }
            return null;
        }

        private object GetStringValue(string Value)
        {
            // �ѪR TypeName'Value'
            if (!string.IsNullOrEmpty(Value) && Value != "''")
            {
                int startIndex = Value.IndexOf('\'');
                int endIndex = Value.LastIndexOf('\'');
                if (startIndex != -1 && endIndex != -1)
                {
                    string StringValue = null, TypeName;
                    TypeName = Value.Substring(0, startIndex);
                    if (endIndex - startIndex >= 2)
                    {
                        StringValue = PropertyExtensions.DecodingString(Value.Substring(startIndex + 1, endIndex - startIndex - 1), '%');
                    }
                    switch (TypeName)
                    {
                        case "Decimal":
                            return decimal.Parse(StringValue);

                        case "Float":
                            return float.Parse(StringValue);

                        case "Double":
                            return double.Parse(StringValue);

                        case "Bool":
                            return bool.Parse(StringValue);

                        case "Int16":
                            return short.Parse(StringValue);

                        case "Int32":
                            return int.Parse(StringValue);

                        case "Int64":
                            return long.Parse(StringValue);

                        case "Byte":
                            return byte.Parse(StringValue);

                        case "Char":
                            return char.Parse(StringValue);

                        default:
                            return StringValue;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// �^�ǰѼƧ���ԭz
        /// </summary>
        public string ParameterDescription
        {
            get
            {
                string StringDefaultValue = (DefaultValue != null) ? DefaultValue.ToString() : "";
                if (Value == null)
                {
                    return string.Format("{0},{1},{2},{3},{4},{5},'{6}',''",
                        ColumnType, Size, NumericPrecision, NumericScale, Direction, ArgmentType,
                        PropertyExtensions.EncodingStringValue(StringDefaultValue, "\'\"", '%')
                    );
                }
                else
                {
                    string StringValue = (Value != null) ? Value.ToString() : "";
                    Type ValueType = (Value != null) ? Value.GetType() : typeof(object);

                    // TODO : �[�J BehaviorArgumentType Mulit and Scope
                    // ������нs�X, ����Ÿ���� %
                    return string.Format("{0},{1},{2},{3},{4},{5},'{6}',{7}'{8}'",
                        ColumnType, Size, NumericPrecision, NumericScale, Direction, ArgmentType,
                        PropertyExtensions.EncodingStringValue(StringDefaultValue, "\'\"", '%'),
                        ValueType.Name,
                        PropertyExtensions.EncodingStringValue(StringValue, "\'\"", '%')
                    );
                }
            }
        }

        /// <summary>
        /// ���ܬO�_���\�૬�O���ѼƱ���(Mutli/Scope/MutliScope)
        /// </summary>
        public ArgumentType ArgmentType
        {
            get
            {
                return pd.ArgmentType;
            }
            set
            {
                pd.ArgmentType = value;
            }
        }

        /// <summary>
        /// ���w�P�s���ѼƬ����d�ߤ����Ѽƫ��O�C
        /// </summary>
        public ParameterDirection Direction
        {
            get
            {
                return pd.Direction;
            }
            set
            {
                pd.Direction = value;
            }
        }

        /// <summary>
        /// �Ѽƪ���ƫ��A , ColumnType.Auto ��ܩ|�Ѩt�Φۦ�M�w��ƫ��A.
        /// </summary>
        public ColumnType ColumnType
        {
            get
            {
                return pd.ColumnType;
            }
            set
            {
                pd.SetColumnType(value);
            }
        }

        /// <summary>
        /// �ѼƦW��(�@�ߥH @ �@���e�m��)
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// �r���Ƴ̤j����, ���ƭȸ��(Decimal) �ɫh����Ʀ�� , �T�w�Ȫ���ƫ��A���h�� 0
        /// </summary>
        public int Size
        {
            get
            {
                return pd.Size;
            }
            set
            {
                pd.SetColumnLength(value);
            }
        }

        /// <summary>
        /// ������p�Ʀ�� NumericScale. �D�ƭȸ��(Decimal)�ɩT�w�� 0
        /// </summary>
        public short NumericScale
        {
            get
            {
                return pd.NumericScale;
            }
            set
            {
                pd.SetColumnScale(value);
            }
        }

        /// <summary>
        /// �������Ʀ�� NumericPrecision , �D�ƭȸ��(Decimal)�ɩT�w�� 0
        /// </summary>
        public short NumericPrecision
        {
            get
            {
                return pd.NumericPrecision;
            }
            set
            {
                pd.SetColumnPrecision(value);
            }
        }

        /// <summary>
        /// ��ƹw�]��, �Y�����w��ƭȤ@�ߥѹw�]�Ȩ��N
        /// </summary>
        public object DefaultValue
        {
            get
            {
                return pd.DefaultValue;
            }
            set
            {
                pd.DefaultValue = value;
            }
        }

        /// <summary>
        /// ��ƭ�.
        /// </summary>
        public object Value
        {
            get
            {
                return pd._value;
            }
            set
            {
                // TODO: �ˬd���O�OSQL Server �i�H�ϥΪ���ƫ��O
                // No mapping exists from object type <>f__AnonymousType23`2[
                //[System.Int64, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],
                //[System.Int64, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]
                //] to a known managed provider native type.
                pd.IsValueSet = true;
                pd._value = value;
                if (Changed != null) Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// ��ƭȬO�_���ŭ�.
        /// </summary>
        public bool IsClear
        {
            get
            {
                return !pd.IsValueSet;
            }
        }

        /// <summary>
        /// �M����ƭ�.
        /// </summary>
        public void ClearValue()
        {
            pd._value = null; pd.IsValueSet = false;
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        private string[] Split(string source)
        {
            List<string> splitList = new List<string>();
            int index = 0;
            StringBuilder sb = new StringBuilder();

            // ColumnType,Size,Precision,Scale,Direction,ArgmentType,'DefaultValue',Type'Value'
            bool InSide = false;
            for (index = 0; index < source.Length; index++)
            {
                char ch = source[index];
                switch (ch)
                {
                    case '\'':
                        if (InSide)
                        {
                            sb.Append(ch);
                            splitList.Add(sb.ToString()); sb = new StringBuilder();
                            InSide = false;
                        }
                        else
                        {
                            sb.Append(ch);
                            InSide = true;
                        }
                        break;

                    case ',':
                        if (sb.Length != 0) { splitList.Add(sb.ToString()); sb = new StringBuilder(); }
                        break;

                    default:
                        sb.Append(ch);
                        break;
                }
            }
            if (sb.Length != 0) { splitList.Add(sb.ToString()); sb = new StringBuilder(); }
            return splitList.ToArray();
        }

        /// <summary>
        /// �Ǧ^��ܥثe���󪺦r��C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Parameter : {0}({1})", Name, ParameterDescription);
        }
    }
}