using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using NodaTime.TimeZones;
using System.Globalization;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class LocalizationService.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LocalizationService(IContext context)
        {
            _context = context;
        }

        public List<string> GetTimeZoneIds(string iso2country)
        {
            if (TzdbDateTimeZoneSource.Default is TzdbDateTimeZoneSource source)
            {
                var locations = source.ZoneLocations?
                    .Where(key => key.CountryCode.Equals(iso2country.ToUpper()));

                return locations.Select(s => s.ZoneId).ToList();
            }

            return default;
        }

        /// <summary>
        /// Sets the localization language.
        /// </summary>
        /// <param name="language">The iso language.</param>
        public void SetLocalizationLanguage(string language)
        {
            var systemCulture = CultureInfo.CurrentCulture;

            if (systemCulture.TwoLetterISOLanguageName.Equals(language))
            {
                CultureInfo.CurrentCulture = systemCulture;
                CultureInfo.CurrentUICulture = systemCulture;
            }
            else
            {
                var applicationCulture = new CultureInfo(language);
                applicationCulture.NumberFormat = systemCulture.NumberFormat;

                CultureInfo.CurrentCulture = applicationCulture;
                CultureInfo.CurrentUICulture = applicationCulture;
            }

            // Currency Symbol is retrieved from Current Culture. 
            // Alternatively, could be also a deployment wide setting. 
            _context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }
    }
}
