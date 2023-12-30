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
    /// SQL �ѪR�r��R�O�@�Τ���. �̦r��R�O���� SqlModel �۹������󪫥�
    /// </summary>
    internal interface ISqlParserObject
    {
        /// <summary>
        /// �ѪR�r�� (TokenList) �è̦r��R�O���� SqlModel �۹������󪫥�
        /// </summary>
        /// <param name="TokenScanner">�� CommandText �g Token Scan �Უ�ͤ� TokenList.</param>
        /// <param name="SqlModel"></param>
        /// <returns>�۹������󪫥����O</returns>
        SqlModel PaserCommand(SQLTokenScanner TokenScanner, SqlModel SqlModel, object parameters = null);

        /// <summary>
        /// ���ܥ�����, �Ҳ��ͤ�����  SqlModel �۹������󪫥����O �p SelectModel/UpdateModel/InsertModel/DeleteModel
        /// </summary>
        Type ModelType { get; }
    }
}