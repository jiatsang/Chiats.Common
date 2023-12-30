// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Data;

namespace Chiats.SQL
{
    /// <summary>
    ///  表示物件是 SQL Command/Statement 建立者
    /// </summary>
    public interface ICommandBuilder
    {
        /// <summary>
        /// 傳回  SQL Command/Statement  的執行命令.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// 傳回 Command 的型態. 如標準 SQL Command , Table , StoredProcedure
        /// </summary>
        CommandType CommandType { get; }
    }

    /// <summary>
    ///  Specifies that this object supports a simple, transacted notification for
    ///   batch initialization.
    /// </summary>
    public interface ICommandInitialize
    {
        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        void BeginInit();

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void EndInit();
    }
}