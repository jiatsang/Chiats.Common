using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Chiats
{
    /// <summary>
    /// 資料屬性的編解碼公用程序
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// ParameterValue 資料屬性的編碼公用程序
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ControlChar"></param>
        /// <returns></returns>
        public static string EncodingParameterValueString(string name, char ControlChar = '$')
        {
            return EncodingStringValue(name, "\"|,");
        }

        /// <summary>
        /// StringValue 資料屬性的編碼公用程序
        /// </summary>
        /// <param name="value"></param>
        /// <param name="include_string"></param>
        /// <param name="ControlChar"></param>
        /// <returns></returns>
        public static string EncodingStringValue(string value, string include_string = "\"", char ControlChar = '$')
        {
            StringBuilder sb = new StringBuilder();
            if (value != null)
            {
                foreach (var ch in value)
                {
                    if (Char.IsControl(ch))
                        sb.AppendFormat("{0}{1:X2}", ControlChar, (int)ch);
                    else
                    {
                        if (ch != ControlChar && include_string.IndexOf(ch) == -1)
                            sb.Append(ch);
                        else
                            sb.AppendFormat("{0}{1:X2}", ControlChar, (int)ch);
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 資料屬性名稱編碼公用程序
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string EncodingName(string name)
        {
            // name
            // (sample_name) => $28sample_name$29
            // sample name => sample$20name
            return EncodingStringValue(name, "(){}[]\"':;<>=+-*/%^&$~`|\\ "); // PS: _#@ 不需要轉換
        }

        /// <summary>
        /// 名稱/ParameterValue/StringValue 資料屬性的解碼公用程序
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ControlChar"></param>
        /// <returns></returns>
        public static string DecodingString(string name, char ControlChar = '$')
        {
            Func<Match, string> DecodingNameReplace = match =>
            {
                StringBuilder sb = new StringBuilder();
                string number = match.Value.Substring(1);
                int code = int.Parse(number, System.Globalization.NumberStyles.HexNumber);
                sb.Append((char)code);
                return sb.ToString();
            };

            if (name != null && name.Contains(ControlChar))
            {
                Regex NameRegex = new Regex(string.Format("\\x{0:X2}([0-9A-F]{{2}})", (short)ControlChar), RegexOptions.IgnoreCase);
                return NameRegex.Replace(name, new MatchEvaluator(DecodingNameReplace));
            }

            return name;
        }
    }
}