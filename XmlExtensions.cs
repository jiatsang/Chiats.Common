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
    /// �B�z XmlDocument ���������U�{���w.
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
        /// ���o�`�I���󤤫��w�W�٪��ݩʭ�.
        /// </summary>
        /// <typeparam name="T">�^�ǭȫ��A</typeparam>
        /// <param name="node">���o�ݩʪ��`�I����</param>
        /// <param name="name">�ݩʭȦW��</param>
        /// <returns>�ݩʭ�</returns>
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
        /// ���o�`�I���󤤫��w�W�٪��ݩʭ�. �p�G�ݩʭȦW�٤��s�b�ɫh�^���ݩʹw�]��(defaultvalue)
        /// </summary>
        /// <typeparam name="T">�^�ǭȫ��A</typeparam>
        /// <param name="node">���o�ݩʪ��`�I����</param>
        /// <param name="name">�ݩʭȦW��</param>
        /// <param name="default_value">�ݩʹw�]��</param>
        /// <returns>�ݩʭ�</returns>
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
        /// ��s�Ϋإ� XmlAttribute �ë��w�s��
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
        /// ��s�Ϋإ� XmlAttribute �ë��w�s��
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
        /// �b���w�� XML �`�I�U.�إߤ@�ӷs�� XML �`�I.
        /// </summary>
        /// <param name="node">���w�� XML �`�I</param>
        /// <param name="tag_name">TAG �W��</param>
        /// <param name="innerText">���t�r���</param>
        /// <returns>�s�ت�XML �`�I����</returns>
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
        /// �b���w�� XML �`�I�U.�إߤ@�ӷs�� XML �`�I.
        /// </summary>
        /// <param name="node">���w�� XML �`�I</param>
        /// <param name="tag_name">TAG �W��</param>
        /// <returns>�s�ت�XML �`�I����</returns>
        public static XmlNode CreateXmlNode(this XmlNode node, string tag_name)
        {
            return CreateXmlNode(node, tag_name, null);
        }

        /// <summary>
        /// ���o�`�I���󤤫��w�W�٪��ݩʭ�. �p�G�ݩʭȦW�٤��s�b��, �h�^���ݩʹw�]��(defaultvalue)
        /// </summary>
        /// <param name="node">���o�ݩʪ��`�I����</param>
        /// <param name="name">�ݩʭȦW��</param>
        /// <param name="defaultvalue">�ݩʹw�]��</param>
        /// <returns>�ݩʭ�</returns>
        public static string GetAttributeValue(this XmlNode node, string name, string defaultvalue)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            XmlAttribute attr = node.Attributes[name];
            return (attr != null) ? attr.Value : defaultvalue;
        }

        /// <summary>
        /// ���o�`�I���󤤫��w�W�٪��ݩʭ�. �p�G�ݩʭȦW�٤��s�b�ɫh�^�� null
        /// </summary>
        /// <param name="node">���o�ݩʪ��`�I����</param>
        /// <param name="name">�ݩʭȦW��</param>
        /// <returns>�ݩʭ�</returns>
        public static string GetAttributeValue(this XmlNode node, string name)
        {
            return GetAttributeValue(node, name, null);
        }

        /// <summary>
        /// �إ߭ӷs���ݩʸ`�I����
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, long _value)
        {
            CreateNewAttribute(node, name, _value.ToString());
        }

        /// <summary>
        /// �إ߭ӷs���ݩʸ`�I����
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, float _value)
        {
            CreateNewAttribute(node, name, _value.ToString());
        }

        /// <summary>
        /// �إ߭ӷs���ݩʸ`�I����
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, double _value)
        {
            CreateNewAttribute(node, name, _value.ToString());
        }

        /// <summary>
        /// �إ߭ӷs���ݩʸ`�I����
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public static void CreateNewAttribute(this XmlNode node, string name, DateTime _value)
        {
            CreateNewAttribute(node, name, _value.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// �إ߭ӷs���ݩʸ`�I����
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
        /// �إ߭ӷs���`�I����
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
        /// �إ߭ӷs���`�I����
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
        /// �إ߭ӷs���`�I����
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