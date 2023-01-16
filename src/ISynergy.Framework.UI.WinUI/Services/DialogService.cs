using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.UI.Xaml.Controls;
using Application = Microsoft.UI.Xaml.Application;
using Style = Microsoft.UI.Xaml.Style;
using Window = ISynergy.Framework.UI.Controls.Window;

namespace ISynergy.Framework.UI.Services
{
    public class DialogService : BaseDialogService
    {
        private Window _activeDialog = null;

        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="languageService">The language service.</param>
        /// <param name="dispatcherService"></param>
        public DialogService(IServiceProvider serviceProvider, ILanguageService languageService, IDispatcherService dispatcherService)
            : base(languageService, dispatcherService)
        {
            _serviceProvider = serviceProvider;
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
                    dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                case MessageBoxButton.YesNoCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.SecondaryButtonText = _languageService.GetString("No");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");
                    dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.SecondaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                case MessageBoxButton.YesNo:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.CloseButtonText = _languageService.GetString("No");
                    dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                    break;
                default:
                    dialog.CloseButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
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

        /// <summary>
        /// Shows dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="dialog">The dialog.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        public override async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
        {
            if (dialog is Window window)
            {
                if (Application.Current is BaseApplication baseApplication)
                    window.XamlRoot = baseApplication.MainWindow.Content.XamlRoot;

                window.ViewModel = viewmodel;

                window.PrimaryButtonCommand = viewmodel.Submit_Command;
                window.SecondaryButtonCommand = viewmodel.Close_Command;
                window.CloseButtonCommand = viewmodel.Close_Command;

                window.PrimaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                window.SecondaryButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                window.CloseButtonStyle = (Style)Application.Current.Resources["DefaultDialogButtonStyle"];

                if (!viewmodel.IsInitialized)
                    await viewmodel.InitializeAsync();

                await OpenDialogAsync(window);
            }
        }

        private async Task<ContentDialogResult> OpenDialogAsync(Window dialog)
        {
            if (_activeDialog is not null)
                await CloseDialogAsync(_activeDialog);

            _activeDialog = dialog;

            return await _activeDialog.ShowAsync().AsTask();
        }

        public override Task CloseDialogAsync(IWindow dialog)
        {
            _activeDialog?.Close();
            _activeDialog = null;
            return Task.CompletedTask;
        }
    }
}
