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
    /// SQL 表格名稱物件類別或是 SQL Select Command, 表格名稱格式如下 [database].[owner].[tablename]
    /// </summary>
    public class TableSource
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string owner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string table;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SelectModel _SelectModel;

#pragma warning disable CS0414 // 已指派欄位 'TableSource.DataChanged'，但從未使用過其值。
        /// <summary>
        /// TableJoin 變更事件通知
        /// </summary>
        public event EventHandler DataChanged;
#pragma warning restore CS0414 // 已指派欄位 'TableSource.DataChanged'，但從未使用過其值。

        /// <summary>
        ///
        /// </summary>
        public static TableSource Empty = new TableSource(null, null, null);

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="db">資料庫名稱</param>
        /// <param name="owner">Owner</param>
        /// <param name="table">表格名稱</param>
        public TableSource(string db, string owner, string table)
        {
            this.db = SQLHelper.UnpackObjectName(db);
            this.owner = SQLHelper.UnpackObjectName(owner);
            this.table = SQLHelper.UnpackObjectName(table);
            this._SelectModel = null;
            this.DataChanged = null;
            this.hints = null;
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="selectModel"></param>
        public TableSource(SelectModel selectModel)
        {
            this.db = null;
            this.owner = null;
            this.table = null;
            this._SelectModel = selectModel;
            this.DataChanged = null;
            this.hints = null;
        }

        public TableSource()
        {
            this.db = null;
            this.owner = null;
            this.table = null;
            this._SelectModel = null;
            this.DataChanged = null;
            this.hints = null;
        }

        internal TableSource(SQL.StringObjectName k123)
        {
            switch (k123.KeyIndex)
            {
                case StringObjectName.KeyIndexType.K1:
                    this.db = null;
                    this.owner = null;
                    this.table = k123.Name1;
                    break;

                case StringObjectName.KeyIndexType.K2:
                    this.db = null;
                    this.owner = k123.Name1;
                    this.table = k123.Name2;
                    break;

                case StringObjectName.KeyIndexType.K3:
                    this.db = k123.Name1;
                    this.owner = k123.Name2;
                    this.table = k123.Name3;
                    break;

                default:
                    throw new SqlModelSyntaxException("Invalid K123.");
            }
            this._SelectModel = null;
            this.DataChanged = null;
            this.hints = null;
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="fullname">資料庫名稱 [database].[owner].[tablename]</param>
        public TableSource(string fullname)
        {
            SQLTokenScanner list = new SQLTokenScanner(fullname);

            db = owner = table = null;
            this._SelectModel = null;
            this.DataChanged = null;
            this.hints = null;

            StringObjectName k123 = StringObjectName.NameTest(list);
            switch (k123.KeyIndex)
            {
                case StringObjectName.KeyIndexType.K1:
                    table = SQLHelper.UnpackObjectName(k123.Name1);
                    break;

                case StringObjectName.KeyIndexType.K2:
                    owner = SQLHelper.UnpackObjectName(k123.Name1);
                    table = SQLHelper.UnpackObjectName(k123.Name2);
                    break;

                case StringObjectName.KeyIndexType.K3:
                    db = SQLHelper.UnpackObjectName(k123.Name1);
                    owner = SQLHelper.UnpackObjectName(k123.Name2);
                    table = SQLHelper.UnpackObjectName(k123.Name3);
                    break;
                default:
                    this._SelectModel = k123.SelectModel;
                    // throw
                    break;
            }
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="table"></param>
        public TableSource(TableName table) : this(table.Database, table.Owner, table.Name) { }

        private string hints;

        /// <summary>
        /// 只找 with () 內的字串存至 Select.Tables.Hints ,(目前以處理 SQL Server 為主)
        /// 不理會其內容 這部份可能需對不同資料庫的作法進行分析後, 才能進行細部設計
        /// </summary>
        public string Hints
        {
            get { return hints; }
            set { hints = value; }
        }

        /// <summary>
        /// 是否是子查詢(subquery)的表格描述.
        /// </summary>
        public bool IsSelectModel
        {
            get
            {
                return SelectModel != null;
            }
        }

        /// <summary>
        /// 子查詢(subquery)的 SelectModel.
        /// </summary>
        public SelectModel SelectModel
        {
            get
            {
                return _SelectModel;
            }
        }

        /// <summary>
        /// 表格名稱
        /// </summary>
        public string Name
        {
            get
            {
                return table;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsEmpty
        {
            get { return table == null && _SelectModel == null; }
        }

        /// <summary>
        /// Owner
        /// </summary>
        public string Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string Database
        {
            get
            {
                return db;
            }
        }

        /// <summary>
        /// 傳回表格全名 [database].[owner].[tablename]
        /// </summary>
        public string FullName
        {
            get
            {
                if (_SelectModel == null)
                {

                    if (string.IsNullOrEmpty(db) && string.IsNullOrEmpty(owner))
                        return SQLHelper.PackObjectName(table);
                    else if (string.IsNullOrEmpty(db))
                        return $"{SQLHelper.PackObjectName(owner)}.{SQLHelper.PackObjectName(table)}";
                    else
                        return $"{SQLHelper.PackObjectName(db)}.{SQLHelper.PackObjectName(owner)}.{SQLHelper.PackObjectName(table)}";
                }
                else
                    return $"({_SelectModel.CommandText})";
            }
        }
        /// <summary>
        /// 支援表格名稱強制轉換小寫字母 , (不含 db/owner )
        /// </summary>
        /// <param name="ForceLowerName"></param>
        /// <returns></returns>
        public string GetFullName(SqlOptions Options)
        {
            if (_SelectModel == null)
            {
                if (string.IsNullOrEmpty(db) && string.IsNullOrEmpty(owner))
                    return SQLHelper.PackObjectName(table, Options);
                else if (string.IsNullOrEmpty(db))
                    return $"{SQLHelper.PackObjectName(owner)}.{SQLHelper.PackObjectName(table, Options)}";
                else
                    return $"{SQLHelper.PackObjectName(db)}.{SQLHelper.PackObjectName(owner)}.{SQLHelper.PackObjectName(table, Options)}";
            }
            else
                return $"({_SelectModel.CommandText})"; /* SelectModel 會自行轉換, 不需要處理 */
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="full_name"></param>
        /// <returns></returns>
        public static implicit operator TableSource(string full_name)
        {
            if (string.IsNullOrEmpty(full_name))
            {
                return TableSource.Empty;
            }
            return new TableSource(full_name);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="object_name"></param>
        /// <returns></returns>
        public static implicit operator string(TableSource object_name)
        {
            return object_name.FullName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="object_name"></param>
        /// <returns></returns>
        public static implicit operator TableSource(TableName object_name)
        {
            return new TableSource(object_name);
        }

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FullName;
        }
    }
}