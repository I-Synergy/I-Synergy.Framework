using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Functions
{
    public class LocalizationFunctions
    {
        private readonly IContext Context;
        private readonly IBaseSettingsService SettingsService;

        public LocalizationFunctions(IContext context, IBaseSettingsService settingsService)
        {
            Context = context;
            SettingsService = settingsService;
        }

        public void SetLocalizationLanguage(string isoLanguage)
        {
            SettingsService.Application_Culture = isoLanguage;

            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = isoLanguage;

            // After setting PrimaryLanguageOverride ResourceContext should be reset
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();

            // Check if current assembly is run from an unit test or from regular application
            // If run form test, the current view is not available and would generate an exception:
            // Resource contexts may not be created on threads that do not have a corewindow
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(
                a => a.FullName.ToLowerInvariant().StartsWith("xunit")))
            {
                Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
            }

            CultureInfo.CurrentCulture = new CultureInfo(isoLanguage);
            CultureInfo.CurrentUICulture = new CultureInfo(isoLanguage);

            // Currency Symbol is retrieved from Current Culture. 
            // Alternatively, could be also a deployment wide setting. 
            Context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }
    }
}
