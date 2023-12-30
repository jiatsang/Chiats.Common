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
    /// �\�૬������ (Condition) {$Name:Scope  ColumnExpression,@ParameName }
    /// </summary>
    /// <remarks>
    /// �\�૬������.�]�t�F. ArgumentType �����w���U�إ\�઺�S��d�߱��󪺫��w
    /// �䤤�ѼƦW�٥u�|���@�� @ParameName
    /// </remarks>
    public class FunctionCondition : Condition
    {
        /// <summary>
        /// �O�_�ҥα��󪫥� , True:�ҥ� Flase:���ϥ� Null:�۰�
        /// </summary>
        private ArgumentType argumentType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string columnExpression;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string paramName;

        /// <summary>
        /// �\�૬������ (Condition) �Ѽƥu�|���@��
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NamedCollection<Parameter> Params = new NamedCollection<Parameter>();

        public string ColumnExpression
        {
            get
            {
                return columnExpression;
            }
        }

        /// <summary>
        /// �\�૬�ѼƱ���ҥ]�t���Ѽƪ���
        /// </summary>
        public Parameter Parameter
        {
            get
            {
                if (Parameters.Count > 0)
                    return Parameters[0];
                return null;
            }
        }

        /// <summary>
        /// �\�૬�ѼƱ���ҥ]�t���ѼƦW��
        /// </summary>
        public string ParamName
        {
            get
            {
                return paramName;
            }
        }

        public ArgumentType ArgumentType
        {
            get
            {
                return argumentType;
            }
        }

        /// <summary>
        /// �\�૬����Ѽ�.
        /// </summary>
        /// <param name="Conditions"></param>
        /// <param name="name"></param>
        /// <param name="ArgumentType"></param>
        /// <param name="columnExpression"></param>
        /// <param name="paramName"></param>
        /// <param name="uplink"></param>
        /// <param name="export"></param>
        public FunctionCondition(Conditions Conditions, string name, ArgumentType ArgumentType,
            string columnExpression,
            string paramName,
            ConditionLink uplink,
            bool export)
            : base(Conditions, name, uplink, export)
        {
            this.argumentType = ArgumentType;
            this.columnExpression = columnExpression;
            this.paramName = paramName;

            Params.Add(new Parameter(paramName, ArgumentType));
            this.initializeFinished();
        }

        /// <summary>
        /// ���ܱ���(Condition) �O�_���@�ӪŪ�����(Condition)
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return Params.Count == 0 || Params[0].IsClear; // �Ѽƥu�|���@��
            }
        }

        /// <summary>
        /// �^�Ǳ���(Condition) �ҥ]�t���Ѽƶ��X. �Ѽƥu�|���@�� �ҥH Parameters.Count �û��� 1
        /// </summary>
        public override NamedCollection<Parameter> Parameters
        {
            get { return Params; }
        }

        /// <summary>
        /// �^�Ǳ��� (Condition) SQLCTL ��r���e.
        /// </summary>
        public override string ConditionSource
        {
            get
            {
                return ConditionExpression(CommandBuildType.SQLCTL, ParameterMode.Parameter, null);
            }
        }

        /// <summary>
        /// �^�Ǳ��� (Condition).�ѪR�� SQL/SQLCTL ��r���e
        /// </summary>
        public override string ConditionExpression(CommandBuildType BuildType, ParameterMode mode, ExportParameter Exporter )
        {
            if (BuildType == CommandBuildType.SQLCTL)
            {
                if (!string.IsNullOrEmpty(this.Name))
                    return string.Format("{{${0}:{1} {2},{3}}}", this.Name, this.argumentType, this.ColumnExpression, this.ParamName);
                else
                    return string.Format("{{$:{0} {1},{2}}}", this.argumentType, this.ColumnExpression, this.ParamName);
            }
            // �զX�d���/�h�ȼg�k
            // this.funcName
            if (Exporter != null)
            {
                CommandBuilder CommandBuilder = new CommandBuilder();
                Exporter(
                    CommandBuilder,
                    new ExportParameterEventArgs(
                        ParamName,
                        argumentType,
                        ColumnExpression,
                        BuildType,
                        mode,
                        Params[0]));

                return CommandBuilder.ToString();
            }
            return ColumnExpression; // columnExpression.RebuildExpression(BuildType, mode, Exporter);
        }

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ConditionSource))
                return "";

            if (Name == null)
                return string.Format("Expression=[{0}],(Condition={1})", ConditionSource, Link);
            else
                return string.Format("${0} Expression=[{1}],Condition={2}", Name, ConditionSource, Link);
        }
    }
}