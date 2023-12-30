// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace Chiats.SQL.Expression
{
    /// <summary>
    /// �ƭȹB��s���l.
    /// </summary>
    [Flags]
    public enum ArithmeticLogicalLink
    {
        /// <summary>
        /// �L�Ī��s���l, �q�`�Ω�̫�@�ӮɩάO��@�B�⦡
        /// </summary>
        [StringValue(null), Description("�L�Ī��s���l")]
        Empty = 0,

        [StringValue(null), Description("��N�s���l")]
        ArithmeticLink = 0x1000,

        [StringValue(null), Description("Logical �s���l")]
        LogicalLink = 0x2000,

        /// <summary>
        /// �[�k�s���l, �����N�@�~�A�N��Ӽƥ[�b�@�_�C�ζǦ^�ƭȹB�⦡�����ȡC�]�i�Ω��s��Ӧr��B�⦡�C
        /// </summary>
        [StringValue("+"), Description("�[�k�s���l")]
        Add = ArithmeticLink | 1,

        /// <summary>
        /// ��k�s���l, �Ǧ^��ӼƭȹB�⦡�������t���A�μƭȹB�⦡���t�ȡC
        /// </summary>
        [StringValue("-"), Description("��k�s���l")]
        Subtract = ArithmeticLink | 2,

        /// <summary>
        /// ���k�s���l, ����H�@�Ӽƥح��W�t�@�Ӽƥت���N�B��
        /// </summary>
        [StringValue("*"), Description("���k�s���l")]
        Multiply = ArithmeticLink | 3,

        /// <summary>
        /// ���k�s���l(��N���k�B��l), ����H�@�Ӽƥذ��H�t�@�Ӽƥت���N�B��C
        /// </summary>
        /// <remarks>
        /// ������ƽаѦ� Microsoft SQL Server Online Help
        /// <example>
        /// �U�C�d�ҷ|�Q�ΰ��k��N�B��l�ӭp��P��H���C�몺�P��ؼСC
        /// <code lang="SQL">
        /// SELECT SalesPersonID, FirstName, LastName, SalesQuota, SalesQuota/12 AS 'Sales Target Per Month'
        /// FROM Sales.SalesPerson s
        /// JOIN HumanResources.Employee e ON s.SalesPersonID = e.EmployeeID
        /// JOIN Person.Contact c ON e.ContactID = c.ContactID;
        /// </code>
        /// </example>
        /// </remarks>
        [StringValue("/"), Description("���k�s���l")]
        Divide = ArithmeticLink | 4,

        /// <summary>
        /// AND �s���l
        /// </summary>
        [StringValue("AND"), Description("AND �s���l")]
        And = LogicalLink | 1,

        /// <summary>
        /// OR �s���l
        /// </summary>
        [StringValue("OR"), Description("OR �s���l")]
        Or = LogicalLink | 2,

        /// <summary>
        /// NOT �s���l
        /// </summary>
        [StringValue("NOT"), Description("NOT �s���l")]
        Not = LogicalLink | 2,

        /// <summary>
        /// ����s���l
        /// </summary>
        [StringValue("="), Description("����s���l")]
        Equal = LogicalLink | 3,

        /// <summary>
        /// �p�󵥩�s���l
        /// </summary>
        [StringValue("<="), Description("�p�󵥩�s���l")]
        LessEqual = LogicalLink | 4,

        /// <summary>
        /// �p��s���l
        /// </summary>
        [StringValue("<"), Description("�p��s���l")]
        Less = LogicalLink | 5,

        /// <summary>
        /// ������s���l
        /// </summary>
        [StringValue("<>"), Description("������s���l")]
        NotEqual = LogicalLink | 6,

        /// <summary>
        /// �j��s���l
        /// </summary>
        [StringValue(">"), Description("�j��s���l")]
        Greater = LogicalLink | 7,

        /// <summary>
        /// �j��s���l
        /// </summary>
        [StringValue(">="), Description("�j��s���l")]
        GreaterEqual = LogicalLink | 8,

        /// <summary>
        /// IS �s���l
        /// </summary>
        [StringValue("IS"), Description("IS �s���l")]
        IS = LogicalLink | 9,

        /// <summary>
        /// AS �s���l
        /// </summary>
        [StringValue("AS"), Description("AS �s���l")]
        AS = LogicalLink | 10,
    }
}