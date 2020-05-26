using System.Globalization;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;

namespace ISynergy.Framework.Windows.Functions
{
    public class LocalizationFunctions
    {
        public readonly IContext Context;
        public readonly IApplicationSettingsService SettingsService;

        public LocalizationFunctions(IContext context, IApplicationSettingsService settingsService)
        {
            Context = context;
            SettingsService = settingsService;
        }

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
