// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// �ѼƤ��e����@�r���
    /// </summary>
    public struct ParameterValue : IParameterSingleValue
    {
        /// <summary>
        /// �ѼƤ��e����@�r��� ,�غc�l
        /// </summary>
        /// <param name="Value"></param>
        public ParameterValue(string Value) { this._value = Value; }

        private string _value;

        /// <summary>
        /// �ѼƦr���
        /// </summary>
        public string StringValue
        {
            get { return _value; }
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }
    }

    /// <summary>
    /// �ѼƤ��e����@�ƭ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ParameterValue<T> : IParameterSingleValue where T : struct
    {
        /// <summary>
        /// �ѼƤ��e����@�ƭ� ,�غc�l
        /// </summary>
        /// <param name="value"></param>
        public ParameterValue(T value) { this._value = value; }

        private T _value;

        /// <summary>
        /// �ѼƼƭ�
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// �ѼƦr���
        /// </summary>
        public string StringValue
        {
            get { return _value.ToString(); }
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}