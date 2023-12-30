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
    /// SQL 表格名稱物件類別, 表格名稱格式如下 [database].[owner].[tablename]
    /// </summary>
    public struct TableName
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string owner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string table;

        /// <summary>
        ///
        /// </summary>
        public static TableName Empty = new TableName(null, null, null);

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="db">資料庫名稱</param>
        /// <param name="owner">Owner</param>
        /// <param name="table">表格名稱</param>
        public TableName(string db, string owner, string table)
        {
            this.db = SQLHelper.UnpackObjectName(db);
            this.owner = SQLHelper.UnpackObjectName(owner);
            this.table = SQLHelper.UnpackObjectName(table);
        }

        internal TableName(StringObjectName k123)
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
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="fullname">資料庫名稱 [database].[owner].[tablename]</param>
        public TableName(string fullname)
        {
            SQLTokenScanner list = new SQLTokenScanner(fullname);
            db = owner = table = null;

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
                    // throw

                    break;
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
            get { return table == null; }
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
                if (string.IsNullOrEmpty(db) && string.IsNullOrEmpty(owner))
                    return SQLHelper.PackObjectName(table);
                else if (string.IsNullOrEmpty(db))
                    return string.Format("{0}.{1}", SQLHelper.PackObjectName(owner), SQLHelper.PackObjectName(table));
                else
                    return string.Format("{0}.{1}.{2}", SQLHelper.PackObjectName(db), SQLHelper.PackObjectName(owner), SQLHelper.PackObjectName(table));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="full_name"></param>
        /// <returns></returns>
        public static implicit operator TableName(string full_name)
        {
            if (string.IsNullOrEmpty(full_name))
            {
                return TableName.Empty;
            }
            return new TableName(full_name);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="object_name"></param>
        /// <returns></returns>
        public static implicit operator string(TableName object_name)
        {
            return object_name.FullName;
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