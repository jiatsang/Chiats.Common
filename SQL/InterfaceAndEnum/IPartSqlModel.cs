// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// ���ܸӪ���O SqlModel ���h���@�������O���� ,
    /// </summary>
    public interface IPartSqlModel
    {
        /// <summary>
        /// �Ǧ^�̤W�@�h�� CommonModel ��������
        /// </summary>
        /// <returns>�̤W�@�h�� CommonModel ��������</returns>
        SqlModel GetTopModel();
    }
}