// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Data;

namespace Chiats.SQL
{
    internal class ParameterData : ColumnTypeInfo
    {
        public ParameterData() : base(ColumnType.Auto, 0)
        {
        }

        public object _value = null;
        public bool IsValueSet = false;       // 指示是否己己指定值.
        public object DefaultValue = null;
        public ArgumentType ArgmentType;
        public ParameterDirection Direction = ParameterDirection.Input;
    }
}