using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Utilities
{
    /// <summary>
    /// Class StringUtility.
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// Adds the decimal seperator.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AddDecimalSeperator()
        {
            return true;
        }

        /// <summary>
        /// Converts the string to decimal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="input">The input.</param>
        /// <param name="seperatoradded">if set to <c>true</c> [seperatoradded].</param>
        /// <returns>System.Decimal.</returns>
        public static decimal ConvertStringToDecimal(decimal value, string input, bool seperatoradded)
        {
            string placeholder;

            if (seperatoradded)
            {
                var culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
                placeholder = value.ToString() + culture.NumberFormat.CurrencyDecimalSeparator + input;
            }
            else
            {
                placeholder = value.ToString() + input;
            }

            if (placeholder.StartsWith("0") && placeholder.Length > 1)
            {
                placeholder = placeholder.Remove(0, 1);
            }

            if (decimal.TryParse(placeholder, out var result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Extension where encoded string is converted to a byte array.
        /// </summary>
        /// <typeparam name="TEncoding">The type of the t encoding.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytes<TEncoding>(string value) where TEncoding : Encoding, new() =>
            new TEncoding().GetBytes(value);
    }

    /// <summary>
    /// Class StringWriterUTF8.
    /// Implements the <see cref="StringWriter" />
    /// </summary>
    /// <seealso cref="StringWriter" />
    public class StringWriterUTF8 : StringWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringWriterUTF8"/> class.
        /// </summary>
        public StringWriterUTF8()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringWriterUTF8"/> class.
        /// </summary>
        /// <param name="formatProvider">An <see cref="T:System.IFormatProvider"></see> object that controls formatting.</param>
        public StringWriterUTF8(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        /// <summary>
        /// Gets the <see cref="T:System.Text.Encoding"></see> in which the output is written.
        /// </summary>
        /// <value>The encoding.</value>
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    /// <summary>
    /// Class AlphanumericStringComparer.
    /// Implements the <see cref="IComparer" />
    /// </summary>
    /// <seealso cref="IComparer" />
    public class AlphanumericStringComparer : IComparer
    {
        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        private static List<string> GetList(string source)
        {
            var result = new List<string>();
            var stringItem = "";
            var flag = char.IsDigit(source[0]);

            foreach (var c in source.EnsureNotNull())
            {
                if (flag != char.IsDigit(c) || c == '\'')
                {
                    if (!string.IsNullOrEmpty(stringItem))
                        result.Add(stringItem);

                    stringItem = "";
                    flag = char.IsDigit(c);
                }
                if (char.IsDigit(c))
                {
                    stringItem += c;
                }
                if (char.IsLetter(c))
                {
                    stringItem += c;
                }
            }

            result.Add(stringItem);
            return result;
        }

        /// <summary>
        /// Compares the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Int32.</returns>
        public int Compare(object x, object y)
        {
            if (!(x is string s1))
            {
                return 0;
            }


            if (!(y is string s2))
            {
                return 0;
            }

            if (s1 == s2)
            {
                return 0;
            }

            var len1 = s1.Length;
            var len2 = s2.Length;

            // Walk through two the strings with two markers.
            var str1 = GetList(s1);
            var str2 = GetList(s2);

            while (str1.Count != str2.Count)
            {
                if (str1.Count < str2.Count)
                {
                    str1.Add("");
                }
                else
                {
                    str2.Add("");
                }
            }

            var x1 = 0;
            var x2 = 0;
            var result = 0;

            for (var i = 0; i < str1.Count && i < str2.Count; i++)
            {
                var status = int.TryParse(str1[i].ToString(), out var res);

                bool s1Status;
                
                if (res == 0)
                {
                    var y1 = str1[i].ToString();
                    s1Status = false;
                }
                else
                {
                    x1 = Convert.ToInt32(str1[i].ToString());
                    s1Status = true;
                }

                status = int.TryParse(str2[i].ToString(), out res);

                bool s2Status;

                if (res == 0)
                {
                    var y2 = str2[i].ToString();
                    s2Status = false;
                }
                else
                {
                    x2 = Convert.ToInt32(str2[i].ToString());
                    s2Status = true;
                }

                //checking --the data comparision
                if (!s2Status && !s1Status)    //both are strings
                {
                    result = str1[i].CompareTo(str2[i]);
                }
                else if (s2Status && s1Status) //both are intergers
                {
                    if (x1 == x2)
                    {
                        if (str1[i].ToString().Length < str2[i].ToString().Length)
                        {
                            result = 1;
                        }
                        else if (str1[i].ToString().Length > str2[i].ToString().Length)
                            result = -1;
                        else
                            result = 0;
                    }
                    else
                    {
                        var st1ZeroCount = str1[i].ToString().Trim().Length - str1[i].ToString().TrimStart(new char[] { '0' }).Length;
                        var st2ZeroCount = str2[i].ToString().Trim().Length - str2[i].ToString().TrimStart(new char[] { '0' }).Length;
                        if (st1ZeroCount > st2ZeroCount)
                            result = -1;
                        else if (st1ZeroCount < st2ZeroCount)
                            result = 1;
                        else
                            result = x1.CompareTo(x2);
                    }
                }
                else
                {
                    result = str1[i].CompareTo(str2[i]);
                }
                if (result == 0)
                {
                    continue;
                }
                else
                    break;
            }
            return result;
        }
    }

    /// <summary>
    /// Class StringOperations.
    /// </summary>
    public static class StringOperations
    {
        /// <summary>
        /// Truncates at.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxWidth">The maximum width.</param>
        /// <returns>System.String.</returns>
        public static string TruncateAt(string text, int maxWidth)
        {
            var result = text;

            if (text.Length > maxWidth)
            {
                result = text.Substring(0, maxWidth);
            }

            return result;
        }

        /// <summary>
        /// Converts to csv.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>System.String.</returns>
        public static string ToCsv(params string[] items)
        {
            var sb = new StringBuilder(255);

            foreach (var i in items.EnsureNotNull())
            {
                sb.Append('"');
                sb.Append(i);
                sb.Append('"');
                sb.Append(',');
            }

            return sb.ToString().Chop();
        }

        /// <summary>
        /// xes the element to string.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>System.String.</returns>
        public static string XElementToString(XElement xml)
        {
            var sw = new StringWriterUTF8(CultureInfo.CurrentCulture);
            var writer = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true, IndentChars = "\t", Encoding = Encoding.UTF8 });
            xml.WriteTo(writer);
            writer.Flush();
            writer.Dispose();
            return sw.ToString();
        }
    }
}
