using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Globalization;
using Windows.Foundation;

namespace ISynergy.Framework.UI.Services;

public class DialogService : BaseDialogService, IDialogService, IDisposable
{
    private const string DefaultDialogButtonStyleKey = "DefaultDialogButtonStyle";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILocalSettings _localSettings;
    private readonly ILanguageService _languageService;
    private readonly ILogger<DialogService> _logger;

    private Window? _activeDialog = null;
    private readonly SemaphoreSlim _dialogSemaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="scopeFactory"></param>
    /// <param name="localSettings"></param>
    /// <param name="languageService"></param>
    /// <param name="logger"></param>
    public DialogService(
        IServiceScopeFactory scopeFactory,
        ILocalSettings localSettings,
        ILanguageService languageService,
        ILogger<DialogService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogTrace("DialogService instance created with ID: {InstanceId}", Guid.NewGuid());

        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _localSettings = localSettings ?? throw new ArgumentNullException(nameof(localSettings));
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
    }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string? title = "")
    {
        Argument.IsNotNull(error);

        return ShowMessageAsync(
            error.ToMessage(Environment.StackTrace),
            !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(string message, string? title = "")
    {
        Argument.IsNotNullOrEmpty(message);

        return ShowMessageAsync(
            message,
            !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// Shows the information asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowInformationAsync(string message, string? title = "")
    {
        Argument.IsNotNullOrEmpty(message);

        return ShowMessageAsync(
            message,
            !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleInfo"),
            MessageBoxButtons.OK);
    }

    /// <summary>
    /// Shows the warning asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowWarningAsync(string message, string? title = "")
    {
        Argument.IsNotNullOrEmpty(message);

        return ShowMessageAsync(
            message,
            !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleWarning"),
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

        var (message, title) = ResolveGreeting(name, _languageService);
        return ShowMessageAsync(message, title, MessageBoxButtons.OK);
    }

    /// <summary>
    /// show an Content Dialog.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="buttons">The buttons.</param>
    /// <param name="notificationTypes"></param>
    /// <returns>MessageBoxResult.</returns>
    public async Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default) // NOSONAR
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

            if (CultureInfo.GetCultureInfo(_localSettings.Language).TextInfo.IsRightToLeft)
                dialog.FlowDirection = FlowDirection.RightToLeft;

            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Ok");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");

                    try
                    {
                        dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set button styles");
                    }
                    break;
                case MessageBoxButtons.YesNoCancel:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.SecondaryButtonText = _languageService.GetString("No");
                    dialog.CloseButtonText = _languageService.GetString("Cancel");

                    try
                    {
                        dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                        dialog.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set button styles");
                    }
                    break;
                case MessageBoxButtons.YesNo:
                    dialog.PrimaryButtonText = _languageService.GetString("Yes");
                    dialog.CloseButtonText = _languageService.GetString("No");

                    try
                    {
                        dialog.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to set button styles");
                    }
                    break;
                default:
                    dialog.CloseButtonText = _languageService.GetString("Ok");

                    try
                    {
                        dialog.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
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
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="window">The window.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <remarks>
    /// The <paramref name="window"/> must already be resolved from the caller's DI scope together
    /// with the <paramref name="viewmodel"/>. Both share the same scope so they are disposed
    /// together when the dialog closes — no additional scope is created here.
    /// </remarks>
    public async Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
    {
        Argument.IsNotNull(window);
        Argument.IsNotNull(viewmodel);

        try
        {
            if (window is Window dialog)
            {
                // Window was already resolved from the caller's scope (same scope as the ViewModel).
                // No additional scope or Closed handler is needed — the caller manages the scope
                // lifecycle and disposes both the ViewModel and the Window when Closed fires.
                await CreateDialogAsync(dialog, viewmodel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing dialog for {WindowType}", window.GetType().Name);
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
            // Create the Window via ActivatorUtilities so it is NOT tracked by a DI
            // scope. The ViewModel was resolved from the caller's scope and must not
            // be paired with a Window from a second scope. The Window lifecycle is
            // managed explicitly by CreateDialogAsync.
            using var tempScope = _scopeFactory.CreateScope();
            var dialog = ActivatorUtilities.CreateInstance(tempScope.ServiceProvider, type) as Window;

            if (dialog is not null)
                await CreateDialogAsync(dialog, viewmodel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing dialog for {WindowType}", type.Name);
        }
    }

    /// <inheritdoc />
    public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
        TEntity e,
        Func<TEntity, Task> submittedEventHandler,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TViewModel>? initialization = null)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
    {
        Argument.IsNotNull(submittedEventHandler);

        try
        {
            var scope = _scopeFactory.CreateScope();
            try
            {
                if (scope.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
                    scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
                {
                    initialization?.Invoke((TViewModel)viewmodel);
                    viewmodel.SetSelectedItem(e);

                    async void OnSubmitted(object? sender, SubmitEventArgs<TEntity> args)
                    {
                        try { await submittedEventHandler(args.Result); }
                        catch (Exception ex) { _logger.LogError(ex, "Submit callback failed for {WindowType}/{ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name); }
                    }

                    viewmodel.Submitted += OnSubmitted;

                    EventHandler? onCancelledHandler = null;
                    if (cancelledEventHandler is not null)
                    {
                        onCancelledHandler = async (_, _) =>
                        {
                            try { await cancelledEventHandler(); }
                            catch (Exception ex) { _logger.LogError(ex, "Cancel callback failed for {WindowType}/{ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name); }
                        };
                        viewmodel.Cancelled += onCancelledHandler;
                    }

                    viewmodel.Closed += async (_, _) =>
                    {
                        viewmodel.Submitted -= OnSubmitted;

                        if (onCancelledHandler is not null)
                            viewmodel.Cancelled -= onCancelledHandler;

                        if (closedEventHandler is not null)
                        {
                            try { await closedEventHandler(); }
                            catch (Exception ex) { _logger.LogError(ex, "Close callback failed for {WindowType}/{ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name); }
                        }

                        scope.Dispose();
                    };

                    await CreateDialogAsync(dialog, viewmodel);
                }
                else
                {
                    scope.Dispose();
                }
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing dialog for {WindowType} with {ViewModelType} and entity {EntityType}",
                typeof(TWindow).Name, typeof(TViewModel).Name, typeof(TEntity).Name);
        }
    }

    /// <inheritdoc />
    public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
        Func<TEntity, Task> submittedEventHandler,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TViewModel>? initialization = null)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
    {
        Argument.IsNotNull(submittedEventHandler);

        try
        {
            var scope = _scopeFactory.CreateScope();
            try
            {
                if (scope.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
                    scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
                {
                    initialization?.Invoke((TViewModel)viewmodel);

                    async void OnSubmitted(object? sender, SubmitEventArgs<TEntity> args)
                    {
                        try { await submittedEventHandler(args.Result); }
                        catch (Exception ex) { _logger.LogError(ex, "Submit callback failed for {WindowType}/{ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name); }
                    }

                    viewmodel.Submitted += OnSubmitted;

                    EventHandler? onCancelledHandler = null;
                    if (cancelledEventHandler is not null)
                    {
                        onCancelledHandler = async (_, _) =>
                        {
                            try { await cancelledEventHandler(); }
                            catch (Exception ex) { _logger.LogError(ex, "Cancel callback failed for {WindowType}/{ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name); }
                        };
                        viewmodel.Cancelled += onCancelledHandler;
                    }

                    viewmodel.Closed += async (_, _) =>
                    {
                        viewmodel.Submitted -= OnSubmitted;

                        if (onCancelledHandler is not null)
                            viewmodel.Cancelled -= onCancelledHandler;

                        if (closedEventHandler is not null)
                        {
                            try { await closedEventHandler(); }
                            catch (Exception ex) { _logger.LogError(ex, "Close callback failed for {WindowType}/{ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name); }
                        }

                        scope.Dispose();
                    };

                    await CreateDialogAsync(dialog, viewmodel);
                }
                else
                {
                    scope.Dispose();
                }
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing dialog for {WindowType} with {ViewModelType}",
                typeof(TWindow).Name, typeof(TViewModel).Name);
        }
    }

    /// <summary>
    /// Shows dialog as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="dialog">The dialog.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel) // NOSONAR
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

                if (CultureInfo.GetCultureInfo(_localSettings.Language).TextInfo.IsRightToLeft)
                    window.FlowDirection = FlowDirection.RightToLeft;

                window.ViewModel = viewmodel;

                window.PrimaryButtonCommand = viewmodel.SubmitCommand;
                window.SecondaryButtonCommand = viewmodel.CancelCommand;
                window.CloseButtonCommand = viewmodel.CancelCommand;

                try
                {
                    window.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                    window.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                    window.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources[DefaultDialogButtonStyleKey];
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set button styles");
                }

                EventHandler? viewModelClosedHandler = null;
                viewModelClosedHandler = async (sender, e) =>
                {
                    try
                    {
                        if (sender is IViewModelDialog<TEntity> vm)
                            vm.Closed -= viewModelClosedHandler;

                        await window.CloseAsync();
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

                try
                {
                    await OpenDialogAsync(window);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error opening dialog");
                    viewmodel.Closed -= viewModelClosedHandler;
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dialog");
                throw;
            }
        }
    }

    private async Task<ContentDialogResult> OpenDialogAsync(Window dialog)
    {
        Argument.IsNotNull(dialog);

        // Use semaphore to prevent multiple dialogs from being opened simultaneously
        var semaphoreAcquired = false;
        try
        {
            await _dialogSemaphore.WaitAsync();
            semaphoreAcquired = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring dialog semaphore");
            return ContentDialogResult.None;
        }

        try
        {
            if (_activeDialog is not null)
            {
                try
                {
                    var tcs = new TaskCompletionSource<bool>();

                    // Add self-removing event handler to know when dialog is closed
                    var capturedDialog = _activeDialog;
                    TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs>? handler = null;
                    handler = (s, e) =>
                    {
                        capturedDialog.Closed -= handler;
                        tcs.TrySetResult(true);
                    };
                    capturedDialog.Closed += handler;

                    // Close the active dialog
                    await capturedDialog.CloseAsync();

                    // Wait for dialog to close with timeout
                    using (var cts = new CancellationTokenSource())
                    {
                        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(500), cts.Token);
                        var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                        if (completedTask == timeoutTask)
                        {
                            _logger.LogTrace("Dialog close operation timed out");
                            // Force cleanup of previous dialog reference
                            _activeDialog = null;
                        }
                        else
                        {
                            // Cancel the timeout task since dialog closed successfully
                            cts.Cancel();
                        }
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
                using (var cts = new CancellationTokenSource())
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
                    var completedTask = await Task.WhenAny(showTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        _logger.LogTrace("Dialog show operation timed out");
                        return ContentDialogResult.None;
                    }

                    // Cancel the timeout task since dialog completed successfully
                    cts.Cancel();
                    return await showTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing dialog");
                return ContentDialogResult.None;
            }
            finally
            {
                // Clear the active dialog reference after the dialog has been dismissed.
                // ContentDialog.Hide() does NOT raise the Closed event, so we must
                // manually clear this to prevent the next dialog from trying to close
                // an already-dismissed dialog and hitting the 500ms timeout.
                _activeDialog = null;
            }
        }
        finally
        {
            // Only release if we successfully acquired the semaphore
            if (semaphoreAcquired)
            {
                try
                {
                    _dialogSemaphore.Release();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error releasing dialog semaphore");
                }
            }
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
                        // Synchronous close during disposal — async context not available
                        _ = _activeDialog.CloseAsync(); // NOSONAR - fire-and-forget acceptable in Dispose
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


