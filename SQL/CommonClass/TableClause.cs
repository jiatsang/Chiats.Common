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
    /// SQL Model 的表格(Table)敘述類別.
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
        ///  Join Table 的變更事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<JoinTable>> JoinTableChanged;

        /// <summary>
        /// Table 的變更事件
        /// </summary>
        public event EventHandler TableChanged;

        /// <summary>
        /// 指定主表格名稱
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
                        // 建立資料變更的連結通知 ...
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
        /// 指定主表格別名名稱
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
        /// 指定加入 JOIN 表格名稱 , JOIN 表格必須在 指定 主表格名稱後加入 允許有多個 JOIN 表格名稱.(實際上限請查閱 資料庫文件)
        /// </summary>
        /// <param name="tableName">表格名稱</param>
        /// <param name="tableAliasName">表格別名</param>
        /// <param name="joinType">JOIN 型態</param>
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
        /// 指定加入 JOIN 表格名稱 , JOIN 表格必須在 指定 主表格名稱後加入 允許有多個 JOIN 表格名稱.(實際上限請查閱 資料庫文件)
        /// </summary>
        /// <param name="tableName">表格名稱</param>
        /// <param name="tableAliasName">表格別名</param>
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
        /// 指定加入 JOIN 表格名稱 , JOIN 表格必須在 指定 主表格名稱後加入 允許有多個 JOIN 表格名稱.(實際上限請查閱 資料庫文件)
        /// </summary>
        /// <param name="tableName">表格名稱</param>
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
        /// 加入 JoinTable 物件 , 主表格名稱後加入 允許有多個 JOIN 表格名稱.(實際上限請查閱 資料庫文件)
        /// </summary>
        /// <param name="JoinTable">表格名稱</param>
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
        /// 移除 JoinTable 物件
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
        /// 傳回 JoinTable 的數量.
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// 回傳指定位置 JoinTable 物件.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public JoinTable this[int index]
        {
            get { return list[index]; }
        }

        /// <summary>
        /// String，表示目前的 Object。
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
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<JoinTable> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable<TableJoin> Members

        #region IEnumerable Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable Members

    }
}