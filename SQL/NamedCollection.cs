// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections.Generic;

namespace Chiats.SQL
{
    /// <summary>
    /// 實作支援 CoverLinker 所連結的物件集合. 的基礎類別.
    /// </summary>
    /// <typeparam name="T">內含物件的 Value 值型別</typeparam>
    public class NamedCollection<T> : IEnumerable<T> where T : IVariantName
    {
        private List<T> list = new List<T>();

        /// <summary>
        /// 連結的物件集合建構子
        /// </summary>
        public NamedCollection() { }

        /// <summary>
        /// 以物件名稱找尋目前所在位置. 不存在時會回傳 -1, 注意 回傳值有可能因為特定物件操作, 而改變.
        /// </summary>
        /// <param name="name">物件名稱</param>
        /// <returns>所在位置</returns>
        public int GetIndexByName(string name)
        {
            //TODO: Sort and Binary Seacher
            for (int index = 0; index < list.Count; index++)
            {
                T item = list[index];
                if (item.Name == name)
                    return index;
            }
            return -1;
        }

        /// <summary>
        /// 移除指名物件.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            int index = this.GetIndexByName(name);
            if (index != -1)
            {
                T item = list[index];
                list.Remove(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 回傳指定位置的物件,若不存在則會產生一個 Exception
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return list[index]; }
        }

        /// <summary>
        /// 回傳指名的物件, 若不存在則回傳 null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T this[string name]
        {
            get
            {
                int index = this.GetIndexByName(name);
                if (index != -1)
                    return list[index];
                return default(T);
            }
        }

        #region ICollection Members

        /// <summary>
        /// 加入一個指名的物件.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            list.Add(item);
        }

        /// <summary>
        /// 移除一個指名的物件.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            bool result = list.Remove(item);
            return result;
        }

        /// <summary>
        /// 清除所有指名的物件.
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// 回傳是否有內含指名的物件.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// CopyTo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///  回傳內含指名的物件個數.
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return list.ToArray();
        }

        /// <summary>
        /// 是否為 ReadOnly 集合物件.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion ICollection Members

        #region IEnumerable Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable<T> Members
    }
}