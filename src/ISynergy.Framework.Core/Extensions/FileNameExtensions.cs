namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class FileNameExtensions.
    /// </summary>
    public static class FileNameExtensions
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
        /// <param name="_self">Name of the file.</param>
        /// <returns><c>true</c> if [is valid file name] [the specified file name]; otherwise, <c>false</c>.</returns>
        public static bool IsValidFileName(this string _self) =>
            !Regex.IsMatch(_self, invalidRegStr);

        /// <summary>
        /// Makes the name of the valid file.
        /// </summary>
        /// <param name="_self">Name of the file.</param>
        /// <returns>System.String.</returns>
        public static string MakeValidFileName(this string _self) =>
            Regex.Replace(_self, invalidRegStr, "_");
    }
}
