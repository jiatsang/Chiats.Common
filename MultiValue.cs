//// ------------------------------------------------------------------------
//// Chiats Common&Data Library V4.1.21 (2021/08)
//// Chiats@Studio(http://www.chiats.com/Common)
//// Design&Coding By Chia Tsang Tsai
//// Copyright(C) 2005-2022 Chiats@Studio All 
//// ------------------------------------------------------------------------

//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Chiats
//{
//    /// <summary>
//    /// 多重字串列. Ex "'a1','a340','300'" 以逗號(,)區隔的字串陣列. 子串字串允許含特別字元.
//    /// </summary>
//    public sealed class MultiStringValue : MultiValue<string>
//    {
//        /// <summary>
//        /// 預設建構子
//        /// </summary>
//        public MultiStringValue() { }

//        /// <summary>
//        /// 建構子
//        /// </summary>
//        /// <param name="values"></param>
//        public MultiStringValue(string values) : base(values) { }

//        /// <summary>
//        /// 建構子
//        /// </summary>
//        /// <param name="values"></param>
//        public MultiStringValue(params string[] values) : base(values) { }

//        /// <summary>
//        /// 文字串剖析程序.
//        /// </summary>
//        /// <param name="list"></param>
//        /// <param name="values"></param>
//        protected override void StringPaser(List<string> list, string values)
//        {
//            string[] vals = StringExtensions.AutoSplit(values, this.SplitSymbol);
//            AddRange(vals);
//        }

//        /// <summary>
//        /// 建構子
//        /// </summary>
//        /// <param name="objs"></param>
//        public MultiStringValue(IEnumerable<string> objs) : base(objs) { }

//        /// <summary>
//        /// 資料型別和字串間轉換, ConvertTo and ConvertFrom
//        /// </summary>
//        /// <param name="val"></param>
//        /// <returns></returns>
//        protected override string ConvertTo(string val) { return val; }

//        /// <summary>
//        /// 轉換物件為文字串列的方法. 資料型別和字串間轉換, ConvertTo and ConvertFrom
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        protected override string ConvertFrom(string obj)
//        {
//            if (obj != null && obj.IndexOf('\'') != -1)
//            {
//                obj = obj.Replace("'", "''");  // 幾二個單引號取代一個, 防止和字串單引號衝突.
//            }
//            return $"'{obj}'";// 輸出 'string'
//        }

//        protected override bool Compare(string val, string val2)
//        {
//            return (string.Compare(val, val2, true) == 0);
//        }

//        public string Keys { get { return this.ToString(); } }

//        /// <summary>
//        /// 轉換 MultiNumValue 為陣列.
//        /// </summary>
//        /// <param name="d"></param>
//        /// <returns></returns>
//        public static implicit operator string[] (MultiStringValue d) { return d.ToArray(); }
//    }

//    /// <summary>
//    /// 多重字串列. Ex "a1,a340,300" 以逗號(,)區隔的字串陣列. 子串字串不允許含特別字元 如逗號
//    /// </summary>
//    /// <remarks>
//    /// 如要使用要含有 特別字元時 , 可以使用  StringExtensions.Split
//    /// </remarks>
//    public sealed class MultiNameValue : MultiValue<string>
//    {
//        /// <summary>
//        /// 預設建構子
//        /// </summary>
//        public MultiNameValue() { }

//        /// <summary>
//        /// 建構子
//        /// </summary>
//        /// <param name="values"></param>
//        public MultiNameValue(string values) : base(values) { }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="values"></param>
//        public MultiNameValue(params string[] values) : base(values) { }

//        // public MultiNameValue(string[] values) : base(values) { }

//        /// <summary>
//        /// 建構子
//        /// </summary>
//        /// <param name="objs"></param>
//        public MultiNameValue(IEnumerable<string> objs) : base(objs) { }

//        protected override string ConvertTo(string val)
//        {
//            return val.Trim();
//        }

//        protected override string ConvertFrom(string obj)
//        {
//            return obj.Trim();
//        }

//        protected override bool Compare(string val, string val2)
//        {
//            return (string.Compare(val, val2, true) == 0);
//        }

//        public string Keys { get { return this.ToString(); } }

//        public bool Contains(string ColumnName)
//        {
//            foreach (var v_1 in this)
//            {
//                if (string.Compare(ColumnName, v_1, true) == 0) return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// 轉換 MultiNumValue 為陣列.
//        /// </summary>
//        /// <param name="d"></param>
//        /// <returns></returns>
//        public static implicit operator string[] (MultiNameValue d) { return d.ToArray(); }
//    }

//    /// <summary>
//    /// 多重數字列 以逗號(,)隔的數字陣列
//    /// </summary>
//    public sealed class MultiNumValue : MultiValue<int>
//    {
//        /// <summary>
//        /// 多重數值列建構子, 以逗號區隔各別數值欄位
//        /// </summary>
//        /// <param name="vals"></param>
//        public MultiNumValue(string vals) : base(vals) { }

//        /// <summary>
//        /// 多重數值列建構子
//        /// </summary>
//        /// <param name="value"></param>
//        public MultiNumValue(int value) { this.Add(value); }

//        /// <summary>
//        /// 資料型別轉換
//        /// </summary>
//        /// <param name="val"></param>
//        /// <returns></returns>
//        protected override int ConvertTo(string val)
//        {
//            int r = 0;
//            int.TryParse(val, out r);
//            return r;
//        }

//        protected override bool Compare(int val, int val2)
//        {
//            return (val == val2);
//        }

//        /// <summary>
//        /// 轉換 MultiNumValue 第一個值(int)
//        /// </summary>
//        /// <param name="d"></param>
//        /// <returns></returns>
//        public static implicit operator int(MultiNumValue d) { return d.GetFirstValue(); }

