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
    /// 資料庫的欄位及其型態資訊描述. Example : colunm_name decimal(10,2)
    /// </summary>
    public class ColumnDescription : ColumnTypeInfo, IVariantName
    {
        private string name = null;
        private ColumnIdentity identity = null;
        private ColumnDefault columnDefault = null;

        /// <summary>
        /// 解析資料表欄位型態資訊描述成為 ColumnDescription 物件 . Example : varchar(10),  decimal(10,2) <br/>
        /// TODO: 這裡需要定義表單的欄位名稱同義字..及對應原生資料庫的欄位名稱定義. 才能進行實際的解析工作.
        /// </summary>
        /// <param name="ColumnDescription"></param>
        public ColumnDescription(string ColumnDescription)
        {
            // TODO: 這裡需要定義表單的欄位名稱同義字..及對應原生資料庫的欄位名稱定義. 才能進行實際的解析工作.
            throw new NotImplementedException();
        }

        /// <summary>
        /// 資料表欄位型態資訊描述成為 ColumnDescription 物件
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
        /// 建立 ColumnDescription 物件.
        /// </summary>
        /// <param name="ColumnName">欄位名稱</param>
        /// <param name="ColumnType">欄位型態</param>
        /// <param name="size"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        public ColumnDescription(string ColumnName, ColumnType ColumnType, int size, short precision, short scale)
            : base(ColumnType, size, precision, scale, true)
        {
            this.name = ColumnName;
        }

        /// <summary>
        /// 建立 ColumnDescription 物件.
        /// </summary>
        /// <param name="ColumnName">欄位名稱</param>
        /// <param name="ColumnType">欄位型態</param>
        /// <param name="size"></param>
        public ColumnDescription(string ColumnName, ColumnType ColumnType, int size)
            : this(ColumnName, ColumnType, size, 0, 0) { }

        /// <summary>
        /// 資料表中的識別欄位.
        /// </summary>
        public ColumnIdentity Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        /// <summary>
        /// 資料表中的欄位預設值.
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
    /// 資料表中的欄位預設值
    /// </summary>
    public class ColumnDefault
    {
        /// <summary>
        /// 資料表中的欄位預設值.
        /// </summary>
        /// <param name="ColumnDefault"></param>
        public ColumnDefault(ColumnDefault ColumnDefault)
        {
            Name = ColumnDefault.Name;
            Definition = ColumnDefault.Definition;
        }

        /// <summary>
        /// 資料表中的欄位預設值.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definition"></param>
        public ColumnDefault(string name, string definition)
        {
            Name = name;
            Definition = definition;
        }

        /// <summary>
        /// 資料表中的欄位預設值名稱.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 資料表中的欄位預設值.
        /// </summary>
        public string Definition;
    }

    /// <summary>
    /// 建立資料表中的識別欄位。這個屬性會搭配 CREATE TABLE 和 ALTER TABLE 陳述式使用 <br/>
    /// DENT_INCR 傳回在含有識別欄位的資料表或檢視中建立識別欄位期間所指定的遞增值 (以 numeric (@@MAXPRECISION,0) 傳回)。<br/>
    /// IDENT_SEED ( 'table_or_view' ) 傳回在資料表或檢視表中建立識別欄位時所指定的原始初始值 <br/>
    /// </summary>
    public class ColumnIdentity
    {
        /// <summary>
        /// 識別欄位建構子
        /// </summary>
        /// <param name="id"></param>
        public ColumnIdentity(ColumnIdentity id) { Seed = id.Seed; Increment = id.Increment; }

        /// <summary>
        /// 識別欄位建構子
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="increment"></param>
        public ColumnIdentity(int seed, int increment) { Seed = seed; Increment = increment; }

        /// <summary>
        /// 這這是載入資料表的第一個資料列所用的值。
        /// </summary>
        public readonly int Increment;

        /// <summary>
        /// 這是要加入資料表中後續資料列之 seed 值的整數值。
        /// </summary>
        public readonly int Seed;
    }
}