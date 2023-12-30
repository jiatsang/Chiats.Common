// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Text;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL Command Clause �����ͤ���
    /// </summary>
    public interface ISqlClauseExport
    {
        /// <summary>
        /// ���X SQL Command Clause �r��,
        /// </summary>
        /// <param name="sb">��X�� Clause �r�� </param>
        /// <param name="sql_model"></param>
        /// <returns>�^�ǬO��ڿ�X�Ӽ�.</returns>
        int Export(StringBuilder sb, SqlModel sql_model);
    }
}