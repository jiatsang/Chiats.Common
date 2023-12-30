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
    ///  ��ܪ���O SQL Command/Statement �إߪ�
    /// </summary>
    public interface ICommandBuilder
    {
        /// <summary>
        /// �Ǧ^  SQL Command/Statement  ������R�O.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// �Ǧ^ Command �����A. �p�з� SQL Command , Table , StoredProcedure
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