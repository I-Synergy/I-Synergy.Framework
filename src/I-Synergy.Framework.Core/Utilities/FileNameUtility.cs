using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Utilities
{
    /// <summary>
    /// Class FileNameUtility.
    /// </summary>
    public class FileNameUtility
    {
        /// <summary>
        /// The invalid chars
        /// </summary>
        private static readonly string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
        /// <summary>
        /// The invalid reg string
        /// </summary>
        private static readonly string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        /// <summary>
        /// Determines whether [is valid file name] [the specified file name].
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns><c>true</c> if [is valid file name] [the specified file name]; otherwise, <c>false</c>.</returns>
        public static bool IsValidFileName(string fileName)
        {
            return !Regex.IsMatch(fileName, invalidRegStr);
        }

        /// <summary>
        /// Makes the name of the valid file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        public static string MakeValidFileName(string fileName)
        {
            return Regex.Replace(fileName, invalidRegStr, "_");
        }
    }
}
