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
    /// �Ѽ� (Parameter) �ܼƿ�X���B�z�{�Ǫ����޼�
    /// </summary>
    public class ExportParameterEventArgs : EventArgs
    {
        /// <summary>
        /// �Ѽ��ܼƿ�X���B�z�޼ƫغc�l
        /// </summary>
        /// <param name="Name">�ѼƦW��, ���]�t @ �r��</param>
        /// <param name="ArgmentType">�\�૬������ (Condition)�W��</param>
        /// <param name="ColumnName">���W��</param>
        /// <param name="BuildType">��X�� CommandBuildType</param>
        /// <param name="ParameterMode">��X�� ParameterMode</param>
        /// <param name="Parameter"> ��X�� Parameter �����Ψ��</param>
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
        /// �Ѽ��ܼƿ�X���B�z�޼ƫغc�l
        /// </summary>
        /// <param name="Name">�\�૬������ (Condition)�W��</param>
        /// <param name="BuildType">��X�� CommandBuildType</param>
        /// <param name="ParameterMode">��X�� ParameterMode</param>
        /// <param name="Parameter"> ��X�� Parameter �����Ψ��</param>
        public ExportParameterEventArgs(string Name,
                                  CommandBuildType BuildType,
                                  ParameterMode ParameterMode,
                                  Parameter Parameter)
            : this(Name, ArgumentType.None, null, BuildType, ParameterMode, Parameter) { }

        /// <summary>
        /// �\�૬������ (Condition) �W�� , FunctionCondition.FuncName.None ��ܫD�\�૬������.
        /// </summary>
        public ArgumentType ArgmentType { get; private set; }

        /// <summary>
        /// �\�૬������ (Condition) ���B�⦡ , �� FuncName ���� None �ɬ��������B�⦡
        /// </summary>
        public Expression.ColumnExpression ColumnExpression { get; private set; }

        /// <summary>
        /// �ѼƦW��, ���]�t @ �r��
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// ��X�� CommandBuildType
        /// </summary>
        public CommandBuildType BuildType { get; private set; }

        /// <summary>
        /// ��X�� ParameterMode
        /// </summary>
        public ParameterMode pMode { get; private set; }

        /// <summary>
        /// ��X�� Parameter �����Ψ��.
        /// </summary>
        public Parameter Parameter { get; private set; }
    }

    /// <summary>
    /// ��X����k�w�q
    /// </summary>
    /// <param name="CommandBuilder"></param>
    /// <param name="e"></param>
    public delegate void ExportParameter(CommandBuilder CommandBuilder, ExportParameterEventArgs e);
}