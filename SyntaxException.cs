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
    /// �ѪR�B�⦡�ɵo�{��k���~
    /// </summary>
    [Serializable]
    public class SyntaxException : CommonException
    {
        /// <summary>
        /// �䴩 Serializable �������O���غc�l(Constructor) <seealso cref="T:SerializableAttribute"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// CommonException �غc�l
        /// </summary>
        /// <param name="message">�r��T�����e</param>
        public SyntaxException(string message) : base(message) { }

        /// <summary>
        /// CommonException �غc�l
        /// </summary>
        /// <param name="fmt">�t�榡�Ʀr��T�����e, �榡�Ʀr�������ơA�� <see cref="String.Format(string, object[])"/> Method.</param>
        /// <param name="args">�榡�Ʀr�ꤧ�޼�</param>
        public SyntaxException(string fmt, params object[] args) : base(string.Format(fmt, args)) { }

        /// <summary>
        /// </summary>
        /// <param name="innerException">�ǤJ��l�޵o�ҥ~����.</param>
        /// <param name="fmt">�t�榡�Ʀr��T�����e, �榡�Ʀr�������ơA�� <see cref="String.Format(string, object[])"/>.</param>
        /// <param name="args">�榡�Ʀr�ꤧ�޼�</param>
        public SyntaxException(Exception innerException, string fmt, params object[] args) : base(string.Format(fmt, args), innerException) { }
    }
}