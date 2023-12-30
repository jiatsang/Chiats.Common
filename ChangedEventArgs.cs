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
    /// �ƥ�����
    /// </summary>
    public enum ChangedEventType
    {
        None,
        /// <summary>
        /// �s�W
        /// </summary>
        Add,

        /// <summary>
        /// ����
        /// </summary>
        Removed,

        /// <summary>
        /// �ܧ�
        /// </summary>
        Changed,

        /// <summary>
        /// ���N
        /// </summary>
        Replace
    }

    /// <summary>
    /// ��� �s�W/����/�ܧ�/���N �ƥ�޼�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Changed �ƥ�޼����O �غc�l
        /// </summary>
        /// <param name="ChangedObject">�޵o�ƥ󪺪���</param>
        public ChangedEventArgs(T ChangedObject)
        {
            this.ChangedEventType = ChangedEventType.Changed;
            this.ChangedObject = ChangedObject;
            this.ReplaceObject = default(T);
        }

        /// <summary>
        /// Changed �ƥ�޼����O �غc�l
        /// </summary>
        /// <param name="ChangedEventType">�ƥ�����</param>
        /// <param name="ChangedObject">�޵o�ƥ󪺪���</param>
        public ChangedEventArgs(ChangedEventType ChangedEventType, T ChangedObject)
        {
            this.ChangedEventType = ChangedEventType;
            this.ChangedObject = ChangedObject;
            this.ReplaceObject = default(T);
        }

        /// <summary>
        /// ClauseChanged �ƥ�޼����O �غc�l
        /// </summary>
        /// <param name="ChangedEventType">�ƥ�����</param>
        /// <param name="ChangedObject">�޵o�ܧ󪺪���</param>
        /// <param name="ReplaceObject">�޵o���N�ܧ󪺪���</param>
        public ChangedEventArgs(ChangedEventType ChangedEventType, T ChangedObject, T ReplaceObject)
        {
            this.ChangedEventType = ChangedEventType;
            this.ChangedObject = ChangedObject;
            this.ReplaceObject = ReplaceObject;
        }

        /// <summary>
        /// �ƥ����� �s�W/����/�ܧ�
        /// </summary>
        public readonly ChangedEventType ChangedEventType;

        /// <summary>
        /// �޵o�ƥ󪺪���
        /// </summary>
        public readonly T ChangedObject;

        /// <summary>
        /// �޵o(���N)�ƥ󪺪���
        /// </summary>
        public readonly T ReplaceObject;
    }
}