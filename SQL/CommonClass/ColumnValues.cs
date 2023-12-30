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
    /// ���W�٩M���Ȫ����X�w�q ,�q�`�Ω� Update/Insert �ԭz
    /// </summary>
    public class ColumnValues : CommonList<ColumnValue>, IPartSqlModel
    {
        /// <summary>
        /// ����B�⦡ �s�W�ƥ�/�s�W�ƥ�/�ܧ�ƥ�
        /// </summary>
        public event EventHandler<ChangedEventArgs<ColumnValue>> ColumnValueChanged;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SqlModel parent;

        /// <summary>
        /// ���W�٩M���Ȫ����X�w�q�غc�l.
        /// </summary>
        /// <param name="parent"></param>
        public ColumnValues(SqlModel parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// �Ǧ^�̤W�@�h�� CommonModel ��������
        /// </summary>
        /// <returns>�̤W�@�h�� CommonModel ��������</returns>
        public SqlModel GetTopModel()
        {
            return parent.GetTopModel();
        }

        /// <summary>
        /// �s�[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="name">�W��</param>
        /// <param name="ExpressionValue">Expression Value</param>
        public void AddExpression(string name, string ExpressionValue)
        {
            ColumnValue columnValue = new ColumnValue(this, name, null, ExpressionValue);

            this.BaseAdd(columnValue);
            columnValue.ColumnValueChanged += new EventHandler<ChangedEventArgs<ColumnValue>>(ColumnValue_ColumnValueChanged);

            if (ColumnValueChanged != null)
                ColumnValueChanged(this, new ChangedEventArgs<ColumnValue>(ChangedEventType.Add, columnValue));
        }
        /// <summary>
        /// �۰ʱa�ܼƪ�  Ex Insert into ....(col) values {@col} , ParameterValue �۰ʥ[�J Parameters  �Ѽƪ�.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="ParameterValue"></param>
        public void AddColumnParameter(string columnName, object ParameterValue = null)
        {
            var paramColumnName = $"@{columnName}";

            ColumnValue columnValue = new ColumnValue(this, columnName, null, paramColumnName);
            this.Add(new ColumnValue(this, columnName, null, paramColumnName));
            if (ParameterValue != null)
            {
                this.GetTopModel().Parameters[paramColumnName].Value = ParameterValue;
            }

            columnValue.ColumnValueChanged += new EventHandler<ChangedEventArgs<ColumnValue>>(ColumnValue_ColumnValueChanged);

            if (ColumnValueChanged != null)
                ColumnValueChanged(this, new ChangedEventArgs<ColumnValue>(ChangedEventType.Add, columnValue));
        }

        public void AddColumnParameter(object Parameters, Func<string, object, bool> IsAdd = null)
        {
            if (Parameters != null)
            {
                foreach (var p in Parameters.GetType().GetProperties())
                {
                    var _val = p.GetValue(Parameters, null);
                    if (IsAdd != null)
                        if (!IsAdd(p.Name, _val)) continue;
                    AddColumnParameter(p.Name, _val);
                }
            }
        }


        public void Clear()
        {
            this.BaseClear();
        }
        private void ColumnValue_ColumnValueChanged(object sender, ChangedEventArgs<ColumnValue> e)
        {
            if (ColumnValueChanged != null) ColumnValueChanged(this, e);
        }

        /// <summary>
        /// �[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="columnValue"></param>
        protected void Add(ColumnValue columnValue)
        {
            this.BaseAdd(columnValue);
            columnValue.ColumnValueChanged += new EventHandler<ChangedEventArgs<ColumnValue>>(ColumnValue_ColumnValueChanged);

            if (ColumnValueChanged != null)
                ColumnValueChanged(this, new ChangedEventArgs<ColumnValue>(ChangedEventType.Add, columnValue));
        }

        /// <summary>
        ///  Add ���W�٦����a��, �@�� Insert into .... select  ... �y�k����  See. Test_InsertModel_008()
        /// </summary>
        /// <param name="columnName"></param>
        /// <remarks>
        ///  �ݭn�۰ʱa�ܼƪ�  Ex Insert into ....(col) values {@col}  see. AddParameterValue
        /// </remarks>
        public void Add(string columnName)
        {
            if (columnName != null)
            {
                string _col = columnName.Trim();
                if (_col != "" && !this.Contains(_col))
                {
                    Add(new ColumnValue(this, _col, null, null));
                }
            }
        }

        public void AddRange(string columnNames)
        {
            string[] Columns = columnNames.Split(',');
            foreach (var col in Columns)
            {
                string _col = col.Trim();
                if (_col != "" && !this.Contains(_col))
                    Add(new ColumnValue(this, _col, null, $"@{_col}"));
            }
        }

    

        /// <summary>
        ///
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="constantValue"></param>
        public void Add(string columnName, object constantValue)
        {
            string expressionValue = null;
            if (constantValue is DateTime)
            {
                DateTime _val = (DateTime)constantValue;
                if (_val != DateTime.MinValue)
                {
                    constantValue = _val.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    constantValue = null;
                    expressionValue = "null"; // set dbnull
                }
            }
            Add(new ColumnValue(this, columnName, constantValue.ChangeType<string>(), expressionValue));
        }

        public bool Contains(string Name)
        {
            return this.BaseContains(Name);
        }

        /// <summary>
        /// �[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="columnName">�W��</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">���`�ƭ�</param>
        public void Add(string columnName, ColumnType columnType, string constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue, null));
        }

        /// <summary>
        /// �[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="columnName">�W��</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">���`�ƭ�</param>
        public void Add(string columnName, ColumnType columnType, int constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue.ToString(), null));
        }

        /// <summary>
        /// �[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="columnName">�W��</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">���`�ƭ�</param>
        public void Add(string columnName, ColumnType columnType, long constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue.ToString(), null));
        }

        /// <summary>
        /// �[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="columnName">�W��</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">���`�ƭ�</param>
        public void Add(string columnName, ColumnType columnType, decimal constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue.ToString(), null));
        }

        /// <summary>
        /// �[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="columnName">���W��</param>
        /// <param name="constantValue">���`�ƭ�, �p�G�Ǧ^�D null ��.�h��� ColumnValue �����Ȭ� �`�ƭ� �Ϋh�����B�⦡</param>
        /// <param name="expressionValue">���B�⦡</param>
        public void Add(string columnName, string constantValue, string expressionValue)
        {
            Add(new ColumnValue(this, columnName, constantValue, expressionValue));
        }

        /// <summary>
        /// �[�J�@�� ColumnValue ����
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="columnType"></param>
        /// <param name="constantValue"></param>
        /// <param name="expressionValue"></param>
        public void Add(string columnName, ColumnType columnType, string constantValue, string expressionValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue, expressionValue));
        }

        /// <summary>
        /// �����@�� ColumnValue ����
        /// </summary>
        /// <param name="columnValue"></param>
        public void Remove(ColumnValue columnValue)
        {
            if (this.BaseContains(columnValue))
            {
                columnValue.Expression = null;
                columnValue.ColumnValueChanged -= new EventHandler<ChangedEventArgs<ColumnValue>>(ColumnValue_ColumnValueChanged);
                this.BaseRemove(columnValue);
                if (ColumnValueChanged != null)
                    ColumnValueChanged(this, new ChangedEventArgs<ColumnValue>(ChangedEventType.Removed, columnValue));
            }
        }

        /// <summary>
        /// �����@�� ColumnValue ����
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            ColumnValue columnValue = this[name];
            if (columnValue != null)
            {
                columnValue.Expression = null;
                columnValue.ColumnValueChanged -= new EventHandler<ChangedEventArgs<ColumnValue>>(ColumnValue_ColumnValueChanged);
                this.BaseRemove(columnValue);
                if (ColumnValueChanged != null)
                    ColumnValueChanged(this, new ChangedEventArgs<ColumnValue>(ChangedEventType.Removed, columnValue));
            }
        }
    }
}