// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Chiats
{
    /// <summary>
    /// 通用的陣列類別定義 ,提供以名稱為主鍵的方法, 和 SearchList 相同會有一個以 Name 找資料的方法.
    /// </summary>
    /// <typeparam name="T">陣列物件的型別</typeparam>
    public abstract class CommonList<T> : IEnumerable<T> where T : IVariantName
    {
        private readonly List<T> list = new List<T>();

        /// <summary>
        /// 回傳陣列個數
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// 回傳指定位置的物件.
        /// </summary>
        /// <param name="index">指定位置</param>
        /// <returns>回傳物件</returns>
        public T this[int index]
        {
            get { return list[index]; }
        }

        /// <summary>
        /// 依指定的名稱回傳物件位置
        /// </summary>
        /// <param name="name">指定的名稱</param>
        /// <returns></returns>
        protected int GetIndexByName(string name)
        {
            lock (this)
            {
                for (int index = 0; index < list.Count; index++)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(name, list[index].Name))
                    {
                        return index;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 回傳指定物件名稱的物件.
        /// </summary>
        /// <param name="name">物件名稱</param>
        /// <returns>回傳物件</returns>
        public T this[string name]
        {
            get
            {
                lock (this)
                {
                    int index = GetIndexByName(name);
                    return index != -1 ? list[index] : default(T);
                }
            }
        }

        /// <summary>
        /// 回傳指定物件名稱的 Replace 物件.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected T BaseReplace(T item)
        {
            if (item != null)
            {
                lock (this)
                {
                    int index = GetIndexByName(item.Name);
                    if (index != -1)
                    {
                        T ReplaceObject = list[index];
                        list[index] = item;
                        return ReplaceObject;
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// 加入一個新的物件實體
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected T BaseAdd(T item)
        {
            if (item != null)
            {
                list.Add(item);
            }
            return item;
        }

        /// <summary>
        /// 移除一個物件實體
        /// </summary>
        /// <param name="item"></param>
        protected void BaseRemove(T item)
        {
            if (item != null && BaseContains(item))
            {
                list.Remove(item);
            }
        }

        /// <summary>
        /// 移除一個物件實體
        /// </summary>
        /// <param name="name"></param>
        protected void BaseRemove(string name)
        {
            T item = this[name];
            if (item != null)
            {
                list.Remove(item);
            }
        }

        /// <summary>
        /// 回傳是否包含該物件
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool BaseContains(T item)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// 回傳是否包含該指名物件
        /// </summary>
        /// <param name="name">物件名稱</param>
        /// <returns></returns>
        protected bool BaseContains(string name)
        {
            return GetIndexByName(name) != -1;
        }

        /// <summary>
        /// 移除所有物件實體
        /// </summary>
        protected void BaseClear()
        {
            list.Clear();
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別子的物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable<T> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable Members

        /// <summary>
        /// 傳回表示目前物件的字串。，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}(Count={1})", typeof(T).Name, this.Count);
        }
    }
}