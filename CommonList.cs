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
    /// �q�Ϊ��}�C���O�w�q ,���ѥH�W�٬��D�䪺��k, �M SearchList �ۦP�|���@�ӥH Name ���ƪ���k.
    /// </summary>
    /// <typeparam name="T">�}�C���󪺫��O</typeparam>
    public abstract class CommonList<T> : IEnumerable<T> where T : IVariantName
    {
        private readonly List<T> list = new List<T>();

        /// <summary>
        /// �^�ǰ}�C�Ӽ�
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// �^�ǫ��w��m������.
        /// </summary>
        /// <param name="index">���w��m</param>
        /// <returns>�^�Ǫ���</returns>
        public T this[int index]
        {
            get { return list[index]; }
        }

        /// <summary>
        /// �̫��w���W�٦^�Ǫ����m
        /// </summary>
        /// <param name="name">���w���W��</param>
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
        /// �^�ǫ��w����W�٪�����.
        /// </summary>
        /// <param name="name">����W��</param>
        /// <returns>�^�Ǫ���</returns>
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
        /// �^�ǫ��w����W�٪� Replace ����.
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
        /// �[�J�@�ӷs���������
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
        /// �����@�Ӫ������
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
        /// �����@�Ӫ������
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
        /// �^�ǬO�_�]�t�Ӫ���
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool BaseContains(T item)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// �^�ǬO�_�]�t�ӫ��W����
        /// </summary>
        /// <param name="name">����W��</param>
        /// <returns></returns>
        protected bool BaseContains(string name)
        {
            return GetIndexByName(name) != -1;
        }

        /// <summary>
        /// �����Ҧ��������
        /// </summary>
        protected void BaseClear()
        {
            list.Clear();
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO�l������C
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
        /// �Ǧ^��ܥثe���󪺦r��C�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}(Count={1})", typeof(T).Name, this.Count);
        }
    }
}