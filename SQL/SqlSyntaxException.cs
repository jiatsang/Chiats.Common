// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using System;

namespace Chiats.SQL
{
    /// <summary>
    /// SqlSyntaxException �ҥ~���O
    /// </summary>
    [Serializable]
    public class SqlExpressionSyntaxException : CommonException
    {
        /// <summary>
        /// �䴩 Serializable �������O���غc�l(Constructor) <seealso cref="T:SerializableAttribute"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SqlExpressionSyntaxException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// SqlSyntaxException �غc�l
        /// </summary>
        /// <param name="message">�r��T�����e</param>
        public SqlExpressionSyntaxException(string message) : base(message) { }

        /// <summary>
        /// SqlSyntaxException �غc�l
        /// </summary>
        /// <param name="fmt">�t�榡�Ʀr��T�����e, �榡�Ʀr�������ơA�� <see cref="String.Format(string, object[])"/> Method.</param>
        /// <param name="args">�榡�Ʀr�ꤧ�޼�</param>
        public SqlExpressionSyntaxException(string fmt, params object[] args) : base(string.Format(fmt, args)) { }

        /// <summary>
        /// </summary>
        /// <param name="innerException">�ǤJ��l�޵o�ҥ~����.</param>
        /// <param name="fmt">�t�榡�Ʀr��T�����e, �榡�Ʀr�������ơA�� <see cref="String.Format(string, object[])"/>.</param>
        /// <param name="args">�榡�Ʀr�ꤧ�޼�</param>
        public SqlExpressionSyntaxException(Exception innerException, string fmt, params object[] args) : base(string.Format(fmt, args), innerException) { }
    }
}