using ISynergy.Framework.Core.Enumerations;
using NodaTime.TimeZones;
using System.Globalization;

namespace ISynergy.Framework.UI.Extensions;

public static class LanguageExtensions
{
    /// <summary>
    /// Sets the localization language.
    /// </summary>
    /// <param name="language">The iso language.</param>
    public static void SetLocalizationLanguage(this Languages language)
    {
        var systemCulture = CultureInfo.CurrentCulture.Clone() as CultureInfo;

        if (systemCulture.TwoLetterISOLanguageName.Equals(language))
        {
            CultureInfo.DefaultThreadCurrentCulture = systemCulture;
            CultureInfo.DefaultThreadCurrentUICulture = systemCulture;
        }
        else
        {
            CultureInfo applicationCulture;

            var cultureMap = new Dictionary<Languages, string>
            {
                { Languages.Dutch, "nl" },
                { Languages.German, "de" },
                { Languages.French, "fr" },
                { Languages.English, "en" }
            };

            if (cultureMap.TryGetValue(language, out var cultureString))
                applicationCulture = new CultureInfo(cultureString);
            else
                applicationCulture = new CultureInfo("en");

            CultureInfo.DefaultThreadCurrentCulture = applicationCulture;
            CultureInfo.DefaultThreadCurrentUICulture = applicationCulture;
        }
    }

    /// <summary>
    /// Gets the time zones.
    /// </summary>
    /// <param name="isoCountryCode"></param>
    /// <returns></returns>
    public static List<string> ToTimeZoneIds(this string isoCountryCode)
    {
        if (TzdbDateTimeZoneSource.Default is { } source)
        {
            var locations = source.ZoneLocations?
                .Where(key => key.CountryCode.Equals(isoCountryCode.ToUpper()));

            return locations.Select(s => s.ZoneId).ToList();
        }

        return new List<string>();
    }

    public static List<string> ToCountryIds(this string timezoneId)
    {
        if (TzdbDateTimeZoneSource.Default is { } source)
        {
            var locations = source.ZoneLocations?
                .Where(key => key.ZoneId.Equals(timezoneId));

            return locations.Select(s => s.CountryCode).ToList();
        }

        return new List<string>();
    }
}
