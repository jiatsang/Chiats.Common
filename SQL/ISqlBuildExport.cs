// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 為 SqlModel 產生 SQL Command (敘述)的實作介面.
    /// </summary>
    /// <remarks>
    /// Ezslab 基於對組成標準 SQL Commnad 的提供更方更的處理方法 . 而 SqlModel 只提供標準的 SQL 指令的模型的資訊.
    /// (以求資料庫的最大相容性為原則). 對於不同資料庫的差異則由 SqlModel 輸出時產生相容的 SQL 敘述並加上在不同資料庫上面實作出共同的函數庫.
    /// 來決解異質資料庫的程式共用的問題.
    /// </remarks>
    public interface ISqlBuildExport
    {
        /// <summary>
        /// 指示建立 SQL 命令的文字敘述的方式
        /// </summary>
        CommandBuildType BuildType { get; }

        /// <summary>
        /// 強制轉換成小寫名稱. 欄位名稱/表格名稱
        /// </summary>
        SqlOptions Options { get; set; }

        /// <summary>
        /// 指示依 SQL Commnad 的文字敘述選項參數處理 SQL Commnad 的文字輸出
        /// </summary>
        CommandBuildOptions BuildOptions { get; set; }

        /// <summary>
        /// 指示依 SQL Commnad 的文字敘述格式參數處理 SQL Commnad 的文字輸出. 如自動加入換行字元
        /// </summary>
        CommandFormatOptions FormatOptions { get; set; }

        /// <summary>
        /// 取得資料庫相關參數. 當 DbInformation 為 NULL 時, 則表示因無法識別資料庫廠商或版本時而取不到資料庫相關參數. 因為資料庫相關參數是和資料庫系統不同而有所不同
        /// </summary>
        IDbInformation DbInformation { get; set; }

        /// <summary>
        /// 產生 ColumnValue SQL 敘述的實作介面. Example Column1=Vaule1,Column2=Vaule2
        /// </summary>
        /// <param name="column">欄位名稱和欄位值的定義</param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForSetValues(ColumnValue column, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        /// 產生 ColumnValue SQL 敘述的實作介面. Example Vaule1,Vaule2
        /// </summary>
        /// <param name="column">欄位名稱和欄位值的定義</param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForInsertValues(ColumnValue column, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        /// 產生 WhereClause/HavingClause 敘述的實作介面
        /// </summary>
        /// <param name="where"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForWhereClause(WhereClause where, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        /// 產生 OrderByClause 敘述的實作介面
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForOrderByClause(OrderByClause orderBy, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        /// 產生 ColumnsClause 敘述的實作介面
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForColumnsClause(ColumnsClause columns, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        /// 產生 TableClause 敘述的實作介面
        /// </summary>
        /// <param name="table">表格(Table) 敘述物件</param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForTableClause(TableClause table, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        /// 產生 GroupByClause 敘述的實作介面
        /// </summary>
        /// <param name="groupBy">Group By 敘述物件</param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForGroupByClause(GroupByClause groupBy, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        /// 產生 ColumnDescriptionCollection (Create Table) 敘述的實作介面
        /// </summary>
        /// <param name="Columns"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForColumns(ColumnDescriptionCollection Columns, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);

        /// <summary>
        ///
        /// </summary>
        /// <param name="indexes"></param>
        /// <param name="CommandBuilder">SQL Command 的字串產生器</param>
        /// <param name="SqlModel">為產生SQL Command (敘述) 的 SqlModel 主體物件</param>
        /// <param name="Options">SQL Command 輸出選項</param>
        /// <returns></returns>
        int ExportForIndexes(IndexDescriptionCollection indexes, CommandBuilder CommandBuilder, SqlModel SqlModel, BuildExportOptions Options = BuildExportOptions.Default);
    }
}