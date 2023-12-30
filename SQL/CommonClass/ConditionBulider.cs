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
    /// SqlModel 的條件物件產生器(ConditionBulider).
    /// </summary>
    public class ConditionBulider : IDisposable
    {
        private ExpressionCondition condition = null;
        private Conditions conditions = null;

        /// <summary>
        /// 加入一個條件式.
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
        /// 加入一個條件式.
        /// </summary>
        /// <param name="condition"></param>
        public void Add(string condition)
        {
            Add(condition, ConditionLink.And);
        }

        /// <summary>
        /// 加入一個條件式.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="conditionExpression"></param>
        public void Add(string conditionExpression, ConditionLink link)
        {
            condition.Attach(link, conditionExpression);
        }

        /// <summary>
        /// 是否啟用條件物件 , ExportStatu.Enabled:啟用 ExportStatus.Disabled:不使用 ExportStatus.Auto:自動
        /// </summary>
        public bool Enabled
        {
            get { return condition.Enabled; }
            set { condition.Enabled = value; }
        }

        #region IDisposable 成員

        /// <summary>
        /// 釋放物件及其所擁有的資源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable 成員

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
                // 組合完成後再通知加入 Conditions 集合中. 才能正確處理 Parameters 連結.
                conditions.Add(condition);
                FinalizerDispose = true;
            }
        }
    }
}