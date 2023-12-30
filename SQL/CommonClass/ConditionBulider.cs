// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// SqlModel �����󪫥󲣥;�(ConditionBulider).
    /// </summary>
    public class ConditionBulider : IDisposable
    {
        private ExpressionCondition condition = null;
        private Conditions conditions = null;

        /// <summary>
        /// �[�J�@�ӱ���.
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="condition"></param>
        public ConditionBulider(Conditions conditions, ExpressionCondition condition)
        {
            Debug.Assert(condition != null);
            this.condition = condition;
            this.conditions = conditions;
        }

        /// <summary>
        /// �[�J�@�ӱ���.
        /// </summary>
        /// <param name="condition"></param>
        public void Add(string condition)
        {
            Add(condition, ConditionLink.And);
        }

        /// <summary>
        /// �[�J�@�ӱ���.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="conditionExpression"></param>
        public void Add(string conditionExpression, ConditionLink link)
        {
            condition.Attach(link, conditionExpression);
        }

        /// <summary>
        /// �O�_�ҥα��󪫥� , ExportStatu.Enabled:�ҥ� ExportStatus.Disabled:���ϥ� ExportStatus.Auto:�۰�
        /// </summary>
        public bool Enabled
        {
            get { return condition.Enabled; }
            set { condition.Enabled = value; }
        }

        #region IDisposable ����

        /// <summary>
        /// ���񪫥�Ψ�Ҿ֦����귽.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable ����

        /// <summary>
        ///
        /// </summary>
        ~ConditionBulider()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        private bool FinalizerDispose = false;

        /// <summary>
        /// The bulk of the clean-up code is implemented in Dispose(bool)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!FinalizerDispose)
            {
                // �զX������A�q���[�J Conditions ���X��. �~�ॿ�T�B�z Parameters �s��.
                conditions.Add(condition);
                FinalizerDispose = true;
            }
        }
    }
}