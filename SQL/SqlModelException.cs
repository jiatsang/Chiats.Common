using System;
using System.Runtime.Serialization;

namespace Chiats.SQL
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SqlModelException : CommonException
    {
        /// <summary>
        /// 支援 Serializable 物件類別的建構子(Constructor) <seealso cref="T:SerializableAttribute"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SqlModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// SqlModelException 建構子
        /// </summary>
        /// <param name="ExceptionMessage">字串訊息內容</param>
        public SqlModelException(string ExceptionMessage) : base(ExceptionMessage) { }

        /// <summary>
        /// SqlModelException 建構子
        /// </summary>
        /// <param name="MessageFormat"></param>
        /// <param name="args"></param>
        public SqlModelException(string MessageFormat, params object[] args) :
            base(MessageFormat, args)
        { }
    }

    /// <summary>
    ///
    /// </summary>
    public class SqlModelParameterException : CommonException
    {
        /// <summary>
        /// 支援 Serializable 物件類別的建構子(Constructor) <seealso cref="T:SerializableAttribute"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SqlModelParameterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// SqlModelException 建構子
        /// </summary>
        /// <param name="ExceptionMessage">字串訊息內容</param>
        public SqlModelParameterException(string ExceptionMessage) : base(ExceptionMessage) { }

        /// <summary>
        /// SqlModelException 建構子
        /// </summary>
        /// <param name="MessageFormat"></param>
        /// <param name="args"></param>
        public SqlModelParameterException(string MessageFormat, params object[] args) :
            base(MessageFormat, args)
        { }
    }
}