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
    /// 無效的資料轉換.
    /// </summary>
    [Serializable]
    public class InvalidConvertException : CommonException
    {
        /// <summary>
        /// InvalidConvertException 建構子
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidConvertException(System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// InvalidConvertException 建構子
        /// </summary>
        /// <param name="message">字串訊息內容</param>
        public InvalidConvertException(string message) : base(message) { }

        /// <summary>
        /// InvalidConvertException 建構子
        /// </summary>
        /// <param name="fmt">含格式化字串訊息內容, 格式化字串相關資料，見 <see cref="String.Format(string, object[])"/> Method.</param>
        /// <param name="args">格式化字串之引數</param>
        public InvalidConvertException(string fmt, params object[] args) : base(string.Format(fmt, args)) { }
    }
}