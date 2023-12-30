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
    /// SQL ���W�٪������O�άO SQL Select Command, ���W�ٮ榡�p�U [database].[owner].[tablename]
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

#pragma warning disable CS0414 // �w������� 'TableSource.DataChanged'�A���q���ϥιL��ȡC
        /// <summary>
        /// TableJoin �ܧ�ƥ�q��
        /// </summary>
        public event EventHandler DataChanged;
#pragma warning restore CS0414 // �w������� 'TableSource.DataChanged'�A���q���ϥιL��ȡC

        /// <summary>
        ///
        /// </summary>
        public static TableSource Empty = new TableSource(null, null, null);

        /// <summary>
        /// �غc�l
        /// </summary>
        /// <param name="db">��Ʈw�W��</param>
        /// <param name="owner">Owner</param>
        /// <param name="table">���W��</param>
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
        /// �غc�l
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
        /// �غc�l
        /// </summary>
        /// <param name="fullname">��Ʈw�W�� [database].[owner].[tablename]</param>
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
        /// �غc�l
        /// </summary>
        /// <param name="table"></param>
        public TableSource(TableName table) : this(table.Database, table.Owner, table.Name) { }

        private string hints;

        /// <summary>
        /// �u�� with () �����r��s�� Select.Tables.Hints ,(�ثe�H�B�z SQL Server ���D)
        /// ���z�|�䤺�e �o�����i��ݹ藍�P��Ʈw���@�k�i����R��, �~��i��ӳ��]�p
        /// </summary>
        public string Hints
        {
            get { return hints; }
            set { hints = value; }
        }

        /// <summary>
        /// �O�_�O�l�d��(subquery)�����y�z.
        /// </summary>
        public bool IsSelectModel
        {
            get
            {
                return SelectModel != null;
            }
        }

        /// <summary>
        /// �l�d��(subquery)�� SelectModel.
        /// </summary>
        public SelectModel SelectModel
        {
            get
            {
                return _SelectModel;
            }
        }

        /// <summary>
        /// ���W��
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
        /// ��Ʈw�W��
        /// </summary>
        public string Database
        {
            get
            {
                return db;
            }
        }

        /// <summary>
        /// �Ǧ^�����W [database].[owner].[tablename]
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
        /// �䴩���W�ٱj���ഫ�p�g�r�� , (���t db/owner )
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
                return $"({_SelectModel.CommandText})"; /* SelectModel �|�ۦ��ഫ, ���ݭn�B�z */
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
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FullName;
        }
    }
}