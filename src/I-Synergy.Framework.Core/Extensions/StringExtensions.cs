using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a hex string to a .NET Color object.
        /// </summary>
        /// <param name="self">a hex string: "FFFFFF", "#000000"</param>
        public static Color HexStringToColor(this string self)
        {
            string hc = self.ExtractHexDigits();

            if (hc.Length != 6)
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("hexColor is not exactly 6 digits.");
                return Color.Empty;
            }

            string r = hc.Substring(0, 2);
            string g = hc.Substring(2, 2);
            string b = hc.Substring(4, 2);

            Color color = Color.Empty;

            try
            {
                int ri
                   = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi
                   = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi
                   = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);

                color = Color.FromArgb(ri, gi, bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return Color.Empty;
            }

            return color;
        }

        public static int CovertString2Numeric(this string self)
        {
            List<string> iChars = new List<string>();
            string strResult = string.Empty;

            foreach (string iChar in iChars.EnsureNotNull())
            {
                if (IsInteger(iChar) == true)
                {
                    strResult = strResult + iChar;
                }
            }

            return Convert.ToInt32(strResult);
        }

        public static bool IsFloat(this string self)
        {
            try
            {
                return float.TryParse(self, out float output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsDecimal(this string self)
        {
            try
            {
                return decimal.TryParse(self, out decimal output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsInteger(this string self)
        {
            try
            {
                return int.TryParse(self, out int output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string IncreaseString2Long(this string self, long summand)
        {
            string numberString = string.Empty;
            int codePos = self.Length;

            // Go back from end of id to the begin while a char is a number
            for (int i = self.Length - 1; i >= 0; i--)
            {
                if (Char.IsDigit(self.Substring(i, 1).ToCharArray()[0]))
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
                long num = long.Parse(numberString) + summand;
                return self.Substring(0, codePos) + num.ToString();
            }
        }

        public static string WordWrap(this string self, int width)
        {
            if (self.Length <= width)
                return self;
            // dont need to do anything

            string sResult = self;
            string sChar = null;
            // temp holder for current string char
            int iEn = 0;
            int iLineNO = width;

            while (sResult.Length >= iLineNO)
            {
                // work backwards from the max len to 1 looking for a space
                for (iEn = iLineNO; iEn >= 1; iEn += -1)
                {
                    sChar = sResult.Substring(iEn, 1);
                    // found a space
                    if (sChar == " ")
                    {
                        sResult = sResult.Remove(iEn, 1);
                        // Remove the space
                        sResult = sResult.Insert(iEn, System.Environment.NewLine);
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
        public static string ExtractHexDigits(this string self)
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            string newnum = "";

            foreach (char c in self.EnsureNotNull())
                if (isHexDigit.IsMatch(c.ToString())) newnum += c.ToString();

            return newnum;
        }

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