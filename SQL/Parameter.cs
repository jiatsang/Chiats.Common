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
    /// 戈畐把计砞﹚ず甧 , 戈篈,戈程,戈计,戈箇砞 单单
    /// </summary>
    /// <remarks>
    /// Parameter 琌ざ SqlModel 把计㎝戈畐把计ぇ丁硈么ノ. <br/>
    /// 1. SqlModel 把计セō﹟ゼ 把计ぇぇ闽癟(场惠璶パ 迭(Lexical) ㄓ干)<br/>
    /// 2. SqlModel Τ把计兵ン. 把计兵ン(Mutli/Scope/MutliScope) 疭┦<br/>
    ///     (1) 把计癸莱戈畐把计. 把计嘿耎甶ㄌ﹚ぇ砏玥 Example @Name#1,@Name#2From,@Name#2To  <br/>
    ///     (2) 把计Τ  ILinkObject ぃ穦硈挡ㄤ嘿把计. 玥莱赣玻ネ Execption ㄆン.<br/>
    ///     (3) 皌 BehaviorArgument 笲ㄤぇ眔.
    /// </remarks>
    public class Parameter : ILinkObject
    {
        /// <summary>
        /// 戈钵笆硄 . LinkObject 钵笆穦ㄆン硄じン. (讽ずン Value 跑┮惠ま祇ㄆン.)
        /// </summary>
        public event EventHandler Changed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ParameterData pd = new ParameterData();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string name = null;

        /// <summary>
        /// 肚把计ン┮妮兵ンン, 璝縒ミ把计ン玥 null
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
                    this.pd = p.pd;  // ㄏ嘿ぇ把计ノ舱 ParameterData
                else
                    throw new SqlModelSyntaxException(@"把计兵ン {0} ぃす砛滦", name);
            }
        }

        /// <summary>
        /// 戈畐把计篶
        /// </summary>
        /// <param name="name">把计嘿</param>
        public Parameter(string name) : this(name, ArgumentType.None) { }

        /// <summary>
        /// 戈畐把计篶
        /// </summary>
        /// <param name="name">把计嘿</param>
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
            throw new SqlModelSyntaxException(@"把计嘿岿粇. ゲ斗 @ 秨繷.ぃす砛Τフ┪ㄤ疭じ.");
        }

        /// <summary>
        /// 戈畐把计篶
        /// </summary>
        /// <param name="param"></param>
        public Parameter(Parameter param)
        {
            this.name = param.Name;
            this.pd = param.pd;
        }

        /// <summary>
        /// ゅ磞瓃把计よΑ篶把计 ,
        /// ColumnType,Size,Precision,Scale,Direction(In/Out/InOut),ArgmentType,'DefaultValue',Type'Value'
        /// </summary>
        /// <param name="ParameterDescription">ゅ磞瓃把计</param>
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
                // ňゎ滦絪絏, 北才腹эノ %
                return PropertyExtensions.DecodingString(DefaultValue.Substring(1, DefaultValue.Length - 2), '%');
            }
            return null;
        }

        private object GetStringValue(string Value)
        {
            // 秆猂 TypeName'Value'
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
        /// 肚把计Ч俱痹瓃
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

                    // TODO :  BehaviorArgumentType Mulit and Scope
                    // ňゎ滦絪絏, 北才腹эノ %
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
        /// ボ琌把计兵ン(Mutli/Scope/MutliScope)
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
        /// ﹚籔把计闽琩高い把计
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
        /// 把计戈篈 , ColumnType.Auto ボ﹟パ╰参︽∕﹚戈篈.
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
        /// 把计嘿( @ 玡竚迭)
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// ﹃戈程, 讽计戈(Decimal) 玥俱计计 , ㏕﹚戈篈玥 0
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
        /// 戈逆计计 NumericScale. 獶计戈(Decimal)㏕﹚ 0
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
        /// 戈逆俱计计 NumericPrecision , 獶计戈(Decimal)㏕﹚ 0
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
        /// 戈箇砞, ゼ﹚戈パ箇砞
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
        /// 戈.
        /// </summary>
        public object Value
        {
            get
            {
                return pd._value;
            }
            set
            {
                // TODO: 浪琩琌SQL Server ㄏノ戈
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
        /// 戈琌.
        /// </summary>
        public bool IsClear
        {
            get
            {
                return !pd.IsValueSet;
            }
        }

        /// <summary>
        /// 睲埃戈.
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
        /// 肚ボヘ玡ン﹃
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Parameter : {0}({1})", Name, ParameterDescription);
        }
    }
}