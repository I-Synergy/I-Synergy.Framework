using ISynergy.Framework.Core.Services;
using System.Globalization;

namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Builder for configuring supported cultures via a fluent API.
/// </summary>
public interface ISupportedCulturesBuilder
{
    /// <summary>
    /// Creates a new instance of the default <see cref="ISupportedCulturesBuilder"/> implementation.
    /// </summary>
    static ISupportedCulturesBuilder Create() => new SupportedCulturesBuilder();

    /// <summary>
    /// Adds a culture by its name (e.g., "en", "nl", "nl-NL").
    /// </summary>
    ISupportedCulturesBuilder AddCulture(string cultureName);

    /// <summary>
    /// Adds a <see cref="CultureInfo"/> instance.
    /// </summary>
    ISupportedCulturesBuilder AddCulture(CultureInfo culture);
}
