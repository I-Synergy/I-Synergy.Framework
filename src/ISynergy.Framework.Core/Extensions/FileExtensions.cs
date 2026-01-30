using ISynergy.Framework.Core.Constants;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class FileNameExtensions.
/// </summary>
public static class FileExtensions
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
        !Regex.IsMatch(_self, invalidRegStr, RegexOptions.None, TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Makes the name of the valid file.
    /// </summary>
    /// <param name="_self">Name of the file.</param>
    /// <returns>System.String.</returns>
    public static string MakeValidFileName(this string _self) =>
        Regex.Replace(_self, invalidRegStr, "_", RegexOptions.None, TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Converts filename to contenttype.
    /// </summary>
    /// <param name="self">The full filename.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotSupportedException">$"File: {self} has an unsupported extension.</exception>
    public static string ToContentType(this string self)
    {
        var result = MasterData
            .FileTypes
            .SingleOrDefault(q => q.Extension.Equals(Path.GetExtension(self)));

        if (result is not null)
        {
            return result.ContentType;
        }

        throw new NotSupportedException($"File: {self} has an unsupported extension.");
    }
}
