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
    /// 欄位名稱和欄位值的集合定義 ,通常用於 Update/Insert 敘述
    /// </summary>
    public class ColumnValues : CommonList<ColumnValue>, IPartSqlModel
    {
        /// <summary>
        /// 條件運算式 新增事件/新增事件/變更事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<ColumnValue>> ColumnValueChanged;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SqlModel parent;

        /// <summary>
        /// 欄位名稱和欄位值的集合定義建構子.
        /// </summary>
        /// <param name="parent"></param>
        public ColumnValues(SqlModel parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// 傳回最上一層的 CommonModel 父階物件
        /// </summary>
        /// <returns>最上一層的 CommonModel 父階物件</returns>
        public SqlModel GetTopModel()
        {
            return parent.GetTopModel();
        }

        /// <summary>
        /// 新加入一個 ColumnValue 物件
        /// </summary>
        /// <param name="name">名稱</param>
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
        /// 自動帶變數者  Ex Insert into ....(col) values {@col} , ParameterValue 自動加入 Parameters  參數表中.
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
        /// 加入一個 ColumnValue 物件
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
        ///  Add 欄位名稱但不帶值, 作為 Insert into .... select  ... 語法之用  See. Test_InsertModel_008()
        /// </summary>
        /// <param name="columnName"></param>
        /// <remarks>
        ///  需要自動帶變數者  Ex Insert into ....(col) values {@col}  see. AddParameterValue
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
        /// 加入一個 ColumnValue 物件
        /// </summary>
        /// <param name="columnName">名稱</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">欄位常數值</param>
        public void Add(string columnName, ColumnType columnType, string constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue, null));
        }

        /// <summary>
        /// 加入一個 ColumnValue 物件
        /// </summary>
        /// <param name="columnName">名稱</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">欄位常數值</param>
        public void Add(string columnName, ColumnType columnType, int constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue.ToString(), null));
        }

        /// <summary>
        /// 加入一個 ColumnValue 物件
        /// </summary>
        /// <param name="columnName">名稱</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">欄位常數值</param>
        public void Add(string columnName, ColumnType columnType, long constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue.ToString(), null));
        }

        /// <summary>
        /// 加入一個 ColumnValue 物件
        /// </summary>
        /// <param name="columnName">名稱</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">欄位常數值</param>
        public void Add(string columnName, ColumnType columnType, decimal constantValue)
        {
            Add(new ColumnValue(this, columnName, columnType, constantValue.ToString(), null));
        }

        /// <summary>
        /// 加入一個 ColumnValue 物件
        /// </summary>
        /// <param name="columnName">欄位名稱</param>
        /// <param name="constantValue">欄位常數值, 如果傳回非 null 值.則表示 ColumnValue 的欄位值為 常數值 或則為欄位運算式</param>
        /// <param name="expressionValue">欄位運算式</param>
        public void Add(string columnName, string constantValue, string expressionValue)
        {
            Add(new ColumnValue(this, columnName, constantValue, expressionValue));
        }

        /// <summary>
        /// 加入一個 ColumnValue 物件
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
        /// 移除一個 ColumnValue 物件
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
        /// 移除一個 ColumnValue 物件
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