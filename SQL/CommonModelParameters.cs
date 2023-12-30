// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Chiats.SQL
{
    [Flags]
    public enum ParameterValidType
    {
        /// <summary>
        /// 縒ミ把计ン
        /// </summary>
        Independent = 0x01,

        /// <summary>
        /// 币ノ兵ンΑ把计ン
        /// </summary>
        ConditionEnabled = 0x03,

        /// <summary>
        /// 縒ミ把计ン㎝币ノ兵ンΑ把计ン
        /// </summary>
        Enabled = Independent | ConditionEnabled
    }

    /// <summary>
    /// SqlModel Parameters ざ. (Internal Used)
    /// </summary>
    public class CommonModelParameters : IEnumerable<Parameter>
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private CoverLinker<Parameter> CoverParameters = null;

        /// <summary>
        /// 戈钵笆硄. LinkObject 钵笆穦ㄆン硄じン.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// 肚琌Τ把计ン.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ValidType"></param>
        /// <returns></returns>
        public bool IsValidParameter(string name, ParameterValidType ValidType = ParameterValidType.Enabled)
        {
            if (CoverParameters.Contains(name))
            {
                return IsValidParameter(CoverParameters[name], ValidType);
            }
            return false;
        }

        private bool IsValidParameter(CoverLinker<Parameter>.CoverObject CoverObject, ParameterValidType ValidType)
        {
            int ConditionEnabled = 0;
            foreach (Parameter p in CoverObject)
            {
                if (p.Condition?.Name == null ) // 縒ミ把计ン. 跌Τ把计ン
                {
                    if (ValidType.HasFlag(ParameterValidType.Independent)) ConditionEnabled++;
                }
                else if (p.Condition.Enabled)
                {
                    // Condition ず把计ン. 玥跌兵ンン琌 Enabled
                    if (ValidType.HasFlag(ParameterValidType.ConditionEnabled)) ConditionEnabled++;
                }
            }
            return ConditionEnabled > 0;
        }

        /// <summary>
        /// 肚Τ把计ン. ぃ 礚竖 Condition ず把计ン
        /// </summary>
        /// <returns></returns>
        public IList<Parameter> GetValidParameters(ParameterValidType ValidType = ParameterValidType.Enabled)
        {
            List<Parameter> list = new List<Parameter>();
            foreach (CoverLinker<Parameter>.CoverObject CoverObject in CoverParameters)
            {
                if (IsValidParameter(CoverObject, ValidType)) list.Add(CoverObject.Object);
            }
            return list;
        }

        private void LinkObject_ObjectChanged(object sender, EventArgs e)
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// SqlModel Parameters ざ. (Internal Used)
        /// </summary>
        /// <param name="CoverParameters"></param>
        internal CommonModelParameters(CoverLinker<Parameter> CoverParameters)
        {
            this.CoverParameters = CoverParameters;
            CoverParameters.Changed += LinkObject_ObjectChanged;
        }

        /// <summary>
        ///
        /// </summary>
        ~CommonModelParameters()
        {
            CoverParameters.Changed -= LinkObject_ObjectChanged;
        }

        /// <summary>
        /// 肚ン计
        /// </summary>
        public int Count
        {
            get { return CoverParameters.Count; }
        }

        /// <summary>
        /// 肚﹚竚ぇン
        /// </summary>
        /// <param name="index"></param>
        /// <returns>﹚竚ン</returns>
        public Parameter this[int index]
        {
            get
            {
                return CoverParameters[index].Object;
            }
        }

        /// <summary>
        /// 肚﹚ン
        /// </summary>
        /// <param name="name">ン嘿</param>
        /// <returns>ン</returns>
        public Parameter this[string name]
        {
            get
            {
                return CoverParameters[name].Object;
            }
        }

        /// <summary>
        /// 肚琌赣ン
        /// </summary>
        /// <param name="name">ン嘿</param>
        /// <returns></returns>
        public bool Contains(string name) { return CoverParameters.Contains(name); }

        /// <summary>
        /// 耝硋琩硂ンいン
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Parameter> GetEnumerator()
        {
            return new ParameterEnumerator((IEnumerator<CoverLinker<Parameter>.CoverObject>)CoverParameters.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ParameterEnumerator((IEnumerator<CoverLinker<Parameter>.CoverObject>)CoverParameters.GetEnumerator());
        }


        public void Fill(object parameters)
        {
            if (parameters != null)
            {
                foreach (var p in parameters.GetType().GetProperties())
                {
                    var p_name = $"@{p.Name}";
                    if (this.Contains(p_name))
                        this[p_name].Value = p.GetValue(parameters, null);
                    else
                        throw new CommonException($"ゼ﹚竡 Parameter Name {p_name} in {nameof(parameters)}");
                }
            }
        }
        #region Class ParameterEnumerator

        /// <summary>
        /// 杆 (IEnumerator&lt;CoverLinker&lt;Parameter&gt;.CoverObject&gt;) TO IEnumerator&lt;Parameter&gt;
        /// </summary>
        private class ParameterEnumerator : IEnumerator<Parameter>
        {
            private IEnumerator<CoverLinker<Parameter>.CoverObject> Enumerator;

            public ParameterEnumerator(IEnumerator<CoverLinker<Parameter>.CoverObject> Enumerator)
            {
                this.Enumerator = Enumerator;
            }

            public Parameter Current
            {
                get { return Enumerator.Current.Object; }
            }

            /// <summary>
            /// 睦ンのㄤ┮局Τ戈方.
            /// </summary>
            public void Dispose()
            {
                Enumerator.Dispose();
            }

            object IEnumerator.Current
            {
                get { return Enumerator.Current.Object; }
            }

            public bool MoveNext()
            {
                return Enumerator.MoveNext();
            }

            public void Reset()
            {
                Enumerator.Reset();
            }
        }

        #endregion Class ParameterEnumerator

        /// <summary>
        /// Stringボヘ玡 Object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Parameter(Count={0})", this.Count);
        }
    }
}