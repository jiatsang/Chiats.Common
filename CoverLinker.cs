// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Chiats.SQL
{
    /// <summary>
    /// CoverLinker 以連結二個或二個以上的 Collection(Collection 的 Key/Value 必須有相同的型態),
    /// CoverLinker 可以用來管理其所有被連結的值. <br/>
    /// CoverLinker 基本上就是一個介面層, 對外只需要對 CoverLinker 作處理, 不需要理會下面有多個連結 Collection.  Collection 的增加變更原本
    /// 就不是 CoverLinker 需要處理的一部份. 而是各連結 Collection 自行需要處理的行為.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CoverLinker<T> : IEnumerable<CoverLinker<T>.CoverObject> where T : ILinkObject
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<CoverObject> link = new List<CoverObject>();

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private HashSet<NamedCollection<T>> objectLinkers = new HashSet<NamedCollection<T>>();

        /// <summary>
        /// 資料異動通知 . LinkObject 異動時會以此事件通知父元件.
        /// </summary>
        public event EventHandler Changed;

        private void LinkObject_ObjectChanged(object sender, EventArgs e)
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// 空的 CoverObject Instance, 當你查詢一個不存在的名稱連結時它會回傳 CoverObject.Empty 而非 NULL.
        /// 用以防止 CoverLinker["Na"].Value 執行失敗. 你可以用 CoverLinker["Na"].IsEmpty 檢查.
        /// </summary>
        public static CoverObject Empty = new CoverObject();

        /// <summary>
        /// CoverLinker 以連結物件之管理物件.
        /// </summary>
        public class CoverObject : IEnumerable<T>
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string name = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private CoverLinker<T> parent = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private List<T> object_list = new List<T>();

            internal CoverObject()
            {
            }

            /// <summary>
            /// 資料異動通知 . LinkObject 異動時會以此事件通知父元件.
            /// </summary>
            public event EventHandler Changed;

            private void LinkObject_ObjectChanged(object sender, EventArgs e)
            {
                if (Changed != null) Changed(this, EventArgs.Empty);
            }

            /// <summary>
            /// CoverObject  建構子
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="Object"></param>
            public CoverObject(CoverLinker<T> parent, T Object)
            {
                this.parent = parent;
                if (Object != null && Object.Name != null)
                {
                    name = Object.Name;
                    object_list = new List<T>();
                    Add(Object);
                    return;
                }
                throw new ArgumentNullException("Object", "Object 不可以空值並且 Object.Name 不可以空值.");
            }

            /// <summary>
            /// 是否為空的物件.
            /// </summary>
            public bool IsEmpty
            {
                get
                {
                    return (name == null);
                }
            }

            /// <summary>
            /// 連結物件的值.
            /// </summary>
            public T Object
            {
                get
                {
                    if (!IsEmpty && object_list.Count > 0)
                        return object_list[0];
                    return default(T);
                }
                set
                {
                    if (!IsEmpty)
                    {
                        // 更新所有的 Value
                        foreach (T data in object_list)
                        {
                            data.Link(value);
                        }
                    }
                    else
                        throw new Exception("CoverObject is Empty can't Assign Value.");
                }
            }

            /// <summary>
            /// 連結物件的名稱.
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            /// <summary>
            /// 加入指名物件.
            /// </summary>
            /// <param name="LinkObject"></param>
            public void Add(T LinkObject)
            {
                if (!IsEmpty)
                {
                    if (name == LinkObject.Name && !object_list.Contains(LinkObject))
                    {
                        if (object_list.Count > 0)
                            LinkObject.Link(object_list[0]);
                        object_list.Add(LinkObject);
                        LinkObject.Changed += LinkObject_ObjectChanged;
                    }
                    else
                        throw new Exception("must has same name to added");
                }
                else
                    throw new Exception("CoverObject is Empty can't Add any one.");
            }

            /// <summary>
            /// 移除指名物件.
            /// </summary>
            /// <param name="LinkObject"></param>
            public void Remove(T LinkObject)
            {
                if (!IsEmpty)
                {
                    if (object_list.Remove(LinkObject))
                        LinkObject.Changed -= LinkObject_ObjectChanged;
                }
                else
                    throw new Exception("CoverObject is Empty can't Add any one.");
            }

            /// <summary>
            /// 回傳內含指名物件個數.
            /// </summary>
            public int Count
            {
                get
                {
                    if (!IsEmpty)
                        return object_list.Count;
                    return -1;
                }
            }

            #region IEnumerable<INamedObject<T>> Members

            /// <summary>
            /// 擷取可以逐一查看這個物件中個別的子物件。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<T> GetEnumerator()
            {
                return object_list.GetEnumerator();
            }

            #endregion IEnumerable<INamedObject<T>> Members

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return object_list.GetEnumerator();
            }

            #endregion IEnumerable Members

            /// <summary>
            /// 傳回表示目前物件的字串。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("{0}(Count={1})", typeof(T).Name, this.Count);
            }
        }
        /// <summary>
        /// 加入一個連結的物件集合
        /// </summary>
        /// <param name="ObjectLinker"></param>
        public void AddLinker(NamedCollection<T> ObjectLinker)
        {
            lock (this)
            {
                if (ObjectLinker != null)
                {
                    if (!objectLinkers.Contains(ObjectLinker))
                    {
                        // 加入追?關係
                        foreach (T LinkObject in ObjectLinker)
                        {
                            CoverObject co = GetCoverObject(LinkObject.Name);
                            if (co == null)
                            {
                                co = new CoverLinker<T>.CoverObject(this, LinkObject);
                                link.Add(co);
                                co.Changed += LinkObject_ObjectChanged;
                            }
                            else
                                co.Add(LinkObject);
                        }
                        objectLinkers.Add(ObjectLinker);
                    }
                    else
                    {
                        throw new Exception("元件重覆加入");
                    }
                }
            }
        }

        /// <summary>
        /// 移除一個連結的物件集合
        /// </summary>
        /// <param name="ObjectLinker"></param>
        public void RemoveLinker(NamedCollection<T> ObjectLinker)
        {
            lock (this)
            {
                if (ObjectLinker != null)
                {
                    if (objectLinkers.Contains(ObjectLinker))
                    {
                        // 移除追?關係
                        foreach (T LinkObject in ObjectLinker)
                        {
                            CoverObject co = GetCoverObject(LinkObject.Name);
                            if (co != null)
                                co.Remove(LinkObject);
                        }
                        objectLinkers.Remove(ObjectLinker);
                    }
                }
            }
        }

        /// <summary>
        /// 清除所有連結的物件集合之關連
        /// </summary>
        public void LinkerReset()
        {
            lock (this)
            {
                objectLinkers.Clear();
                foreach (var co in link)
                {
                    co.Changed -= LinkObject_ObjectChanged;
                }
                link.Clear();
            }
        }

        private CoverObject GetCoverObject(string name)
        {
            lock (this)
            {
                // bool IsAtChar = name.StartsWith("@");
                return (from conver in link
                        where StringComparer.OrdinalIgnoreCase.Equals(conver.Name, name)
                        select conver).FirstOrDefault();
                // 檢查 
                //foreach (CoverObject link_data in link)
                //{
                //    if (StringComparer.OrdinalIgnoreCase.Equals(link_data.Name, name))
                //    {
                //        return link_data;
                //    }
                //}
                //return null;
            }
        }

        /// <summary>
        /// 回傳是否包含該指名物件
        /// </summary>
        /// <param name="name">物件名稱</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return GetCoverObject(name) != null;
        }

        /// <summary>
        /// 回傳指定位置之物件
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CoverObject this[int index]
        {
            get
            {
                return link[index];
            }
        }

        /// <summary>
        /// 回傳指定指名物件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CoverObject this[string name]
        {
            get
            {
                lock (this)
                {
                    CoverObject data_link = GetCoverObject(name);
                    if (data_link != null)
                    {
                        return data_link;
                    }
                    return CoverLinker<T>.Empty;
                }
            }
        }

        /// <summary>
        /// 回傳物件的個數
        /// </summary>
        public int Count
        {
            get { return link.Count; }
        }

        #region IEnumerable<CoverLink> 成員

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CoverObject> GetEnumerator()
        {
            return (IEnumerator<CoverObject>)link.GetEnumerator();
        }

        #endregion IEnumerable<CoverLink> 成員

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return link.GetEnumerator();
        }

        #endregion IEnumerable 成員

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}(Count={1})", typeof(T).Name, this.Count);
        }
    }
}