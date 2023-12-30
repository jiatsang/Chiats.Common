// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// 把计絛瞅﹃ (From~TO)
    /// </summary>
    public struct ParameterScope : IParameterScopeValue
    {
        /// <summary>
        /// 把计絛瞅﹃ (From~TO) 篶
        /// </summary>
        /// <param name="fromValue">秨﹍</param>
        /// <param name="toValue">挡</param>
        public ParameterScope(string fromValue, string toValue) { this.fromValue = fromValue; this.toValue = toValue; }

        // TODO: 笆 SWAP FromValue and TOValue
        private string fromValue;

        private string toValue;

        /// <summary>
        /// 秨﹍﹃
        /// </summary>
        public string FromStringValue
        {
            get { return fromValue; }
        }

        /// <summary>
        /// 挡﹃
        /// </summary>
        public string ToStringValue
        {
            get { return toValue; }
        }

        /// <summary>
        /// 肚ボヘ玡ン﹃
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Scope {0} to {1}", fromValue, toValue);
        }
    }

    /// <summary>
    /// 把计絛瞅计 (From~TO) Examlpe : ParameterScope&lt;int&gt;
    /// </summary>
    /// <typeparam name="T">计</typeparam>
    public struct ParameterScope<T> : IParameterScopeValue where T : struct
    {
        /// <summary>
        /// 把计絛瞅计 (From~TO) 篶
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        public ParameterScope(T Start, T End) { this.Start = Start; this.End = End; }

        private T Start;
        private T End;

        /// <summary>
        /// 秨﹍﹃
        /// </summary>
        public string FromStringValue
        {
            // TODO: 猔種肚Α
            get { return Start.ToString(); }
        }

        /// <summary>
        /// 挡﹃
        /// </summary>
        public string ToStringValue
        {
            get { return End.ToString(); }
        }

        /// <summary>
        /// 秨﹍计
        /// </summary>
        public T ToValue
        {
            get { return End; }
        }

        /// <summary>
        /// 挡计
        /// </summary>
        public T FromValue
        {
            get { return Start; }
        }
    }
}