// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// SqlModel ������ Condition �y�k����. ���]�t�b WhereClause �y�k����. &lt; &gt;
    /// Condition �䴩����M�ΦW����.
    /// </summary>
    public class ExpressionCondition : Condition
    {
        /// <summary>
        /// �O�_�ҥα��󪫥� , True:�ҥ� Flase:���ϥ� Null:�۰�
        /// </summary>
        private NonCompleteExpression expression;

        /// <summary>
        ///  Condition �y�k����غc�l
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="name"></param>
        /// <param name="Expression"></param>
        /// <param name="uplink"></param>
        /// <param name="export"></param>
        public ExpressionCondition(Conditions Parent, string name, string Expression, ConditionLink uplink, bool export)
            : base(Parent, name, uplink, export)
        {
            if (!string.IsNullOrEmpty(Expression))
            {
                this.expression = new NonCompleteExpression(this, Expression);
            }
            this.initializeFinished();
        }

        /// <summary>
        ///  Condition �y�k����غc�l
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="name"></param>
        /// <param name="Expression"></param>
        /// <param name="uplink"></param>
        public ExpressionCondition(Conditions Parent, string name, string Expression, ConditionLink uplink)
            : this(Parent, name, Expression, uplink, true)
        {
            this.initializeFinished();
        }

        /// <summary>
        /// Condition �y�k����غc�l
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="name"></param>
        /// <param name="Expression"></param>
        /// <param name="export"></param>
        public ExpressionCondition(Conditions Parent, string name, string Expression, bool export)
            : this(Parent, name, Expression, ConditionLink.And, export)
        {
            this.initializeFinished();
        }

        /// <summary>
        /// Condition �y�k����غc�l
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Expression"></param>
        /// <param name="link"></param>
        public ExpressionCondition(Conditions Parent, string Expression, ConditionLink link)
            : this(Parent, null, Expression, link)
        { }

        /// <summary>
        /// Condition �y�k����غc�l
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Expression"></param>
        public ExpressionCondition(Conditions Parent, string Expression)
            : this(Parent, null, Expression, ConditionLink.And)
        {
            this.initializeFinished();
        }

        /// <summary>
        /// Condition �y�k����غc�l
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Parent"></param>
        /// <param name="export"></param>
        public ExpressionCondition(Conditions Parent, string name, bool export)
            : this(Parent, name, null, ConditionLink.And, export)
        {
            this.initializeFinished();
        }

        /// <summary>
        /// Condition �y�k����غc�l
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Parent"></param>
        public ExpressionCondition(string name, Conditions Parent)
            : this(Parent, name, null, ConditionLink.And)
        {
            this.initializeFinished();
        }

        /// <summary>
        /// ���ܱ���(Condition) �O�_���@�ӪŪ�����(Condition)
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return (Name == null && expression == null);
            }
        }

        public override NamedCollection<Parameter> Parameters
        {
            get { return (expression != null) ? expression.Parameters : null; }
        }

        internal void Attach(string Expression)
        {
            if (expression == null)
                expression = new NonCompleteExpression(this, Expression);
            else
                expression.Attach(Expression);

            RasieConditionChanged();
        }

        internal void Attach(ConditionLink link, string Expression)
        {
            if (expression == null)
                expression = new NonCompleteExpression(this, Expression);
            else
                expression.Attach(link, Expression);

            RasieConditionChanged();
        }

        /// <summary>
        /// �^�Ǳ��� (Condition) CTL SQL ��r���e.
        /// </summary>
        public override string ConditionSource
        {
            get
            {
                if (expression != null)
                {
                    var topModel = expression.TopModel;
                    var Options = (topModel != null) ? topModel.Options : SqlOptions.None;

                    return expression.RebuildExpression(CommandBuildType.SQLCTL, ParameterMode.Parameter, null, Options);
                }
                return null;
            }
            //set
            //{
            //    expression = new NonCompleteExpression(this, value);
            //    RasieConditionChanged();
            //}
        }

        /// <summary>
        /// �^�Ǳ��� (Condition).�ѪR�� SQL/SQLCTL ��r���e
        /// </summary>
        public override string ConditionExpression(CommandBuildType BuildType, ParameterMode mode, ExportParameter Exporter)
        {
            if (expression != null)
            {
                var topModel = expression.TopModel;
                var Options = (topModel != null) ? topModel.Options : SqlOptions.None;

                if (BuildType == CommandBuildType.SQLCTL)
                {
                    if (!string.IsNullOrEmpty(this.Name))
                        return string.Format("{{${0} {1}}}", this.Name,
                            expression.RebuildExpression(CommandBuildType.SQLCTL, ParameterMode.Parameter, Exporter , Options)
                        );
                    else
                        return string.Format("{{$ {0}}}",
                             expression.RebuildExpression(CommandBuildType.SQLCTL, ParameterMode.Parameter, Exporter, Options)
                         );
                }
                return expression.RebuildExpression(BuildType, mode, Exporter, Options);
            }
            return null;
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
                return $"Expression=[{ConditionSource}],(Condition={Link})";
            else
                return $"${Name} Expression=[{ConditionSource}],Condition={Link}";
        }
    }
}