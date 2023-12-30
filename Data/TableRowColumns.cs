using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chiats.Data
{
    public sealed class TableRowColumns : IEnumerable<TableRowColumn>
    {
        private TableRowColumn[] Columns = null;
        public TableRowColumns(IEnumerable<TableRowColumn> NewColumns)
        {
            int index = 0;
            Columns = new TableRowColumn[NewColumns.Count()];
            foreach (var NewColumn in NewColumns)
            {
                Columns[index] = NewColumn;
                index++;
            }
        }

        public TableRowColumns(TableRowColumn[] NewColumns)
        {

            this.Columns = new TableRowColumn[NewColumns.Length];
            int index = 0;
            Columns = new TableRowColumn[NewColumns.Length];
            foreach (var NewColumn in NewColumns)
            {
                Columns[index] = NewColumn;
                index++;
            }
        }

        internal TableRowColumns Copy()
        {
            List<TableRowColumn> NewColumns = new List<TableRowColumn>();

            foreach (var column in NewColumns)
                NewColumns.Add(new TableRowColumn(column));

            return new TableRowColumns(NewColumns);

        }

        public bool ContainKey(string ColumnName)
        {
            foreach (var column in this.Columns)
            {
                if (string.Compare(column.Name, ColumnName, true) == 0) return true;
            }
            return false;
        }

        public TableRowColumn this[int index]
        {
            get
            {
                if (Columns == null)
                    return null;
                return Columns[index];
            }
        }

        public TableRowColumn this[string name]
        {
            get
            {
                if (Columns != null)
                {
                    int index = GetIndexByName(name);
                    if (index != -1)
                    {
                        return Columns[index];
                    }
                }
                return null;
            }
        }

        public int Count
        {
            get
            {
                if (Columns == null)
                    return 0;
                return Columns.Length;
            }
        }

        public int GetIndexByName(string ColumnName)
        {
            if (Columns != null)
            {
                for (int index = 0; index < Columns.Length; index++)
                {
                    if (string.Compare(Columns[index].Name, ColumnName, true) == 0)
                        return index;
                }
            }
            return -1;
        }

        public string GetNameByIndex(int index)
        {
            if (index >= 0 && index < Count)
                return Columns[index].Name;
            return null;
        }

        /// <summary>
        /// 傳回會逐一查看集合的列舉程式。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TableRowColumn> GetEnumerator()
        {
            foreach (var Column in Columns)
                yield return Column;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Columns.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("TableColumns(Count={0})", Count);
        }
    }
}
