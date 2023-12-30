// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

namespace Chiats.SQL
{
    /// <summary>
    /// SQL Model Process Helper Function
    /// </summary>
    internal static class SQLHelper
    {
        /// <summary>
        /// 處理表格欄位名稱 - 特殊字元 ,
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string PackObjectName(string name, SqlOptions Options  = SqlOptions.None)
        {
            // TODO: 處理特殊字元
            string AllowStr = "_@*$#";
            if (name == null) return null;

            if (name.StartsWith("[") && name.EndsWith("]")) return ForceToLower(name, Options);

            foreach (char c in name)
            {
                if (!char.IsLetterOrDigit(c) && AllowStr.IndexOf(c) == -1)
                {
                    return $"[{ForceToLower(name, Options)}]";
                }
            }
            return ForceToLower(name, Options);
        }
        private static string ForceToLower(string name, SqlOptions Options)
        {
            if(Options.HasFlag(SqlOptions.LowerName))
                return name?.ToLower();
            return name;
        }
        /// <summary>
        /// 處理表格欄位名稱 - 特殊字元 ,  [Object Named]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string UnpackObjectName(string name)
        {
            if (name == null) return null;
            name = name.Trim();
            if (name == "") return null;  // empty string as null
            if (name.StartsWith("[") && name.EndsWith("]")) // 去除號
            {
                name = name.Substring(1, name.Length - 2);
            }
            name = name.Trim();
            if (name == "") return null;  // empty string as null
            return name;
        }
    }
}