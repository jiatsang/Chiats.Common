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
    /// SQL Model �����(Table)�ԭz���O.
    /// </summary>
    public sealed class TableClause : CommonClause, IPartSqlModel, IEnumerable<JoinTable>
    {
        internal TableClause(SqlModel parent) : base(parent) { }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TableSource tableSource = new TableSource();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string tableAliasName;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<JoinTable> list = new List<JoinTable>();

        /// <summary>
        ///  Join Table ���ܧ�ƥ�
        /// </summary>
        public event EventHandler<ChangedEventArgs<JoinTable>> JoinTableChanged;

        /// <summary>
        /// Table ���ܧ�ƥ�
        /// </summary>
        public event EventHandler TableChanged;

        /// <summary>
        /// ���w�D���W��
        /// </summary>
        public TableSource PrimaryTable
        {
            get
            {
                return this.tableSource;
            }
            set
            {
                if (tableSource != value)
                {
                    if (tableSource.SelectModel != null)
                    {
                        tableSource.SelectModel.Parent = null;
                        // TODO: remove Parameters at CoverParameters
                        tableSource.SelectModel.CommandChanged -= new EventHandler(SelectModelChanged);
                    }
                    tableSource = value;
                    if (tableSource.SelectModel != null)
                    {
                        // �إ߸���ܧ󪺳s���q�� ...
                        tableSource.SelectModel.Parent = this.Parent;
                        foreach (var parameter in tableSource.SelectModel.Parameters)
                        {
                            if (this.Parent.CoverParameters.Contains(parameter.Name))
                                this.Parent.CoverParameters[parameter.Name].Add(parameter);
                            else
                            {
                                NamedCollection<Parameter> CurrentLinker = new NamedCollection<Parameter>();
                                CurrentLinker.Add(parameter);
                                this.Parent.CoverParameters.AddLinker(CurrentLinker);
                            }
                        }
                        tableSource.SelectModel.CommandChanged += new EventHandler(SelectModelChanged);
                    }
                }
            }
        }

        private void SelectModelChanged(object sender, EventArgs e)
        {
            Parent.Changed();
            if (TableChanged != null)
            {
                TableChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// ���w�D���O�W�W��
        /// </summary>
        public string PrimaryAliasName
        {
            get { return tableAliasName; }
            set
            {
                if (tableAliasName != value)
                {
                    tableAliasName = value;
                    Parent.Changed();

                    TableChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ���w�[�J JOIN ���W�� , JOIN ��楲���b ���w �D���W�٫�[�J ���\���h�� JOIN ���W��.(��ڤW���Ьd�\ ��Ʈw���)
        /// </summary>
        /// <param name="tableName">���W��</param>
        /// <param name="tableAliasName">���O�W</param>
        /// <param name="joinType">JOIN ���A</param>
        /// <param name="expression">JOIN Expression</param>
        public void Add(string tableName, string tableAliasName, JoinType joinType, string expression)
        {
            JoinTable NewJoinTable = new JoinTable(this, tableName, tableAliasName, joinType, expression);

            list.Add(NewJoinTable);

            NewJoinTable.DataChanged += new EventHandler(JoinTable_DataChanged);
            Parent.Changed();

            JoinTableChanged?.Invoke(ChangedEventType.Add, new ChangedEventArgs<JoinTable>(NewJoinTable));
        }

        private void JoinTable_DataChanged(object sender, EventArgs e)
        {
            Parent.Changed();
            JoinTableChanged?.Invoke(ChangedEventType.Changed, new ChangedEventArgs<JoinTable>((JoinTable)sender));
        }

        /// <summary>
        /// ���w�[�J JOIN ���W�� , JOIN ��楲���b ���w �D���W�٫�[�J ���\���h�� JOIN ���W��.(��ڤW���Ьd�\ ��Ʈw���)
        /// </summary>
        /// <param name="tableName">���W��</param>
        /// <param name="tableAliasName">���O�W</param>
        /// <param name="expression">JOIN Expression</param>
        public void Add(string tableName, string tableAliasName, string expression)
        {
            JoinTable NewJoinTable = new JoinTable(this, tableName, tableAliasName, JoinType.LeftOuter, expression);
            list.Add(NewJoinTable);

            if (NewJoinTable.Table.IsSelectModel)
                NewJoinTable.Table.SelectModel.Parent = this.Parent;

            NewJoinTable.DataChanged += new EventHandler(JoinTable_DataChanged);
            Parent.Changed();

            JoinTableChanged?.Invoke(ChangedEventType.Add, new ChangedEventArgs<JoinTable>(NewJoinTable));
        }

        /// <summary>
        /// ���w�[�J JOIN ���W�� , JOIN ��楲���b ���w �D���W�٫�[�J ���\���h�� JOIN ���W��.(��ڤW���Ьd�\ ��Ʈw���)
        /// </summary>
        /// <param name="tableName">���W��</param>
        /// <param name="expression">JOIN Expression</param>
        public void Add(string tableName, string expression)
        {
            JoinTable NewJoinTable = new JoinTable(this, tableName, null, JoinType.LeftOuter, expression);
            list.Add(NewJoinTable);

            if (NewJoinTable.Table.IsSelectModel)
                NewJoinTable.Table.SelectModel.Parent = this.Parent;

            NewJoinTable.DataChanged += new EventHandler(JoinTable_DataChanged);
            Parent.Changed();

            JoinTableChanged?.Invoke(ChangedEventType.Add, new ChangedEventArgs<JoinTable>(NewJoinTable));
        }

        /// <summary>
        /// �[�J JoinTable ���� , �D���W�٫�[�J ���\���h�� JOIN ���W��.(��ڤW���Ьd�\ ��Ʈw���)
        /// </summary>
        /// <param name="JoinTable">���W��</param>
        public void Add(JoinTable JoinTable)
        {
            if (JoinTable != null)
            {
                JoinTable NewJoinTable = new JoinTable(this, JoinTable.Table, JoinTable.Alias, JoinTable.JoinType, JoinTable.Expression);
                NewJoinTable.Table.Hints = JoinTable.Table.Hints;
                list.Add(NewJoinTable);

                if (NewJoinTable.Table.IsSelectModel)
                {
                    NewJoinTable.Table.SelectModel.Parent = this.Parent;
                    // add Paramterts  to top 
                    if (NewJoinTable.Table.SelectModel.Parameters.Count > 0)
                    {
                        foreach (var parameter in NewJoinTable.Table.SelectModel.Parameters)
                        {

                            if (GetTopModel().CoverParameters.Contains(parameter.Name))
                                GetTopModel().CoverParameters[parameter.Name].Add(parameter);
                            else
                            {
                                NamedCollection<Parameter> CurrentLinker = new NamedCollection<Parameter>();
                                CurrentLinker.Add(parameter);
                                GetTopModel().CoverParameters.AddLinker(CurrentLinker);
                            }
                        }
                        // GetTopModel().Parameters..Add(NewJoinTable.Table.SelectModel.Parameters);
                    }
                }
                NewJoinTable.DataChanged += new EventHandler(JoinTable_DataChanged);
                Parent.Changed();

                JoinTableChanged?.Invoke(ChangedEventType.Add, new ChangedEventArgs<JoinTable>(NewJoinTable));
            }
        }

        /// <summary>
        /// ���� JoinTable ����
        /// </summary>
        /// <param name="table"></param>
        public void Remove(JoinTable table)
        {
            if (list.Contains(table))
            {
                list.Remove(table);
                table.DataChanged -= new EventHandler(JoinTable_DataChanged);
                Parent.Changed();

                JoinTableChanged?.Invoke(ChangedEventType.Removed, new ChangedEventArgs<JoinTable>(table));
            }
        }

        /// <summary>
        /// �Ǧ^ JoinTable ���ƶq.
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// �^�ǫ��w��m JoinTable ����.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public JoinTable this[int index]
        {
            get { return list[index]; }
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ISqlBuildExport BuildExport = DefaultBuildExport.SQLBuildExport;
            CommandBuilder CommandBuilder = new CommandBuilder();
            BuildExport.ExportForTableClause(this, CommandBuilder, this.Parent, BuildExportOptions.None);
            return CommandBuilder.ToString();
        }

        #region IEnumerable<TableJoin> Members

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
        /// </summary>
        /// <returns></returns>
        public IEnumerator<JoinTable> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable<TableJoin> Members

        #region IEnumerable Members

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable Members

    }
}