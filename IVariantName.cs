// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats
{
    /// <summary>
    /// 表示為一個有名稱物件類別. 通常用於需要以名稱查詢的作業功能所使用的介面
    /// </summary>
    public interface IVariantName
    {
        /// <summary>
        /// 回傳物件名稱
        /// </summary>
        string Name { get; }
    }
}