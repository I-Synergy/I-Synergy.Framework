using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Services.Base
{
    public abstract class BaseDialogService : IDialogService
    {
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        protected readonly ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDialogService"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        protected BaseDialogService(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// show an Content Dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns>MessageBoxResult.</returns>
        public abstract Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK);

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "") =>
            ShowMessageAsync(error.Message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButton.OK);

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "") =>
            ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButton.OK);

        /// <summary>
        /// Shows the information asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "") =>
            ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleInfo"), MessageBoxButton.OK);

        /// <summary>
        /// Shows the warning asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "") =>
            ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleWarning"), MessageBoxButton.OK);

        /// <summary>
        /// Shows the greeting asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task.</returns>
        public Task ShowGreetingAsync(string name)
        {
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Night"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK);
            }
            else if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Morning"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK);
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Afternoon"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK);
            }
            else
            {
                return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Evening"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK);
            }
        }
    }
}
