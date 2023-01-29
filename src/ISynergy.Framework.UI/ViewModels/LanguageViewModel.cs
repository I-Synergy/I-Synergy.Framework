using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

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
        /// Gets or sets the color command.
        /// </summary>
        /// <value>The color command.</value>
        public RelayCommand<string> SetLanguage_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="culture"></param>
        public LanguageViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger,
            string culture)
            : base(context, commonServices, logger)
        {
            SetLanguage_Command = new RelayCommand<string>((e) => SelectedItem = e);
            SelectedItem = culture;
        }
    }
}
