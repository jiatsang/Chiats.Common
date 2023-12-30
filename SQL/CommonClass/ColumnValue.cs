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
    /// ���W�٩M���Ȫ��w�q, ���ȥi�H�O���`�ƭ�, �r��άO���ȹB�⦡.�Ω� Update/Insert �ԭz
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
        /// ����B�⦡ �s�W�ƥ�/�s�W�ƥ�/�ܧ�ƥ�
        /// </summary>
        public event EventHandler<ChangedEventArgs<ColumnValue>> ColumnValueChanged;

        /// <summary>
        /// ���� ColumnValue �O�̪��b���@�� ColumnValue ���X����U.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ColumnValues Parent = null;

        /// <summary>
        /// ColumnValue ���غc�l. ���`�ƭ�, �r�� �άO���B�⦡�u���P�ɦs�b�@��.
        /// </summary>
        /// <param name="Parent">������</param>
        /// <param name="columnName">���W��</param>
        /// <param name="columnType"></param>
        /// <param name="constantValue">���`�ƭ�, �p�G�Ǧ^�D null ��.�h��� ColumnValue �����Ȭ� �`�ƭ� �Ϋh�����B�⦡</param>
        /// <param name="expressionValue">���B�⦡</param>
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
        /// ColumnValue ���غc�l. ���`�ƭ�, �r�� �άO���B�⦡�u���P�ɦs�b�@��.
        /// </summary>
        /// <param name="Parent">������</param>
        /// <param name="columnName">���W��</param>
        /// <param name="constantValue">���`�ƭ�, �p�G�Ǧ^�D null ��.�h��� ColumnValue �����Ȭ� �`�ƭ� �Ϋh�����B�⦡</param>
        /// <param name="expressionValue">���B�⦡</param>
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
                    constantValue = constantValue.Substring(1, constantValue.Length - 2); // �h����޸�
                }
                else if (constantValue.StartsWith("N'") && constantValue.EndsWith("'"))
                {
                    constantValue = constantValue.Substring(2, constantValue.Length - 3); // Support N'....' �h����޸�
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
        /// ���`�ƭ�. ���]���`�ƭȮ�, �p�G�v�s�b ���B�⦡ �h���B�⦡�����e�|�Q�M��.
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
        /// ���� ���B�⦡.
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
        /// ���B�⦡ ���]���B�⦡��, �p�G�v�s�b ���`�ƭ� �h���`�ƭȪ����e�|�Q�M��.
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
        /// ���W��
        /// </summary>
        public string Name
        {
            get { return columnName; }
        }

        #endregion INamed Members

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

        /// <summary>
        /// String�A��ܥثe�� Object�C
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