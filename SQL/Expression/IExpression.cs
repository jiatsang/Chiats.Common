// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.ComponentModel;

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// SqlModel 運算式標準介面. 它涵蓋了欄位運算式和條件運算式.運算式字串表示式 = 運算式物件之轉換式可能性
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// 傳回是否為終端運算子
        /// </summary>
        bool IsTerminalExp { get; }

        /// <summary>
        /// 輸出運算式的字串表示式
        /// </summary>
        /// <param name="CommandBuilder">運算式的字串表示式產生器</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName);
    }

    /// <summary>
    /// 比較連接子 =,&lt;, see SyntaxRuleB1
    /// </summary>
    internal enum ComparisonOperator
    {
        [StringValue("null"), Description("Single Column Comparison Operator Example: Column or Function")]
        Empty,

        /// <summary>
        /// 比較兩個運算式是否相等 (比較運算子)。
        /// </summary>
        [StringValue("="), Description("= (等於)")]
        Eqauls,

        /// <summary>
        /// 比較兩個運算式 (比較運算子)。當您在比較非 Null 運算式時，如果左運算元的值大於右運算元，
        /// 則結果為 TRUE，否則結果就是 FALSE。如果其中任一個運算元或是兩者皆為 NULL，則傳回 NULL。
        /// </summary>
        [StringValue(">"), Description("> (大於)")]
        Greater,

        /// <summary>
        /// 比較兩個運算式 (比較運算子)。當您在比較非 Null 運算式時，如果左運算元的值小於右運算元，
        /// 則結果為 TRUE，否則結果就是 FALSE。如果其中任一個運算元或是兩者皆為 NULL，則傳回 NULL。
        /// </summary>
        [StringValue("<"), Description("< (小於)")]
        Less,

        /// <summary>
        /// 比較大於或等於 (比較運算子) 的兩個運算式。
        /// </summary>
        [StringValue(">="), Description(">= (大於或等於)")]
        GreaterEqauls,

        /// <summary>
        /// 比較兩個運算式 (比較運算子)。當您在比較非 Null 運算式時，如果左運算元的值小於或等於右運算元，
        /// 則結果為 TRUE，否則結果就是 FALSE。如果其中任一個運算元或是兩者皆為 NULL，則傳回 NULL。
        /// </summary>
        [StringValue("<="), Description("<= (小於或等於)")]
        LessEqauls,

        /// <summary>
        /// 測試一個運算式是否等於另一個運算式 (比較運算子)。如果其中任一個運算元或是兩者皆為 NULL，
        /// 則傳回 NULL。函數與 &lt;&gt; (不等於) 比較運算子相同。
        /// </summary>
        [StringValue("<>"), Description("<> or !=")]
        NotEqauls,

        /// <summary>
        /// 比較兩個運算式 (比較運算子)。當您在比較非 Null 運算式時，如果左運算元的值不小於右運算元，
        /// 則結果為 TRUE，否則結果就是 FALSE。如果其中任一個運算元或是兩者皆為 NULL，則傳回 NULL。
        /// </summary>
        [StringValue("!<"), Description("!< (不小於) ")]
        NotLess,

        /// <summary>
        /// 比較兩個運算式 (比較運算子)。當您在比較非 Null 運算式時，如果左運算元的值不大於右運算元，
        /// 則結果為 TRUE，否則結果就是 FALSE。如果其中任一個運算元或是兩者皆為 NULL，則傳回 NULL。
        /// </summary>
        [StringValue("!>"), Description("!> (不大於)")]
        NotGreater,

        ////[StringValue("like"), Description("like ")]
        //Like,
        //[StringValue("not like"), Description("like ")]
        //NotLike,
    }

    internal enum LogicalOperator
    {
        [StringValue(null), Description("無效的連接子")]
        Empty,

        /// <summary>
        /// 結合兩個布林運算式，並在兩個運算式都是 TRUE 時，傳回 TRUE。當在陳述式中使用一個以上的邏輯運算子時，
        /// 會先評估 AND 運算子。您可以使用括號來變更驗算的順序。
        /// </summary>
        [StringValue("and"), Description("Logical And")]
        And,

        [StringValue("or"), Description("Logical Or")]
        Or,

        [StringValue("not and"), Description("Logical Not And")]
        NotAnd,

        [StringValue("not"), Description("Logical Not")]
        Not,

        [StringValue("not or"), Description("Logical Not Or")]
        NotOr,

        [StringValue("like"), Description("Logical Like")]
        Like,

        [StringValue("not like"), Description("Logical Not Like")]
        NotLike,
    }
}