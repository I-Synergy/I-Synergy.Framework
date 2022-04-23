using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using System.Globalization;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Services
{
    /// <summary>
    /// Class LocalizationFunctions.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IContext _context;

        /// <summary>
        /// The settings service
        /// </summary>
        private readonly IBaseApplicationSettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settingsService">The settings service.</param>
        public LocalizationService(IContext context, IBaseApplicationSettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Sets the localization language.
        /// </summary>
        /// <param name="isoLanguage">The iso language.</param>
        public async Task SetLocalizationLanguageAsync(string isoLanguage)
        {
            await _settingsService.LoadSettingsAsync();
            _settingsService.Settings.Culture = isoLanguage;
            await _settingsService.SaveSettingsAsync();

            CultureInfo.CurrentCulture = new CultureInfo(isoLanguage);
            CultureInfo.CurrentUICulture = new CultureInfo(isoLanguage);

            // Currency Symbol is retrieved from Current Culture. 
            // Alternatively, could be also a deployment wide setting. 
            _context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }
    }
}
