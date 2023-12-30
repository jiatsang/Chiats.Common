// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Chiats
{
    /// <summary>
    /// Ezslab ����¦�ҥ~���O
    /// </summary>
    [Serializable]
    public class CommonException : ApplicationException, ISerializable
    {
        private string moreMessage;

        /// <summary>
        /// �䴩 Serializable �������O���غc�l(Constructor) <seealso cref="T:SerializableAttribute"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CommonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            MoreMessage = info.GetString("MoreMessage");
        }

        /// <summary>
        /// CommonException �غc�l
        /// </summary>
        /// <param name="message">�r��T�����e</param>
        public CommonException(string message) : base(message) { }

        /// <summary>
        /// CommonException �غc�l
        /// </summary>
        /// <param name="fmt">�t�榡�Ʀr��T�����e, �榡�Ʀr�������ơA�� <see cref="String.Format(string, object[])"/> Method.</param>
        /// <param name="args">�榡�Ʀr�ꤧ�޼�</param>
        public CommonException(string fmt, params object[] args) : base(string.Format(fmt, args)) { }

        /// <summary>
        /// </summary>
        /// <param name="innerException">�ǤJ��l�޵o�ҥ~����.</param>
        /// <param name="fmt">�t�榡�Ʀr��T�����e, �榡�Ʀr�������ơA�� <see cref="String.Format(string, object[])"/>.</param>
        /// <param name="args">�榡�Ʀr�ꤧ�޼�</param>
        public CommonException(Exception innerException, string fmt, params object[] args) : base(string.Format(fmt, args), innerException) { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);

            //info.AddValue("Level", Level.ToString());
            info.AddValue("MoreMessage", MoreMessage);
        }

        /// <summary>
        /// �Ǧ^��h�ԲӸ��.
        /// </summary>
        public string MoreMessage
        {
            get
            {
                return moreMessage;
            }
            protected set { moreMessage = value; }
        }
    }
}