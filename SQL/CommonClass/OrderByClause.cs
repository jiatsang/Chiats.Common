// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Chiats.SQL
{
    /// <summary>
    ///  SqlModel 的 Order By 語法物件. 它包含在 SelectModel 語法物件.
    /// </summary>
    public class OrderByClause : CommonClause, IEnumerable<OrderByColumn>
    {
        internal OrderByClause(SqlModel parent) : base(parent) { }

        /// <summary>
        /// OrderByColumn 的變更事件
        /// </summary>
        public event EventHandler<ChangedEventArgs<OrderByColumn>> OrderByChanged;

        private List<OrderByColumn> list = new List<OrderByColumn>();

        /// <summary>
        /// 新增一個欄位
        /// </summary>
        /// <param name="ColumnNamme"></param>
        public void Add(string ColumnNamme)
        {
            OrderSorting defaultSorting = OrderSorting.Ascending;

            if (ColumnNamme.EndsWith(" desc", StringComparison.OrdinalIgnoreCase))
            {
                ColumnNamme = ColumnNamme.Substring(0, ColumnNamme.Length - 5);
                defaultSorting = OrderSorting.Descending;
            }

            if (ColumnNamme.EndsWith(" asc", StringComparison.OrdinalIgnoreCase))
            {
                ColumnNamme = ColumnNamme.Substring(0, ColumnNamme.Length - 4);
                defaultSorting = OrderSorting.Ascending;
            }

            Add(new OrderByColumn(ColumnNamme, defaultSorting));
        }

        /// <summary>
        /// 新增一個欄位 OrderByColumn
        /// </summary>
        /// <param name="ColumnNamme"></param>
        /// <param name="ColumnOrder"></param>
        public void Add(string ColumnNamme, OrderSorting ColumnOrder)
        {
            Add(new OrderByColumn(ColumnNamme, ColumnOrder));
        }

        /// <summary>
        /// 加入 OrderByColumn 物件
        /// </summary>
        /// <param name="column">表格名稱</param>
        public void Add(OrderByColumn column)
        {
            if (column != null)
            {
                list.Add(column);

                column.DataChanged += new EventHandler(OrderByColumn_DataChanged);
                Parent.Changed();
                if (OrderByChanged != null)
                {
                    OrderByChanged(ChangedEventType.Add, new ChangedEventArgs<OrderByColumn>(column));
                }
            }
        }

        /// <summary>
        /// 移除 OrderByColumn 物件
        /// </summary>
        /// <param name="column"></param>
        public void Remove(OrderByColumn column)
        {
            if (list.Contains(column))
            {
                list.Remove(column);
                column.DataChanged -= new EventHandler(OrderByColumn_DataChanged);
                Parent.Changed();

                if (OrderByChanged != null)
                {
                    OrderByChanged(ChangedEventType.Removed, new ChangedEventArgs<OrderByColumn>(column));
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void RemoveAll()
        {
            if (list.Count > 0)
            {
                list.Clear();
                Parent.Changed();
                if (OrderByChanged != null)
                {
                    OrderByChanged(ChangedEventType.Removed, new ChangedEventArgs<OrderByColumn>(null));
                }
            }
        }

        private void OrderByColumn_DataChanged(object sender, EventArgs e)
        {
            Parent.Changed();
            if (OrderByChanged != null)
            {
                OrderByChanged(ChangedEventType.Changed, new ChangedEventArgs<OrderByColumn>((OrderByColumn)sender));
            }
        }

        /// <summary>
        /// 傳回 OrderByColumn 之個數.
        /// </summary>
        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        /// <summary>
        /// 傳回指定 OrderByColumn 物件.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OrderByColumn this[int index]
        {
            get { return list[index]; }
        }

        #region IEnumerable Members

        /// <summary>
        /// 擷取可以逐一查看這個物件中個別的子物件。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region IEnumerable<OrderKey> Members

        IEnumerator<OrderByColumn> IEnumerable<OrderByColumn>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion IEnumerable<OrderKey> Members

        /// <summary>
        /// String，表示目前的 Object。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ISqlBuildExport BuildExport = DefaultBuildExport.SQLBuildExport;
            CommandBuilder CommandBuilder = new CommandBuilder();
            BuildExport.ExportForOrderByClause(this, CommandBuilder, this.Parent, BuildExportOptions.None);
            return CommandBuilder.ToString();
        }
    }

    /// <summary>
    ///  SqlModel OrderByColumn 物件
    /// </summary>
    public class OrderByColumn
    {
        /// <summary>
        /// OrderByColumn 物件 建構子
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="OrderSorting"></param>
        public OrderByColumn(string ColumnName, OrderSorting OrderSorting)
        {
            this.ColumnName = ColumnName;
            this.orderSorting = OrderSorting;
        }

        /// <summary>
        /// OrderByColumn 物件 建構子
        /// </summary>
        /// <param name="ColumnName"></param>
        public OrderByColumn(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }

        private OrderSorting orderSorting = OrderSorting.Ascending;

        /// <summary>
        /// 資料內容變更
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// 欄位名稱
        /// </summary>
        public readonly string ColumnName;

        /// <summary>
        /// 指定排序的方式.
        /// </summary>
        public OrderSorting OrderSorting
        {
            get { return orderSorting; }
            set
            {
                if (orderSorting != value)
                {
                    orderSorting = value;

                    if (DataChanged != null)
                    {
                        DataChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}