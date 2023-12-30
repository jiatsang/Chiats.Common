// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// 數值運算連接子.
    /// </summary>
    [Flags]
    public enum ArithmeticLogicalLink
    {
        /// <summary>
        /// 無效的連接子, 通常用於最後一個時或是單一運算式
        /// </summary>
        [StringValue(null), Description("無效的連接子")]
        Empty = 0,

        [StringValue(null), Description("算術連接子")]
        ArithmeticLink = 0x1000,

        [StringValue(null), Description("Logical 連接子")]
        LogicalLink = 0x2000,

        /// <summary>
        /// 加法連接子, 執行算術作業，將兩個數加在一起。或傳回數值運算式的正值。也可用於串連兩個字串運算式。
        /// </summary>
        [StringValue("+"), Description("加法連接子")]
        Add = ArithmeticLink | 1,

        /// <summary>
        /// 減法連接子, 傳回兩個數值運算式之間的差異，或數值運算式的負值。
        /// </summary>
        [StringValue("-"), Description("減法連接子")]
        Subtract = ArithmeticLink | 2,

        /// <summary>
        /// 乘法連接子, 執行以一個數目乘上另一個數目的算術運算
        /// </summary>
        [StringValue("*"), Description("乘法連接子")]
        Multiply = ArithmeticLink | 3,

        /// <summary>
        /// 除法連接子(算術除法運算子), 執行以一個數目除以另一個數目的算術運算。
        /// </summary>
        /// <remarks>
        /// 相關資料請參考 Microsoft SQL Server Online Help
        /// <example>
        /// 下列範例會利用除法算術運算子來計算銷售人員每月的銷售目標。
        /// <code lang="SQL">
        /// SELECT SalesPersonID, FirstName, LastName, SalesQuota, SalesQuota/12 AS 'Sales Target Per Month'
        /// FROM Sales.SalesPerson s
        /// JOIN HumanResources.Employee e ON s.SalesPersonID = e.EmployeeID
        /// JOIN Person.Contact c ON e.ContactID = c.ContactID;
        /// </code>
        /// </example>
        /// </remarks>
        [StringValue("/"), Description("除法連接子")]
        Divide = ArithmeticLink | 4,

        /// <summary>
        /// AND 連接子
        /// </summary>
        [StringValue("AND"), Description("AND 連接子")]
        And = LogicalLink | 1,

        /// <summary>
        /// OR 連接子
        /// </summary>
        [StringValue("OR"), Description("OR 連接子")]
        Or = LogicalLink | 2,

        /// <summary>
        /// NOT 連接子
        /// </summary>
        [StringValue("NOT"), Description("NOT 連接子")]
        Not = LogicalLink | 2,

        /// <summary>
        /// 等於連接子
        /// </summary>
        [StringValue("="), Description("等於連接子")]
        Equal = LogicalLink | 3,

        /// <summary>
        /// 小於等於連接子
        /// </summary>
        [StringValue("<="), Description("小於等於連接子")]
        LessEqual = LogicalLink | 4,

        /// <summary>
        /// 小於連接子
        /// </summary>
        [StringValue("<"), Description("小於連接子")]
        Less = LogicalLink | 5,

        /// <summary>
        /// 不等於連接子
        /// </summary>
        [StringValue("<>"), Description("不等於連接子")]
        NotEqual = LogicalLink | 6,

        /// <summary>
        /// 大於連接子
        /// </summary>
        [StringValue(">"), Description("大於連接子")]
        Greater = LogicalLink | 7,

        /// <summary>
        /// 大於連接子
        /// </summary>
        [StringValue(">="), Description("大於連接子")]
        GreaterEqual = LogicalLink | 8,

        /// <summary>
        /// IS 連接子
        /// </summary>
        [StringValue("IS"), Description("IS 連接子")]
        IS = LogicalLink | 9,

        /// <summary>
        /// AS 連接子
        /// </summary>
        [StringValue("AS"), Description("AS 連接子")]
        AS = LogicalLink | 10,
    }
}