using NodaTime.TimeZones;
using System.Globalization;

namespace ISynergy.Framework.UI.Extensions;

public static class LanguageExtensions
{
    /// <summary>
    /// Sets the localization language using the specified culture.
    /// Applies the culture to both the current thread and as the default for new threads.
    /// </summary>
    /// <param name="culture">The culture to apply.</param>
    public static void SetLocalizationLanguage(this CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    /// <summary>
    /// Sets the localization language using a culture name string.
    /// </summary>
    /// <param name="cultureName">The culture name (e.g., "en", "nl", "nl-NL").</param>
    public static void SetLocalizationLanguage(this string cultureName)
    {
        if (!string.IsNullOrEmpty(cultureName))
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            culture.SetLocalizationLanguage();
        }
    }

    /// <summary>
    /// Gets the time zones.
    /// </summary>
    /// <param name="isoCountryCode"></param>
    /// <returns></returns>
    public static List<string> ToTimeZoneIds(this string isoCountryCode)
    {
        if (TzdbDateTimeZoneSource.Default is { } source &&
            source.ZoneLocations?
                .Where(key => key.CountryCode.Equals(isoCountryCode.ToUpper())) is IEnumerable<TzdbZoneLocation> locations)
            return locations.Select(s => s.ZoneId).ToList();

        return new List<string>();
    }

    public static List<string> ToCountryIds(this string timezoneId)
    {
        if (TzdbDateTimeZoneSource.Default is { } source &&
            source.ZoneLocations?
                .Where(key => key.ZoneId.Equals(timezoneId)) is IEnumerable<TzdbZoneLocation> locations)
            return locations.Select(s => s.CountryCode).ToList();

        return new List<string>();
    }
}
