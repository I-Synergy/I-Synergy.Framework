using System.Globalization;

namespace ISynergy.Framework.Globalization.Abstractions.Services;

/// <summary>
/// Provides the list of cultures that the application supports.
/// English is always present and serves as the default fallback.
/// </summary>
public interface ISupportedCulturesService
{
    /// <summary>
    /// The default culture (always English "en").
    /// </summary>
    CultureInfo DefaultCulture { get; }

    /// <summary>
    /// The cultures the application supports, with English always first.
    /// </summary>
    IReadOnlyList<CultureInfo> SupportedCultures { get; }

    /// <summary>
    /// Returns true when <paramref name="cultureName"/> matches a supported culture
    /// by either its two-letter ISO name or its full culture name.
    /// </summary>
    bool IsCultureSupported(string cultureName);
}
