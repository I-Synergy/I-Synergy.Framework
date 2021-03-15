using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.Mvvm.Commands;

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
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("Language");
            }
        }

        /// <summary>
        /// The localization functions
        /// </summary>
        private readonly LocalizationFunctions _localizationFunctions;

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
        /// <param name="localizationFunctions">The localization functions.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public LanguageViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            LocalizationFunctions localizationFunctions,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            _localizationFunctions = localizationFunctions;
            SetLanguage_Command = new Command<string>((e) => SelectedItem = e);
            SelectedItem = BaseCommonServices.ApplicationSettingsService.Culture;
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public override Task SubmitAsync(string e)
        {
            BaseCommonServices.ApplicationSettingsService.Culture = e;
            _localizationFunctions.SetLocalizationLanguage(e);
            return base.SubmitAsync(e);
        }
    }
}
