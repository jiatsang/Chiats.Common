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
    ///  SqlModel �� Order By �y�k����. ���]�t�b SelectModel �y�k����.
    /// </summary>
    public class OrderByClause : CommonClause, IEnumerable<OrderByColumn>
    {
        internal OrderByClause(SqlModel parent) : base(parent) { }

        /// <summary>
        /// OrderByColumn ���ܧ�ƥ�
        /// </summary>
        public event EventHandler<ChangedEventArgs<OrderByColumn>> OrderByChanged;

        private List<OrderByColumn> list = new List<OrderByColumn>();

        /// <summary>
        /// �s�W�@�����
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
        /// �s�W�@����� OrderByColumn
        /// </summary>
        /// <param name="ColumnNamme"></param>
        /// <param name="ColumnOrder"></param>
        public void Add(string ColumnNamme, OrderSorting ColumnOrder)
        {
            Add(new OrderByColumn(ColumnNamme, ColumnOrder));
        }

        /// <summary>
        /// �[�J OrderByColumn ����
        /// </summary>
        /// <param name="column">���W��</param>
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
        /// ���� OrderByColumn ����
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
        /// �Ǧ^ OrderByColumn ���Ӽ�.
        /// </summary>
        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        /// <summary>
        /// �Ǧ^���w OrderByColumn ����.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OrderByColumn this[int index]
        {
            get { return list[index]; }
        }

        #region IEnumerable Members

        /// <summary>
        /// �^���i�H�v�@�d�ݳo�Ӫ��󤤭ӧO���l����C
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
        /// String�A��ܥثe�� Object�C
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
    ///  SqlModel OrderByColumn ����
    /// </summary>
    public class OrderByColumn
    {
        /// <summary>
        /// OrderByColumn ���� �غc�l
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="OrderSorting"></param>
        public OrderByColumn(string ColumnName, OrderSorting OrderSorting)
        {
            this.ColumnName = ColumnName;
            this.orderSorting = OrderSorting;
        }

        /// <summary>
        /// OrderByColumn ���� �غc�l
        /// </summary>
        /// <param name="ColumnName"></param>
        public OrderByColumn(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }

        private OrderSorting orderSorting = OrderSorting.Ascending;

        /// <summary>
        /// ��Ƥ��e�ܧ�
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// ���W��
        /// </summary>
        public readonly string ColumnName;

        /// <summary>
        /// ���w�ƧǪ��覡.
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