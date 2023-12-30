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
    /// SQL CTL 條件式(Condition) 基礎類別.
    /// </summary>
    public abstract class Condition : IVariantName, IPartSqlModel
    {
        private Conditions Conditions = null;

        /// <summary>
        /// 表示條件式(Condition) 的輸出方法(啟用/禁用)選項.
        /// </summary>
        private bool enabled = true;

        /// <summary>
        /// 條件式(Condition)名稱. 若為匿名(Anonymous)條件式,此值為 null
        /// </summary>
        private string name = null;

        /// <summary>
        /// 和上一個條件式(Condition) 的連結運算子. 包含 AND 或 OR  ,若為第一個條件式(Condition), 其值無用
        /// </summary>
        protected ConditionLink uplink = ConditionLink.And;

        /// <summary>
        /// 條件式(Condition) 物件的建構子.
        /// </summary>
        /// <param name="Conditions">Belong Conditions</param>
        /// <param name="name">條件式(Condition)名稱. 若為匿名(Anonymous)條件式,此值為 null</param>
        /// <param name="uplink">和上一個條件式(Condition) 的連結運算子</param>
        /// <param name="enabled"> 表示條件式(Condition) 的輸出方法(啟用/禁用)選項. </param>
        public Condition(Conditions Conditions, string name, ConditionLink uplink, bool enabled)
        {
            this.Conditions = Conditions;
            this.enabled = enabled;
            this.Name = name;
            this.uplink = uplink;
        }

        /// <summary>
        /// 變更條件式(Condition) 所屬的 Conditions 容器.
        /// </summary>
        /// <param name="Conditions">所屬的 Conditions 容器</param>
        internal void ChangeBelongConditions(Conditions Conditions)
        {
            this.Conditions = Conditions;
        }

        protected void initializeFinished()
        {
            ProcessLinkParameters();
        }

        private void ProcessLinkParameters()
        {
            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (Parameter p in Parameters) p.Condition = this;
            }
        }

        /// <summary>
        /// 通知所屬的 Conditions 容器, 條件式(Condition) 己有變更.
        /// </summary>
        protected void RasieConditionChanged()
        {
            if (Conditions != null)
                Conditions.RasieWhereConditionChanged(this);

            ProcessLinkParameters();
        }

        /// <summary>
        /// 設定或取得目前條件物件啟用與否
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    RasieConditionChanged();
                }
            }
        }

        /// <summary>
        /// 條件物件是否為輸出中狀態 ( Enabled為 True 並且 所包含之參數值內容均有設定其值時 )
        /// </summary>
        /// <returns></returns>
        public bool ExportEnabled(ParameterMode Mode)
        {
            if (Mode == ParameterMode.Parameter) return this.enabled;
            if (this.enabled && this.Parameters != null)
            {
                foreach (Parameter Parameter in this.Parameters)
                    if (Parameter.IsClear) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 和上一個條件式(Condition) 的連結運算子. 包含 AND 或 OR  ,若為第一個條件式(Condition), 其值無用
        /// </summary>
        public ConditionLink Link
        {
            get { return uplink; }
            set
            {
                if (uplink != value)
                {
                    uplink = value;
                    RasieConditionChanged();
                }
            }
        }

        /// <summary>
        /// 指示條件式(Condition) 是否為一個空的條件式(Condition)
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// 回傳條件式(Condition) 所包含的參數集合.
        /// </summary>
        public abstract NamedCollection<Parameter> Parameters { get; }

        /// <summary>
        /// 回傳條件式 (Condition) SQLCTL 文字內容.
        /// </summary>
        public abstract string ConditionSource { get; }

        /// <summary>
        /// 回傳條件式 (Condition).解析後 SQL/SQLCTL 文字內容
        /// </summary>
        /// <param name="BuildType"></param>
        /// <param name="mode"></param>
        /// <param name="Export"></param>
        /// <returns></returns>
        public abstract string ConditionExpression(CommandBuildType BuildType, ParameterMode mode, ExportParameter Export);

        /// <summary>
        /// 回傳是否含有指定之參數名稱存在
        /// </summary>
        /// <param name="name">指定之參數名稱</param>
        /// <returns></returns>
        public bool BelongParameter(string name)
        {
            foreach (Parameter p in Parameters)
            {
                if (string.Compare(p.Name, name, true) == 0) return true;
            }
            return false;
        }

        /// <summary>
        /// 回傳是否含有指定之參數存在
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public bool BelongParameter(Parameter Parameter)
        {
            foreach (Parameter p in Parameters)
            {
                if (p == Parameter) return true;
            }
            return false;
        }

        /// <summary>
        /// 條件物件名稱, 匿名(Anonymous)則回傳回 null
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    if (Conditions != null)
                    {
                        Conditions.RasieWhereConditionChanged(this);
                    }
                }
            }
        }

        /// <summary>
        /// 傳回最上一層的 CommonModel 父階物件
        /// </summary>
        /// <returns>最上一層的 CommonModel 父階物件</returns>
        public SqlModel GetTopModel()
        {
            Debug.Assert(Conditions != null);
            return Conditions.Parent.GetTopModel();
        }
    }
}