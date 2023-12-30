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
    /// 事件類型
    /// </summary>
    public enum ChangedEventType
    {
        None,
        /// <summary>
        /// 新增
        /// </summary>
        Add,

        /// <summary>
        /// 移除
        /// </summary>
        Removed,

        /// <summary>
        /// 變更
        /// </summary>
        Changed,

        /// <summary>
        /// 取代
        /// </summary>
        Replace
    }

    /// <summary>
    /// 資料 新增/移除/變更/取代 事件引數.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Changed 事件引數類別 建構子
        /// </summary>
        /// <param name="ChangedObject">引發事件的物件</param>
        public ChangedEventArgs(T ChangedObject)
        {
            this.ChangedEventType = ChangedEventType.Changed;
            this.ChangedObject = ChangedObject;
            this.ReplaceObject = default(T);
        }

        /// <summary>
        /// Changed 事件引數類別 建構子
        /// </summary>
        /// <param name="ChangedEventType">事件類型</param>
        /// <param name="ChangedObject">引發事件的物件</param>
        public ChangedEventArgs(ChangedEventType ChangedEventType, T ChangedObject)
        {
            this.ChangedEventType = ChangedEventType;
            this.ChangedObject = ChangedObject;
            this.ReplaceObject = default(T);
        }

        /// <summary>
        /// ClauseChanged 事件引數類別 建構子
        /// </summary>
        /// <param name="ChangedEventType">事件類型</param>
        /// <param name="ChangedObject">引發變更的物件</param>
        /// <param name="ReplaceObject">引發取代變更的物件</param>
        public ChangedEventArgs(ChangedEventType ChangedEventType, T ChangedObject, T ReplaceObject)
        {
            this.ChangedEventType = ChangedEventType;
            this.ChangedObject = ChangedObject;
            this.ReplaceObject = ReplaceObject;
        }

        /// <summary>
        /// 事件類型 新增/移除/變更
        /// </summary>
        public readonly ChangedEventType ChangedEventType;

        /// <summary>
        /// 引發事件的物件
        /// </summary>
        public readonly T ChangedObject;

        /// <summary>
        /// 引發(取代)事件的物件
        /// </summary>
        public readonly T ReplaceObject;
    }
}