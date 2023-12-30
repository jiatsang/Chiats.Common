// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chiats.SQL
{
    /// <summary>
    /// SqlModel ������  Update �y�k����.
    /// </summary>
    public class UpdateModel : SqlModel, ISqlConditions
    {
        /// <summary>
        /// ���W�� ���W�ٮ榡�p�U [database].[owner].[tablename]
        /// </summary>
        public TableName Table;

        /// <summary>
        /// �䴩�з� Update �y�k [ FROM { table_source } [ ,...n ] ]
        /// </summary>
        public TableClause Tables { get; internal set; }       /*[ FROM { < table_source > } [ ,...n ] ]*/

        /// <summary>
        /// �䴩�з� Update �y�k [ WHERE search_condition ]
        /// </summary>
        public WhereClause Where { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ModelConditions conditions;

        /// <summary>
        /// Update �y�k���� Set �����w�q�μg�J��.
        /// </summary>
        public ColumnValues Columns { get; private set; }

        /// <summary>
        ///  UpdateModel ��¦�غc�l
        /// </summary>
        public UpdateModel()
        {
            Where = new WhereClause(this);
            Columns = new ColumnValues(this);
            conditions = new UpdateModel.ModelConditions(this);
            Where.WhereConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(Where_WhereConditionChanged);
            Columns.ColumnValueChanged += new EventHandler<ChangedEventArgs<ColumnValue>>(Columns_ColumnValueChanged);
        }

        /// <summary>
        /// UpdateModel �غc�l
        /// </summary>
        /// <param name="CTLSQL">���]�t�ťզr�����W�ٵ������W��</param>
        /// <param name="parameters">CTLSQL �����W�ٮ� , parameters �h�����n��s�����W�٤Ωw�q. �_�h���Ѽ�</param>
        /// <param name="condition">CTLSQL �����W�ٮ� , ���a�� SQL ����</param>
        /// <param name="conditionParameters">CTLSQL �����W�ٮ� ,�B���a���� SQL ����, �|�۰ʱa�J�ѼƩw�q, ���M parameters �ۦP�����|������s����� </param>
        public UpdateModel(string CTLSQL, object parameters = null, string condition = null, object conditionParameters = null)
            : this()
        {
            // ctlsql ������@���W�ٵ��� SQL Command, 
            // TODO : �H [] �����W��, �i��t���ťզr��
            if (CTLSQL.Trim().IndexOf('\x20') != -1)
            {
                SqlModel.Parse<UpdateModel>(CTLSQL, this);
                if (parameters != null)
                {
                    Parameters.Fill(parameters);
                    //foreach (var p in parameters.GetType().GetProperties())
                    //{
                    //    var p_name = $"@{p.Name}";
                    //    if (Parameters.Contains(p_name))
                    //        this.Parameters[p_name].Value = p.GetValue(parameters, null);
                    //    else
                    //        throw new CommonException($"���w�q�� Parameter Name {p_name} in {nameof(parameters)}");
                    //}
                }
            }
            else
            {
                Table = CTLSQL.Trim();  // ���t�ťժ� CTLSQL �i�������W��
                this.Columns.AddColumnParameter(parameters);  /* CTLSQL �����W�ٮ� , parameters �h�����n�s�W�����W�٤Ωw�q */
            }

            if (condition != null)
            {
                Where.Add(condition);
                if (conditionParameters != null)
                {
                    Parameters.Fill(conditionParameters);
                    //foreach (var p in conditionParameters.GetType().GetProperties())
                    //{
                    //    var p_name = $"@{p.Name}";
                    //    if (Parameters.Contains(p_name))
                    //        this.Parameters[p_name].Value = p.GetValue(conditionParameters, null);
                    //    else
                    //        throw new CommonException($"���w�q�� Parameter Name {p_name} in {nameof(conditionParameters)}");
                    //}
                }
            }
        }

        /// <summary>
        /// �^�� UpdateModel ���󪫥󶰦X
        /// </summary>
        public UpdateModel.ModelConditions Conditions
        {
            get { return conditions; }
        }

        IConditionModels ISqlConditions.Conditions
        {
            get { return conditions; }
        }

        private void Columns_ColumnValueChanged(object sender, ChangedEventArgs<ColumnValue> e)
        {
            this.Changed();
        }

        private void Where_WhereConditionChanged(object sender, ChangedEventArgs<Condition> e)
        {
            switch (e.ChangedEventType)
            {
                case ChangedEventType.Add:
                    this.CoverParameters.AddLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Removed:
                    this.CoverParameters.RemoveLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Replace:
                    this.CoverParameters.RemoveLinker(e.ReplaceObject.Parameters);
                    this.CoverParameters.AddLinker(e.ChangedObject.Parameters);
                    break;

                case ChangedEventType.Changed:
                    break;
            }
            this.Changed();
        }

        /// <summary>
        /// UpdateModel Condition ���X���޲z���O
        /// </summary>
        public class ModelConditions : CommonList<IConditionModel>, IConditionModels
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private UpdateModel update;

            /// <summary>
            /// �����ܧ�ƥ�
            /// </summary>
            public event EventHandler<ChangedEventArgs<IConditionModel>> ConditionChanged;

            internal ModelConditions(UpdateModel update)
            {
                this.update = update;
                this.update.Where.WhereConditionChanged += new EventHandler<ChangedEventArgs<Condition>>(WhereConditionChanged);
            }

            /// <summary>
            /// �d�߫��w�ѼƩ��ݪ����� (Condition) �W�� , �P���ݩ�h�ӱ��󦡥H ',' �Ϲj
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public string QueyBelongCondition(string name)
            {
                string condition_names = null;
                foreach (var Condition in update.Conditions)
                {
                    if (Condition.BelongParameter(name))
                    {
                        if (condition_names != null)
                            condition_names += "," + Condition.Name;
                        else
                            condition_names += Condition.Name;
                    }
                }
                return condition_names;
            }

            /// <summary>
            /// �d�߫��w�ѼƩ��ݪ����� (Condition) �W�� , �P���ݩ�h�ӱ��󦡥H ',' �Ϲj
            /// </summary>
            /// <param name="Parameter"></param>
            /// <returns></returns>
            public string QueyBelongCondition(Parameter Parameter)
            {
                string condition_names = null;
                foreach (var Condition in update.Conditions)
                {
                    if (Condition.BelongParameter(Parameter))
                    {
                        if (condition_names != null)
                            condition_names += "," + Condition.Name;
                        else
                            condition_names += Condition.Name;
                    }
                }
                return condition_names;
            }

            private void WhereConditionChanged(object sender, ChangedEventArgs<Condition> e)
            {
                update.Changed();

                if (e.ChangedObject.Name == null) return; // �����ΦW(Anonymous) Condition
                ModelCondition current = (ModelCondition)this[e.ChangedObject.Name];
                switch (e.ChangedEventType)
                {
                    case ChangedEventType.Add:
                        if (current == null)
                        {
                            current = new ModelCondition(e.ChangedObject.Name);
                            this.BaseAdd(current);
                            current.Where = e.ChangedObject;
                        }
                        else
                        {
                            if (current.Where == e.ChangedObject) break; /* �p�G�[�J Condition �O�ۦP�ɫh����*/
                            if (!current.Where.IsEmpty)
                                throw new SqlModelSyntaxException("���Х[�J Condition - {0}", e.ChangedObject.Name);

                            current.Where = e.ChangedObject;
                        }
                        break;

                    case ChangedEventType.Removed: BaseRemove(current); break;
                    case ChangedEventType.Replace: BaseReplace(current); break;
                }
                if (ConditionChanged != null)
                    ConditionChanged(sender, new ChangedEventArgs<IConditionModel>(e.ChangedEventType, current));
            }

            /// <summary>
            /// �^�ǬO�_�]�t�Ӫ���
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool Contains(string name)
            {
                return this.BaseContains(name);
            }

            /// <summary>
            /// �^�ǬO�_�]�t�Ӫ���
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(ModelCondition item)
            {
                return this.BaseContains(item);
            }

            /// <summary>
            /// ���o���w��m�����󦡪���.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public new UpdateModel.ModelCondition this[int index]
            {
                get { return (UpdateModel.ModelCondition)base[index]; }
            }

            /// <summary>
            /// ���o���w�W�٤����󦡪���.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public new UpdateModel.ModelCondition this[string name]
            {
                get { return (UpdateModel.ModelCondition)base[name]; }
            }

            /// <summary>
            /// String�A��ܥثe�� Object�C
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("Conditions(Count={0})", this.Count);
            }

            public FunctionCondition[] GetFunctionConditions(ConditionFlags Flags)
            {
                List<FunctionCondition> list = new List<FunctionCondition>();
                // TODO:
                return list.ToArray();
            }
        }

        /// <summary>
        ///  UpdateModel Condition �޲z���O
        /// </summary>
        public class ModelCondition : IConditionModel
        {
            /// <summary>
            /// UpdateModel Condition �޲z���O�غc�l
            /// </summary>
            /// <param name="name"></param>
            public ModelCondition(string name) { this.name = name; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string name = null;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Condition Condition = null;

            /// <summary>
            /// ���w�ѼƬO�_���ݩ󪺥ثe������ (Condition)
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool BelongParameter(string name)
            {
                return Condition.BelongParameter(name);
            }

            /// <summary>
            /// ���w�ѼƬO�_���ݩ󪺥ثe������ (Condition)
            /// </summary>
            /// <param name="Parameter"></param>
            /// <returns></returns>
            public bool BelongParameter(Parameter Parameter)
            {
                return Condition.BelongParameter(Parameter);
            }

            /// <summary>
            /// �O�_�ҥα��󪫥� , True:�ҥ� Flase:���ϥ� Null:�۰�
            /// </summary>
            public bool Enabled
            {
                get
                {
                    return Condition.Enabled;
                }
                set
                {
                    Condition.Enabled = value;
                }
            }

            #region INamed Members

            /// <summary>
            /// Condition ����W��
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            /// <summary>
            /// Where Condition ���󤺮e
            /// </summary>
            public Condition Where
            {
                get { return Condition; }
                internal set
                {
                    Condition = value;
                }
            }

            #endregion INamed Members

            /// <summary>
            /// String�A��ܥثe�� Object�C
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Condition == null)
                    return "Condition : {EMPTY}";
                else
                    return string.Format("Condition : {0}", Name);
            }
        }

        /// <summary>
        /// �� BuildType ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        /// <param name="BuildType">���ܫإ� SQL �R�O����r�ԭz���覡</param>
        /// <param name="formatFlags"></param>
        /// <returns>SQL �R�O����r�ԭz</returns>
        protected override string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions formatFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null)
        {
            ISqlBuildExport BuildExport = GetBuildExport(BuildType, formatFlags, buildExport);

            CommandBuilder sb = new CommandBuilder(formatFlags);
            sb.AppendKeywordToken("UPDATE");
            sb.AppendToken(Table.FullName);
            sb.AppendKeywordToken("SET");

            int col_count = 0;
            foreach (ColumnValue cv in Columns)
            {
                if (col_count++ != 0)
                    sb.AppendToken(",");
                BuildExport.ExportForSetValues(cv, sb, this);
            }

            if (this.Tables != null)
            {
                sb.AppendToken("FROM");
                BuildExport.ExportForTableClause(Tables, sb, this);
            }

            BuildExport.ExportForWhereClause(Where, sb, this);
            return sb.ToString();
        }
    }
}