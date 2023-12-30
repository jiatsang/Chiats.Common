// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// ���ܫإ� SQL �R�O����r�ԭz����X�榡���̴`�W�h, SQLCTL �� �з� SQL �W�檺��r�ԭz.
    /// </summary>
    public enum CommandBuildType
    {
        /// <summary>
        /// (�w�]��) �ŦX�з� SQL �W�檺��r�ԭz���覡 (���ե��x : Microsoft SQL Server 2005/2008)
        /// </summary>
        SQL,

        /// <summary>
        /// �ŦX SQL Condition Template Language  �W�檺��r�ԭz���覡. CTL �O�إߦb�@�ӲŦX�q�� SQL �W�檺��r���W.
        /// </summary>
        SQLCTL
    }
}