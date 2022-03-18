using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Functions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ISynergy.Framework.UI.ViewModels
{
    /// <summary>
    /// Class LanguageViewModel.
    /// Implements the <see cref="ILanguageViewModel" />
    /// </summary>
    /// <seealso cref="ILanguageViewModel" />
    public class LanguageViewModel : ViewModelDialog<string>, ILanguageViewModel
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title => BaseCommonServices.LanguageService.GetString("Language");

        /// <summary>
        /// The localization functions
        /// </summary>
        private readonly LocalizationFunctions _localizationFunctions;

        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly IBaseApplicationSettingsService _appSettingsService;

        /// <summary>
        /// Gets or sets the color command.
        /// </summary>
        /// <value>The color command.</value>
        public Command<string> SetLanguage_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="appSettingsService">The settings services.</param>
        /// <param name="localizationFunctions">The localization functions.</param>
        /// <param name="logger">The logger factory.</param>
        public LanguageViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            IBaseApplicationSettingsService appSettingsService,
            LocalizationFunctions localizationFunctions,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            _localizationFunctions = localizationFunctions;
            _appSettingsService = appSettingsService;

            SetLanguage_Command = new Command<string>((e) => SelectedItem = e);
            SelectedItem = _appSettingsService.Settings.Culture;
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public override async Task SubmitAsync(string e)
        {
            _appSettingsService.Settings.Culture = e;
            _localizationFunctions.SetLocalizationLanguage(e);
            await base.SubmitAsync(e);
        }
    }
}
