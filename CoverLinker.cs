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
    /// CoverLinker �H�s���G�өΤG�ӥH�W�� Collection(Collection �� Key/Value �������ۦP�����A),
    /// CoverLinker �i�H�ΨӺ޲z��Ҧ��Q�s������. <br/>
    /// CoverLinker �򥻤W�N�O�@�Ӥ����h, ��~�u�ݭn�� CoverLinker �@�B�z, ���ݭn�z�|�U�����h�ӳs�� Collection.  Collection ���W�[�ܧ�쥻
    /// �N���O CoverLinker �ݭn�B�z���@����. �ӬO�U�s�� Collection �ۦ�ݭn�B�z���欰.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CoverLinker<T> : IEnumerable<CoverLinker<T>.CoverObject> where T : ILinkObject
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<CoverObject> link = new List<CoverObject>();

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private HashSet<NamedCollection<T>> objectLinkers = new HashSet<NamedCollection<T>>();

        /// <summary>
        /// ��Ʋ��ʳq�� . LinkObject ���ʮɷ|�H���ƥ�q��������.
        /// </summary>
        public event EventHandler Changed;

        private void LinkObject_ObjectChanged(object sender, EventArgs e)
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// �Ū� CoverObject Instance, ��A�d�ߤ@�Ӥ��s�b���W�ٳs���ɥ��|�^�� CoverObject.Empty �ӫD NULL.
        /// �ΥH���� CoverLinker["Na"].Value ���楢��. �A�i�H�� CoverLinker["Na"].IsEmpty �ˬd.
        /// </summary>
        public static CoverObject Empty = new CoverObject();

        /// <summary>
        /// CoverLinker �H�s�����󤧺޲z����.
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
            /// ��Ʋ��ʳq�� . LinkObject ���ʮɷ|�H���ƥ�q��������.
            /// </summary>
            public event EventHandler Changed;

            private void LinkObject_ObjectChanged(object sender, EventArgs e)
            {
                if (Changed != null) Changed(this, EventArgs.Empty);
            }

            /// <summary>
            /// CoverObject  �غc�l
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
                throw new ArgumentNullException("Object", "Object ���i�H�ŭȨåB Object.Name ���i�H�ŭ�.");
            }

            /// <summary>
            /// �O�_���Ū�����.
            /// </summary>
            public bool IsEmpty
            {
                get
                {
                    return (name == null);
                }
            }

            /// <summary>
            /// �s�����󪺭�.
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
                        // ��s�Ҧ��� Value
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
            /// �s�����󪺦W��.
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            /// <summary>
            /// �[�J���W����.
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
            /// �������W����.
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
            /// �^�Ǥ��t���W����Ӽ�.
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
            /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
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
            /// �Ǧ^��ܥثe���󪺦r��C
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("{0}(Count={1})", typeof(T).Name, this.Count);
            }
        }
        /// <summary>
        /// �[�J�@�ӳs�������󶰦X
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
                        // �[�J�l?���Y
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
                        throw new Exception("�����Х[�J");
                    }
                }
            }
        }

        /// <summary>
        /// �����@�ӳs�������󶰦X
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
                        // �����l?���Y
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
        /// �M���Ҧ��s�������󶰦X�����s
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
                // �ˬd 
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
        /// �^�ǬO�_�]�t�ӫ��W����
        /// </summary>
        /// <param name="name">����W��</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return GetCoverObject(name) != null;
        }

        /// <summary>
        /// �^�ǫ��w��m������
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
        /// �^�ǫ��w���W����
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
        /// �^�Ǫ��󪺭Ӽ�
        /// </summary>
        public int Count
        {
            get { return link.Count; }
        }

        #region IEnumerable<CoverLink> ����

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CoverObject> GetEnumerator()
        {
            return (IEnumerator<CoverObject>)link.GetEnumerator();
        }

        #endregion IEnumerable<CoverLink> ����

        #region IEnumerable ����

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return link.GetEnumerator();
        }

        #endregion IEnumerable ����

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}(Count={1})", typeof(T).Name, this.Count);
        }
    }
}