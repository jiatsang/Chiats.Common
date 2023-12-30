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
    /// 處理 SQL 語法的基礎物件.
    /// </summary>
    public abstract class CommonClause : IPartSqlModel
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SqlModel parent;

        /// <summary>
        ///
        /// </summary>
        /// <param name="parent"></param>
        public CommonClause(SqlModel parent)
        {
            Debug.Assert(parent != null);
            this.parent = parent;
        }

        /// <summary>
        ///
        /// </summary>
        protected SqlModel Parent
        {
            get { return parent; }
        }

        #region IPartSqlModel Members

        /// <summary>
        /// 傳回最上一層的 CommonModel 父階物件
        /// </summary>
        /// <returns>最上一層的 CommonModel 父階物件</returns>
        public SqlModel GetTopModel()
        {
            return Parent.GetTopModel();
        }

        #endregion IPartSqlModel Members

        ///// <summary>
        ///// ChangedVariable 管理者物件.
        ///// </summary>
        //protected readonly ChangedVariableManager ChangedVariableManager;

        ///// <summary>
        ///// 物件建構子
        ///// </summary>
        //public CommonClause() {
        //    ChangedVariableManager = new ChangedVariableManager(this);

        //}
        ///// <summary>
        ///// 清除所有異動計數器.
        ///// </summary>
        //public void Reset()
        //{
        //    ChangedVariableManager.Reset();
        //}

        ///// <summary>
        ///// 依指定的識別名稱回傳變更次數.
        ///// </summary>
        ///// <param name="Name">當傳入 "ALL" 時表示要回傳全部的變更次數總合, 注意:識別名稱不區分大寫字母.</param>
        ///// <returns>回傳變更次數</returns>
        //public int GetChangedCount(string Name)
        //{
        //    return ChangedVariableManager.ChangedCountByName(Name);
        //}
    }
}