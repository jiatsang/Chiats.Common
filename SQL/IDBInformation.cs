// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// ���o��Ʈw���t�θ�T�ɭ�. ���P����Ʈw�Ϊ���.���ݹ�@���ɭ�.
    /// </summary>
    public interface IDbInformation
    {
        /// <summary>
        /// ���o���w���W�٤���ƪ�檺�ԲӸ�T.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        TableInfo QueryTableInfo(string name);
    }
}