//        /// <summary>
//        /// 轉換 數值(int), 為 MultiNumValue
//        /// </summary>
//        /// <param name="d"></param>
//        /// <returns></returns>
//        public static implicit operator MultiNumValue(int d)
//        {
//            return new MultiNumValue(d);
//        }

//        /// <summary>
//        /// 轉換 MultiNumValue 為陣列.
//        /// </summary>
//        /// <param name="d"></param>
//        /// <returns></returns>
//        public static implicit operator int[] (MultiNumValue d) { return d.ToArray(); }
//    }

//    /// <summary>
//    /// 通用型別旳物件陣列. 以逗號(,)隔的通用型別陣列
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public abstract class MultiValue<T> : IEnumerable<T>
//    {
//        protected List<T> list = new List<T>();

//        /// <summary>
//        /// 建立一個空的物件陣列
//        /// </summary>
//        public MultiValue() { }

//        /// <summary>
//        /// 通用型別多重值列建構子
//        /// </summary>
//        /// <param name="values"></param>
//        public MultiValue(string values) { this.StringPaser(list, values); }

//        public MultiValue(params string[] values)
//        {
//            foreach (string s in values)
//                this.StringPaser(list, s);
//        }

//        public MultiValue(params T[] values)
//        {
//            AddRange(values);
//        }

//        /// <summary>
//        /// 物件轉換程序.
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public static implicit operator string(MultiValue<T> value)
//        {
//            return value.ToString();
//        }

//        /// <summary>
//        /// 通用型別多重值列建構子
//        /// </summary>
//        /// <param name="objs"></param>
//        public MultiValue(IEnumerable<T> objs) { this.AddRange(objs); }

//        /// <summary>
//        /// 文字串剖析程序.
//        /// </summary>
//        /// <param name="list"></param>
//        /// <param name="values"></param>
//        protected virtual void StringPaser(List<T> list, string values)
//        {
//            if (values != null)
//            {
//                string[] vals = values.Split(SplitSymbol);
//                foreach (var val in vals) list.Add(ConvertTo(val));
//            }
//        }

//        /// <summary>
//        /// 加入一串數值資料
//        /// </summary>
//        /// <param name="objs"></param>
//        public void AddRange(IEnumerable<T> objs)
//        {
//            foreach (var val in objs)
//            {
//                Add(val);
//            }
//        }

//        /// <summary>
//        /// 加入一個數值資料
//        /// </summary>
//        /// <param name="obj"></param>
//        public void Add(T obj)
//        {
//            list.Add(obj);
//        }

//        /// <summary>
//        /// 移除指定位置數值資料.
//        /// </summary>
//        /// <param name="index"></param>
//        public void RemoveAt(int index)
//        {
//            list.RemoveAt(index);
//        }

//        /// <summary>
//        /// 回傳數值列的個數
//        /// </summary>
//        public int Count { get { return list.Count; } }

//        /// <summary>
//        /// 傳回指定位置的數值. 超過索引位置時. 則回傳數值列最後一個數值.
//        /// </summary>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public T this[int index]
//        {
//            get
//            {
//                if (index >= 0 && index < list.Count)
//                {
//                    return list[index];
//                }
//                return GetLastValue();
//            }
//        }

//        /// <summary>
//        /// 取得最後一個位置的數值 如果無資料時回傳空值或 0
//        /// </summary>
//        /// <returns></returns>
//        protected T GetLastValue()
//        {
//            if (list.Count == 0)
//                return default(T);
//            return list[list.Count - 1];
//        }

//        /// <summary>
//        /// 取得第一個位置的數值. 如果無資料時回傳空值或 0
//        /// </summary>
//        /// <returns></returns>
//        protected T GetFirstValue()
//        {
//            if (list.Count == 0)
//                return default(T);
//            return list[0];
//        }

//        /// <summary>
//        /// 清空資料數值.
//        /// </summary>
//        public void Clear() { list.Clear(); }

//        /// <summary>
//        /// 資料型別和字串間轉換, ConvertTo and ConvertFrom
//        /// </summary>
//        /// <param name="val"></param>
//        /// <returns></returns>
//        protected abstract T ConvertTo(string val);

//        protected abstract bool Compare(T val, T val2);

//        /// <summary>
//        /// 取得使用的分隔符號. 逗號(,)
//        /// </summary>
//        protected virtual char SplitSymbol { get { return ','; } }

//        /// <summary>
//        /// 轉換物件為文字串列的方法. 資料型別和字串間轉換, ConvertTo and ConvertFrom
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        protected virtual string ConvertFrom(T obj) { return obj.ToString(); }

//        /// <summary>
//        /// 轉換 MultiNumValue 為文字字串, 並以逗號為區分數值列.
//        /// </summary>
//        /// <returns></returns>
//        public override string ToString()
//        {
//            if (list != null)
//            {
//                StringBuilder sb = new StringBuilder();
//                int index = 0;
//                foreach (T cc in list)
//                {
//                    if (index != 0)
//                    {
//                        sb.Append(SplitSymbol);
//                    }
//                    sb.Append(ConvertFrom(cc));
//                    index++;
//                }
//                return sb.ToString();
//            }
//            return "";
//        }

//        public int GetIndex(T val)
//        {
//            for (int i = 0; i < list.Count; i++)
//            {
//                if (Compare(val, list[i])) return i;
//            }
//            return -1;
//        }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <returns></returns>
//        public T[] ToArray()
//        {
//            return list.ToArray<T>();
//        }

//        /// <summary>
//        /// 傳回會逐一查看集合的列舉程式。
//        /// </summary>
//        /// <returns></returns>
//        public IEnumerator<T> GetEnumerator()
//        {
//            return list.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return list.GetEnumerator();
//        }
//    }
//}