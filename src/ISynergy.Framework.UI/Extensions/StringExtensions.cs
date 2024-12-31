using ISynergy.Framework.Core.Services;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Class ResourceExtensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Gets the localized.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>System.String.</returns>
    public static string GetLocalized(this string resourceKey) =>
        LanguageService.Default.GetString(resourceKey);
}
