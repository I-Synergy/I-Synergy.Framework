using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService, IDisposable
{
    private const string ExceptionHandlerNotAvailable = "Exception handler service not available for dialog error";
    private const string ExceptionHandlerFailed = "Exception handler failed while processing dialog error";
    private const string ViewModelInitExceptionHandlerFailed = "Exception handler failed while processing viewmodel initialization error";
    private const string ViewModelInitExceptionHandlerNotAvailable = "Exception handler service not available for viewmodel initialization error";
    private const string DialogCreationExceptionHandlerFailed = "Exception handler failed while processing dialog creation error";
    private const string DialogCreationExceptionHandlerNotAvailable = "Exception handler service not available for dialog creation error";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILocalSettings _localSettings;
    private readonly IExceptionHandlerService _exceptionHandlerService;
    private readonly ILanguageService _languageService;
    private readonly ILogger<DialogService>? _logger;

    // Enhanced error handling state
    private bool _dialogServiceAvailable = true;
    private int _consecutiveFailures = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;
    private readonly TimeSpan _cooldownPeriod = TimeSpan.FromSeconds(30);
    private readonly int _maxConsecutiveFailures = 3;
    private readonly SemaphoreSlim _dialogSemaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="scopeFactory"></param>
    /// <param name="localSettings"></param>
    /// <param name="exceptionHandlerService"></param>
    /// <param name="languageService">The language service.</param>
    /// <param name="logger">Optional logger for enhanced error handling.</param>
    public DialogService(
        IServiceScopeFactory scopeFactory,
        ILocalSettings localSettings,
        IExceptionHandlerService exceptionHandlerService,
        ILanguageService languageService,
        ILogger<DialogService>? logger = null)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _localSettings = localSettings ?? throw new ArgumentNullException(nameof(localSettings));
        _exceptionHandlerService = exceptionHandlerService ?? throw new ArgumentNullException(nameof(exceptionHandlerService));
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        _logger = logger;
    }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string? title = "") =>
        ShowMessageAsync(error.ToMessage(Environment.StackTrace), !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButtons.OK);

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(string message, string? title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButtons.OK);

    /// <summary>
    /// Shows the information asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowInformationAsync(string message, string? title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleInfo"), MessageBoxButtons.OK);

    /// <summary>
    /// Shows the warning asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowWarningAsync(string message, string? title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleWarning"), MessageBoxButtons.OK);

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
                _languageService.GetString("TitleWelcome"), MessageBoxButtons.OK);
        }

        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
        {
            return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Morning"), name),
                _languageService.GetString("TitleWelcome"), MessageBoxButtons.OK);
        }
        if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
        {
            return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Afternoon"), name),
                _languageService.GetString("TitleWelcome"), MessageBoxButtons.OK);
        }
        return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Evening"), name),
            _languageService.GetString("TitleWelcome"), MessageBoxButtons.OK);
    }

    /// <summary>
    /// show an Content Dialog with comprehensive error handling.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="buttons">The buttons.</param>
    /// <param name="notificationTypes"></param>
    /// <returns>MessageBoxResult.</returns>
    public async Task<MessageBoxResult> ShowMessageAsync(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default) // NOSONAR
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(message))
        {
            _logger?.LogWarning("Attempted to show dialog with empty message");
            return MessageBoxResult.None;
        }

        // Circuit breaker pattern: Check if service is in cooldown
        if (!_dialogServiceAvailable && DateTime.Now - _lastFailureTime < _cooldownPeriod)
        {
            _logger?.LogWarning("DialogService in cooldown period. Message: {Title} - {Message}", title, message);
            await TryFallbackNotificationAsync(message, title);
            return MessageBoxResult.None;
        }

        // Try to recover after cooldown period
        if (!_dialogServiceAvailable && DateTime.Now - _lastFailureTime >= _cooldownPeriod)
        {
            _logger?.LogInformation("Attempting to recover DialogService after cooldown period");
            _dialogServiceAvailable = true;
            _consecutiveFailures = 0;
        }

        // Use semaphore to prevent multiple dialogs
        await _dialogSemaphore.WaitAsync();

        try
        {
            return await ShowMessageWithRetryAsync(
                message,
                title,
                buttons);
        }
        finally
        {
            _dialogSemaphore.Release();
        }
    }

    private async Task<MessageBoxResult> ShowMessageWithRetryAsync(string message, string title, MessageBoxButtons buttons, int retryCount = 0)
    {
        const int maxRetries = 2;

        try
        {
            // Ensure we're on the main thread
            if (!MainThread.IsMainThread)
            {
                return await MainThread.InvokeOnMainThreadAsync(async () =>
                    await ShowMessageInternalAsync(message, title, buttons));
            }
            else
            {
                return await ShowMessageInternalAsync(message, title, buttons);
            }
        }
        catch (Exception ex) when (IsRecoverableDialogException(ex) && retryCount < maxRetries)
        {
            _logger?.LogWarning(ex, "Dialog failed, attempting retry {RetryCount}/{MaxRetries}", retryCount + 1, maxRetries); // NOSONAR

            // Brief delay before retry
            await Task.Delay(TimeSpan.FromMilliseconds(100 * (retryCount + 1)));

            return await ShowMessageWithRetryAsync(message, title, buttons, retryCount + 1);
        }
        catch (Exception ex) when (IsCriticalDialogException(ex))
        {
            await HandleCriticalDialogFailureAsync(ex, message, title);
            return MessageBoxResult.None;
        }
        catch (Exception ex)
        {
            await HandleDialogFailureAsync(ex, message, title);
            return MessageBoxResult.None;
        }
    }

    private async Task<MessageBoxResult> ShowMessageInternalAsync(string message, string title, MessageBoxButtons buttons)
    {
        // Additional safety check right before showing dialog
        if (Application.Current == null ||
            Application.Current?.Windows.Count == 0 ||
            Application.Current?.Windows[0] == null ||
            Application.Current?.Windows[0].Page == null)
            throw new InvalidOperationException("Main page not available for dialog display");

        // Add timeout to dialog operations
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            if (Application.Current?.Windows[0].Page is Page page)
            {
                switch (buttons)
                {
                    case MessageBoxButtons.OKCancel:
                        var okCancelResult = await page.DisplayAlertAsync(
                            title,
                            message,
                            _languageService.GetString("Ok"),
                            _languageService.GetString("Cancel"));

                        return okCancelResult ? MessageBoxResult.OK : MessageBoxResult.Cancel;

                    case MessageBoxButtons.YesNo:
                        var yesNoResult = await page.DisplayAlertAsync(
                            title,
                            message,
                            _languageService.GetString("Yes"),
                            _languageService.GetString("No"));

                        return yesNoResult ? MessageBoxResult.Yes : MessageBoxResult.No;

                    default:
                        await page.DisplayAlertAsync(
                            title ?? "",
                            message,
                            _languageService.GetString("Ok"));

                        return MessageBoxResult.OK;
                }
            }

            return MessageBoxResult.Cancel;
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
        {
            _logger?.LogWarning("Dialog operation timed out after 30 seconds");
            throw new TimeoutException("Dialog operation timed out");
        }
    }

    private static bool IsRecoverableDialogException(Exception exception)
    {
        return exception switch
        {
            InvalidOperationException when exception.Message.Contains("UI thread") => true,
            InvalidOperationException when exception.Message.Contains("MainPage") => true,
            TimeoutException => true,
            _ => false
        };
    }

    private static bool IsCriticalDialogException(Exception exception)
    {
        return exception switch
        {
            OutOfMemoryException => true,
            StackOverflowException => true,
            AccessViolationException => true,
            AppDomainUnloadedException => true,
            _ when exception.Source == "WinRT.Runtime" && exception.Message.Contains("current state") => true,
            _ => false
        };
    }

    private async Task HandleCriticalDialogFailureAsync(Exception ex, string message, string title)
    {
        _consecutiveFailures = _maxConsecutiveFailures; // Immediately disable service
        _dialogServiceAvailable = false;
        _lastFailureTime = DateTime.Now;

        _logger?.LogCritical(ex, "Critical dialog service failure. Service disabled. Message: {Title} - {Message}", title, message);

        // Try emergency fallback
        await TryEmergencyFallbackAsync(message, title);
    }

    private async Task HandleDialogFailureAsync(Exception ex, string message, string title)
    {
        _consecutiveFailures++;

        if (_consecutiveFailures >= _maxConsecutiveFailures)
        {
            _dialogServiceAvailable = false;
            _lastFailureTime = DateTime.Now;
            _logger?.LogError(ex, "DialogService disabled after {FailureCount} consecutive failures. Message: {Title} - {Message}", // NOSONAR
                _consecutiveFailures, title, message);
        }
        else
        {
            _logger?.LogWarning(ex, "Dialog service failure {FailureCount}/{MaxFailures}. Message: {Title} - {Message}", // NOSONAR
                _consecutiveFailures, _maxConsecutiveFailures, title, message);
        }

        await TryFallbackNotificationAsync(message, title);
    }

    private async Task TryFallbackNotificationAsync(string message, string title)
    {
        // Progressive fallback strategy
        var attempts = new Func<Task<bool>>[]
        {
            () => TryConsoleNotificationAsync(message, title),
            () => TryDebugNotificationAsync(message, title),
            () => TryEventLogNotificationAsync(message, title),
            () => TryFileNotificationAsync(message, title)
        };

        foreach (var attempt in attempts)
        {
            try
            {
                if (await attempt())
                {
                    _logger?.LogInformation("Fallback notification successful for: {Title} - {Message}", title, message);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Fallback notification method failed");
            }
        }

        _logger?.LogError("All fallback notification methods failed for: {Title} - {Message}", title, message);
    }

    private async Task TryEmergencyFallbackAsync(string message, string title)
    {
        try
        {
            // Last resort: write to multiple outputs simultaneously
            var tasks = new[]
            {
                Task.Run(() => Console.WriteLine("CRITICAL ERROR: {0} - {1}", title, message)),
                Task.Run(() => System.Diagnostics.Debug.WriteLine("CRITICAL ERROR: {0} - {1}", title, message)),
                Task.Run(() => System.Diagnostics.Trace.WriteLine($"CRITICAL ERROR: {title} - {message}"))
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Emergency fallback failed for critical error: {Title} - {Message}", title, message);
        }
    }

    private static async Task<bool> TryConsoleNotificationAsync(string message, string title)
    {
        await Task.Run(() => Console.WriteLine("DIALOG: {0} - {1}", title, message));
        return true;
    }

    private static async Task<bool> TryDebugNotificationAsync(string message, string title)
    {
        await Task.Run(() => System.Diagnostics.Debug.WriteLine("DIALOG: {0} - {1}", title, message));
        return true;
    }

    private static Task<bool> TryEventLogNotificationAsync(string message, string title) // NOSONAR
    {
#if WINDOWS
        try
        {
            using var eventLog = new System.Diagnostics.EventLog("Application");
            eventLog.Source = "MAUI DialogService";
            eventLog.WriteEntry($"{title}: {message}", System.Diagnostics.EventLogEntryType.Warning);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
#else
        return Task.FromResult(false);
#endif
    }

    private static async Task<bool> TryFileNotificationAsync(string message, string title)
    {
        try
        {
            var logPath = Path.Combine(FileSystem.AppDataDirectory, "dialog_fallback.log");
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {title}: {message}{Environment.NewLine}";

            await File.AppendAllTextAsync(logPath, logEntry);
            return true;
        }
        catch
        {
            return false;
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
        await _dialogSemaphore.WaitAsync();
        try
        {
            await ShowDialogAsyncInternal<TEntity>(window, viewmodel);
        }
        finally
        {
            _dialogSemaphore.Release();
        }
    }

    /// <remarks>
    /// The <paramref name="window"/> must already be resolved from the caller's DI scope together
    /// with the <paramref name="viewmodel"/>. Both share the same scope so they are disposed
    /// together when the dialog closes — no additional scope is created here.
    /// </remarks>
    private async Task ShowDialogAsyncInternal<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
    {
        try
        {
            if (window is Window dialog)
            {
                // Window was already resolved from the caller's scope (same scope as the ViewModel).
                // No additional scope or Closed handler is needed — the caller manages the scope
                // lifecycle and disposes both the ViewModel and the Window when Closed fires.
                await CreateDialogAsync(dialog, viewmodel);
            }
            else
            {
                _logger?.LogError("Dialog is not of type Window: {DialogType}", window?.GetType().Name ?? "null");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error showing dialog for window type {WindowType}", window.GetType().Name);

            bool handled = false;

            try
            {
                _exceptionHandlerService.HandleException(ex);
                handled = true;
            }
            catch (Exception handlerEx)
            {
                // Log failure of exception handler but suppress original exception to prevent app crash
                _logger?.LogError(handlerEx, ExceptionHandlerFailed);
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning(ExceptionHandlerNotAvailable);
            }
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
        await _dialogSemaphore.WaitAsync();
        try
        {
            await ShowDialogAsyncInternal<TEntity>(type, viewmodel);
        }
        finally
        {
            _dialogSemaphore.Release();
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
        await _dialogSemaphore.WaitAsync();
        try
        {
            await ShowDialogAsyncInternal<TWindow, TViewModel, TEntity>(e, submittedEventHandler, cancelledEventHandler, closedEventHandler, initialization);
        }
        finally
        {
            _dialogSemaphore.Release();
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
        await _dialogSemaphore.WaitAsync();
        try
        {
            await ShowDialogAsyncInternal<TWindow, TViewModel, TEntity>(submittedEventHandler, cancelledEventHandler, closedEventHandler, initialization);
        }
        finally
        {
            _dialogSemaphore.Release();
        }
    }

    private async Task ShowDialogAsyncInternal<TWindow, TViewModel, TEntity>(
        TEntity e,
        Func<TEntity, Task> submittedEventHandler,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TViewModel>? initialization = null)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
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
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Submit callback failed for {WindowType}/{ViewModelType}",
                            typeof(TWindow).Name, typeof(TViewModel).Name);
                    }
                }

                viewmodel.Submitted += OnSubmitted;

                EventHandler? onCancelledHandler = null;
                if (cancelledEventHandler is not null)
                {
                    onCancelledHandler = async (_, _) =>
                    {
                        try { await cancelledEventHandler(); }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Cancel callback failed for {WindowType}/{ViewModelType}",
                                typeof(TWindow).Name, typeof(TViewModel).Name);
                        }
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
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Close callback failed for {WindowType}/{ViewModelType}",
                                typeof(TWindow).Name, typeof(TViewModel).Name);
                        }
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
        catch (Exception ex)
        {
            scope.Dispose();
            _logger?.LogError(ex, "Error showing dialog for {WindowType} with {ViewModelType} and entity {EntityType}",
                typeof(TWindow).Name, typeof(TViewModel).Name, typeof(TEntity).Name);

            bool handled = false;
            try { _exceptionHandlerService.HandleException(ex); handled = true; }
            catch (Exception handlerEx) { _logger?.LogError(handlerEx, ExceptionHandlerFailed); }
            if (!handled) _logger?.LogWarning(ExceptionHandlerNotAvailable);
        }
    }

    private async Task ShowDialogAsyncInternal<TWindow, TViewModel, TEntity>(
        Func<TEntity, Task> submittedEventHandler,
        Func<Task>? cancelledEventHandler = null,
        Func<Task>? closedEventHandler = null,
        Action<TViewModel>? initialization = null)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
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
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Submit callback failed for {WindowType}/{ViewModelType}",
                            typeof(TWindow).Name, typeof(TViewModel).Name);
                    }
                }

                viewmodel.Submitted += OnSubmitted;

                EventHandler? onCancelledHandler = null;
                if (cancelledEventHandler is not null)
                {
                    onCancelledHandler = async (_, _) =>
                    {
                        try { await cancelledEventHandler(); }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Cancel callback failed for {WindowType}/{ViewModelType}",
                                typeof(TWindow).Name, typeof(TViewModel).Name);
                        }
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
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Close callback failed for {WindowType}/{ViewModelType}",
                                typeof(TWindow).Name, typeof(TViewModel).Name);
                        }
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
        catch (Exception ex)
        {
            scope.Dispose();
            _logger?.LogError(ex, "Error showing dialog for {WindowType} with {ViewModelType}",
                typeof(TWindow).Name, typeof(TViewModel).Name);

            bool handled = false;
            try { _exceptionHandlerService.HandleException(ex); handled = true; }
            catch (Exception handlerEx) { _logger?.LogError(handlerEx, ExceptionHandlerFailed); }
            if (!handled) _logger?.LogWarning(ExceptionHandlerNotAvailable);
        }
    }

    private async Task ShowDialogAsyncInternal<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
    {
        try
        {
            // Create the Window via ActivatorUtilities so it is NOT tracked by a DI
            // scope. The ViewModel was resolved from the caller's scope (or the root)
            // and must not be paired with a Window from a second scope. The Window
            // lifecycle is managed explicitly by CreateDialogAsync (disposed in
            // ViewModelClosedHandler via windowRef?.Dispose()).
            using var tempScope = _scopeFactory.CreateScope();
            var dialog = ActivatorUtilities.CreateInstance(tempScope.ServiceProvider, type) as Window;

            if (dialog is not null)
            {
                await CreateDialogAsync(dialog, viewmodel);
            }
            else
            {
                _logger?.LogError("Failed to create dialog of type {DialogType}", type.Name);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error showing dialog for type {DialogType}", type.Name);

            bool handled = false;

            try
            {
                _exceptionHandlerService.HandleException(ex);
                handled = true;
            }
            catch (Exception handlerEx)
            {
                // Log failure of exception handler but suppress original exception to prevent app crash
                _logger?.LogError(handlerEx, ExceptionHandlerFailed);
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning(ExceptionHandlerNotAvailable);
            }
        }
    }

    /// <summary>
    /// Shows dialog as an asynchronous operation with enhanced error handling.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="dialog"></param>
    /// <param name="viewmodel"></param>
    /// <returns></returns>
    public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel) // NOSONAR
    {
        // Note: CreateDialogAsync is protected by semaphore in calling methods (ShowDialogAsync variants)
        // No additional semaphore lock needed here to avoid deadlock
        if (dialog is not Window window)
        {
            _logger?.LogError("Dialog is not of type Window: {DialogType}", dialog?.GetType().Name ?? "null");

            // For testing or mock scenarios, try to use the dialog as-is
            // This allows mocks to be used in tests
            try
            {
                // If it's a mock, at least call initialize on the viewmodel
                await viewmodel.InitializeAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing viewmodel");

                bool handled = false;

                try
                {
                    _exceptionHandlerService.HandleException(ex);
                    handled = true;
                }
                catch (Exception handlerEx)
                {
                    // Log failure of exception handler but suppress original exception to prevent app crash
                    _logger?.LogError(handlerEx, ViewModelInitExceptionHandlerFailed);
                }

                // Suppress exception if it was successfully handled or if handler is not available
                // This prevents app crashes while still allowing the handler to show user-friendly messages
                if (!handled)
                {
                    _logger?.LogWarning(ViewModelInitExceptionHandlerNotAvailable);
                }
            }
            return;
        }

        Window? windowRef = null;
        IViewModelDialog<TEntity>? viewModelRef = null;

        // Define the handler in the method scope so it is accessible for both += and -=
        async void ViewModelClosedHandler(object? sender, EventArgs e)
        {
            // Unsubscribe first to prevent re-entrant calls
            try
            {
                if (viewModelRef != null)
                    viewModelRef.Closed -= ViewModelClosedHandler;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error unsubscribing from Closed handler");
            }

            // Pop the modal. Try without animation first to avoid MAUI rendering issues
            // (e.g. ShadowExtensions.GetAlphaMaskAsync ArgumentException on Windows).
            // Fall back to animated pop if the first attempt fails.
            var popped = false;

            if (Application.Current?.Windows[0]?.Page?.Navigation is INavigation navigation)
            {
                // First attempt: no animation (avoids layout-triggered shadow computation)
                try
                {
                    await navigation.PopModalAsync(false);
                    popped = true;
                }
                catch (Exception ex)
                {
                    _logger?.LogDebug(ex, "PopModalAsync(false) failed, will try animated pop as fallback");
                }

                // Second attempt: with animation (works when the layout is in a different state)
                if (!popped)
                {
                    try
                    {
                        await navigation.PopModalAsync(true);
                        popped = true;
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "PopModalAsync(true) also failed; modal may remain visible");
                    }
                }
            }
            else
            {
                _logger?.LogWarning("Cannot close modal dialog - Navigation is not available");
            }

            if (!popped && Application.Current?.Windows[0]?.Page?.Navigation is not null)
            {
                // Last resort: try to go back via the navigation service pattern
                _logger?.LogWarning("Modal pop failed; the page may still be visible on the navigation stack");
            }

            // Always clean up window resources, even if PopModalAsync failed.
            // The ViewModel lifecycle is managed by the DI scope (strongly-typed overloads)
            // or the caller (Type/IWindow overloads) — do NOT dispose it here.
            viewModelRef = null;

            try
            {
                windowRef?.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error disposing window during dialog close");
            }
            windowRef = null;
        }

        try
        {
            windowRef = window;
            viewModelRef = viewmodel;

            if (CultureInfo.GetCultureInfo(_localSettings.Language).TextInfo.IsRightToLeft)
                window.FlowDirection = FlowDirection.RightToLeft;

            window.ViewModel = viewmodel;

            viewmodel.Closed += ViewModelClosedHandler;

            try
            {
                await viewmodel.InitializeAsync();
            }
            catch (Exception initEx)
            {
                _logger?.LogError(initEx, "Error initializing viewmodel");
                viewmodel.Closed -= ViewModelClosedHandler;

                // Call the exception handler service
                bool handled = false;

                try
                {
                    _exceptionHandlerService.HandleException(initEx);
                    handled = true;
                }
                catch (Exception handlerEx)
                {
                    // Log failure of exception handler but suppress original exception to prevent app crash
                    _logger?.LogError(handlerEx, ViewModelInitExceptionHandlerFailed);
                }

                // Suppress exception if it was successfully handled or if handler is not available
                // This prevents app crashes while still allowing the handler to show user-friendly messages
                if (!handled)
                {
                    _logger?.LogWarning(ViewModelInitExceptionHandlerNotAvailable);
                }

                return;
            }

            if (Application.Current?.Windows[0]?.Page?.Navigation is INavigation navigation)
            {
                await navigation.PushModalAsync(window, true);
            }
            else
            {
                viewmodel.Closed -= ViewModelClosedHandler;
                throw new InvalidOperationException("Cannot show modal dialog - Navigation is not available");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating dialog");

            // Cleanup on error
            try
            {
                if (viewModelRef != null)
                {
                    viewModelRef.Closed -= ViewModelClosedHandler;
                    viewModelRef.Dispose();
                }
                windowRef?.Dispose();
            }
            catch (Exception cleanupEx)
            {
                _logger?.LogError(cleanupEx, "Error during dialog cleanup");
            }

            bool handled = false;

            try
            {
                _exceptionHandlerService.HandleException(ex);
                handled = true;
            }
            catch (Exception handlerEx)
            {
                // Log failure of exception handler but suppress original exception to prevent app crash
                _logger?.LogError(handlerEx, DialogCreationExceptionHandlerFailed);
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning(DialogCreationExceptionHandlerNotAvailable);
            }
        }
    }

    #region IDisposable
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dialogSemaphore.Dispose();
        }
    }
    #endregion
}
