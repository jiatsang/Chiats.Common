// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// SqlModel �� Group By �ԭz����.
    /// </summary>
    public class GroupByClause : CommonClause, IEnumerable<GroupByColumn>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<GroupByColumn> list = new List<GroupByColumn>();

        /// <summary>
        /// OrderByColumn ���ܧ�ƥ�
        /// </summary>
        public event EventHandler<ChangedEventArgs<GroupByColumn>> GroupByChanged;

        /// <summary>
        /// Group By �y�k���� �غc�l
        /// </summary>
        /// <param name="parent"></param>
        public GroupByClause(SqlModel parent) : base(parent) { }

        /// <summary>
        /// �s�W�@�� Group By ColumnName
        /// </summary>
        /// <param name="ColumnName">Group By Column Name</param>
        public void Add(string ColumnName)
        {
            GroupByColumn newColumn = new GroupByColumn(ColumnName);
            Add(newColumn);
            Parent.Changed();
            if (GroupByChanged != null)
            {
                GroupByChanged(ChangedEventType.Add, new ChangedEventArgs<GroupByColumn>(newColumn));
            }
        }

        /// <summary>
        /// �s�W�@�� Group By ColumnName
        /// </summary>
        /// <param name="Column">���W��</param>
        public void Add(GroupByColumn Column)
        {
            if (Column != null)
            {
                GroupByColumn newColumn = new GroupByColumn(Column.Name);
                list.Add(newColumn);
                Parent.Changed();
                if (GroupByChanged != null)
                {
                    GroupByChanged(ChangedEventType.Add, new ChangedEventArgs<GroupByColumn>(newColumn));
                }
            }
        }

        /// <summary>
        /// ���� GroupByColumn ����
        /// </summary>
        /// <param name="column"></param>
        public void Remove(GroupByColumn column)
        {
            if (list.Contains(column))
            {
                list.Remove(column);
                Parent.Changed();

                if (GroupByChanged != null)
                {
                    GroupByChanged(ChangedEventType.Removed, new ChangedEventArgs<GroupByColumn>(column));
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void RemoveAll()
        {
            if (list.Count > 0)
            {
                list.Clear();
                Parent.Changed();
                if (GroupByChanged != null)
                {
                    GroupByChanged(ChangedEventType.Removed, new ChangedEventArgs<GroupByColumn>(null));
                }
            }
        }

        /// <summary>
        /// �Ǧ^ ColumnName ���Ӽ�.
        /// </summary>
        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        #region IEnumerable<GroupColumn> Members

        /// <summary>
        ///�^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
        /// </summary>
        /// <returns></returns>
        public IEnumerator<GroupByColumn> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable<GroupColumn> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable Members

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ISqlBuildExport BuildExport = DefaultBuildExport.SQLBuildExport; 
            CommandBuilder CommandBuilder = new CommandBuilder();
            BuildExport.ExportForGroupByClause(this, CommandBuilder, this.Parent, BuildExportOptions.None);
            return CommandBuilder.ToString();
        }
    }

    /// <summary>
    /// GroupByColumn for Group by
    /// </summary>
    public class GroupByColumn
    {
        /// <summary>
        /// GroupByColumn for Group by
        /// </summary>
        /// <param name="name"></param>
        public GroupByColumn(string name) { Name = name; }

        /// <summary>
        /// ColumnName for Group by
        /// </summary>
        public readonly string Name;
    }
}