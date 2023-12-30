// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.Data
{
    /// <summary>
    /// 指示該欄位為為填入相對資料的 Field or Property
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnPassAttribute : Attribute
    {
        public string ColumnName { get; protected set; } = null;
        public object Arg1 { get; protected set; } = null;
        public object Arg2 { get; protected set; } = null;
        public Type ValueConvertType { get; protected set; } = null;

        public ColumnPassAttribute() { }

        /// <summary>
        /// 指示該欄位為為填入相對資料的 Field or Property
        /// </summary>
        /// <param name="columnName">ColumnName 為 null 則為  Field or Property 名稱</param>
        /// <param name="valueConvert">是否有欄位值轉換器</param>
        public ColumnPassAttribute(string columnName = null, 
            Type valueConvertType = null, 
            object arg1 = null,
            object arg2 = null)
        {
            this.ColumnName = columnName;
            this.ValueConvertType = valueConvertType;
            this.Arg1 = arg1;
            this.Arg2 = arg2;
        }

        /// <summary>
        /// 指示該欄位為為填入相對資料的 Field or Property
        /// </summary>
        /// <param name="valueConvert">是否有欄位值轉換器</param>
        public ColumnPassAttribute(Type valueConvertType, object Arg1 = null, object Arg2 = null)
        {
            this.ValueConvertType = valueConvertType;
            this.Arg1 = Arg1;
            this.Arg2 = Arg2;
        }
    }
}