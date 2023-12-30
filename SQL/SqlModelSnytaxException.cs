// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Chiats.SQL
{
    /// <summary>
    /// ���ܵo�� SQL Model CTL ��k���~
    /// </summary>
    [Serializable]
    public class SqlModelSyntaxException : SqlModelException
    {
        /// <summary>
        /// �䴩 Serializable �������O���غc�l(Constructor) <seealso cref="T:SerializableAttribute"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SqlModelSyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// SQL Model CTL ��k���~
        /// </summary>
        /// <param name="ExceptionMessage"></param>
        public SqlModelSyntaxException(string ExceptionMessage) : base(ExceptionMessage) { }

        /// <summary>
        /// SQL Model CTL ��k���~
        /// </summary>
        /// <param name="MessageFormat"></param>
        /// <param name="args"></param>
        public SqlModelSyntaxException(string MessageFormat, params object[] args) :
            base(MessageFormat, args)
        { }
    }
}