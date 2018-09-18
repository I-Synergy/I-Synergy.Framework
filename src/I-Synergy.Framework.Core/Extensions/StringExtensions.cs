using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ISynergy.Extensions
{
    public static class StringExtensions
    {
        //identical to Ruby's chop
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

        public static int CountLines(this string self)
        {
            int pos = 0;
            int count = 1;
            do
            {
                pos = self.IndexOf(Environment.NewLine, pos);
                if (pos >= 0)
                    ++count;
                ++pos;
            } while (pos > 0);

            return count;
        }

        public static string FirstLine(this string self)
        {
            int pos = self.IndexOf(Environment.NewLine);
            if (pos == -1)
                return self;

            return self.Substring(0, pos);
        }

        public static bool IsAlphaNumeric(this string s)
        {
            return !(new Regex("[^a-zA-Z0-9]")).IsMatch(s);
        }

        public static char LastCharAt(this string self, int pos)
        {
            return self[self.Length + pos];
        }

        public static string MakeProper(this string s)
        {
            return s[0].ToString().ToUpper() + s.Substring(1).ToLower();
        }

        public static string RemoveLastChar(this string self)
        {
            if (self.Length >= 1)
                return self.Remove(self.Length - 1, 1);
            else
                return self;
        }

        public static IEnumerable<string> SplitAt(this string self, params int[] positions)
        {
            var ret = new List<string>();

            if (positions is null) { ret.Add(self); return ret; }

            var poses = positions.Distinct().OrderBy(n => n).ToList();

            var indicesToRemove = new Queue<int>();
            var total = poses.Count();
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
                    var sb = new StringBuilder(255);
                    var pos1 = 0;
                    var len = poses[0];
                    ret.Add(self.Substring(pos1, len));

                    pos1 = poses[0];
                    for (int j = 1; j <= poses.Count; ++j)
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

        public static string TrimEnd(this string self, int len)
        {
            return self.Substring(0, self.Length - len);
        }

        public static string ReplaceLastOf(this string str, string fromStr, string toStr)
        {
            int lastIndexOf = str.LastIndexOf(fromStr);
            if (lastIndexOf < 0)
                return str;

            string leading = str.Substring(0, lastIndexOf);
            int charsToEnd = str.Length - (lastIndexOf + fromStr.Length);
            string trailing = str.Substring(lastIndexOf + fromStr.Length, charsToEnd);

            return leading + toStr + trailing;
        }

        /// <summary>
		/// Convert camel-cased string to camel-cased string with separator.
		/// </summary>
		/// <param name="source">Source string</param>
		/// <param name="separator">Separator char</param>
		/// <returns>String with separator</returns>
		public static string ToCamelWithSeparator(this string source, char separator)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            var sourceArray = source.ToCharArray();

            var sb = new StringBuilder(sourceArray[0].ToString());

            for (int i = 1; i <= sourceArray.Length - 1; i++) // Index 0 is skipped.
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

        public static string Wrap(this string a, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return a;

            // Parse each line of text
            for (pos = 0; pos < a.Length; pos = next)
            {
                // Find end of line
                int eol = a.IndexOf(Environment.NewLine, pos);

                if (eol == -1)
                    next = eol = a.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;

                        if (len > width)
                            len = BreakLine(a, pos, width);

                        sb.Append(a, pos, len);
                        sb.Append(Environment.NewLine);

                        // Trim whitespace following break
                        pos += len;

                        while (pos < eol && Char.IsWhiteSpace(a[pos]))
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
        public static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;

            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;
            if (i < 0)
                return max; // No whitespace found; break at maximum length
                            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;
            // Return length of text before whitespace
            return i + 1;
        }

        public static string Remove(this string s, IEnumerable<char> chars)
        {
            return new string(s.Where(c => !chars.Contains(c)).ToArray());
        }

        public static byte[] GetBytes(this string value, int start, int NumBytes)
        {
            StringBuilder g = new StringBuilder(value);
            byte[] Bytes = new byte[NumBytes];
            string temp;
            int CharPos = start;
            for (int i = 0; i < NumBytes; i++)
            {
                temp = g[CharPos++].ToString();
                temp += g[CharPos++].ToString();
                Bytes[i] = byte.Parse(temp, System.Globalization.NumberStyles.HexNumber);
            }
            return Bytes;
        }
    }
}