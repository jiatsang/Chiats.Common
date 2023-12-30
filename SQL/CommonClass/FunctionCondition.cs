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
    /// 功能型的條件式 (Condition) {$Name:Scope  ColumnExpression,@ParameName }
    /// </summary>
    /// <remarks>
    /// 功能型的條件式.包含了. ArgumentType 中指定的各種功能的特色查詢條件的指定
    /// 其中參數名稱只會有一個 @ParameName
    /// </remarks>
    public class FunctionCondition : Condition
    {
        /// <summary>
        /// 是否啟用條件物件 , True:啟用 Flase:不使用 Null:自動
        /// </summary>
        private ArgumentType argumentType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string columnExpression;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string paramName;

        /// <summary>
        /// 功能型的條件式 (Condition) 參數只會有一個
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
        /// 功能型參數條件所包含的參數物件
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
        /// 功能型參數條件所包含的參數名稱
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
        /// 功能型條件參數.
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
        /// 指示條件式(Condition) 是否為一個空的條件式(Condition)
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return Params.Count == 0 || Params[0].IsClear; // 參數只會有一個
            }
        }

        /// <summary>
        /// 回傳條件式(Condition) 所包含的參數集合. 參數只會有一個 所以 Parameters.Count 永遠為 1
        /// </summary>
        public override NamedCollection<Parameter> Parameters
        {
            get { return Params; }
        }

        /// <summary>
        /// 回傳條件式 (Condition) SQLCTL 文字內容.
        /// </summary>
        public override string ConditionSource
        {
            get
            {
                return ConditionExpression(CommandBuildType.SQLCTL, ParameterMode.Parameter, null);
            }
        }

        /// <summary>
        /// 回傳條件式 (Condition).解析後 SQL/SQLCTL 文字內容
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
            // 組合範圍值/多值寫法
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
        /// String，表示目前的 Object。
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