// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Chiats
{
    /// <summary>
    /// 解析運算式時發現文法錯誤
    /// </summary>
    [Serializable]
    public class SyntaxException : CommonException
    {
        /// <summary>
        /// 支援 Serializable 物件類別的建構子(Constructor) <seealso cref="T:SerializableAttribute"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// CommonException 建構子
        /// </summary>
        /// <param name="message">字串訊息內容</param>
        public SyntaxException(string message) : base(message) { }

        /// <summary>
        /// CommonException 建構子
        /// </summary>
        /// <param name="fmt">含格式化字串訊息內容, 格式化字串相關資料，見 <see cref="String.Format(string, object[])"/> Method.</param>
        /// <param name="args">格式化字串之引數</param>
        public SyntaxException(string fmt, params object[] args) : base(string.Format(fmt, args)) { }

        /// <summary>
        /// </summary>
        /// <param name="innerException">傳入原始引發例外物件.</param>
        /// <param name="fmt">含格式化字串訊息內容, 格式化字串相關資料，見 <see cref="String.Format(string, object[])"/>.</param>
        /// <param name="args">格式化字串之引數</param>
        public SyntaxException(Exception innerException, string fmt, params object[] args) : base(string.Format(fmt, args), innerException) { }
    }
}