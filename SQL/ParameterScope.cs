// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// �Ѽƪ��d��r��� (From~TO)
    /// </summary>
    public struct ParameterScope : IParameterScopeValue
    {
        /// <summary>
        /// �Ѽƪ��d��r��� (From~TO) �غc�l
        /// </summary>
        /// <param name="fromValue">�}�l��</param>
        /// <param name="toValue">������</param>
        public ParameterScope(string fromValue, string toValue) { this.fromValue = fromValue; this.toValue = toValue; }

        // TODO: �۰� SWAP FromValue and TOValue
        private string fromValue;

        private string toValue;

        /// <summary>
        /// �}�l�r���
        /// </summary>
        public string FromStringValue
        {
            get { return fromValue; }
        }

        /// <summary>
        /// �����r���
        /// </summary>
        public string ToStringValue
        {
            get { return toValue; }
        }

        /// <summary>
        /// �Ǧ^��ܥثe���󪺦r��C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Scope {0} to {1}", fromValue, toValue);
        }
    }

    /// <summary>
    /// �Ѽƪ��d��ƭ� (From~TO) Examlpe : ParameterScope&lt;int&gt;
    /// </summary>
    /// <typeparam name="T">�ƭȫ��O</typeparam>
    public struct ParameterScope<T> : IParameterScopeValue where T : struct
    {
        /// <summary>
        /// �Ѽƪ��d��ƭ� (From~TO) �غc�l
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        public ParameterScope(T Start, T End) { this.Start = Start; this.End = End; }

        private T Start;
        private T End;

        /// <summary>
        /// �}�l�r���
        /// </summary>
        public string FromStringValue
        {
            // TODO: �`�N�^�Ǯ榡
            get { return Start.ToString(); }
        }

        /// <summary>
        /// �����r���
        /// </summary>
        public string ToStringValue
        {
            get { return End.ToString(); }
        }

        /// <summary>
        /// �}�l�ƭ�
        /// </summary>
        public T ToValue
        {
            get { return End; }
        }

        /// <summary>
        /// �����ƭ�
        /// </summary>
        public T FromValue
        {
            get { return Start; }
        }
    }
}