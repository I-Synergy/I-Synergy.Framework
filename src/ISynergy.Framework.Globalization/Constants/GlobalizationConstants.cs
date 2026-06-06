using System.Globalization;

namespace ISynergy.Framework.Globalization.Constants;

public class GlobalizationConstants
{
    public const string IsoDateTimeFormat = "O";

    /// <summary>
    /// The default culture information name
    /// </summary>
    public const string DefaultCultureInfoName = "en";

    /// <summary>
    /// The date time offset format
    /// </summary>
    public const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ssZ";

    /// <summary>
    /// Determines whether the specified language code represents a right-to-left (RTL) language.
    /// </summary>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    /// <returns><c>true</c> if the language is right-to-left; otherwise, <c>false</c>.</returns>
    public static bool IsRightToLeftLanguage(string cultureName) =>
        CultureInfo.GetCultureInfo(cultureName).TextInfo.IsRightToLeft;

    /// <summary>
    /// Returns the HTML direction attribute value for the specified language.
    /// </summary>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    /// <returns><c>"rtl"</c> for right-to-left languages; otherwise, <c>"ltr"</c>.</returns>
    public static string GetDirection(string cultureName) =>
        IsRightToLeftLanguage(cultureName) ? "rtl" : "ltr";
}
