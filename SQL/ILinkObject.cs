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
    /// 是 CoverLinker/CoverObject 控管的 Link Object , CoverObject 是管理一個到多個的同類型集合中, 取相同名稱者. 指向同一個資料物件.
    /// </summary>
    public interface ILinkObject : IVariantName
    {
        /// <summary>
        /// 物件變更通知
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// 共同的連結的物件.
        /// </summary>
        /// <param name="obj"></param>
        void Link(ILinkObject obj);
    }
}