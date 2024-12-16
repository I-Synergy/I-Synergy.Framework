using ISynergy.Framework.Core.Abstractions;
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
    /// <param name="context"></param>
    public static void SetLocalizationLanguage(this Languages language, IContext context)
    {
        var systemCulture = CultureInfo.CurrentCulture;

        if (systemCulture.TwoLetterISOLanguageName.Equals(language))
        {
            CultureInfo.CurrentCulture = systemCulture;
            CultureInfo.CurrentUICulture = systemCulture;
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

            applicationCulture.NumberFormat = systemCulture.NumberFormat;

            CultureInfo.CurrentCulture = applicationCulture;
            CultureInfo.CurrentUICulture = applicationCulture;
        }

        // Currency Symbol is retrieved from Current Culture. 
        // Alternatively, could be also a deployment wide setting. 
        context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
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
}
