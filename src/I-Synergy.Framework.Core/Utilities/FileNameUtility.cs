using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Utilities
{
    public class FileNameUtility
    {
        private static readonly string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
        private static readonly string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        public static bool IsValidFileName(string fileName)
        {
            return !Regex.IsMatch(fileName, invalidRegStr);
        }

        public static string MakeValidFileName(string fileName)
        {
            return Regex.Replace(fileName, invalidRegStr, "_");
        }
    }
}
