using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
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

            var r = hc.Substring(0, 2);
            var g = hc.Substring(2, 2);
            var b = hc.Substring(4, 2);

            Color color;
            try
            {
                var ri
                   = int.Parse(r, NumberStyles.HexNumber);
                var gi
                   = int.Parse(g, NumberStyles.HexNumber);
                var bi
                   = int.Parse(b, NumberStyles.HexNumber);

                color = Color.FromArgb(ri, gi, bi);
            }
            catch
            {
                return Color.Empty;
            }

            return color;
        }

        /// <summary>
        /// Coverts the string2 numeric.
        /// </summary>
        /// <param name="_self">The self.</param>
        /// <returns>System.Int32.</returns>
        public static int CovertString2Numeric(this string _self)
        {
            var result = string.Empty;

            foreach (var character in _self)
            {
                if (char.IsDigit(character))
                {
                    result += character;
                }
            }

            if (int.TryParse(result, out var output))
            {
                return output;
            }

            return 0;
        }

        /// <summary>
        /// Determines whether the specified self is float.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns><c>true</c> if the specified self is float; otherwise, <c>false</c>.</returns>
        public static bool IsFloat(this string self)
        {
            try
            {
                return float.TryParse(self, out var output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified self is decimal.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns><c>true</c> if the specified self is decimal; otherwise, <c>false</c>.</returns>
        public static bool IsDecimal(this string self)
        {
            try
            {
                return decimal.TryParse(self, out var output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified self is integer.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns><c>true</c> if the specified self is integer; otherwise, <c>false</c>.</returns>
        public static bool IsInteger(this string self)
        {
            try
            {
                return int.TryParse(self, out var output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Increases the string2 long.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="summand">The summand.</param>
        /// <returns>System.String.</returns>
        public static string IncreaseString2Long(this string self, long summand)
        {
            var numberString = string.Empty;
            var codePos = self.Length;

            // Go back from end of id to the begin while a char is a number
            for (var i = self.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(self.Substring(i, 1).First()))
                {
                    // if found char isdigit set the position one element back
                    codePos--;
                }
                else
                {
                    // if we found a char we can break up
                    break;
                }
            }

            if (codePos < self.Length)
            {
                // the for-loop has found at least one numeric char at the end
                numberString = self.Substring(codePos, self.Length - codePos);
            }

            if (numberString.Length == 0)
            {
                // no number was found at the and so we simply add the summand as string
                return self + summand;
            }
            else
            {
                var num = long.Parse(numberString) + summand;
                return self.Substring(0, codePos) + num.ToString();
            }
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
            // dont need to do anything

            var sResult = self;
            var iLineNO = width;

            while (sResult.Length >= iLineNO)
            {
                // temp holder for current string char
                int iEn;
                // work backwards from the max len to 1 looking for a space
                for (iEn = iLineNO; iEn >= 1; iEn += -1)
                {
                    string sChar = sResult.Substring(iEn, 1);
                    // found a space
                    if (sChar == " ")
                    {
                        sResult = sResult.Remove(iEn, 1);
                        // Remove the space
                        sResult = sResult.Insert(iEn, Environment.NewLine);
                        // insert a line feed here,
                        iLineNO += width;
                        // increment
                        break;
                    }
                }
            }

            return sResult;
        }

        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string ExtractHexDigits(this string self)
        {
            // remove any characters that are not digits (like #)
            var isHexDigit = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            var result = new StringBuilder();

            foreach (var character in self.EnsureNotNull())
                if (isHexDigit.IsMatch(character.ToString())) 
                    result.Append(character.ToString());

            return result.ToString();
        }

        //identical to Ruby's chop
        /// <summary>
        /// Chops the specified self.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string Chop(this string self)
        {
            if (self.Length == 0)
                return self;

            if (self.Length == 1)
                return "";

            if (self.LastCharAt(-1) == '\n' && self.LastCharAt(-2) == '\r')
                return self.Substring(0, self.Length - 2);
            else
                return self.Substring(0, self.Length - 1);
        }

        /// <summary>
        /// Counts the lines.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.Int32.</returns>
        public static int CountLines(this string self)
        {
            var pos = 0;
            var count = 1;
            do
            {
                pos = self.IndexOf(Environment.NewLine, pos);
                if (pos >= 0)
                    ++count;
                ++pos;
            } while (pos > 0);

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
            if (pos == -1)
                return self;

            return self.Substring(0, pos);
        }

        /// <summary>
        /// Determines whether [is alpha numeric] [the specified s].
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns><c>true</c> if [is alpha numeric] [the specified s]; otherwise, <c>false</c>.</returns>
        public static bool IsAlphaNumeric(this string s)
        {
            return !new Regex("[^a-zA-Z0-9]").IsMatch(s);
        }

        /// <summary>
        /// Lasts the character at.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="pos">The position.</param>
        /// <returns>System.Char.</returns>
        public static char LastCharAt(this string self, int pos)
        {
            return self[self.Length + pos];
        }

        /// <summary>
        /// Makes the proper.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.String.</returns>
        public static string MakeProper(this string s)
        {
            return s[0].ToString().ToUpper() + s.Substring(1).ToLower();
        }

        /// <summary>
        /// Removes the last character.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string RemoveLastChar(this string self)
        {
            if (self.Length >= 1)
                return self.Remove(self.Length - 1, 1);
            else
                return self;
        }

        /// <summary>
        /// Splits at.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="positions">The positions.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> SplitAt(this string self, params int[] positions)
        {
            var ret = new List<string>();

            if (positions is null) { ret.Add(self); return ret; }

            var poses = positions.Distinct().OrderBy(n => n).ToList();

            var indicesToRemove = new Queue<int>();
            var total = poses.Count;
            var i = 0;
            while (i < total)
            {
                if ((poses[i] <= 0) || (poses[i] >= self.Length))
                    indicesToRemove.Enqueue(poses[i]);
                ++i;
            }

            while (indicesToRemove.Count > 0)
                poses.Remove(indicesToRemove.Dequeue());

            switch (poses.Count)
            {
                case 0:
                    ret.Add(self); return ret;
                case 1:
                    ret.Add(self.Substring(0, poses[0])); ret.Add(self.Substring(poses[0], self.Length - poses[0])); return ret;
                default:
                    var pos1 = 0;
                    var len = poses[0];
                    ret.Add(self.Substring(pos1, len));

                    pos1 = poses[0];
                    for (var j = 1; j <= poses.Count; ++j)
                    {
                        if (j == poses.Count)
                            len = self.Length - poses[j - 1];
                        else
                            len = poses[j] - poses[j - 1];

                        ret.Add(self.Substring(pos1, len));

                        if (j < poses.Count)
                            pos1 = poses[j];
                    }
                    return ret;
            }
        }

        /// <summary>
        /// Trims the end.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="len">The length.</param>
        /// <returns>System.String.</returns>
        public static string TrimEnd(this string self, int len)
        {
            return self.Substring(0, self.Length - len);
        }

        /// <summary>
        /// Replaces the last of.
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

            var leading = str.Substring(0, lastIndexOf);
            var charsToEnd = str.Length - (lastIndexOf + fromStr.Length);
            var trailing = str.Substring(lastIndexOf + fromStr.Length, charsToEnd);

            return leading + toStr + trailing;
        }

        /// <summary>
        /// Converts to camelwithseparator.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>System.String.</returns>
        public static string ToCamelWithSeparator(this string source, char separator)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            var sourceArray = source.ToCharArray();

            var sb = new StringBuilder(sourceArray[0].ToString());

            for (var i = 1; i <= sourceArray.Length - 1; i++) // Index 0 is skipped.
            {
                if (char.IsUpper(sourceArray[i]) && !char.IsUpper(sourceArray[i - 1]))
                    sb.Append(separator);

                sb.Append(sourceArray[i]);
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
        public static bool IsNullOrEmptyOrEquals(this string a, string b, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(a))
                return string.IsNullOrEmpty(b);

            return string.Equals(a, b, comparisonType);
        }

        /// <summary>
        /// Wraps the specified width.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="width">The width.</param>
        /// <returns>System.String.</returns>
        public static string Wrap(this string a, int width)
        {
            int pos, next;
            var sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return a;

            // Parse each line of text
            for (pos = 0; pos < a.Length; pos = next)
            {
                // Find end of line
                var eol = a.IndexOf(Environment.NewLine, pos);

                if (eol == -1)
                    next = eol = a.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        var len = eol - pos;

                        if (len > width)
                            len = BreakLine(a, pos, width);

                        sb.Append(a, pos, len);
                        sb.Append(Environment.NewLine);

                        // Trim whitespace following break
                        pos += len;

                        while (pos < eol && char.IsWhiteSpace(a[pos]))
                            pos++;

                    } while (eol > pos);
                }
                else sb.Append(Environment.NewLine); // Empty line
            }

            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
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
            return new string(s.Where(c => !chars.Contains(c)).ToArray());
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start.</param>
        /// <param name="NumBytes">The number bytes.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytes(this string value, int start, int NumBytes)
        {
            var g = new StringBuilder(value);
            var Bytes = new byte[NumBytes];
            string temp;
            var CharPos = start;
            for (var i = 0; i < NumBytes; i++)
            {
                temp = g[CharPos++].ToString();
                temp += g[CharPos++].ToString();
                Bytes[i] = byte.Parse(temp, NumberStyles.HexNumber);
            }
            return Bytes;
        }

        /// <summary>
        /// Converts to enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_self">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>T.</returns>
        public static T ToEnum<T>(this string _self, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(_self))
                return defaultValue;

            return Enum.TryParse<T>(_self, true, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string
        /// representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="str">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of str in which the format items have been replaced by
        /// the string representation of the corresponding objects in args.</returns>
        public static string Format(this string str, params object[] args) =>
            String.Format(str, args);

        /// <summary>
        /// Checks whether a type implements a method with the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName">Name of the method.</param>
        /// <returns><c>true</c> if the specified method name has method; otherwise, <c>false</c>.</returns>
        public static bool HasMethod<T>(string methodName)
        {
            try
            {
                var type = typeof(T);
                return type.GetMethod(methodName) is not null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }

        /// <summary>
        /// Convert string to snake casing.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string ToSnakeCase(this string input) =>
            string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();


        /// <summary>
        /// Converts string to a capitalized one where delimiter starts an new capitalization.
        /// Default delimiter is a dot.
        /// </summary>
        /// <param name="_self">The self.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>System.String.</returns>
        public static string ToCapitalized(this string _self, char delimiter = '.')
        {
            _self = _self.ToLower();

            var result = new StringBuilder();
            var parts = _self.SplitAndKeep(delimiter);

            foreach (var part in parts)
            {
                var characters = part.ToCharArray();

                for (int i = 0; i < characters.Length; i++)
                {
                    if (!characters[i].Equals(' ') && !characters[i].ToString().IsInteger())
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
        /// Converts string to a capitalized one where delimiter starts an new capitalization.
        /// Default delimiter is a space.
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
            
            if (!string.IsNullOrEmpty(_self))
            {
                var firstChar = 0;
                do
                {
                    var lastChar = _self.IndexOfAny(delimiters, firstChar);
                    
                    if (lastChar >= 0)
                    {
                        if (lastChar > firstChar)
                            parts.Add(_self.Substring(firstChar, lastChar - firstChar)); //part before the delimiter
                        
                        parts.Add(new string(_self[lastChar], 1));//the delimiter
                        
                        firstChar = lastChar + 1;
                        continue;
                    }

                    //No delimiters were found, but at least one character remains. Add the rest and stop.
                    parts.Add(_self.Substring(firstChar, _self.Length - firstChar));
                    break;

                } while (firstChar < _self.Length);
            }

            return parts;
        }
    }
}
