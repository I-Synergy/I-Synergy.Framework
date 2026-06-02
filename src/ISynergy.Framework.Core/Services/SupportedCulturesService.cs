using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using System.Globalization;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Service that manages the list of cultures the application supports.
/// English is always included as the first entry and serves as the default fallback.
/// </summary>
public sealed class SupportedCulturesService : ISupportedCulturesService
{
    private readonly List<CultureInfo> _supportedCultures;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupportedCulturesService"/> class.
    /// </summary>
    /// <param name="cultures">The cultures to support. English is guaranteed to be first.</param>
    public SupportedCulturesService(IEnumerable<CultureInfo> cultures)
    {
        _supportedCultures = new List<CultureInfo>(cultures ?? throw new ArgumentNullException(nameof(cultures)));

        var english = CultureInfo.GetCultureInfo(GenericConstants.DefaultCultureInfoName);
        var existingEnglishIndex = _supportedCultures.FindIndex(c =>
            string.Equals(c.TwoLetterISOLanguageName, GenericConstants.DefaultCultureInfoName, StringComparison.OrdinalIgnoreCase));

        if (existingEnglishIndex < 0)
        {
            _supportedCultures.Insert(0, english);
        }
        else if (existingEnglishIndex > 0)
        {
            var englishEntry = _supportedCultures[existingEnglishIndex];
            _supportedCultures.RemoveAt(existingEnglishIndex);
            _supportedCultures.Insert(0, englishEntry);
        }
    }

    /// <inheritdoc />
    public CultureInfo DefaultCulture => CultureInfo.GetCultureInfo(GenericConstants.DefaultCultureInfoName);

    /// <inheritdoc />
    public IReadOnlyList<CultureInfo> SupportedCultures => _supportedCultures.AsReadOnly();

    /// <inheritdoc />
    public bool IsCultureSupported(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            return false;

        return _supportedCultures.Any(c =>
            string.Equals(c.TwoLetterISOLanguageName, cultureName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.Name, cultureName, StringComparison.OrdinalIgnoreCase));
    }
}
