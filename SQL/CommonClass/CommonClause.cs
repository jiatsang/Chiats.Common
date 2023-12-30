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
    /// �B�z SQL �y�k����¦����.
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
        /// �Ǧ^�̤W�@�h�� CommonModel ��������
        /// </summary>
        /// <returns>�̤W�@�h�� CommonModel ��������</returns>
        public SqlModel GetTopModel()
        {
            return Parent.GetTopModel();
        }

        #endregion IPartSqlModel Members

        ///// <summary>
        ///// ChangedVariable �޲z�̪���.
        ///// </summary>
        //protected readonly ChangedVariableManager ChangedVariableManager;

        ///// <summary>
        ///// ����غc�l
        ///// </summary>
        //public CommonClause() {
        //    ChangedVariableManager = new ChangedVariableManager(this);

        //}
        ///// <summary>
        ///// �M���Ҧ����ʭp�ƾ�.
        ///// </summary>
        //public void Reset()
        //{
        //    ChangedVariableManager.Reset();
        //}

        ///// <summary>
        ///// �̫��w���ѧO�W�٦^���ܧ󦸼�.
        ///// </summary>
        ///// <param name="Name">��ǤJ "ALL" �ɪ�ܭn�^�ǥ������ܧ󦸼��`�X, �`�N:�ѧO�W�٤��Ϥ��j�g�r��.</param>
        ///// <returns>�^���ܧ󦸼�</returns>
        //public int GetChangedCount(string Name)
        //{
        //    return ChangedVariableManager.ChangedCountByName(Name);
        //}
    }
}