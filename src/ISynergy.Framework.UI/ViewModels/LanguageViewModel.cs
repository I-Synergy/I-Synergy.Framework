using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
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
    public class LanguageViewModel : ViewModelDialog<Languages>, ILanguageViewModel
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title => BaseCommonServices.LanguageService.GetString("Language");

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="language"></param>
        public LanguageViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger,
            Languages language)
            : base(context, commonServices, logger)
        {
            SelectedItem = language;
        }
    }
}
