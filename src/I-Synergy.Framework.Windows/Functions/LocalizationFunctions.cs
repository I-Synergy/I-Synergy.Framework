using System.Globalization;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;

namespace ISynergy.Framework.Windows.Functions
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
        public readonly IApplicationSettingsService SettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationFunctions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settingsService">The settings service.</param>
        public LocalizationFunctions(IContext context, IApplicationSettingsService settingsService)
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

            ApplicationLanguages.PrimaryLanguageOverride = isoLanguage;

            // After setting PrimaryLanguageOverride ResourceContext should be reset
            ResourceContext.GetForViewIndependentUse().Reset();

            // Seems not necessary anymore!
            // Check if current assembly is run from an unit test or from regular application
            // If run form test, the current view is not available and would generate an exception:
            // Resource contexts may not be created on threads that do not have a corewindow
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //if (AppDomain.CurrentDomain.GetAssemblies().Any(
            //    a => a.FullName.StartsWith("Microsoft.TestPlatform", StringComparison.InvariantCultureIgnoreCase)))
            //{
            //    ResourceContext.GetForCurrentView().Reset();
            //}

            CultureInfo.CurrentCulture = new CultureInfo(isoLanguage);
            CultureInfo.CurrentUICulture = new CultureInfo(isoLanguage);

            // Currency Symbol is retrieved from Current Culture. 
            // Alternatively, could be also a deployment wide setting. 
            Context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }
    }
}
