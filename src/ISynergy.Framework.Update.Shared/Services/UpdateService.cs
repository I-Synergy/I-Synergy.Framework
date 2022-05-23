using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Update.Abstractions.Services;

namespace ISynergy.Framework.Update.Services
{
    /// <summary>
    /// Class UpdateService.
    /// </summary>
    internal partial class UpdateService : IUpdateService
    {
        /// <summary>
        /// Gets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService" /> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public UpdateService(
            ILanguageService languageService,
            IDialogService dialogService)
        {
            _languageService = languageService;
            _dialogService = dialogService;
        }
    }
}
