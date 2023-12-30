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
    /// �L�Ī�����ഫ.
    /// </summary>
    [Serializable]
    public class InvalidConvertException : CommonException
    {
        /// <summary>
        /// InvalidConvertException �غc�l
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidConvertException(System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// InvalidConvertException �غc�l
        /// </summary>
        /// <param name="message">�r��T�����e</param>
        public InvalidConvertException(string message) : base(message) { }

        /// <summary>
        /// InvalidConvertException �غc�l
        /// </summary>
        /// <param name="fmt">�t�榡�Ʀr��T�����e, �榡�Ʀr�������ơA�� <see cref="String.Format(string, object[])"/> Method.</param>
        /// <param name="args">�榡�Ʀr�ꤧ�޼�</param>
        public InvalidConvertException(string fmt, params object[] args) : base(string.Format(fmt, args)) { }
    }
}