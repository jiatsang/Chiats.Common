// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// ��ƪ���쫬�A,�w�q�q�Ϊ���T�M��Ʈw�t�Ӫ����L��.
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// �۰ʸ�ƫ��A , ���t�Φۦ�M�w�䫬�O  Ex  NVarChar
        /// </summary>
        Auto = 0x0000,

        /// <summary>
        /// �����w����ƫ��A
        /// </summary>
        UnKnown = 0xFFFF,

        /// <summary>
        ///
        /// </summary>
        Varbinary = ColumnTypeHelper.TypeVAR | ColumnTypeHelper.TypeBinary,

        /// <summary>
        ///
        /// </summary>
        Binary = ColumnTypeHelper.TypeBinary,

        /// <summary>
        ///
        /// </summary>
        Text = ColumnTypeHelper.TypeText,

        /// <summary>
        ///
        /// </summary>
        NText = ColumnTypeHelper.TypeDoubleByte | ColumnTypeHelper.TypeText,

        /// <summary>
        /// �ܰʪ��צr��
        /// </summary>
        Varchar = ColumnTypeHelper.TypeVAR | ColumnTypeHelper.TypeChar,

        /// <summary>
        /// �T�w���צr��
        /// </summary>
        Char = ColumnTypeHelper.TypeChar,

        /// <summary>
        /// �ܰʪ��צr��, �|�H Unicode �s�X�s�b.
        /// </summary>
        Nvarchar = ColumnTypeHelper.TypeVAR | ColumnTypeHelper.TypeDoubleByte | ColumnTypeHelper.TypeChar,

        /// <summary>
        /// �T�w���צr��, �|�H Unicode �s�X�s�b.
        /// </summary>
        NChar = ColumnTypeHelper.TypeDoubleByte | ColumnTypeHelper.TypeChar,

        /// <summary>
        /// 8 bit ��ƭ� , �t���t��
        /// </summary>
        Byte = ColumnTypeHelper.TypeNumber | 0x0001,

        /// <summary>
        /// 16 bit ��ƭ� , �t���t��
        /// </summary>
        Int = ColumnTypeHelper.TypeNumber | 0x0002,

        /// <summary>
        /// 32 bit ��ƭ� , �t���t��
        /// </summary>
        Int32 = ColumnTypeHelper.TypeNumber | 0x0003,

        /// <summary>
        /// 64 bit ��ƭ� , �t���t��
        /// </summary>
        Int64 = ColumnTypeHelper.TypeNumber | 0x0004,

        /// <summary>
        /// ���L��, �i�H�� True/False
        /// </summary>
        Bool = ColumnTypeHelper.TypeBool,

        /// <summary>
        /// �ƭȫ��A , �t���t���Τp�Ʀ��
        /// </summary>
        Decimal = ColumnTypeHelper.TypeNumber | 0x0005,

        /// <summary>
        /// ����� �B�I�ƭ�
        /// </summary>
        Double = ColumnTypeHelper.TypeNumber | 0x0006,

        /// <summary>
        /// ���� �B�I�ƭ�
        /// </summary>
        Single = ColumnTypeHelper.TypeNumber | 0x0007,

        /// <summary>
        /// ����t�ɶ�
        /// </summary>
        DateTime = ColumnTypeHelper.TypeDateTime | 0x0001,

        /// <summary>
        /// XML �r��
        /// </summary>
        XML = ColumnTypeHelper.TypeXml,

        /// <summary>
        /// Image
        /// </summary>
        Image = ColumnTypeHelper.TypeImage,

        /// <summary>
        /// Uniqueidentifier �o�O 16 �줸�ժ� GUID�C
        /// </summary>
        Uniqueidentifier = ColumnTypeHelper.TypeGuid
    }




}