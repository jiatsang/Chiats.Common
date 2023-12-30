// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Chiats
{
    /// <summary>
    /// 處理 XmlDocument 相關的輔助程式庫.
    /// </summary>
    public static class XmlExtensions
    {
        #region Private Internal Function

        private static string GetStringValue<T>(T Value)
        {
            // Type value_type = typeof(T);
            if (Value is string)
            {
                return (string)(object)Value;
            }
            if (Value is DateTime)
            {
                return ((DateTime)(object)Value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return Value.ToString();
        }

        #endregion Private Internal Function

        /// <summary>
        /// 取得節點物件中指定名稱的屬性值.
        /// </summary>
        /// <typeparam name="T">回傳值型態</typeparam>
        /// <param name="node">取得屬性的節點物件</param>
        /// <param name="name">屬性值名稱</param>
        /// <returns>屬性值</returns>
        public static T GetAttributeValue<T>(this XmlNode node, string name)
        {
            return GetAttributeValue<T>(node, name, default(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasAttribute(this XmlNode node, string name)
        {
            return (node.Attributes[name] != null);
        }
#if (!DEBUG)
         [System.Diagnostics.DebuggerNonUserCode]
#endif
        /// <summary>
        /// 取得節點物件中指定名稱的屬性值. 如果屬性值名稱不存在時則回傳屬性預設值(defaultvalue)
        /// </summary>
        /// <typeparam name="T">回傳值型態</typeparam>
        /// <param name="node">取得屬性的節點物件</param>
        /// <param name="name">屬性值名稱</param>
        /// <param name="default_value">屬性預設值</param>
        /// <returns>屬性值</returns>
        public static T GetAttributeValue<T>(this XmlNode node, string name, T default_value)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            Type return_type = typeof(T);

            XmlAttribute attr = node.Attributes[name];

            if (attr == null)
                return default_value;

            object return_value = null;

            if (return_type.IsEnum)
            {
                return_value = Enum.Parse(return_type, attr.Value, true);
                return (T)return_value;
            }

            if (return_type.IsPrimitive)
            {
                MethodInfo method = return_type.GetMethod("Parse", new Type[] { typeof(string) });
                if (method != null && method.IsStatic)
                {
                    return_value = method.Invoke(null, new object[] { attr.Value });
                    return (T)return_value;
                }
                throw new Exception(string.Format("TODO: Convert String To Primitive({0})", return_type.Name));
            }

            switch (return_type.Name)
            {
                case "String":
                    return (T)((object)attr.Value);

                case "DateTime":
                    DateTime t;
                    return (DateTime.TryParse(attr.Value, out t)) ? (T)((object)t) : default_value;

                case "Type":
                    return (T)((object)Type.GetType(attr.Value));
            }

            throw new NotImplementedException("Not Implement Type Converter in MavsLib ('" + return_type.Name + "')");
        }

        /// <summary>
        /// 更新或建立 XmlAttribute 並指定新值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="Value"></param>
        public static XmlAttribute CreateXmlAttribute<T>(this XmlNode node, string name, T Value)
        {
            Debug.Assert(node != null && name != null);
            XmlAttribute atr = node.OwnerDocument.CreateAttribute(name);
            atr.Value = GetStringValue<T>(Value);
            node.Attributes.Append(atr);
            return atr;
        }

        /// <summary>
        /// 更新或建立 XmlAttribute 並指定新值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="Value"></param>
        public static void CreateOrUpdateXmlAttribute<T>(this XmlNode node, string name, T Value)
        {
            Debug.Assert(node != null && name != null);
            XmlAttribute atr = node.Attributes[name];
            if (atr != null)
            {
                atr.Value = GetStringValue<T>(Value);
            }
            else
            {
                atr = node.OwnerDocument.CreateAttribute(name);
                atr.Value = GetStringValue<T>(Value);
                node.Attributes.Append(atr);
            }
        }

        /// <summary>
        /// 在指定的 XML 節點下.建立一個新的 XML 節點.
        /// </summary>
        /// <param name="node">指定的 XML 節點</param>
        /// <param name="tag_name">TAG 名稱</param>
        /// <param name="innerText">內含字串值</param>
        /// <returns>新建的XML 節點物件</returns>
        public static XmlNode CreateXmlNode(this XmlNode node, string tag_name, string innerText)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            XmlNode result;
            if (!(node is XmlDocument))
            {
                result = node.OwnerDocument.CreateElement(tag_name);
            }
            else
            {
                result = ((XmlDocument)node).CreateElement(tag_name);
            }

            if (innerText != null)
                result.InnerText = innerText;

            node.AppendChild(result);
            return result;
        }

        /// <summary>
        /// 在指定的 XML 節點下.建立一個新的 XML 節點.
        /// </summary>
        /// <param name="node">指定的 XML 節點</param>
        /// <param name="tag_name">TAG 名稱</param>
        /// <returns>新建的XML 節點物件</returns>
        public static XmlNode CreateXmlNode(this XmlNode node, string tag_name)
        {
            return CreateXmlNode(node, tag_name, null);
        }

        /// <summary>
        /// 取得節點物件中指定名稱的屬性值. 如果屬性值名稱不存在時, 則回傳屬性預設值(defaultvalue)
        /// </summary>
        /// <param name="node">取得屬性的節點物件</param>
        /// <param name="name">屬性值名稱</param>
        /// <param name="defaultvalue">屬性預設值</param>
        /// <returns>屬性值</returns>
        public static string GetAttributeValue(this XmlNode node, string name, string defaultvalue)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            XmlAttribute attr = node.Attributes[name];
            return (attr != null) ? attr.Value : defaultvalue;
        }

        /// <summary>
        /// 取得節點物件中指定名稱的屬性值. 如果屬性值名稱不存在時則回傳 null
        /// </summary>
        /// <param name="node">取得屬性的節點物件</param>
        /// <param name="name">屬性值名稱</param>
        /// <returns>屬性值</returns>
        public static string GetAttributeValue(this XmlNode node, string name)
        {
            return GetAttributeValue(node, name, null);
        }

        /// <summary>
        /// 建立個新的屬性節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, long _value)
        {
            CreateNewAttribute(node, name, _value.ToString());
        }

        /// <summary>
        /// 建立個新的屬性節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, float _value)
        {
            CreateNewAttribute(node, name, _value.ToString());
        }

        /// <summary>
        /// 建立個新的屬性節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, double _value)
        {
            CreateNewAttribute(node, name, _value.ToString());
        }

        /// <summary>
        /// 建立個新的屬性節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, DateTime _value)
        {
            CreateNewAttribute(node, name, _value.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 建立個新的屬性節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, string _value)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
            attribute.Value = _value;
            node.Attributes.Append(attribute);
        }

        /// <summary>
        /// 建立個新的節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tag_name"></param>
        /// <param name="namespaceuri"></param>
        /// <returns></returns>
        public static XmlNode CreateNewNode(this XmlNode node, string tag_name, string namespaceuri)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            XmlNode result;
            if (namespaceuri == null)
            {
                result = node.OwnerDocument.CreateElement(tag_name);
            }
            else
            {
                result = node.OwnerDocument.CreateElement(tag_name, namespaceuri);
            }

            node.AppendChild(result);
            return result;
        }

        /// <summary>
        /// 建立個新的節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tag_name"></param>
        /// <param name="namespaceuri"></param>
        /// <param name="innerText"></param>
        /// <returns></returns>
        public static XmlNode CreateNewNode(this XmlNode node, string tag_name, string namespaceuri, string innerText)
        {
            XmlNode result = CreateNewNode(node, tag_name, namespaceuri);
            result.InnerText = innerText;
            return result;
        }

        /// <summary>
        /// 建立個新的節點物件
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tag_name"></param>
        /// <returns></returns>
        public static XmlNode CreateNewNode(this XmlNode node, string tag_name)
        {
            return CreateNewNode(node, tag_name, null);
        }
    }
}