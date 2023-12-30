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
    /// SQL Command ��X�ﶵ.  �M�w�@SQL Command �@�U�l�y�O�_��X�D�n Keyword ( ORDER BY/Where/Having .. )
    /// </summary>
    [Flags]
    public enum BuildExportOptions
    {
        /// <summary>
        ///  ���� BuildExport ����X Keyword (ORDER BY/Where/Having ..)
        /// </summary>
        None = 0x0,

        /// <summary>
        ///  ���� BuildExport ��X Keyword (ORDER BY/Where/Having ..)
        /// </summary>
        ExportKeyword = 0x01,

        /// <summary>
        /// �w�]�ﶵ ExportKeyword
        /// </summary>
        Default = ExportKeyword,
    }
}