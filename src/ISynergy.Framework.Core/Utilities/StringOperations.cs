using ISynergy.Framework.Core.Extensions;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ISynergy.Framework.Core.Utilities;

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
