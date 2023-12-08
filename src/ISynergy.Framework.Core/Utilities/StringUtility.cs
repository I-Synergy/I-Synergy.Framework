using ISynergy.Framework.Core.Extensions;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Class StringUtility.
/// </summary>
public static class StringUtility
{
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
public class StringWriterUtf8 : StringWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringWriterUtf8"/> class.
    /// </summary>
    public StringWriterUtf8()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringWriterUtf8"/> class.
    /// </summary>
    /// <param name="formatProvider">An <see cref="T:System.IFormatProvider"></see> object that controls formatting.</param>
    public StringWriterUtf8(IFormatProvider formatProvider) : base(formatProvider)
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
        var flag = char.IsDigit(source[0]);
        var item = new StringBuilder();

        foreach (var character in source.EnsureNotNull())
        {
            if (flag != char.IsDigit(character) || character == '\'')
            {
                if (!string.IsNullOrEmpty(item.ToString()))
                    result.Add(item.ToString());

                item.Clear();
                flag = char.IsDigit(character);
            }

            if (char.IsDigit(character) || char.IsLetter(character))
            {
                item.Append(character);
            }
        }

        result.Add(item.ToString());
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

        var intX = 0;
        var intY = 0;
        var result = 0;

        for (var i = 0; i < str1.Count && i < str2.Count; i++)
        {
            bool is1Integer = false;

            if (int.TryParse(str1[i].ToString(), out int res1))
            {
                intX = res1;
                is1Integer = true;
            }

            bool is2Integer = false;

            if (int.TryParse(str2[i].ToString(), out int res2))
            {
                intY = res2;
                is2Integer = true;
            }

            //checking --the data comparision
            if (!is2Integer && !is1Integer)    //both are strings
            {
                result = str1[i].CompareTo(str2[i]);
            }
            else if (is2Integer && is1Integer) //both are intergers
            {
                if (intX == intY)
                {
                    if (str1[i].ToString().Length < str2[i].ToString().Length)
                        result = 1;
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
                        result = intX.CompareTo(intY);
                }
            }
            else
            {
                result = str1[i].CompareTo(str2[i]);
            }

            if (result == 0)
                continue;
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
        var sw = new StringWriterUtf8(CultureInfo.CurrentCulture);
        var writer = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true, IndentChars = "\t", Encoding = Encoding.UTF8 });
        xml.WriteTo(writer);
        writer.Flush();
        writer.Dispose();
        return sw.ToString();
    }
}
