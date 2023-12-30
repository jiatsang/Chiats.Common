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
    /// ﹚竡把计絛瞅/把计ず甧甧竟 や穿 : ArgumentType.Scope/Multi
    /// </summary>
    public class ParameterValues : IEnumerable<IParameterValue>
    {
        private List<IParameterValue> values = new List<IParameterValue>();

        /// <summary>
        /// 把计ず甧甧竟,篶
        /// </summary>
        public ParameterValues() { }

        /// <summary>
        /// 把计ず甧甧竟,篶
        /// </summary>
        /// <param name="args"></param>
        public ParameterValues(params IParameterValue[] args)
        {
            foreach (var value in args) this.values.Add(value);
        }

        /// <summary>
        /// Ω絛瞅
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        public void AddScope(string fromValue, string toValue)
        {
            this.values.Add(new ParameterScope(fromValue, toValue));
        }

        /// <summary>
        /// Ω絛瞅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        public void AddScope<T>(T fromValue, T toValue) where T : struct
        {
            this.values.Add(new ParameterScope<T>(fromValue, toValue));
        }

        /// <summary>
        /// Ω虫
        /// </summary>
        /// <param name="value"></param>
        public void Add(string value)
        {
            this.values.Add(new ParameterValue(value));
        }

        /// <summary>
        /// Ω虫
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
        /// Ω虫
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
        /// Ω虫
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Add<T>(T value) where T : struct
        {
            this.values.Add(new ParameterValue<T>(value));
        }

        /// <summary>
        /// 眔﹚竚把计ン
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IParameterValue this[int index]
        {
            get { return this.values[index]; }
        }

        /// <summary>
        /// 睲埃甧竟ず┮Τ把计ン
        /// </summary>
        public void Clear()
        {
            this.values.Clear();
        }

        /// <summary>
        /// 睲埃甧竟ず把计ン
        /// </summary>
        /// <param name="value"></param>
        public void Remove(IParameterValue value)
        {
            this.values.Remove(value);
        }

        /// <summary>
        /// 把计计
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