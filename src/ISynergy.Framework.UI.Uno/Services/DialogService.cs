using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Services
{
    public class DialogService : BaseDialogService
    {
        private Window _activeDialog = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        public DialogService(ILanguageService languageService)
            : base(languageService)
        {
        }

        /// <summary>
        /// show an Content Dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns>MessageBoxResult.</returns>
        public override async Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK)
        {
            var dialog = new Window()
            {
                Title = title,
                Content = message
            };

            if (Application.Current is BaseApplication baseApplication)
                dialog.XamlRoot = baseApplication.MainWindow.Content.XamlRoot;

            switch (buttons)
            {
                case MessageBoxButton.OKCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                case MessageBoxButton.YesNoCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.SecondaryButtonText = _languageService.GetString("No");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                case MessageBoxButton.YesNo:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.CloseButtonText = _languageService.GetString("No");
                    dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                default:
                    dialog.CloseButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
            }

            if (await OpenDialogAsync(dialog) is ContentDialogResult result)
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

        private async Task<ContentDialogResult> OpenDialogAsync(Window dialog)
        {
            if (_activeDialog is not null)
            {
                _activeDialog?.Close();
                _activeDialog = null;
            }

            _activeDialog = dialog;

            return await _activeDialog.ShowAsync().AsTask();
        }
    }
}
