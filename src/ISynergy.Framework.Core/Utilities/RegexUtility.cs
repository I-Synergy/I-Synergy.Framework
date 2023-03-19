using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Utilities
{
    /// <summary>
    /// Regex utilities.
    /// </summary>
    public static class RegexUtility
    {
        /// <summary>
        /// Converts Mask to Regex.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static Regex MaskToRegexConverter(string mask) =>
            new Regex(MaskToRegexStringConverter(mask));

        /// <summary>
        /// Converts Mask to Regex string.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string MaskToRegexStringConverter(string mask)
        {
            var regex = new List<string>();
            var capital = false;

            foreach (var character in mask)
            {
                switch (character)
                {
                    case '0':
                        regex.Add(@"\d");
                        break;
                    case '9':
                        regex.Add(@"[\d]?");
                        break;
                    case '#':
                        regex.Add(@"[\d+-]?");
                        break;
                    case 'L':
                        if (capital)
                            regex.Add(@"[A-Z]");
                        else
                            regex.Add(@"[a-z]");

                        break;
                    case '?':
                        if (capital)
                            regex.Add(@"[A-Z]?");
                        else
                            regex.Add(@"[a-z]?");

                        break;
                    case '&':
                        regex.Add(@"[\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}]");
                        break;
                    case 'C':
                        regex.Add(@"[\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}]?");
                        break;
                    case 'A':
                        regex.Add(@"\W");
                        break;
                    case '<':
                        capital = false;
                        break;
                    case '>':
                        capital = true;
                        break;
                    case '|':
                        capital = !capital;
                        break;
                    case '\\':
                        regex.Add(@"\");
                        break;
                    default:
                        if (capital)
                            regex.Add($"[{character}]".ToUpper());
                        else
                            regex.Add($"[{character}]".ToLower());

                        break;
                }
            }

            return string.Join("", regex);
        }
    }
}
