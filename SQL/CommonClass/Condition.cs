// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL CTL ����(Condition) ��¦���O.
    /// </summary>
    public abstract class Condition : IVariantName, IPartSqlModel
    {
        private Conditions Conditions = null;

        /// <summary>
        /// ��ܱ���(Condition) ����X��k(�ҥ�/�T��)�ﶵ.
        /// </summary>
        private bool enabled = true;

        /// <summary>
        /// ����(Condition)�W��. �Y���ΦW(Anonymous)����,���Ȭ� null
        /// </summary>
        private string name = null;

        /// <summary>
        /// �M�W�@�ӱ���(Condition) ���s���B��l. �]�t AND �� OR  ,�Y���Ĥ@�ӱ���(Condition), ��ȵL��
        /// </summary>
        protected ConditionLink uplink = ConditionLink.And;

        /// <summary>
        /// ����(Condition) ���󪺫غc�l.
        /// </summary>
        /// <param name="Conditions">Belong Conditions</param>
        /// <param name="name">����(Condition)�W��. �Y���ΦW(Anonymous)����,���Ȭ� null</param>
        /// <param name="uplink">�M�W�@�ӱ���(Condition) ���s���B��l</param>
        /// <param name="enabled"> ��ܱ���(Condition) ����X��k(�ҥ�/�T��)�ﶵ. </param>
        public Condition(Conditions Conditions, string name, ConditionLink uplink, bool enabled)
        {
            this.Conditions = Conditions;
            this.enabled = enabled;
            this.Name = name;
            this.uplink = uplink;
        }

        /// <summary>
        /// �ܧ����(Condition) ���ݪ� Conditions �e��.
        /// </summary>
        /// <param name="Conditions">���ݪ� Conditions �e��</param>
        internal void ChangeBelongConditions(Conditions Conditions)
        {
            this.Conditions = Conditions;
        }

        protected void initializeFinished()
        {
            ProcessLinkParameters();
        }

        private void ProcessLinkParameters()
        {
            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (Parameter p in Parameters) p.Condition = this;
            }
        }

        /// <summary>
        /// �q�����ݪ� Conditions �e��, ����(Condition) �v���ܧ�.
        /// </summary>
        protected void RasieConditionChanged()
        {
            if (Conditions != null)
                Conditions.RasieWhereConditionChanged(this);

            ProcessLinkParameters();
        }

        /// <summary>
        /// �]�w�Ψ��o�ثe���󪫥�ҥλP�_
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    RasieConditionChanged();
                }
            }
        }

        /// <summary>
        /// ���󪫥�O�_����X�����A ( Enabled�� True �åB �ҥ]�t���ѼƭȤ��e�����]�w��Ȯ� )
        /// </summary>
        /// <returns></returns>
        public bool ExportEnabled(ParameterMode Mode)
        {
            if (Mode == ParameterMode.Parameter) return this.enabled;
            if (this.enabled && this.Parameters != null)
            {
                foreach (Parameter Parameter in this.Parameters)
                    if (Parameter.IsClear) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// �M�W�@�ӱ���(Condition) ���s���B��l. �]�t AND �� OR  ,�Y���Ĥ@�ӱ���(Condition), ��ȵL��
        /// </summary>
        public ConditionLink Link
        {
            get { return uplink; }
            set
            {
                if (uplink != value)
                {
                    uplink = value;
                    RasieConditionChanged();
                }
            }
        }

        /// <summary>
        /// ���ܱ���(Condition) �O�_���@�ӪŪ�����(Condition)
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// �^�Ǳ���(Condition) �ҥ]�t���Ѽƶ��X.
        /// </summary>
        public abstract NamedCollection<Parameter> Parameters { get; }

        /// <summary>
        /// �^�Ǳ��� (Condition) SQLCTL ��r���e.
        /// </summary>
        public abstract string ConditionSource { get; }

        /// <summary>
        /// �^�Ǳ��� (Condition).�ѪR�� SQL/SQLCTL ��r���e
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="mode"></param>
        /// <param name="Export"></param>
        /// <returns></returns>
        public abstract string ConditionExpression(CommandBuildType BuildType, ParameterMode mode, ExportParameter Export);

        /// <summary>
        /// �^�ǬO�_�t�����w���ѼƦW�٦s�b
        /// </summary>
        /// <param name="name">���w���ѼƦW��</param>
        /// <returns></returns>
        public bool BelongParameter(string name)
        {
            foreach (Parameter p in Parameters)
            {
                if (string.Compare(p.Name, name, true) == 0) return true;
            }
            return false;
        }

        /// <summary>
        /// �^�ǬO�_�t�����w���ѼƦs�b
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public bool BelongParameter(Parameter Parameter)
        {
            foreach (Parameter p in Parameters)
            {
                if (p == Parameter) return true;
            }
            return false;
        }

        /// <summary>
        /// ���󪫥�W��, �ΦW(Anonymous)�h�^�Ǧ^ null
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    if (Conditions != null)
                    {
                        Conditions.RasieWhereConditionChanged(this);
                    }
                }
            }
        }

        /// <summary>
        /// �Ǧ^�̤W�@�h�� CommonModel ��������
        /// </summary>
        /// <returns>�̤W�@�h�� CommonModel ��������</returns>
        public SqlModel GetTopModel()
        {
            Debug.Assert(Conditions != null);
            return Conditions.Parent.GetTopModel();
        }
    }
}