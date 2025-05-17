using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger _logger;

    private Window? _activeDialog = null;
    private readonly SemaphoreSlim _dialogSemaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="scopedContextService"></param>
    /// <param name="logger"></param>
    public DialogService(
        IScopedContextService scopedContextService,
        ILogger<DialogService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogTrace($"DialogService instance created with ID: {Guid.NewGuid()}");

        _scopedContextService = scopedContextService ?? throw new ArgumentNullException(nameof(scopedContextService));
    }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "")
    {
        Argument.IsNotNull(error);

        return ShowMessageAsync(
            error.ToMessage(Environment.StackTrace),
            !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleError"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "")
    {
        Argument.IsNotNullOrEmpty(message);

        return ShowMessageAsync(
            message,
            !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleError"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// Shows the information asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "")
    {
        Argument.IsNotNullOrEmpty(message);

        return ShowMessageAsync(
            message,
            !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleInfo"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// Shows the warning asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "")
    {
        Argument.IsNotNullOrEmpty(message);

        return ShowMessageAsync(
            message,
            !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleWarning"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// Shows the greeting asynchronous.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Task.</returns>
    public Task ShowGreetingAsync(string name)
    {
        Argument.IsNotNullOrEmpty(name);

        if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
            return ShowMessageAsync(
                string.Format(LanguageService.Default.GetString("Greeting_Night"), name),
                LanguageService.Default.GetString("TitleWelcome"),
                MessageBoxButtons.OK);

        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
            return ShowMessageAsync(
                string.Format(LanguageService.Default.GetString("Greeting_Morning"), name),
                LanguageService.Default.GetString("TitleWelcome"),
                MessageBoxButtons.OK);

        if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            return ShowMessageAsync(
                string.Format(LanguageService.Default.GetString("Greeting_Afternoon"), name),
                LanguageService.Default.GetString("TitleWelcome"),
                MessageBoxButtons.OK);

        return ShowMessageAsync(
            string.Format(LanguageService.Default.GetString("Greeting_Evening"), name),
            LanguageService.Default.GetString("TitleWelcome"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// show an Content Dialog.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="buttons">The buttons.</param>
    /// <param name="notificationTypes"></param>
    /// <returns>MessageBoxResult.</returns>
    public async Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default)
    {
        Argument.IsNotNullOrEmpty(message);

        try
        {
            var dialog = new Window()
            {
                Title = title,
                Content = message
            };

            // Safely set XamlRoot
            if (Application.MainWindow is not null && Application.MainWindow.Content is not null)
            {
                try
                {
                    dialog.XamlRoot = Application.MainWindow.Content.XamlRoot;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set XamlRoot for dialog");
                }
            }

            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    dialog.PrimaryButtonText = LanguageService.Default.GetString("Ok");
                    dialog.CloseButtonText = LanguageService.Default.GetString("Cancel");

                    try
                    {
                        dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set button styles");
                    }
                    break;
                case MessageBoxButtons.YesNoCancel:
                    dialog.PrimaryButtonText = LanguageService.Default.GetString("Yes");
                    dialog.SecondaryButtonText = LanguageService.Default.GetString("No");
                    dialog.CloseButtonText = LanguageService.Default.GetString("Cancel");

                    try
                    {
                        dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                        dialog.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set button styles");
                    }
                    break;
                case MessageBoxButtons.YesNo:
                    dialog.PrimaryButtonText = LanguageService.Default.GetString("Yes");
                    dialog.CloseButtonText = LanguageService.Default.GetString("No");

                    try
                    {
                        dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set button styles");
                    }
                    break;
                default:
                    dialog.CloseButtonText = LanguageService.Default.GetString("Ok");

                    try
                    {
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set button styles");
                    }
                    break;
            }

            var result = await OpenDialogAsync(dialog);

            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    switch (result)
                    {
                        case ContentDialogResult.Primary:
                            return MessageBoxResult.OK;
                        default:
                            return MessageBoxResult.Cancel;
                    }
                case MessageBoxButtons.YesNoCancel:
                    switch (result)
                    {
                        case ContentDialogResult.Primary:
                            return MessageBoxResult.Yes;
                        case ContentDialogResult.Secondary:
                            return MessageBoxResult.No;
                        default:
                            return MessageBoxResult.Cancel;
                    }
                case MessageBoxButtons.YesNo:
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing message dialog");
            return MessageBoxResult.None;
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
        try
        {
            if (_scopedContextService.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
                _scopedContextService.GetRequiredService(typeof(TWindow)) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error showing dialog for {typeof(TWindow).Name} and {typeof(TViewModel).Name}");
        }
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
        try
        {
            if (_scopedContextService.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
                _scopedContextService.GetRequiredService(typeof(TWindow)) is Window dialog)
            {
                viewmodel.SetSelectedItem(e);
                await CreateDialogAsync(dialog, viewmodel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error showing dialog for {typeof(TWindow).Name} and {typeof(TViewModel).Name} with entity");
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
        Argument.IsNotNull(window);
        Argument.IsNotNull(viewmodel);

        try
        {
            if (_scopedContextService.GetRequiredService(window.GetType()) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error showing dialog for {window.GetType().Name}");
        }
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
        Argument.IsNotNull(type);
        Argument.IsNotNull(viewmodel);

        try
        {
            if (_scopedContextService.GetRequiredService(type) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error showing dialog for {type.Name}");
        }
    }

    /// <summary>
    /// Shows dialog as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="dialog">The dialog.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
    {
        Argument.IsNotNull(dialog);
        Argument.IsNotNull(viewmodel);

        if (dialog is Window window)
        {
            try
            {
                // Safely set XamlRoot
                if (Application.MainWindow is not null && Application.MainWindow.Content is not null)
                {
                    try
                    {
                        window.XamlRoot = Application.MainWindow.Content.XamlRoot;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set XamlRoot for dialog");
                    }
                }

                window.ViewModel = viewmodel;

                window.PrimaryButtonCommand = viewmodel.SubmitCommand;
                window.SecondaryButtonCommand = viewmodel.CancelCommand;
                window.CloseButtonCommand = viewmodel.CancelCommand;

                try
                {
                    window.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                    window.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                    window.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set button styles");
                }

                EventHandler? viewModelClosedHandler = null;
                viewModelClosedHandler = (sender, e) =>
                {
                    try
                    {
                        if (sender is IViewModelDialog<TEntity> vm)
                            vm.Closed -= viewModelClosedHandler;

                        window.ViewModel?.Dispose();
                        window.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in viewmodel closed handler");
                    }
                };

                viewmodel.Closed += viewModelClosedHandler;

                try
                {
                    await viewmodel.InitializeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initializing viewmodel");
                    viewmodel.Closed -= viewModelClosedHandler;
                    return;
                }

                await OpenDialogAsync(window);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dialog");
            }
        }
    }

    private async Task<ContentDialogResult> OpenDialogAsync(Window dialog)
    {
        Argument.IsNotNull(dialog);

        // Use semaphore to prevent multiple dialogs from being opened simultaneously
        await _dialogSemaphore.WaitAsync();

        try
        {
            if (_activeDialog is not null)
            {
                try
                {
                    var tcs = new TaskCompletionSource<bool>();

                    // Add event handler to know when dialog is closed
                    _activeDialog.Closed += (s, e) => tcs.TrySetResult(true);

                    // Close the active dialog
                    _activeDialog.Close();

                    // Wait for dialog to close with timeout
                    var timeoutTask = Task.Delay(500);
                    var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        _logger.LogTrace("Dialog close operation timed out");
                        // Force cleanup of previous dialog reference
                        _activeDialog = null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error closing active dialog");
                    _activeDialog = null;
                }
            }

            _activeDialog = dialog;

            try
            {
                // Add timeout to dialog show operation
                var showTask = dialog.ShowAsync().AsTask();
                var timeoutTask = Task.Delay(10000); // 10 second timeout for showing dialog

                var completedTask = await Task.WhenAny(showTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    _logger.LogTrace("Dialog show operation timed out");
                    return ContentDialogResult.None;
                }

                return await showTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing dialog");
                return ContentDialogResult.None;
            }
        }
        finally
        {
            _dialogSemaphore.Release();
        }
    }

    // Implement IDisposable to clean up resources
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _dialogSemaphore.Dispose();

                // Close any active dialog
                if (_activeDialog != null)
                {
                    try
                    {
                        _activeDialog.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error closing dialog during disposal");
                    }
                    _activeDialog = null;
                }
            }

            _disposed = true;
        }
    }

    ~DialogService()
    {
        Dispose(false);
    }
}


