using System;
using System.Collections.Generic;
using System.Text;

namespace Chiats.SQL
{
    /// <summary>
    ///  集中 SQL 的 Keyword , 作為比較及輸出的 Token 
    /// </summary>
    public static class TokenKeys
    {
        public static TokenKey FROM = new StringTokenKey("FROM");
        public static TokenKey SELECT = new StringTokenKey("SELECT");
        public static TokenKey INSERT = new StringTokenKey("INSERT");
        public static TokenKey DELETE = new StringTokenKey("DELETE");
        public static TokenKey UPDATE = new StringTokenKey("UPDATE");
        public static TokenKey AS = new StringTokenKey("AS");
        public static TokenKey WHERE = new StringTokenKey("WHERE");
        public static TokenKey TOP = new StringTokenKey("TOP");
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class TokenKey
    {
        public abstract bool Compare(string key);
    }

    public class StringTokenKey : TokenKey
    {
        public string key = null;
        public StringTokenKey(string key) { this.key = key; }

        public override bool Compare(string key)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(this.key, key);
        }
        public override string ToString()
        {
            return key;
        }

    }

}

