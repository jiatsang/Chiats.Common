// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 指示該物件是 SqlModel 階層的一部份類別實體 ,
    /// </summary>
    public interface IPartSqlModel
    {
        /// <summary>
        /// 傳回最上一層的 CommonModel 父階物件
        /// </summary>
        /// <returns>最上一層的 CommonModel 父階物件</returns>
        SqlModel GetTopModel();
    }
}