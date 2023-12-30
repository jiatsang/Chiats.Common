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
        /// �W�ߪ��Ѽƪ���
        /// </summary>
        Independent = 0x01,

        /// <summary>
        /// �v�ҥΪ����󦡪��Ѽƪ���
        /// </summary>
        ConditionEnabled = 0x03,

        /// <summary>
        /// �W�ߪ��Ѽƪ���M�v�ҥΪ����󦡪��Ѽƪ���
        /// </summary>
        Enabled = Independent | ConditionEnabled
    }

    /// <summary>
    /// SqlModel Parameters ����. (Internal Used)
    /// </summary>
    public class CommonModelParameters : IEnumerable<Parameter>
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private CoverLinker<Parameter> CoverParameters = null;

        /// <summary>
        /// ��Ʋ��ʳq��. LinkObject ���ʮɷ|�H���ƥ�q��������.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// �^�ǬO�_�����Ī��Ѽƪ���.
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
                if (p.Condition?.Name == null ) // �W�ߪ��Ѽƪ���. �������Ī��Ѽƪ���
                {
                    if (ValidType.HasFlag(ParameterValidType.Independent)) ConditionEnabled++;
                }
                else if (p.Condition.Enabled)
                {
                    // Condition ���t���Ѽƪ���. �h�������󪫥�O�_ Enabled
                    if (ValidType.HasFlag(ParameterValidType.ConditionEnabled)) ConditionEnabled++;
                }
            }
            return ConditionEnabled > 0;
        }

        /// <summary>
        /// �^�Ǧ��Ī��Ѽƪ���. �Y���t �L���� Condition ���t���Ѽƪ���
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
        /// SqlModel Parameters ����. (Internal Used)
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
        /// �^�Ǫ��󪺭Ӽ�
        /// </summary>
        public int Count
        {
            get { return CoverParameters.Count; }
        }

        /// <summary>
        /// �^�ǫ��w��m������
        /// </summary>
        /// <param name="index"></param>
        /// <returns>���w��m����</returns>
        public Parameter this[int index]
        {
            get
            {
                return CoverParameters[index].Object;
            }
        }

        /// <summary>
        /// �^�ǫ��w���W����
        /// </summary>
        /// <param name="name">����W��</param>
        /// <returns>���W����</returns>
        public Parameter this[string name]
        {
            get
            {
                return CoverParameters[name].Object;
            }
        }

        /// <summary>
        /// �^�ǬO�_�]�t�ӫ��W����
        /// </summary>
        /// <param name="name">����W��</param>
        /// <returns></returns>
        public bool Contains(string name) { return CoverParameters.Contains(name); }

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
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
                        throw new CommonException($"���w�q�� Parameter Name {p_name} in {nameof(parameters)}");
                }
            }
        }
        #region Class ParameterEnumerator

        /// <summary>
        /// �ʸ� (IEnumerator&lt;CoverLinker&lt;Parameter&gt;.CoverObject&gt;) TO IEnumerator&lt;Parameter&gt;
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
            /// ���񪫥�Ψ�Ҿ֦����귽.
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
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Parameter(Count={0})", this.Count);
        }
    }
}