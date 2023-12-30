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
    /// ��Ʈw�����Ψ䫬�A��T�y�z. Example : colunm_name decimal(10,2)
    /// </summary>
    public class ColumnDescription : ColumnTypeInfo, IVariantName
    {
        private string name = null;
        private ColumnIdentity identity = null;
        private ColumnDefault columnDefault = null;

        /// <summary>
        /// �ѪR��ƪ���쫬�A��T�y�z���� ColumnDescription ���� . Example : varchar(10),  decimal(10,2) <br/>
        /// TODO: �o�̻ݭn�w�q��檺���W�٦P�q�r..�ι�����͸�Ʈw�����W�٩w�q. �~��i���ڪ��ѪR�u�@.
        /// </summary>
        /// <param name="ColumnDescription"></param>
        public ColumnDescription(string ColumnDescription)
        {
            // TODO: �o�̻ݭn�w�q��檺���W�٦P�q�r..�ι�����͸�Ʈw�����W�٩w�q. �~��i���ڪ��ѪR�u�@.
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ƪ���쫬�A��T�y�z���� ColumnDescription ����
        /// </summary>
        /// <param name="Description"></param>
        public ColumnDescription(ColumnDescription Description)
            : base(Description.ColumnType, Description.Size, Description.NumericPrecision, Description.NumericScale, Description.Nullable)
        {
            name = Description.name;
            if (Description.identity != null) identity = new ColumnIdentity(Description.identity);
            if (Description.columnDefault != null) columnDefault = new ColumnDefault(Description.columnDefault);
        }

        /// <summary>
        /// �إ� ColumnDescription ����.
        /// </summary>
        /// <param name="ColumnName">���W��</param>
        /// <param name="ColumnType">��쫬�A</param>
        /// <param name="size"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        public ColumnDescription(string ColumnName, ColumnType ColumnType, int size, short precision, short scale)
            : base(ColumnType, size, precision, scale, true)
        {
            this.name = ColumnName;
        }

        /// <summary>
        /// �إ� ColumnDescription ����.
        /// </summary>
        /// <param name="ColumnName">���W��</param>
        /// <param name="ColumnType">��쫬�A</param>
        /// <param name="size"></param>
        public ColumnDescription(string ColumnName, ColumnType ColumnType, int size)
            : this(ColumnName, ColumnType, size, 0, 0) { }

        /// <summary>
        /// ��ƪ����ѧO���.
        /// </summary>
        public ColumnIdentity Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        /// <summary>
        /// ��ƪ������w�]��.
        /// </summary>
        public ColumnDefault Default
        {
            get { return columnDefault; }
            set { columnDefault = value; }
        }

        public new bool Nullable
        {
            get { return base.Nullable; }
            set { base.Nullable = value; }
        }

        #region INamed Members

        public string Name
        {
            get { return name; }
        }

        #endregion INamed Members
    }

    /// <summary>
    /// ��ƪ������w�]��
    /// </summary>
    public class ColumnDefault
    {
        /// <summary>
        /// ��ƪ������w�]��.
        /// </summary>
        /// <param name="ColumnDefault"></param>
        public ColumnDefault(ColumnDefault ColumnDefault)
        {
            Name = ColumnDefault.Name;
            Definition = ColumnDefault.Definition;
        }

        /// <summary>
        /// ��ƪ������w�]��.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definition"></param>
        public ColumnDefault(string name, string definition)
        {
            Name = name;
            Definition = definition;
        }

        /// <summary>
        /// ��ƪ������w�]�ȦW��.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// ��ƪ������w�]��.
        /// </summary>
        public string Definition;
    }

    /// <summary>
    /// �إ߸�ƪ����ѧO���C�o���ݩʷ|�f�t CREATE TABLE �M ALTER TABLE ���z���ϥ� <br/>
    /// DENT_INCR �Ǧ^�b�t���ѧO��쪺��ƪ���˵����إ��ѧO�������ҫ��w�����W�� (�H numeric (@@MAXPRECISION,0) �Ǧ^)�C<br/>
    /// IDENT_SEED ( 'table_or_view' ) �Ǧ^�b��ƪ���˵����إ��ѧO���ɩҫ��w����l��l�� <br/>
    /// </summary>
    public class ColumnIdentity
    {
        /// <summary>
        /// �ѧO���غc�l
        /// </summary>
        /// <param name="id"></param>
        public ColumnIdentity(ColumnIdentity id) { Seed = id.Seed; Increment = id.Increment; }

        /// <summary>
        /// �ѧO���غc�l
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="increment"></param>
        public ColumnIdentity(int seed, int increment) { Seed = seed; Increment = increment; }

        /// <summary>
        /// �o�o�O���J��ƪ��Ĥ@�Ӹ�ƦC�ҥΪ��ȡC
        /// </summary>
        public readonly int Increment;

        /// <summary>
        /// �o�O�n�[�J��ƪ������ƦC�� seed �Ȫ���ƭȡC
        /// </summary>
        public readonly int Seed;
    }
}