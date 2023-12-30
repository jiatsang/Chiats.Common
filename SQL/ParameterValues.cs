// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Chiats.SQL
{
    /// <summary>
    /// �w�q�Ѽƪ��d��/�h�Ȫ��ѼƤ��e���s��e�� �䴩 : ArgumentType.Scope/Multi
    /// </summary>
    public class ParameterValues : IEnumerable<IParameterValue>
    {
        private List<IParameterValue> values = new List<IParameterValue>();

        /// <summary>
        /// �ѼƤ��e���s��e��,�غc�l
        /// </summary>
        public ParameterValues() { }

        /// <summary>
        /// �ѼƤ��e���s��e��,�غc�l
        /// </summary>
        /// <param name="args"></param>
        public ParameterValues(params IParameterValue[] args)
        {
            foreach (var value in args) this.values.Add(value);
        }

        /// <summary>
        /// �@���[�J�@�ӽd���
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        public void AddScope(string fromValue, string toValue)
        {
            this.values.Add(new ParameterScope(fromValue, toValue));
        }

        /// <summary>
        /// �@���[�J�@�ӽd���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        public void AddScope<T>(T fromValue, T toValue) where T : struct
        {
            this.values.Add(new ParameterScope<T>(fromValue, toValue));
        }

        /// <summary>
        /// �@���[�J�@�ӳ�@��
        /// </summary>
        /// <param name="value"></param>
        public void Add(string value)
        {
            this.values.Add(new ParameterValue(value));
        }

        /// <summary>
        /// �@���[�J�h�ӳ�@��
        /// </summary>
        /// <param name="values"></param>
        public void Add(params string[] values)
        {
            foreach (var value in values)
            {
                this.values.Add(new ParameterValue(value));
            }
        }

        /// <summary>
        /// �@���[�J�h�ӳ�@��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public void Add<T>(params T[] values) where T : struct
        {
            foreach (var value in values)
            {
                this.values.Add(new ParameterValue<T>(value));
            }
        }

        /// <summary>
        /// �@���[�J�@�ӳ�@��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Add<T>(T value) where T : struct
        {
            this.values.Add(new ParameterValue<T>(value));
        }

        /// <summary>
        /// ���o���w��m���ѼƭȪ���
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IParameterValue this[int index]
        {
            get { return this.values[index]; }
        }

        /// <summary>
        /// �M���e�������Ҧ��ѼƭȪ���
        /// </summary>
        public void Clear()
        {
            this.values.Clear();
        }

        /// <summary>
        /// �M���e�������ѼƭȪ���
        /// </summary>
        /// <param name="value"></param>
        public void Remove(IParameterValue value)
        {
            this.values.Remove(value);
        }

        /// <summary>
        /// �ѼƭȭӼ�
        /// </summary>
        public int Count
        {
            get { return this.values.Count; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IParameterValue> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }
    }
}