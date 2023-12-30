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
    /// 欄位名稱和欄位值的定義, 欄位值可以是欄位常數值, 字串或是欄位值運算式.用於 Update/Insert 敘述
    /// </summary>
    public class ColumnValue : IVariantName, IPartSqlModel
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ColumnType columnType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string columnName;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string constantValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string expressionValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NonCompleteExpression expression;

        /// <summary>
        /// 條件運算式 新增事件/新增事件/變更事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<ColumnValue>> ColumnValueChanged;

        /// <summary>
        /// 指示 ColumnValue 是依附在那一個 ColumnValue 集合物件下.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ColumnValues Parent = null;

        /// <summary>
        /// ColumnValue 的建構子. 欄位常數值, 字串 或是欄位運算式只有同時存在一種.
        /// </summary>
        /// <param name="Parent">父物件</param>
        /// <param name="columnName">欄位名稱</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">欄位常數值, 如果傳回非 null 值.則表示 ColumnValue 的欄位值為 常數值 或則為欄位運算式</param>
        /// <param name="expressionValue">欄位運算式</param>
        internal ColumnValue(ColumnValues Parent, string columnName, ColumnType columnType, string constantValue, string expressionValue)
        {
            this.Parent = Parent;
            this.columnName = columnName;
            this.columnType = columnType;
            if (constantValue != null)
            {
                this.constantValue = SplitConstantValue(constantValue);
            }
            else if (expressionValue != null)
            {
                this.expressionValue = expressionValue;
                this.expression = new NonCompleteExpression(this, expressionValue);
                this.GetTopModel().CoverParameters.AddLinker(expression.Parameters);
            }
        }

        /// <summary>
        /// ColumnValue 的建構子. 欄位常數值, 字串 或是欄位運算式只有同時存在一種.
        /// </summary>
        /// <param name="Parent">父物件</param>
        /// <param name="columnName">欄位名稱</param>
        /// <param name="constantValue">欄位常數值, 如果傳回非 null 值.則表示 ColumnValue 的欄位值為 常數值 或則為欄位運算式</param>
        /// <param name="expressionValue">欄位運算式</param>
        internal ColumnValue(ColumnValues Parent, string columnName, string constantValue, string expressionValue)
            : this(Parent, columnName, ColumnType.Auto, constantValue, expressionValue)
        {
        }

        private string SplitConstantValue(string constantValue)
        {
            if (constantValue != null && constantValue.Length > 2)
            {
                if (constantValue.StartsWith("'") && constantValue.EndsWith("'"))
                {
                    constantValue = constantValue.Substring(1, constantValue.Length - 2); // 去除單引號
                }
                else if (constantValue.StartsWith("N'") && constantValue.EndsWith("'"))
                {
                    constantValue = constantValue.Substring(2, constantValue.Length - 3); // Support N'....' 去除單引號
                }
            }
            return constantValue;
        }

        /// <summary>
        ///
        /// </summary>
        public ColumnType ColumnType
        {
            get
            {
                return columnType;
            }
            set
            {
                columnType = value;
            }
        }

        /// <summary>
        /// 欄位常數值. 重設欄位常數值時, 如果己存在 欄位運算式 則欄位運算式的內容會被清空.
        /// </summary>
        public string Value
        {
            get
            {
                return constantValue;
            }
            set
            {
                lock (this)
                {
                    if (constantValue != value)
                    {
                        constantValue = SplitConstantValue(value);
                        expressionValue = null;
                        if (expression != null)
                        {
                            GetTopModel().CoverParameters.RemoveLinker(expression.Parameters);
                            expression = null;
                        }
                        if (ColumnValueChanged != null)
                            ColumnValueChanged(this, new ChangedEventArgs<ColumnValue>(ChangedEventType.Changed, this));
                    }
                }
            }
        }

        internal NamedCollection<Parameter> Parameters
        {
            get { return (expression != null) ? expression.Parameters : null; }
        }

        /// <summary>
        /// 重組 欄位運算式.
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="pMode"></param>
        /// <param name="Exporter"></param>
        /// <returns></returns>
        public string RebuildExpression(CommandBuildType BuildType, ParameterMode pMode, ExportParameter Exporter, SqlOptions sqlOptions)
        {
            return expression.RebuildExpression(BuildType, pMode, Exporter, GetTopModel().Options);
        }

        /// <summary>
        /// 欄位運算式 重設欄位運算式時, 如果己存在 欄位常數值 則欄位常數值的內容會被清空.
        /// </summary>
        public string Expression
        {
            get
            {
                return expressionValue;
            }
            set
            {
                lock (this)
                {
                    if (expressionValue != value)
                    {
                        if (expression != null)
                        {
                            GetTopModel().CoverParameters.RemoveLinker(expression.Parameters);
                            expression = null;
                        }
                        expressionValue = value;
                        expression = new NonCompleteExpression(this, expressionValue);
                        GetTopModel().CoverParameters.AddLinker(expression.Parameters);
                        constantValue = null;
                        ColumnValueChanged?.Invoke(this, new ChangedEventArgs<ColumnValue>(ChangedEventType.Changed, this));
                    }
                }
            }
        }

        #region INamed Members

        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Name
        {
            get { return columnName; }
        }

        #endregion INamed Members

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

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (constantValue != null)
            {
                if (GetTopModel().Options.HasFlag(SqlOptions.NonStringNational))
                    return string.Format("{0}='{1}'", this.Name, this.constantValue);
                return string.Format("{0}=N'{1}'", this.Name, this.constantValue);
            }
            return string.Format("{0}={1}", this.Name, this.expression);
        }
    }
}