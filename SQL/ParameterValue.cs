// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 把计ず甧虫﹃
    /// </summary>
    public struct ParameterValue : IParameterSingleValue
    {
        /// <summary>
        /// 把计ず甧虫﹃ ,篶
        /// </summary>
        /// <param name="Value"></param>
        public ParameterValue(string Value) { this._value = Value; }

        private string _value;

        /// <summary>
        /// 把计﹃
        /// </summary>
        public string StringValue
        {
            get { return _value; }
        }

        /// <summary>
        /// Stringボヘ玡 Object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }
    }

    /// <summary>
    /// 把计ず甧虫计
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ParameterValue<T> : IParameterSingleValue where T : struct
    {
        /// <summary>
        /// 把计ず甧虫计 ,篶
        /// </summary>
        /// <param name="value"></param>
        public ParameterValue(T value) { this._value = value; }

        private T _value;

        /// <summary>
        /// 把计计
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// 把计﹃
        /// </summary>
        public string StringValue
        {
            get { return _value.ToString(); }
        }

        /// <summary>
        /// Stringボヘ玡 Object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}