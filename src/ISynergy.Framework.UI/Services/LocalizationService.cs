using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using System.Globalization;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;

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

        /// <summary>
        /// Sets the localization language.
        /// </summary>
        /// <param name="language">The iso language.</param>
        public void SetLocalizationLanguage(string language)
        {
            ApplicationLanguages.PrimaryLanguageOverride = language;

#if WINDOWS10_0_18362_0_OR_GREATER && !HAS_UNO
            // After setting PrimaryLanguageOverride ResourceContext should be reset
            ResourceContext.GetForViewIndependentUse().Reset();
#endif

            CultureInfo.CurrentCulture = new CultureInfo(language);
            CultureInfo.CurrentUICulture = new CultureInfo(language);

            // Currency Symbol is retrieved from Current Culture. 
            // Alternatively, could be also a deployment wide setting. 
            _context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }
    }
}
