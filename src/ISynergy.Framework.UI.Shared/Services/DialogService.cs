using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in UWP applications.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        public ILanguageService LanguageService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        public DialogService(ILanguageService languageService)
        {
            LanguageService = languageService;
        }

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "")
        {
            return ShowAsync(error.Message, !string.IsNullOrEmpty(title) ? title : LanguageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "")
        {
            return ShowAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows the information asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "")
        {
            return ShowAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.GetString("TitleInfo"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows the warning asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "")
        {
            return ShowAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.GetString("TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Shows the greeting asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task.</returns>
        public Task ShowGreetingAsync(string name)
        {
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
            {
                return ShowAsync(string.Format(LanguageService.GetString("Greeting_Night"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
            {
                return ShowAsync(string.Format(LanguageService.GetString("Greeting_Morning"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            {
                return ShowAsync(string.Format(LanguageService.GetString("Greeting_Afternoon"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                return ShowAsync(string.Format(LanguageService.GetString("Greeting_Evening"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// show as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="image">The image.</param>
        /// <returns>MessageBoxResult.</returns>
        public virtual async Task<MessageBoxResult> ShowAsync(
            string message,
            string title = "",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
            var result = MessageBoxResult.Cancel;

            var yesCommand = new UICommand(LanguageService.GetString("Yes"), cmd => { });
            var noCommand = new UICommand(LanguageService.GetString("No"), cmd => { });
            var okCommand = new UICommand(LanguageService.GetString("Ok"), cmd => { });
            var cancelCommand = new UICommand(LanguageService.GetString("Cancel"), cmd => { });

            var messageDialog = new MessageDialog(message, title)
            {
                Options = MessageDialogOptions.None,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 0
            };

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    messageDialog.Commands.Add(okCommand);
                    messageDialog.DefaultCommandIndex = 0;
                    break;
                case MessageBoxButton.OKCancel:
                    messageDialog.Commands.Add(okCommand);
                    messageDialog.Commands.Add(cancelCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 1;
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageDialog.Commands.Add(yesCommand);
                    messageDialog.Commands.Add(noCommand);
                    messageDialog.Commands.Add(cancelCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 2;
                    break;
                case MessageBoxButton.YesNo:
                    messageDialog.Commands.Add(yesCommand);
                    messageDialog.Commands.Add(noCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 1;
                    break;
            }

            IUICommand command = default;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAndAwaitAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    command = await messageDialog.ShowAsync();
                });

            if (command is null && cancelCommand != null)
            {
                // back button was pressed
                // invoke the UICommand
                cancelCommand.Invoked(cancelCommand);
                result = MessageBoxResult.Cancel;
            }
            else if (command == okCommand)
                result = MessageBoxResult.OK;
            else if (command == yesCommand)
                result = MessageBoxResult.Yes;
            else if (command == noCommand)
                result = MessageBoxResult.No;

            return result;
        }
    }
}
