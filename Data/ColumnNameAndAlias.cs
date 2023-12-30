using Chiats.SQL;
using System;
using System.Collections.Generic;
using System.Data;

namespace Chiats.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnFilter
    {
        /// <summary>
        /// Filter Column Name
        /// </summary>
        public string ColumnName;
        /// <summary>
        /// Filter Column Index
        /// </summary>
        public int ColumnIndex;
        /// <summary>
        /// Filter Column Value
        /// </summary>
        public object Value;

        public override string ToString()
        {
            return $"{ColumnName}({ColumnIndex}) = '{Value}'";
        }
    }

    public class ColumnNameAndAlias
    {
        public ColumnNameAndAlias(string ColumnDescription)
        {
            if (!string.IsNullOrEmpty(ColumnDescription))
            {
                int startLoc = 0;
                int endLoc = ColumnDescription.Length - 1;
                // 去除中括號
                if (ColumnDescription.StartsWith("[") && ColumnDescription.EndsWith("]"))
                {
                    startLoc++; endLoc--;
                }
                int dotLoc = ColumnDescription.IndexOf(".");
                if (dotLoc != -1)
                {
                    Alias = ColumnDescription.Substring(startLoc, dotLoc - startLoc);
                    Name = ColumnDescription.Substring(dotLoc + 1, ColumnDescription.Length - dotLoc - startLoc - 1);
                }
                else
                {
                    Alias = null;
                    Name = ColumnDescription.Substring(startLoc, endLoc - startLoc + 1);
                }
            }
        }
        public readonly string Alias;
        public readonly string Name;
    }



}
