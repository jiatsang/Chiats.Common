// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL
{
    /// <summary>
    /// 指示建立 SQL 命令的文字敘述的輸出參數選項,
    /// </summary>
    [Flags]
    public enum CommandBuildOptions
    {
        /// <summary>
        /// (預設值) 預設參數選項.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 運用 Table/Columns 資訊查詢子系統. 增加 SelectModel Column list 擴展功能( * -&gt; 欄位名稱)
        /// </summary>
        /// <remarks>
        /// ExpandAsteriskColumnName 只有在特定的條件成立才會有作用. (在考量效能下面會有下列條件限制)
        /// 1. 能夠取得 IDbInformation 介面. IDbInformation 可以取得表格的實際明細資料.
        /// 2. 簡單的 Table 敍述, 過於複雜的 Table 敍述, 會阻礙表格名稱的取得(或制迼效能低落的狀況) 如 子查詢.
        /// 3. 必須為資料實際表格名稱(如 View 是不啟作用的)
        /// 4. ExpandAsteriskColumnName 只會展開最外層 Select Command , 其他的子查詢則不會進行展開工作.
        /// </remarks>
        ExpandAsteriskColumnName = 0x01,

        /// <summary>
        /// 強制輸出含表格名稱或別名的 Alias Column Name
        /// </summary>
        /// <remarks>
        /// 為詳細得到查詢的資料回傳值 SelectModel 增加可以加入 as [a.name] 的功能.  ExpandAliasColumnName
        /// 因為實際執行的結果集合不會包含每個欄位的所屬表格. Products.ProductID -> 只會取得 ProductID 當有需要在結果集合中也包含
        /// 表格名稱或別名時可以使用 ExpandAliasColumnName 強制輸出含表格名稱或別名的 Alias Column Name , 但若己指定有  Alias Column Name
        /// 存在時則不會輸出.
        /// Example : Select a.ProductID as [a.ProductID] .....
        /// </remarks>
        ExpandAliasColumnName = 0x02
    }
}