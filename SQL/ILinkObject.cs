// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats
{
    /// <summary>
    /// �O CoverLinker/CoverObject ���ު� Link Object , CoverObject �O�޲z�@�Ө�h�Ӫ��P�������X��, ���ۦP�W�٪�. ���V�P�@�Ӹ�ƪ���.
    /// </summary>
    public interface ILinkObject : IVariantName
    {
        /// <summary>
        /// �����ܧ�q��
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// �@�P���s��������.
        /// </summary>
        /// <param name="obj"></param>
        void Link(ILinkObject obj);
    }
}