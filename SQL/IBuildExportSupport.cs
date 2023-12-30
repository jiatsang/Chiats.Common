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
    /// 為 SqlModel 物件建立一個 BuildExport 支援物件及其使用範圍.
    /// </summary>
    /// <remarks>
    /// SqlModel 在產生 SQL Command 之間, 會有 BuildExport 物件作為實際輸出的元件(可以為不同資料庫法備各自BuildExport 物件)
    /// 但是 SqlModel 在產生 SQL Command , 會經由不同的 Export 區段組成. 如 Select/Where/From 等等不同的區段所組成.
    /// 如果不指定要用那一個 BuildExport 物件. 則會以預設的 DefaultBuildExport. 但如果要指定自己特定 BuildExport 物件或是需要 buildFlags
    /// 參數加入時則可以使用 IBuildExportSupport 介面進行, 而不同的 Export 區段將會在一個 BeginBuild 到 EndBuild 範圍內執行.
    /// </remarks>
    public interface IBuildExportSupport
    {
        /// <summary>
        /// BuildExport 支援物件的開始點
        /// </summary>
        /// <param name="BuildExport"></param>
        /// <param name="e"></param>
        void BeginBuild(ISqlBuildExport BuildExport, BuildExportSupportEventArgs e);

        /// <summary>
        /// BuildExport 支援物件的結束點
        /// </summary>
        void EndBuild();
    }

    /// <summary>
    /// IBuildExportSupport 的 BeginBuild 引數物件
    /// </summary>
    public class BuildExportSupportEventArgs : EventArgs
    {
        /// <summary>
        /// IBuildExportSupport 的 BeginBuild 引數物件建構子
        /// </summary>
        /// <param name="DbInformation">取得資料庫相關參數</param>
        /// <param name="BuildOptions">指示依 SQL Commnad 的文字敘述選項參數處理 SQL Commnad 的文字輸出</param>
        public BuildExportSupportEventArgs(IDbInformation DbInformation, CommandBuildOptions BuildOptions)
        {
            this.DbInformation = DbInformation;
            this.BuildOptions = BuildOptions;
        }

        /// <summary>
        /// 取得資料庫相關參數. 當 DbInformation 為 NULL 時, 則表示因無法識別資料庫廠商或版本時而取不到資料庫相關參數. 因為資料庫相關參數是和資料庫系統不同而有所不同
        /// </summary>
        public readonly IDbInformation DbInformation;

        /// <summary>
        /// 指示依 SQL Commnad 的文字敘述選項參數處理 SQL Commnad 的文字輸出
        /// </summary>
        public readonly CommandBuildOptions BuildOptions;
    }
}