using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class StringExtensions.
/// </summary>
public static class StringExtensions
{
    // Pre-compiled regex patterns for better performance
    private static readonly Regex HexDigitRegex = new("[abcdefABCDEF\\d]+", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    private static readonly Regex AlphaNumericRegex = new("[^a-zA-Z0-9]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Convert a hex string to a .NET Color object.
    /// </summary>
    /// <param name="self">a hex string: "FFFFFF", "#000000"</param>
    /// <returns>Color.</returns>
    public static Color HexStringToColor(this string self)
    {
        var hc = self.ExtractHexDigits();

        if (hc.Length != 6)
        {
            return Color.Empty;
        }

        try
        {
            var r = int.Parse(hc[..2], NumberStyles.HexNumber);
            var g = int.Parse(hc.Substring(2, 2), NumberStyles.HexNumber);
            var b = int.Parse(hc.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromArgb(r, g, b);
        }
        catch (FormatException)
        {
            return Color.Empty;
        }
    }

    /// <summary>
    /// Coverts the string2 numeric.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <returns>System.Int32.</returns>
    public static int CovertString2Numeric(this string _self)
    {
        var result = new StringBuilder();

        foreach (var character in _self.EnsureNotNull())
        {
            if (char.IsDigit(character))
            {
                result.Append(character);
            }
        }

        return int.TryParse(result.ToString(), out var output) ? output : 0;
    }

    /// <summary>
    /// Determines whether the specified self is float.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns><c>true</c> if the specified self is float; otherwise, <c>false</c>.</returns>
    public static bool IsFloat(this string self) =>
        float.TryParse(self, out _);

    /// <summary>
    /// Determines whether the specified self is decimal.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns><c>true</c> if the specified self is decimal; otherwise, <c>false</c>.</returns>
    public static bool IsDecimal(this string self) =>
        decimal.TryParse(self, out _);

    /// <summary>
    /// Determines whether the specified self is integer.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns><c>true</c> if the specified self is integer; otherwise, <c>false</c>.</returns>
    public static bool IsInteger(this string self) =>
        int.TryParse(self, out _);

    /// <summary>
    /// Increases the string2 long.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <param name="summand">The summand.</param>
    /// <returns>System.String.</returns>
    public static string IncreaseString2Long(this string self, long summand)
    {
        // Find the position where numeric characters start at the end
        var codePos = self.Length;
        for (var i = self.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(self[i]))
            {
                codePos = i + 1;
                break;
            }
        }

        // If codePos is still at self.Length, all characters are digits
        if (codePos == self.Length)
        {
            var num = long.Parse(self) + summand;
            return num.ToString();
        }

        var numberString = self[codePos..];
        var num_value = long.Parse(numberString) + summand;

        // Format the number with leading zeros to maintain the original format
        return self[..codePos] + num_value.ToString().PadLeft(numberString.Length, '0');
    }

    /// <summary>
    /// Words the wrap.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <param name="width">The width.</param>
    /// <returns>System.String.</returns>
    public static string WordWrap(this string self, int width)
    {
        if (self.Length <= width)
            return self;

        var result = new StringBuilder();
        var currentPosition = 0;

        while (currentPosition < self.Length)
        {
            var lineEnd = Math.Min(currentPosition + width, self.Length);

            if (lineEnd == self.Length)
            {
                result.Append(self[currentPosition..]);
                break;
            }

            // Find the last space before the line end
            var lastSpace = self.LastIndexOf(' ', lineEnd - 1, Math.Min(width, lineEnd - currentPosition));

            if (lastSpace > currentPosition)
            {
                result.AppendLine(self.Substring(currentPosition, lastSpace - currentPosition));
                currentPosition = lastSpace + 1;
            }
            else
            {
                result.AppendLine(self.Substring(currentPosition, width));
                currentPosition += width;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Extract only the hex digits from a string.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns>System.String.</returns>
    public static string ExtractHexDigits(this string self)
    {
        var result = new StringBuilder();

        foreach (var character in self.EnsureNotNull())
        {
            if (HexDigitRegex.IsMatch(character.ToString()))
            {
                result.Append(character);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Chops the specified self (removes last character or CRLF).
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns>System.String.</returns>
    public static string Chop(this string self) =>
        self.Length switch
        {
            0 => self,
            1 => "",
            _ when self.EndsWith("\r\n") => self[..^2],
            _ => self[..^1]
        };

    /// <summary>
    /// Counts the lines.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns>System.Int32.</returns>
    public static int CountLines(this string self)
    {
        if (string.IsNullOrEmpty(self))
            return 0;

        var count = 1;
        var index = 0;

        while ((index = self.IndexOf(Environment.NewLine, index)) != -1)
        {
            count++;
            index += Environment.NewLine.Length;
        }

        return count;
    }

    /// <summary>
    /// Firsts the line.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns>System.String.</returns>
    public static string FirstLine(this string self)
    {
        var pos = self.IndexOf(Environment.NewLine);
        return pos == -1 ? self : self[..pos];
    }

    /// <summary>
    /// Determines whether [is alpha numeric] [the specified s].
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns><c>true</c> if [is alpha numeric] [the specified s]; otherwise, <c>false</c>.</returns>
    public static bool IsAlphaNumeric(this string s) =>
        !AlphaNumericRegex.IsMatch(s);

    /// <summary>
    /// Lasts the character at.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <param name="pos">The position.</param>
    /// <returns>System.Char.</returns>
    public static char LastCharAt(this string self, int pos) =>
        self[self.Length + pos];

    /// <summary>
    /// Makes the proper (first character uppercase, rest lowercase).
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns>System.String.</returns>
    public static string MakeProper(this string s) =>
        s.Length == 0 ? s : char.ToUpper(s[0]) + s[1..].ToLower();

    /// <summary>
    /// Removes the last character.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <returns>System.String.</returns>
    public static string RemoveLastChar(this string self) =>
        self.Length >= 1 ? self[..^1] : self;

    /// <summary>
    /// Splits at specified positions.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <param name="positions">The positions.</param>
    /// <returns>IEnumerable&lt;System.String&gt;.</returns>
    public static IEnumerable<string> SplitAt(this string self, params int[] positions)
    {
        if (positions is null or [])
            yield return self;
        else
        {
            var poses = positions.Where(p => p > 0 && p < self.Length).Distinct().OrderBy(n => n).ToList();

            if (poses.Count == 0)
                yield return self;
            else
            {
                var pos = 0;
                foreach (var nextPos in poses)
                {
                    yield return self.Substring(pos, nextPos - pos);
                    pos = nextPos;
                }

                if (pos < self.Length)
                    yield return self[pos..];
            }
        }
    }

    /// <summary>
    /// Trims the end.
    /// </summary>
    /// <param name="self">The self.</param>
    /// <param name="len">The length.</param>
    /// <returns>System.String.</returns>
    public static string TrimEnd(this string self, int len) =>
        self.Length > len ? self[..^len] : string.Empty;

    /// <summary>
    /// Replaces the last occurrence of fromStr with toStr.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="fromStr">From string.</param>
    /// <param name="toStr">To string.</param>
    /// <returns>System.String.</returns>
    public static string ReplaceLastOf(this string str, string fromStr, string toStr)
    {
        var lastIndexOf = str.LastIndexOf(fromStr);
        if (lastIndexOf < 0)
            return str;

        return str[..lastIndexOf] + toStr + str[(lastIndexOf + fromStr.Length)..];
    }

    /// <summary>
    /// Converts to camel case with separator.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>System.String.</returns>
    public static string ToCamelWithSeparator(this string source, char separator)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        var sb = new StringBuilder();
        sb.Append(source[0]);

        for (var i = 1; i < source.Length; i++)
        {
            if (char.IsUpper(source[i]) && !char.IsUpper(source[i - 1]))
                sb.Append(separator);

            sb.Append(source[i]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Determine if specified strings are null or empty or the strings are equal.
    /// </summary>
    /// <param name="a">String to compare</param>
    /// <param name="b">String to compare</param>
    /// <param name="comparisonType">StringComparison</param>
    /// <returns>True if the strings are null or empty or the strings are equal</returns>
    public static bool IsNullOrEmptyOrEquals(this string? a, string? b, StringComparison comparisonType) =>
        string.IsNullOrEmpty(a) ? string.IsNullOrEmpty(b) : string.Equals(a, b, comparisonType);

    /// <summary>
    /// Wraps text to specified width.
    /// </summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="width">The width.</param>
    /// <returns>System.String.</returns>
    public static string Wrap(this string text, int width)
    {
        if (width < 1)
            return text;

        var sb = new StringBuilder();
        var lines = text.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            if (line.Length <= width)
            {
                sb.AppendLine(line);
                continue;
            }

            var pos = 0;
            while (pos < line.Length)
            {
                var len = Math.Min(width, line.Length - pos);
                len = BreakLine(line, pos, len);

                sb.AppendLine(line.Substring(pos, len));
                pos += len;

                // Trim leading whitespace
                while (pos < line.Length && char.IsWhiteSpace(line[pos]))
                    pos++;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Locates position to break the given line so as to avoid breaking words.
    /// </summary>
    /// <param name="text">String that contains line of text</param>
    /// <param name="pos">Index where line of text starts</param>
    /// <param name="max">Maximum line length</param>
    /// <returns>The modified line length</returns>
    public static int BreakLine(this string text, int pos, int max)
    {
        // Find last whitespace in line
        var i = max - 1;

        while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
            i--;

        if (i < 0)
            return max; // No whitespace found; break at maximum length

        // Find start of whitespace
        while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
            i--;

        // Return length of text before whitespace
        return i + 1;
    }

    /// <summary>
    /// Removes the specified chars.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <param name="chars">The chars.</param>
    /// <returns>System.String.</returns>
    public static string Remove(this string s, IEnumerable<char> chars)
    {
        var charsToRemove = new HashSet<char>(chars);
        return new string(s.Where(c => !charsToRemove.Contains(c)).ToArray());
    }

    /// <summary>
    /// Gets the bytes from hex string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="start">The start.</param>
    /// <param name="numBytes">The number bytes.</param>
    /// <returns>System.Byte[].</returns>
    public static byte[] GetBytes(this string value, int start, int numBytes)
    {
        var bytes = new byte[numBytes];

        for (var i = 0; i < numBytes; i++)
        {
            var hexPair = value.Substring(start + (i * 2), 2);
            bytes[i] = byte.Parse(hexPair, NumberStyles.HexNumber);
        }

        return bytes;
    }

    /// <summary>
    /// Converts to enum.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_self">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>T.</returns>
    public static T ToEnum<T>(this string? _self, T defaultValue) where T : struct, Enum =>
        string.IsNullOrEmpty(_self) || !Enum.TryParse<T>(_self, true, out var result) 
            ? defaultValue 
            : result;

    /// <summary>
    /// Replaces the format item in a specified string with the string
    /// representation of a corresponding object in a specified array.
    /// </summary>
    /// <param name="str">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of str in which the format items have been replaced by
    /// the string representation of the corresponding objects in args.</returns>
    public static string Format(this string str, params object?[] args) =>
        string.Format(str, args);

    /// <summary>
    /// Checks whether a type implements a method with the given name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodName">Name of the method.</param>
    /// <returns><c>true</c> if the specified method name has method; otherwise, <c>false</c>.</returns>
    public static bool HasMethod<T>(string methodName) =>
        typeof(T).GetMethod(methodName) is not null;

    /// <summary>
    /// Convert string to snake casing.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>System.String.</returns>
    public static string ToSnakeCase(this string input) =>
        string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();

    /// <summary>
    /// Converts string to a capitalized one where delimiter starts a new capitalization.
    /// Default delimiter is a dot.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>System.String.</returns>
    public static string ToCapitalized(this string _self, char delimiter = '.')
    {
        var lower = _self.ToLower();
        var result = new StringBuilder();
        var parts = lower.SplitAndKeep(delimiter);

        foreach (var part in parts)
        {
            var characters = part.ToCharArray();

            for (var i = 0; i < characters.Length; i++)
            {
                if (char.IsLetter(characters[i]))
                {
                    characters[i] = char.ToUpper(characters[i]);
                    break;
                }
            }

            result.Append(new string(characters));
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts string to a capitalized one where each space starts a new capitalization.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <returns>System.String.</returns>
    public static string ToCapitalizedFirstLetter(this string _self) =>
        _self.ToCapitalized(' ');

    /// <summary>
    /// Splits the string by delimiter, but keeps the delimiter.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <param name="delimiters">The delimiters.</param>
    /// <returns>IList&lt;System.String&gt;.</returns>
    public static IList<string> SplitAndKeep(this string _self, params char[] delimiters)
    {
        var parts = new List<string>();

        if (string.IsNullOrEmpty(_self))
            return parts;

        var firstChar = 0;

        while (firstChar < _self.Length)
        {
            var lastChar = _self.IndexOfAny(delimiters, firstChar);

            if (lastChar < 0)
            {
                // No more delimiters, add the rest
                parts.Add(_self[firstChar..]);
                break;
            }

            if (lastChar > firstChar)
                parts.Add(_self[firstChar..lastChar]); // Part before the delimiter

            parts.Add(_self[lastChar].ToString()); // The delimiter
            firstChar = lastChar + 1;
        }

        return parts;
    }

    /// <summary>
    /// Ensures path ends with directory separator.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>System.String.</returns>
    public static string ToEndingWithDirectorySeparator(this string? path)
    {
        if (string.IsNullOrEmpty(path))
            return path ?? string.Empty;

        return path.EndsWith(Path.DirectorySeparatorChar.ToString())
            ? path
            : path + Path.DirectorySeparatorChar;
    }

    /// <summary>
    /// Extracts environment or country parameters from query string format (e.g., "?environment=development").
    /// </summary>
    /// <param name="queryString">The query string.</param>
    /// <param name="parameterName">The parameter name.</param>
    /// <returns>The parameter value or null.</returns>
    public static string? ExtractQueryParameter(this string? queryString, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(queryString) || string.IsNullOrWhiteSpace(parameterName))
            return null;

        try
        {
            var cleanQuery = queryString.TrimStart('?');
            var parameters = cleanQuery.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var parameter in parameters)
            {
                var keyValue = parameter.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (keyValue.Length == 2 && keyValue[0].Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return Uri.UnescapeDataString(keyValue[1]);
                }
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
