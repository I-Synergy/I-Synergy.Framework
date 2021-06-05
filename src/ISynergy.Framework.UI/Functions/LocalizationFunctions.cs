using System.Globalization;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;

#if NETFX_CORE || (NET5_0 && WINDOWS)
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
#endif

namespace ISynergy.Framework.UI.Functions
{
    /// <summary>
    /// Class LocalizationFunctions.
    /// </summary>
    public class LocalizationFunctions
    {
        /// <summary>
        /// The context
        /// </summary>
        public readonly IContext Context;
        /// <summary>
        /// The settings service
        /// </summary>
        public readonly IBaseSettingsService SettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationFunctions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settingsService">The settings service.</param>
        public LocalizationFunctions(IContext context, IBaseSettingsService settingsService)
        {
            Context = context;
            SettingsService = settingsService;
        }

        /// <summary>
        /// Sets the localization language.
        /// </summary>
        /// <param name="isoLanguage">The iso language.</param>
        public void SetLocalizationLanguage(string isoLanguage)
        {
            SettingsService.Culture = isoLanguage;

#if NETFX_CORE || (NET5_0 && WINDOWS)
            ApplicationLanguages.PrimaryLanguageOverride = isoLanguage;

            // After setting PrimaryLanguageOverride ResourceContext should be reset
            ResourceContext.GetForViewIndependentUse().Reset();
#endif

            CultureInfo.CurrentCulture = new CultureInfo(isoLanguage);
            CultureInfo.CurrentUICulture = new CultureInfo(isoLanguage);

            // Currency Symbol is retrieved from Current Culture. 
            // Alternatively, could be also a deployment wide setting. 
            Context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }
    }
}
