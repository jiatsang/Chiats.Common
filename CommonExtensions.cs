// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------
using Chiats.Data;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Chiats
{
    /// <summary>
    /// 系統相關的輔助程式庫.
    /// </summary>
    public static class CommonExtensions
    {

        public static bool IsSubclassOfGeneric(this Type checkType, Type genericRawType)
        {
            while (checkType != null && checkType != typeof(object))
            {
                var cur = checkType.IsGenericType ? checkType.GetGenericTypeDefinition() : checkType;
                if (genericRawType == cur)
                {
                    return true;
                }
                checkType = checkType.BaseType;
            }
            return false;
        }

        public static Type GetSubclassOfGeneric(this Type checkType, Type genericRawType)
        {
            while (checkType != null && checkType != typeof(object))
            {
                var cur = checkType.IsGenericType ? checkType.GetGenericTypeDefinition() : checkType;
                if (genericRawType == cur)
                {
                    return checkType;
                }
                checkType = checkType.BaseType;
            }
            return null;
        }


        /// <summary>
        /// 檢查是否為   Anonymous Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymous(this Type type)
        {
            if (type.IsGenericType)
            {
                var d = type.GetGenericTypeDefinition();
                if (d.IsClass && d.IsSealed && d.Attributes.HasFlag(TypeAttributes.NotPublic))
                {
                    var attributes = d.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 檢查是否為   Anonymous Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsAnonymousType<T>(this T instance)
        {
            if (instance != null)
            {
                if (typeof(T) == typeof(Type))
                    return IsAnonymous(instance as Type);
                return IsAnonymous(instance.GetType());
            }
            return false;
        }

        private static bool property_setvalue(object newObject, string name, object val)
        {
            // 有指定  __count  更新其值為 更新欄位數            ;
            var target = (from p in newObject.GetType().GetProperties() where p.Name == name select p).FirstOrDefault();
            if (target != null)
            {
                if (target.CanWrite)
                {
                    target.SetValue(newObject, val.ChangeTypeEx(target.PropertyType), null);
                    return true;
                }
                else
                {
                    // 因為 Anonymous Type PropertyInfo 是無法寫入變數內容.要改用 FieldInfo (<ID>i__Field)
                    var f = newObject.GetType().GetField($"<{name}>i__Field", BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (f != null)
                    {
                        f.SetValue(newObject, val.ChangeTypeEx(target.PropertyType));
                        return true;
                    }
                }
            }
            return false;
        }
        public static T QuerySet<T>(this object _object, T result = default(T))
        {
            if (result == null)
                throw new ArgumentException($"QuerySet {nameof(result)} 不可以為 null 值");

            if (_object != null)
            {
                Type SourceType = _object.GetType();
                Type TargetType = result.GetType();

                //var TargetProperties = TargetType.GetProperties();

                int __count = 0;

                foreach (PropertyInfo prop in SourceType.GetProperties())
                {
                    if (prop.Name.StartsWith("__")) continue;   // 不處理  __ 開頭的變數  and IgnoreColumn
                    if (prop.GetCustomAttribute<IgnoreColumnAttribute>(true) == null)
                    {
                        // TODO: ColumnPassAttribute and ColumnValueConvert
                        if (property_setvalue(result, prop.Name, prop.GetValue(_object, null)))
                            __count++;
                    }
                }
                property_setvalue(result, "__count", __count);
            }
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="MinValue"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public static T Range<T>(this T Value, T MinValue, T MaxValue) where T : IComparable
        {
            if (Value.CompareTo(MinValue) < 0) return MinValue;
            if (Value.CompareTo(MaxValue) > 0) return MaxValue;
            return Value;
        }

        public static T Range<T>(this T Value, T MinValue, T MaxValue, T defaultValue) where T : IComparable
        {
            if (Value.CompareTo(MinValue) < 0 || Value.CompareTo(MaxValue) > 0) return defaultValue;
            return Value;
        }

        /// <summary>
        /// 檢查是否為英文字母.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsLetter(this char ch)
        {
            System.Globalization.UnicodeCategory Category = char.GetUnicodeCategory(ch);
            return (Category == UnicodeCategory.LowercaseLetter ||
                    Category == UnicodeCategory.UppercaseLetter);
        }

        /// <summary>
        /// 檢查是否為英文字母或數字.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsLetterOrDigit(this char ch)
        {
            System.Globalization.UnicodeCategory Category = char.GetUnicodeCategory(ch);
            return (Category == UnicodeCategory.LowercaseLetter ||
                    Category == UnicodeCategory.UppercaseLetter ||
                    Category == UnicodeCategory.DecimalDigitNumber);
        }

        /// <summary>
        /// 檢查是否為數字.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsDigit(this char ch)
        {
            System.Globalization.UnicodeCategory Category = char.GetUnicodeCategory(ch);
            return (Category == UnicodeCategory.DecimalDigitNumber);
        }

        /// <summary>
        ///  檢查指定類別是否有包含定義 AttributeType 的屬性類別.
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="Type">指定類別</param>
        /// <param name="Inherit"></param>
        /// <returns></returns>
        public static bool HasCustomAttribute<T>(this MemberInfo Type, bool Inherit = true) where T : Attribute
        {
            var args = Type.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length > 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="ParameterInfo"></param>
        /// <param name="Inherit">是不含所有 Inherit Attribute 屬性類別</param>
        /// <returns></returns>
        public static bool HasCustomAttribute<T>(this ParameterInfo ParameterInfo, bool Inherit = true) where T : Attribute
        {
            var args = ParameterInfo.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length > 0);
        }

        /// <summary>
        /// 回傳是否奝包含定義 AttributeType 的屬性類別.
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="FieldInfo"></param>
        /// <param name="Inherit">是不含所有 Inherit Attribute 屬性類別</param>
        /// <returns></returns>
        public static bool HasCustomAttributeEx<T>(this FieldInfo FieldInfo, bool Inherit = true) where T : Attribute
        {
            var args = FieldInfo.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length > 0);
        }

        // 已有 System.Reflection.CustomAttributeExtensions 取代下列功能
        /// <summary>
        /// 取得指定類別是否有包含定義 AttributeType 的屬性類別.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <param name="Inherit">是不含所有 Inherit Attribute 屬性類別</param>
        /// <returns></returns>
        public static T GetCustomAttributeEx<T>(this MemberInfo Type, bool Inherit = true) where T : Attribute
        {
            var args = Type.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length == 0) ? default(T) : (T)args[0];
        }

        /// <summary>
        /// 取得指定類別是否有包含定義 AttributeType 的屬性類別
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="Asy">組件</param>
        /// <param name="Inherit">是不含所有 Inherit Attribute 屬性類別</param>
        /// <returns></returns>
        public static T GetCustomAttributeEx<T>(this Assembly Asy, bool Inherit = true) where T : Attribute
        {
            var args = Asy.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length == 0) ? default(T) : (T)args[0];
        }

        /// <summary>
        /// 取得指定類別是否有包含定義 AttributeType 的屬性類別.
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="Type">型態物件</param>
        /// <param name="Inherit">是不含所有 Inherit Attribute 屬性類別</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this ParameterInfo Type, bool Inherit = true) where T : Attribute
        {
            var args = Type.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length == 0) ? default(T) : (T)args[0];
        }


        public static T GetCustomAttribute<T>(this Type Type, bool Inherit = true) where T : Attribute
        {
            var args = Type.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length == 0) ? default(T) : (T)args[0];
        }

        /// <summary>
        ///  取得指定類別是否有包含定義 Attribute 的屬性類別.
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="FieldInfo"></param>
        /// <param name="Inherit">是不含所有 Inherit Attribute 屬性類別</param>
        /// <returns></returns>
        public static T GetCustomAttributeEx<T>(this FieldInfo FieldInfo, bool Inherit = true) where T : Attribute
        {
            var args = FieldInfo.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length == 0) ? default(T) : (T)args[0];
        }

        /// <summary>
        ///  取得指定類別是否有包含定義 Attribute 的屬性類別.
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="Type"></param>
        /// <param name="Inherit">是不含所有 Inherit Attribute 屬性類別</param>
        /// <returns></returns>
        public static T GetCustomAttributeEx<T>(this Type Type, bool Inherit = true) where T : Attribute
        {
            var args = Type.GetCustomAttributes(typeof(T), Inherit);
            return (args.Length == 0) ? default(T) : (T)args[0];
        }

        /// <summary>
        /// 取得指定名稱的 Enum 物件值. 會尋找 StringValueAttribute 或 Enum名稱 所定義相符的字串.
        /// 無法轉換時會產生 InvalidConvertException
        /// </summary>
        /// <typeparam name="T">定義 AttributeType 的屬性類別</typeparam>
        /// <param name="val"></param>
        /// <param name="refvalue"></param>
        /// <returns>是否</returns>
        public static bool EnumConvert<T>(this string val, out T refvalue)
        {
            bool result = false;
            refvalue = default(T); // initiailize value

            Type TargetType = typeof(T);
            FieldInfo[] objs = TargetType.GetFields();
            foreach (FieldInfo f in objs)
            {
                if (f.IsStatic)
                {
                    StringValueAttribute sva = f.GetCustomAttributeEx<StringValueAttribute>();
                    if ((sva != null && StringComparer.OrdinalIgnoreCase.Equals(sva.Value, val)) ||  /* 以 StringValue 屬性定義為主 */
                        StringComparer.OrdinalIgnoreCase.Equals(f.Name, val))
                    {
                        refvalue = (T)f.GetValue(null);
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 取得指定名稱的 Enum 物件值. 會尋找 StringValueAttribute 或 Enum 名稱 所定義相符的字串.
        /// </summary>
        /// <typeparam name="T">Enum 物件型別</typeparam>
        /// <param name="val">物件值</param>
        /// <returns>StringValueAttribute 或 Enum名稱.</returns>
        public static string EnumConvert<T>(this T val)
        {
            Type TargetType = typeof(T);
            FieldInfo[] objs = TargetType.GetFields();
            foreach (FieldInfo f in objs)
            {
                if (f.IsStatic)
                {
                    if (val.Equals(f.GetValue(null)))
                    {
                        StringValueAttribute sva = f.GetCustomAttributeEx<StringValueAttribute>();
                        if (sva != null)
                            return sva.Value;
                        return f.Name;
                    }
                }
            }
            throw new InvalidConvertException("Cann't Convert Enum {0} - {1}", TargetType.Name, val);
        }

        /// <summary>
        /// 取得指定名稱的 Enum 物件值. 會尋找 StringValueAttribute 或 Enum 名稱 所定義相符的字串.
        /// </summary>
        /// <typeparam name="T">Enum 類別</typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T EnumConvert<T>(this string val) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            Type TargetType = typeof(T);
            if (val != null)
            {
                FieldInfo[] objs = TargetType.GetFields();
                foreach (FieldInfo f in objs)
                {
                    if (f.IsStatic)
                    {
                        StringValueAttribute sva = f.GetCustomAttributeEx<StringValueAttribute>();
                        if (sva != null && StringComparer.OrdinalIgnoreCase.Equals(sva.Value, val)) /* 以 StringValue 屬性定義為主 */
                            return (T)f.GetValue(null);

                        if (StringComparer.OrdinalIgnoreCase.Equals(f.Name, val))
                            return (T)f.GetValue(null);
                    }
                }
            }
            throw new InvalidConvertException("Cann't Convert {0} to {1}", val, TargetType.Name);
        }


        public static object EnumConvert(Type TargetType, string val)
        {
            if (!TargetType.IsEnum)
            {
                throw new ArgumentException($"{TargetType} must be an enumerated type");
            }
            if (val != null)
            {
                var objs = TargetType.GetFields();
                foreach (var f in objs)
                {
                    if (f.IsStatic)
                    {
                        StringValueAttribute sva = f.GetCustomAttributeEx<StringValueAttribute>();
                        if (sva != null && StringComparer.OrdinalIgnoreCase.Equals(sva.Value, val)) /* 以 StringValue 屬性定義為主 */
                            return f.GetValue(null);

                        if (StringComparer.OrdinalIgnoreCase.Equals(f.Name, val))
                            return f.GetValue(null);
                    }
                }
            }
            throw new InvalidConvertException("Cann't Convert {0} to {1}", val, TargetType.Name);
        }

        /// <summary>
        /// 取得指定名稱的 Enum 物件值. 會尋找 StringValueAttribute 或 Enum名稱 所定義相符的字串.
        /// </summary>
        /// <typeparam name="T">Enum 類別</typeparam>
        /// <param name="val"></param>
        /// /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public static T EnumConvert<T>(this string val, T defaultvalue) where T : struct 
        {
            Type TargetType = typeof(T);
            if (val != null)
            {
                FieldInfo[] objs = TargetType.GetFields();
                foreach (FieldInfo f in objs)
                {
                    if (f.IsStatic)
                    {
                        StringValueAttribute sva = f.GetCustomAttributeEx<StringValueAttribute>();
                        if (sva != null && StringComparer.OrdinalIgnoreCase.Equals(sva.Value, val)) /* 以 StringValue 屬性定義為主 */
                            return (T)f.GetValue(null);

                        if (StringComparer.OrdinalIgnoreCase.Equals(f.Name, val))
                            return (T)f.GetValue(null);
                    }
                }
            }
            return defaultvalue;
        }

        public static A GetEnumCustomAttribute<T, A>(T val) where T : struct where A : Attribute
        {

            return (from f in typeof(T).GetFields()
                    where f.IsStatic && val.Equals(f.GetValue(null))
                    select f.GetCustomAttributeEx<A>()).FirstOrDefault();
        }


        /// <summary>
        /// 取得指定欄位之值
        /// </summary>
        /// <typeparam name="T">傳回值型別</typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="ColumnName">指定欄位名稱</param>
        /// <returns></returns>
        public static T GetValueEx<T>(this DataRow row, string ColumnName)
        {
            return GetValueEx<T>(row, ColumnName, default(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        public static T GetValueEx<T>(this DataRow row, int ColumnIndex)
        {
            return GetValueEx<T>(row, ColumnIndex, default(T));
        }

        /// <summary>
        /// 取得指定欄位之值
        /// </summary>
        /// <param name="row">DataRow</param>
        /// <param name="ColumnName">指定欄位名稱</param>
        /// <returns></returns>
        public static object GetValueEx(this DataRow row, string ColumnName)
        {
            object val = row[ColumnName];
            if (val == DBNull.Value) return null;
            return val;
        }

        /// <summary>
        /// 取得指定欄位之值
        /// </summary>
        /// <param name="row">DataRow</param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        public static object GetValueEx(this DataRow row, int ColumnIndex)
        {
            object val = row[ColumnIndex];
            if (val == DBNull.Value) return null;
            return val;
        }

        /// <summary>
        /// 取值支援指定多種型別
        /// </summary>
        /// <typeparam name="T">傳回值型別</typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="ColumnName">指定欄位名稱</param>
        /// <param name="default_value">預設值</param>
        /// <returns></returns>
        public static T GetValueEx<T>(this DataRow row, string ColumnName, T default_value)
        {
            object val = row[ColumnName];
            if (val == null || val == DBNull.Value)
            {
                return default_value;
            }
            return val.ChangeType<T>();
        }

        /// <summary>
        /// 取值支援指定多種型別
        /// </summary>
        /// <typeparam name="T">傳回值型別</typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="ColumnIndex"></param>
        /// <param name="default_value">預設值</param>
        /// <returns></returns>
        public static T GetValueEx<T>(this DataRow row, int ColumnIndex, T default_value)
        {
            object val = row[ColumnIndex];
            if (val == null || val == DBNull.Value)
            {
                return default_value;
            }
            return val.ChangeType<T>();
        }

        // OleDbDataReader

        /// <summary>
        ///  取值 支援指定多種型別
        /// </summary>
        /// <typeparam name="T">傳回值型別</typeparam>
        /// <param name="reader">IDataReader</param>
        /// <param name="ColumnName">指定欄位名稱</param>
        /// <returns></returns>
        public static T GetValueEx<T>(this IDataReader reader, string ColumnName)
        {
            int index = reader.GetOrdinal(ColumnName);
            return GetValueEx<T>(reader, index);
        }

        /// <summary>
        ///  取值 支援指定多種型別
        /// </summary>
        /// <typeparam name="T">傳回值型別</typeparam>
        /// <param name="reader">IDataReader</param>
        /// <param name="index">指定欄位索引值</param>
        /// <returns></returns>
        public static T GetValueEx<T>(this IDataReader reader, int index)
        {
            object val = reader.GetValue(index);
            if (val == null || val == DBNull.Value)
                return default(T);
            return val.ChangeType<T>();
        }
#if (!DEBUG)
        [System.Diagnostics.DebuggerNonUserCode]
#endif
        /// <summary>
        /// 型別自動型別函數, 支援 DBNull.Value 的自轉型為 NULL
        /// </summary>
        /// <typeparam name="T">傳回值型別</typeparam>
        /// <param name="val">原始值</param>
        /// <param name="_default"></param>
        /// <returns></returns>
        public static T ChangeType<T>(this object val, T _default = default(T), IFormatProvider formatProvider = null)
        {
            if (val == null || val == DBNull.Value)
                return _default;
            else if (val is string && (string)val == "")
            {
                // 空字串的處理方法
                if (typeof(T) == typeof(string))
                    return (T)val;
                return _default;
            }
            else
            {
                Type converionType = typeof(T);

                // 檢查是否為 Nullable 的型別
                if (converionType.IsGenericType && converionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    converionType = Nullable.GetUnderlyingType(converionType);

                if (val.GetType() == converionType) return (T)val;
                if (converionType == typeof(Type)) return (T)val;
                try
                {
                    return (T)(Convert.ChangeType(val, converionType, formatProvider) ?? _default);
                }
                catch (Exception /*ex*/)
                {
                    // 對轉型失敗時 回傳  _default
                    Debug.Print("Exception Found in ChangeType({1} to {2}) '{0}' return default value {3} ", val, val.GetType().Name, converionType.Name, _default);
                    return _default;
                }
            }
        }

        /// <summary>
        /// 強迫自動轉型, 在可能的範圍內執行自動轉型的工作. (會自動轉換 DBNull 為 null)
        /// </summary>
        /// <param name="converionType"></param>
        /// <param name="val"></param>
        /// <param name="_default"></param>
        /// <returns></returns>
        [DebuggerNonUserCode()]
        public static object ChangeTypeEx(this object val, Type converionType, object _default = null, IFormatProvider formatProvider = null)
        {

            // a thread-safe way to hold default instances created at run-time
            if (val == null || val == DBNull.Value)
                return converionType.IsValueType ? Activator.CreateInstance(converionType) : _default;

            var valType = val.GetType();

            if (val is string && (string)val == "")
            {
                // 空字串的處理方法
                if (converionType == typeof(string))
                    return val;
                return converionType.IsValueType ? Activator.CreateInstance(converionType) : _default;
            }
            else
            {
                if (converionType.IsGenericType && converionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    converionType = Nullable.GetUnderlyingType(converionType);
                }

                if (valType == converionType) return val;
                if (converionType == typeof(Type)) return val;
                if (converionType.IsEnum)
                {
                    if (valType == typeof(int)) return val;

                    if (valType == typeof(string))
                    {
                        try
                        {
                            return Enum.Parse(converionType, (string)val, true);
                        }
                        catch
                        {
                            return _default; // Convert Failed.
                        }
                    }
                }
                try
                {
                    return Convert.ChangeType(val, converionType, formatProvider) ?? _default;
                }
                catch
                {
                    return _default; // Convert Failed.
                }
            }
        }

        /// <summary>
        /// 限制值的大小範圍.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MinValue"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public static T RangeTo<T>(this T value, T MinValue, T MaxValue) where T : IComparable<T>
        {
            if (value.CompareTo(MinValue) < 0) return MinValue;
            if (value.CompareTo(MaxValue) > 0) return MaxValue;

            return value;
        }

        ///// <summary>
        ///// 字串轉回物件
        ///// </summary>
        ///// <param name="SerializeStringObject"></param>
        ///// <returns></returns>
        //public static object DeserializeBinaryString(string SerializeStringObject)
        //{
        //    // full buffer byte array
        //    byte[] bu = BinaryStringToBytes(SerializeStringObject);

        //    // Deserialize Object
        //    BinaryFormatter myBinaryFormatter = new BinaryFormatter();
        //    using (MemoryStream oStream = new MemoryStream(bu))
        //    {
        //        return myBinaryFormatter.Deserialize(oStream);
        //    }
        //}

        ///// <summary>
        ///// 傳回是否為字串物件(可轉回物件的字串)
        ///// </summary>
        ///// <param name="SerializeObject"></param>
        ///// <returns></returns>
        //public static bool IsSerializableObject(object SerializeObject)
        //{
        //    if (SerializeObject != null)
        //    {
        //        return SerializeObject.GetType().GetCustomAttributeEx<SerializableAttribute>() != null;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// 物件轉字串
        ///// </summary>
        ///// <param name="SerializeObject"></param>
        ///// <returns></returns>
        //public static string SerializeBinaryString(object SerializeObject)
        //{
        //    //建立物件
        //    using (MemoryStream oStream = new MemoryStream())
        //    {
        //        BinaryFormatter myBinaryFormatter = new BinaryFormatter();
        //        myBinaryFormatter.Serialize(oStream, SerializeObject);
        //        return BytesToString(oStream.GetBuffer());
        //    }
        //}

        /// <summary>
        /// full buffer byte array
        /// </summary>
        /// <param name="bytestring"></param>
        /// <returns></returns>
        public static byte[] BinaryStringToBytes(string bytestring)
        {
            Func<char, int> GetByte = (ch) =>
            {
                if (ch >= 0x30 && ch <= 0x39) { return (ch - 0x30); }
                else if (ch >= 97 && ch <= 102) { return (ch - 87); }  /* a-f */
                return 0;
            };

            byte[] bu = new byte[bytestring.Length / 2];
            for (int index = 0; index < bu.Length; index++)
            {
                int location = index * 2;
                int x = GetByte(bytestring[location]);
                int y = GetByte(bytestring[location + 1]);

                bu[index] = (byte)((x << 4) | y);
            }
            return bu;
        }

        /// <summary>
        /// 字元陣列轉字串物件
        /// </summary>
        /// <param name="byte_array"></param>
        /// <param name="byte_format"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] byte_array, string byte_format = "{0:x2}")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in byte_array) sb.AppendFormat(byte_format, b);
            return sb.ToString();
        }

        /// <summary>
        /// 字串陣列轉字串物件(以 , 區隔)
        /// </summary>
        /// <param name="s_array"></param>
        /// <param name="split_char"></param>
        /// <returns></returns>
        public static string ArrayToString(string[] s_array, char split_char = ',')
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in s_array)
            {
                if (sb.Length > 0)
                    sb.Append(split_char);
                sb.Append(s);
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 標示 Enum 型別的物件值所相對應的文字。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// 所相對應的文字內容
        /// </summary>
        public string Value { get; private set; } = null;

        /// <summary>
        /// 所相對應的文字說明內容
        /// </summary>
        public string Description { get; private set; } = null;

        /// <summary>
        /// StringValueAttribute 建構子
        /// </summary>
        /// <param name="Value">所相對應的文字內容</param>
        /// <param name="Description">文字說明</param>
        public StringValueAttribute(string Value, string Description = "")
        {
            this.Value = Value;
            this.Description = Description;
        }
    }
}