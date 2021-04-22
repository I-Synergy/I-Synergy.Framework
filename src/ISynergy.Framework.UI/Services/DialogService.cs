using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ISynergy.Framework.Mvvm.Commands;

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
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        public DialogService(ILanguageService languageService)
        {
            _languageService = languageService;
        }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "")
        {
            return ShowAsync(error.Message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows the error asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "")
        {
            return ShowAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows the information asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "")
        {
            return ShowAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleInfo"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows the warning asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
        public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "")
        {
            return ShowAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning);
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
                return ShowAsync(string.Format(_languageService.GetString("Greeting_Night"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
            {
                return ShowAsync(string.Format(_languageService.GetString("Greeting_Morning"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            {
                return ShowAsync(string.Format(_languageService.GetString("Greeting_Afternoon"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                return ShowAsync(string.Format(_languageService.GetString("Greeting_Evening"), name),
                    _languageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        

        /// <summary>
        /// show an Content Dialog.
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
            var dialog = new ContentDialog()
            {
                Title = title,
                Content = message
            };

            switch (buttons)
            {
                case MessageBoxButton.OKCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    break;
                case MessageBoxButton.YesNoCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.SecondaryButtonText = _languageService.GetString("No");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    break;
                case MessageBoxButton.YesNo:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.CloseButtonText = _languageService.GetString("No");
                    break;
                default:
                    dialog.CloseButtonText = _languageService.GetString("Ok");
                    break;
            }

            if(await dialog.ShowAsync() is ContentDialogResult result)
            {
                switch (buttons)
                {
                    case MessageBoxButton.OKCancel:
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                return MessageBoxResult.OK;
                            default:
                                return MessageBoxResult.Cancel;
                        }
                    case MessageBoxButton.YesNoCancel:
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                return MessageBoxResult.Yes;
                            case ContentDialogResult.Secondary:
                                return MessageBoxResult.No;
                            default:
                                return MessageBoxResult.Cancel;
                        }
                    case MessageBoxButton.YesNo:
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                return MessageBoxResult.Yes;
                            default:
                                return MessageBoxResult.No;
                        }
                    default:
                        return MessageBoxResult.OK;
                }
            }

            return MessageBoxResult.Cancel;
        }

        /// <summary>
        /// show an asynchronous message dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="image">The image.</param>
        /// <returns>MessageBoxResult.</returns>
        public virtual async Task<MessageBoxResult> ShowMessageAsync(
            string message,
            string title = "",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
            var result = MessageBoxResult.Cancel;

            var _yesCommand = new Windows.UI.Popups.UICommand(_languageService.GetString("Yes"), cmd => { result = MessageBoxResult.Yes; }, "yesCommand");
            var _noCommand = new Windows.UI.Popups.UICommand(_languageService.GetString("No"), cmd => { result = MessageBoxResult.No; }, "noCommand");
            var _okCommand = new Windows.UI.Popups.UICommand(_languageService.GetString("Ok"), cmd => { result = MessageBoxResult.OK; }, "okCommand");
            var _cancelCommand = new Windows.UI.Popups.UICommand(_languageService.GetString("Cancel"), cmd => { result = MessageBoxResult.Cancel; }, "cancelCommand");

            var messageDialog = new MessageDialog(message, title)
            {
                Options = MessageDialogOptions.None,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 0
            };

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    messageDialog.Commands.Add(_okCommand);
                    messageDialog.DefaultCommandIndex = 0;
                    break;
                case MessageBoxButton.OKCancel:
                    messageDialog.Commands.Add(_okCommand);
                    messageDialog.Commands.Add(_cancelCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 1;
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageDialog.Commands.Add(_yesCommand);
                    messageDialog.Commands.Add(_noCommand);
                    messageDialog.Commands.Add(_cancelCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 2;
                    break;
                case MessageBoxButton.YesNo:
                    messageDialog.Commands.Add(_yesCommand);
                    messageDialog.Commands.Add(_noCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 1;
                    break;
            }

#if NETFX_CORE || NET5_0
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
#else
            await CoreDispatcher.Main.RunAsync(
                CoreDispatcherPriority.Normal,
#endif
                async () =>
                {
                    await messageDialog.ShowAsync();
                });

            return result;
        }
    }
}
