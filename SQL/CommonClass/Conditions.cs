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
    /// SqlModel 的條件運算式集合
    /// </summary>
    public class Conditions : CommonList<Condition> // IEnumerable<Condition>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private WhereClause where_case = null;

        internal Conditions(WhereClause where_case)
        {
            this.where_case = where_case;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal WhereClause Parent
        {
            get
            {
                Debug.Assert(where_case != null);
                return where_case;
            }
        }

        /// <summary>
        ///  加入一個功能型的條件運算式 . Scope/Multi/MultiScope.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ArgmentType"></param>
        /// <param name="ColumnnExpress"></param>
        /// <param name="ParamName"></param>
        /// <param name="link"></param>
        /// <param name="export"></param>
        public void AddFunction(string Name, ArgumentType ArgmentType,
            string ColumnnExpress,
            string ParamName,
            ConditionLink link,
            bool export)
        {
            if (ArgmentType == ArgumentType.None)
                throw new SqlModelSyntaxException("Conditions.AddFunction FuncName 不可以為 None.");

            //Expression.ColumnExpression ColumnExpression
            //    = new Expression.ColumnExpression(ColumnnExpress);
            // TODO : Parameter Change to FuncParameter 功能型的條件運算式專用參數.
            // Parameter Parameter = new Parameter(ParamName, ArgmentType);
            FunctionCondition FC =
                new FunctionCondition(this, Name, ArgmentType,
                ColumnnExpress,
                ParamName,
                link,
                export);

            InternalAdd(FC);
        }

        /// <summary>
        /// 加入一個條件運算式
        /// </summary>
        /// <param name="conditionExpression"></param>
        public void Add(string conditionExpression)
        {
            InternalAdd(new ExpressionCondition(this, conditionExpression));
        }

        /// <summary>
        /// 加入一個條件運算式
        /// </summary>
        /// <param name="condition"></param>
        public void Add(ExpressionCondition condition)
        {
            InternalAdd(condition);
        }

        /// <summary>
        /// 加入一個條件運算式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conditionExpression"></param>
        public void Add(string name, string conditionExpression)
        {
            InternalAdd(new ExpressionCondition(this, name, conditionExpression, ConditionLink.And));
        }

        /// <summary>
        /// 加入一個條件運算式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conditionExpression"></param>
        /// <param name="export"></param>
        public void Add(string name, string conditionExpression, bool export)
        {
            InternalAdd(new ExpressionCondition(this, name, conditionExpression, ConditionLink.And, export));
        }

        /// <summary>
        /// 加入一個條件運算式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conditionExpression"></param>
        /// <param name="link"></param>
        public void Add(string name, string conditionExpression, ConditionLink link)
        {
            InternalAdd(new ExpressionCondition(this, name, conditionExpression, link));
        }

        /// <summary>
        /// 加入一個條件運算式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conditionExpression"></param>
        /// <param name="link"></param>
        /// <param name="export"></param>
        public void Add(string name, string conditionExpression, ConditionLink link, bool export)
        {
            InternalAdd(new ExpressionCondition(this, name, conditionExpression, link, export));
        }

        /// <summary>
        /// 加入一個條件運算式
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <param name="link"></param>
        public void Add(string conditionExpression, ConditionLink link)
        {
            InternalAdd(new ExpressionCondition(this, conditionExpression, link));
        }

        private void InternalAdd(Condition condition)
        {
            // condition.Enabled = (condition.Name == null); // 系統預設, 匿名(Anonymous) 為 True;

            if (string.IsNullOrEmpty(condition.Name) || this.GetIndexByName(condition.Name) == -1)
            {
                this.BaseAdd(condition);
                condition.ChangeBelongConditions(this);
                where_case.RaiseWhereConditionChanged(new ChangedEventArgs<Condition>(ChangedEventType.Add, condition));
            }
            else
            {
                // Replace
                Condition ReplaceCondition = this.BaseReplace(condition);
                condition.ChangeBelongConditions(this);
                where_case.RaiseWhereConditionChanged(new ChangedEventArgs<Condition>(ChangedEventType.Replace, condition, ReplaceCondition));
            }
        }

        internal void RasieWhereConditionAdd(Condition condition)
        {
            where_case.RaiseWhereConditionChanged(new ChangedEventArgs<Condition>(ChangedEventType.Add, condition));
        }

        internal void RasieWhereConditionChanged(Condition condition)
        {
            where_case.RaiseWhereConditionChanged(new ChangedEventArgs<Condition>(ChangedEventType.Changed, condition));
        }
    }

    /// <summary>
    /// 支援條件式之 SqlModel , 包含 SelectModel/UpdateModel/DeleteModel
    /// </summary>
    public interface ISqlConditions
    {
        /// <summary>
        /// 條件式物件集合, SelectModel 同時包含 Where/Having 條件式 ,  UpdateModel/DeleteModel 包含 Where 條件式
        /// </summary>
        IConditionModels Conditions { get; }
    }

    /// <summary>
    /// 參數物件的型別.
    /// </summary>
    [Flags]
    public enum ConditionFlags
    {
        /// <summary>
        /// 具有指定名稱參數物件
        /// </summary>
        Named = 0x01,

        /// <summary>
        /// 指示目前為 Anonymous 的參數物件
        /// </summary>
        Anonymous = 0x02,

        /// <summary>
        /// 同时具有指定名稱和 Anonymous 參數物件
        /// </summary>
        All = Named | Anonymous
    }

    /// <summary>
    /// 條件式物件集合,
    /// </summary>
    public interface IConditionModels : IEnumerable<IConditionModel>
    {
        /// <summary>
        /// 條件式變更事件
        /// </summary>
        event EventHandler<ChangedEventArgs<IConditionModel>> ConditionChanged;

        /// <summary>
        /// 取得條件式物件收量.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 取得指定名稱之條件式物件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IConditionModel this[string name] { get; }

        /// <summary>
        /// 取得指定位置之條件式物件
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IConditionModel this[int index] { get; }

        /// <summary>
        /// 查詢指定參數所屬的條件式 (Condition) 名稱 , 同時屬於多個條件式以 ',' 區隔
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string QueyBelongCondition(string name);

        /// <summary>
        /// 查詢指定參數所屬的條件式 (Condition) 名稱 , 同時屬於多個條件式以 ',' 區隔
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        string QueyBelongCondition(Parameter Parameter);

        //  void RaiseConditionChanged(object sender, ChangedEventArgs<Condition> e);

        FunctionCondition[] GetFunctionConditions(ConditionFlags Flags);
    }
}