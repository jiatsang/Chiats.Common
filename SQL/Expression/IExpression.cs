// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.ComponentModel;

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// SqlModel �B�⦡�зǤ���. ���[�\�F���B�⦡�M����B�⦡.�B�⦡�r���ܦ� = �B�⦡�����ഫ���i���
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// �Ǧ^�O�_���׺ݹB��l
        /// </summary>
        bool IsTerminalExp { get; }

        /// <summary>
        /// ��X�B�⦡���r���ܦ�
        /// </summary>
        /// <param name="CommandBuilder">�B�⦡���r���ܦ����;�</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName);
    }

    /// <summary>
    /// ����s���l =,&lt;, see SyntaxRuleB1
    /// </summary>
    internal enum ComparisonOperator
    {
        [StringValue("null"), Description("Single Column Comparison Operator Example: Column or Function")]
        Empty,

        /// <summary>
        /// �����ӹB�⦡�O�_�۵� (����B��l)�C
        /// </summary>
        [StringValue("="), Description("= (����)")]
        Eqauls,

        /// <summary>
        /// �����ӹB�⦡ (����B��l)�C��z�b����D Null �B�⦡�ɡA�p�G���B�⤸���Ȥj��k�B�⤸�A
        /// �h���G�� TRUE�A�_�h���G�N�O FALSE�C�p�G�䤤���@�ӹB�⤸�άO��̬Ҭ� NULL�A�h�Ǧ^ NULL�C
        /// </summary>
        [StringValue(">"), Description("> (�j��)")]
        Greater,

        /// <summary>
        /// �����ӹB�⦡ (����B��l)�C��z�b����D Null �B�⦡�ɡA�p�G���B�⤸���Ȥp��k�B�⤸�A
        /// �h���G�� TRUE�A�_�h���G�N�O FALSE�C�p�G�䤤���@�ӹB�⤸�άO��̬Ҭ� NULL�A�h�Ǧ^ NULL�C
        /// </summary>
        [StringValue("<"), Description("< (�p��)")]
        Less,

        /// <summary>
        /// ����j��ε��� (����B��l) ����ӹB�⦡�C
        /// </summary>
        [StringValue(">="), Description(">= (�j��ε���)")]
        GreaterEqauls,

        /// <summary>
        /// �����ӹB�⦡ (����B��l)�C��z�b����D Null �B�⦡�ɡA�p�G���B�⤸���Ȥp��ε���k�B�⤸�A
        /// �h���G�� TRUE�A�_�h���G�N�O FALSE�C�p�G�䤤���@�ӹB�⤸�άO��̬Ҭ� NULL�A�h�Ǧ^ NULL�C
        /// </summary>
        [StringValue("<="), Description("<= (�p��ε���)")]
        LessEqauls,

        /// <summary>
        /// ���դ@�ӹB�⦡�O�_����t�@�ӹB�⦡ (����B��l)�C�p�G�䤤���@�ӹB�⤸�άO��̬Ҭ� NULL�A
        /// �h�Ǧ^ NULL�C��ƻP &lt;&gt; (������) ����B��l�ۦP�C
        /// </summary>
        [StringValue("<>"), Description("<> or !=")]
        NotEqauls,

        /// <summary>
        /// �����ӹB�⦡ (����B��l)�C��z�b����D Null �B�⦡�ɡA�p�G���B�⤸���Ȥ��p��k�B�⤸�A
        /// �h���G�� TRUE�A�_�h���G�N�O FALSE�C�p�G�䤤���@�ӹB�⤸�άO��̬Ҭ� NULL�A�h�Ǧ^ NULL�C
        /// </summary>
        [StringValue("!<"), Description("!< (���p��) ")]
        NotLess,

        /// <summary>
        /// �����ӹB�⦡ (����B��l)�C��z�b����D Null �B�⦡�ɡA�p�G���B�⤸���Ȥ��j��k�B�⤸�A
        /// �h���G�� TRUE�A�_�h���G�N�O FALSE�C�p�G�䤤���@�ӹB�⤸�άO��̬Ҭ� NULL�A�h�Ǧ^ NULL�C
        /// </summary>
        [StringValue("!>"), Description("!> (���j��)")]
        NotGreater,

        ////[StringValue("like"), Description("like ")]
        //Like,
        //[StringValue("not like"), Description("like ")]
        //NotLike,
    }

    internal enum LogicalOperator
    {
        [StringValue(null), Description("�L�Ī��s���l")]
        Empty,

        /// <summary>
        /// ���X��ӥ��L�B�⦡�A�æb��ӹB�⦡���O TRUE �ɡA�Ǧ^ TRUE�C��b���z�����ϥΤ@�ӥH�W���޿�B��l�ɡA
        /// �|������ AND �B��l�C�z�i�H�ϥάA�����ܧ���⪺���ǡC
        /// </summary>
        [StringValue("and"), Description("Logical And")]
        And,

        [StringValue("or"), Description("Logical Or")]
        Or,

        [StringValue("not and"), Description("Logical Not And")]
        NotAnd,

        [StringValue("not"), Description("Logical Not")]
        Not,

        [StringValue("not or"), Description("Logical Not Or")]
        NotOr,

        [StringValue("like"), Description("Logical Like")]
        Like,

        [StringValue("not like"), Description("Logical Not Like")]
        NotLike,
    }
}