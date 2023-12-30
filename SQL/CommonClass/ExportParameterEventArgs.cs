// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL
{
    /// <summary>
    /// 參數 (Parameter) 變數輸出之處理程序的之引數
    /// </summary>
    public class ExportParameterEventArgs : EventArgs
    {
        /// <summary>
        /// 參數變數輸出之處理引數建構子
        /// </summary>
        /// <param name="Name">參數名稱, 它包含 @ 字首</param>
        /// <param name="ArgmentType">功能型的條件式 (Condition)名稱</param>
        /// <param name="ColumnName">欄位名稱</param>
        /// <param name="BuildType">輸出的 CommandBuildType</param>
        /// <param name="ParameterMode">輸出的 ParameterMode</param>
        /// <param name="Parameter"> 輸出的 Parameter 類型及其值</param>
        public ExportParameterEventArgs(string Name, ArgumentType ArgmentType,
                                        string ColumnName,
                                        CommandBuildType BuildType,
                                        ParameterMode ParameterMode,
                                        Parameter Parameter)
        {
            this.Name = Name;
            if (ColumnName != null)
                this.ColumnExpression = new Expression.ColumnExpression(ColumnName);

            this.ArgmentType = ArgmentType;
            this.BuildType = BuildType;
            this.pMode = ParameterMode;
            this.Parameter = Parameter;
        }

        /// <summary>
        /// 參數變數輸出之處理引數建構子
        /// </summary>
        /// <param name="Name">功能型的條件式 (Condition)名稱</param>
        /// <param name="BuildType">輸出的 CommandBuildType</param>
        /// <param name="ParameterMode">輸出的 ParameterMode</param>
        /// <param name="Parameter"> 輸出的 Parameter 類型及其值</param>
        public ExportParameterEventArgs(string Name,
                                  CommandBuildType BuildType,
                                  ParameterMode ParameterMode,
                                  Parameter Parameter)
            : this(Name, ArgumentType.None, null, BuildType, ParameterMode, Parameter) { }

        /// <summary>
        /// 功能型的條件式 (Condition) 名稱 , FunctionCondition.FuncName.None 表示非功能型的條件式.
        /// </summary>
        public ArgumentType ArgmentType { get; private set; }

        /// <summary>
        /// 功能型的條件式 (Condition) 欄位運算式 , 當 FuncName 不為 None 時為條件之欄位運算式
        /// </summary>
        public Expression.ColumnExpression ColumnExpression { get; private set; }

        /// <summary>
        /// 參數名稱, 它包含 @ 字首
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 輸出的 CommandBuildType
        /// </summary>
        public CommandBuildType BuildType { get; private set; }

        /// <summary>
        /// 輸出的 ParameterMode
        /// </summary>
        public ParameterMode pMode { get; private set; }

        /// <summary>
        /// 輸出的 Parameter 類型及其值.
        /// </summary>
        public Parameter Parameter { get; private set; }
    }

    /// <summary>
    /// 輸出之方法定義
    /// </summary>
    /// <param name="CommandBuilder"></param>
    /// <param name="e"></param>
    public delegate void ExportParameter(CommandBuilder CommandBuilder, ExportParameterEventArgs e);
}