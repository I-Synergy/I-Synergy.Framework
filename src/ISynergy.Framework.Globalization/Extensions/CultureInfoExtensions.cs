using System.Globalization;

namespace ISynergy.Framework.Globalization.Extensions;

/// <summary>
/// Centralized culture management. Applies a <see cref="CultureInfo"/> as the
/// process-wide default so every new thread inherits the correct culture and
/// number/currency formatting.
/// </summary>
/// <remarks>
/// Platform UI projects must additionally set <see cref="CultureInfo.CurrentCulture"/>
/// and <see cref="CultureInfo.CurrentUICulture"/> on their dispatcher thread — those
/// are per-thread and must be applied on the already-running UI thread.
/// </remarks>
public static class CultureInfoExtensions
{
    /// <summary>
    /// Applies <paramref name="culture"/> as the process-wide default culture.
    /// </summary>
    /// <param name="culture">The culture to apply. Cloned internally so callers retain ownership.</param>
    /// <param name="configure">
    /// Optional callback to customize the cloned <see cref="CultureInfo"/> before it is applied
    /// (e.g. override <see cref="NumberFormatInfo.CurrencySymbol"/>).
    /// </param>
    public static CultureInfo ApplyCulture(this CultureInfo culture, Action<CultureInfo>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(culture);

        // Clone so the caller's instance is not mutated and we get a writable copy.
        culture = (CultureInfo)culture.Clone();

        configure?.Invoke(culture);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        return culture;
    }
}
