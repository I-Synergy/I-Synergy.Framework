using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Services
{
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        private readonly ILanguageService _languageService;
        private readonly IContext _context;

        private Window _activeDialog = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="languageService">The language service.</param>
        public DialogService(
            IContext context,
            ILanguageService languageService)
        {
            _context = context;
            _languageService = languageService;
        }

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

        /// <summary>
        /// show an Content Dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns>MessageBoxResult.</returns>
        public async Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK)
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

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public Task ShowDialogAsync<TWindow, TViewModel, TEntity>()
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
        {
            var viewmodel = (IViewModelDialog<TEntity>)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel));
            return CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TWindow)), viewmodel);
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow">The type of the t window.</typeparam>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="e">The selected item.</param>
        /// <returns>Task{TEntity}.</returns>
        public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>(TEntity e)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
        {
            var viewmodel = (IViewModelDialog<TEntity>)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel));
            await viewmodel.SetSelectedItemAsync(e);
            await CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TWindow)), viewmodel);
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="window">The window.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(window.GetType()), viewmodel);

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel) =>
            CreateDialogAsync((IWindow)_context.ScopedServices.ServiceProvider.GetRequiredService(type), viewmodel);

        /// <summary>
        /// Shows dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="dialog">The dialog.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
        {
            using (var window = dialog as Window)
            {
                if (Application.Current is BaseApplication baseApplication)
                    window.XamlRoot = baseApplication.MainWindow.Content.XamlRoot;

                window.ViewModel = viewmodel;

                window.PrimaryButtonCommand = viewmodel.SubmitCommand;
                window.SecondaryButtonCommand = viewmodel.CloseCommand;
                window.CloseButtonCommand = viewmodel.CloseCommand;

                window.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                window.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
                window.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];

                await viewmodel.InitializeAsync();

                viewmodel.Closed += (sender, e) =>
                {
                    if (window is not null)
                    {
                        window.Close();
                        window.Dispose();
                    }
                };

                await OpenDialogAsync(window);
            }
        }

        private async Task<ContentDialogResult> OpenDialogAsync(Window dialog)
        {
            if (_activeDialog is not null)
            {
                _activeDialog.Close();
                _activeDialog.Dispose();
            }

            _activeDialog = dialog;

            return await _activeDialog.ShowAsync().AsTask();
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public Task CloseDialogAsync(IWindow dialog)
        {
            if (dialog is not null)
            {
                dialog.Close();
                dialog.Dispose();
            }
            
            return Task.CompletedTask;
        }
    }
}
