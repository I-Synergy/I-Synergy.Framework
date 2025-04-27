using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger _logger;

    private Window? _activeDialog = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="scopedContextService"></param>
    /// <param name="logger"></param>
    public DialogService(
        IScopedContextService scopedContextService,
        ILogger<DialogService> logger)
    {
        _logger = logger;
        _logger.LogTrace($"DialogService instance created with ID: {Guid.NewGuid()}");

        _scopedContextService = scopedContextService;
    }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "") =>
        ShowMessageAsync(error.ToMessage(Environment.StackTrace), !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleError"), MessageBoxButton.OK);

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleError"), MessageBoxButton.OK);

    /// <summary>
    /// Shows the information asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleInfo"), MessageBoxButton.OK);

    /// <summary>
    /// Shows the warning asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleWarning"), MessageBoxButton.OK);

    /// <summary>
    /// Shows the greeting asynchronous.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Task.</returns>
    public Task ShowGreetingAsync(string name)
    {
        if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
        {
            return ShowMessageAsync(string.Format(LanguageService.Default.GetString("Greeting_Night"), name),
                LanguageService.Default.GetString("TitleWelcome"), MessageBoxButton.OK);
        }

        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
        {
            return ShowMessageAsync(string.Format(LanguageService.Default.GetString("Greeting_Morning"), name),
                LanguageService.Default.GetString("TitleWelcome"), MessageBoxButton.OK);
        }
        if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
        {
            return ShowMessageAsync(string.Format(LanguageService.Default.GetString("Greeting_Afternoon"), name),
                LanguageService.Default.GetString("TitleWelcome"), MessageBoxButton.OK);
        }
        return ShowMessageAsync(string.Format(LanguageService.Default.GetString("Greeting_Evening"), name),
            LanguageService.Default.GetString("TitleWelcome"), MessageBoxButton.OK);
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

        if (Application.MainWindow is not null)
            dialog.XamlRoot = Application.MainWindow.Content.XamlRoot;

        switch (buttons)
        {
            case MessageBoxButton.OKCancel:
                dialog.PrimaryButtonText = LanguageService.Default.GetString("Ok");
                dialog.CloseButtonText = LanguageService.Default.GetString("Cancel");
                dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                break;
            case MessageBoxButton.YesNoCancel:
                dialog.PrimaryButtonText = LanguageService.Default.GetString("Yes");
                dialog.SecondaryButtonText = LanguageService.Default.GetString("No");
                dialog.CloseButtonText = LanguageService.Default.GetString("Cancel");
                dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                dialog.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                break;
            case MessageBoxButton.YesNo:
                dialog.PrimaryButtonText = LanguageService.Default.GetString("Yes");
                dialog.CloseButtonText = LanguageService.Default.GetString("No");
                dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                break;
            default:
                dialog.CloseButtonText = LanguageService.Default.GetString("Ok");
                dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                break;
        }

        var result = await OpenDialogAsync(dialog);

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

    /// <summary>
    /// Shows the dialog asynchronous.
    /// </summary>
    /// <typeparam name="TWindow"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>()
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
    {
        if (_scopedContextService.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
            _scopedContextService.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
            await CreateDialogAsync(dialog, viewmodel);
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
        if (_scopedContextService.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
            _scopedContextService.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
        {
            viewmodel.SetSelectedItem(e);
            await CreateDialogAsync(dialog, viewmodel);
        }
    }

    /// <summary>
    /// Shows the dialog asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="window">The window.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public async Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
    {
        if (_scopedContextService.ServiceProvider.GetRequiredService(window.GetType()) is Window dialog)
            await CreateDialogAsync(dialog, viewmodel);
    }

    /// <summary>
    /// Shows the dialog asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="type">The type.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public async Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
    {
        if (_scopedContextService.ServiceProvider.GetRequiredService(type) is Window dialog)
            await CreateDialogAsync(dialog, viewmodel);
    }

    /// <summary>
    /// Shows dialog as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="dialog">The dialog.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
    {
        if (dialog is Window window)
        {
            if (Application.MainWindow is not null)
                window.XamlRoot = Application.MainWindow.Content.XamlRoot;

            window.ViewModel = viewmodel;

            window.PrimaryButtonCommand = viewmodel.SubmitCommand;
            window.SecondaryButtonCommand = viewmodel.CancelCommand;
            window.CloseButtonCommand = viewmodel.CancelCommand;

            window.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
            window.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
            window.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];

            void ViewModelClosedHandler(object? sender, EventArgs e)
            {
                if (sender is IViewModelDialog<TEntity> vm)
                    vm.Closed -= ViewModelClosedHandler;

                window.ViewModel?.Dispose();
                window.Close();
            }
            ;

            viewmodel.Closed += ViewModelClosedHandler;

            await viewmodel.InitializeAsync();

            await OpenDialogAsync(window);
        }
    }

    private async Task<ContentDialogResult> OpenDialogAsync(Window dialog)
    {
        if (_activeDialog is not null)
        {
            _activeDialog.Close();
        }

        _activeDialog = dialog;

        return await _activeDialog.ShowAsync().AsTask();
    }
}
