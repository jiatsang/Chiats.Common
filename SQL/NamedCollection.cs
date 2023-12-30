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
    /// ��@�䴩 CoverLinker �ҳs�������󶰦X. ����¦���O.
    /// </summary>
    /// <typeparam name="T">���t���� Value �ȫ��O</typeparam>
    public class NamedCollection<T> : IEnumerable<T> where T : IVariantName
    {
        private List<T> list = new List<T>();

        /// <summary>
        /// �s�������󶰦X�غc�l
        /// </summary>
        public NamedCollection() { }

        /// <summary>
        /// �H����W�٧�M�ثe�Ҧb��m. ���s�b�ɷ|�^�� -1, �`�N �^�ǭȦ��i��]���S�w����ާ@, �ӧ���.
        /// </summary>
        /// <param name="name">����W��</param>
        /// <returns>�Ҧb��m</returns>
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
        /// �������W����.
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
        /// �^�ǫ��w��m������,�Y���s�b�h�|���ͤ@�� Exception
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return list[index]; }
        }

        /// <summary>
        /// �^�ǫ��W������, �Y���s�b�h�^�� null
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
        /// �[�J�@�ӫ��W������.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            list.Add(item);
        }

        /// <summary>
        /// �����@�ӫ��W������.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            bool result = list.Remove(item);
            return result;
        }

        /// <summary>
        /// �M���Ҧ����W������.
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// �^�ǬO�_�����t���W������.
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
        ///  �^�Ǥ��t���W������Ӽ�.
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
        /// �O�_�� ReadOnly ���X����.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion ICollection Members

        #region IEnumerable Members

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
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