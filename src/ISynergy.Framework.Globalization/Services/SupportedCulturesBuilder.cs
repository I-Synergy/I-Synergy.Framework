using ISynergy.Framework.Globalization.Abstractions.Services;
using System.Globalization;

namespace ISynergy.Framework.Globalization.Services;

/// <summary>
/// Default implementation of <see cref="ISupportedCulturesBuilder"/>.
/// </summary>
public sealed class SupportedCulturesBuilder : ISupportedCulturesBuilder
{
    private readonly List<CultureInfo> _cultures = [];

    /// <inheritdoc />
    public ISupportedCulturesBuilder AddCulture(string cultureName)
    {
        _cultures.Add(CultureInfo.GetCultureInfo(cultureName));
        return this;
    }

    /// <inheritdoc />
    public ISupportedCulturesBuilder AddCulture(CultureInfo culture)
    {
        _cultures.Add(culture);
        return this;
    }

    /// <summary>
    /// Returns the collected cultures.
    /// </summary>
    public IEnumerable<CultureInfo> Build() => _cultures;
}
