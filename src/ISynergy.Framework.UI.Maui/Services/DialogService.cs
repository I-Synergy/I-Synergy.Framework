using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly IScopedContextService _scopedContextService;
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
    /// <param name="scopedContextService"></param>
    /// <param name="exceptionHandlerService"></param>
    /// <param name="languageService">The language service.</param>
    /// <param name="logger">Optional logger for enhanced error handling.</param>
    public DialogService(
        IScopedContextService scopedContextService,
        IExceptionHandlerService exceptionHandlerService,
        ILanguageService languageService,
        ILogger<DialogService>? logger = null)
    {
        _scopedContextService = scopedContextService ?? throw new ArgumentNullException(nameof(scopedContextService));
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
    public async Task<MessageBoxResult> ShowMessageAsync(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default)
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
            // Pre-flight checks
            if (!await ValidateApplicationStateAsync())
            {
                await TryFallbackNotificationAsync(message, title);
                return MessageBoxResult.None;
            }

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
            _logger?.LogWarning(ex, "Dialog failed, attempting retry {RetryCount}/{MaxRetries}", retryCount + 1, maxRetries);

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

    private async Task<bool> ValidateApplicationStateAsync()
    {
        try
        {
            // Additional safety check right before showing dialog
            if (Application.Current == null ||
                Application.Current?.Windows.Count == 0 ||
                Application.Current?.Windows[0] == null ||
                Application.Current?.Windows[0].Page == null)
                throw new InvalidOperationException("Main page not available for dialog display");

            // Check MainPage with timeout
            var timeoutTask = Task.Delay(5000);
            var checkTask = Task.Run(() => Application.Current?.Windows[0]?.Page != null);

            var completedTask = await Task.WhenAny(checkTask, timeoutTask);
            if (completedTask == timeoutTask)
            {
                _logger?.LogError("MainPage validation timed out");
                return false;
            }

            var hasMainPage = await checkTask;
            if (!hasMainPage)
            {
                _logger?.LogError("Application.Current.MainPage is null");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error validating application state");
            return false;
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
            _logger?.LogError(ex, "DialogService disabled after {FailureCount} consecutive failures. Message: {Title} - {Message}",
                _consecutiveFailures, title, message);
        }
        else
        {
            _logger?.LogWarning(ex, "Dialog service failure {FailureCount}/{MaxFailures}. Message: {Title} - {Message}",
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
                Task.Run(() => Console.WriteLine($"CRITICAL ERROR: {title} - {message}")),
                Task.Run(() => System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR: {title} - {message}")),
                Task.Run(() => System.Diagnostics.Trace.WriteLine($"CRITICAL ERROR: {title} - {message}"))
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Emergency fallback failed for critical error: {Title} - {Message}", title, message);
        }
    }

    private async Task<bool> TryConsoleNotificationAsync(string message, string title)
    {
        await Task.Run(() => Console.WriteLine($"DIALOG: {title} - {message}"));
        return true;
    }

    private async Task<bool> TryDebugNotificationAsync(string message, string title)
    {
        await Task.Run(() => System.Diagnostics.Debug.WriteLine($"DIALOG: {title} - {message}"));
        return true;
    }

    private Task<bool> TryEventLogNotificationAsync(string message, string title)
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

    private async Task<bool> TryFileNotificationAsync(string message, string title)
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
                _scopedContextService.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error showing dialog for {WindowType} with {ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name);

            bool handled = false;

            try
            {
                _exceptionHandlerService.HandleException(ex);
                handled = true;
            }
            catch (Exception handlerEx)
            {
                // Log failure of exception handler but suppress original exception to prevent app crash
                _logger?.LogError(handlerEx, "Exception handler failed while processing dialog error");
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning("Exception handler service not available for dialog error");
            }
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
            _logger?.LogError(ex, "Error showing dialog for {WindowType} with {ViewModelType} and entity {EntityType}",
                typeof(TWindow).Name, typeof(TViewModel).Name, typeof(TEntity).Name);

            bool handled = false;

            try
            {
                _exceptionHandlerService.HandleException(ex);
                handled = true;
            }
            catch (Exception handlerEx)
            {
                // Log failure of exception handler but suppress original exception to prevent app crash
                _logger?.LogError(handlerEx, "Exception handler failed while processing dialog error");
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning("Exception handler service not available for dialog error");
            }
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
        try
        {
            if (_scopedContextService.GetRequiredService(window.GetType()) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
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
                _logger?.LogError(handlerEx, "Exception handler failed while processing dialog error");
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning("Exception handler service not available for dialog error");
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
        try
        {
            if (_scopedContextService.GetRequiredService(type) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
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
                _logger?.LogError(handlerEx, "Exception handler failed while processing dialog error");
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning("Exception handler service not available for dialog error");
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
    public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
    {
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
                    _logger?.LogError(handlerEx, "Exception handler failed while processing viewmodel initialization error");
                }

                // Suppress exception if it was successfully handled or if handler is not available
                // This prevents app crashes while still allowing the handler to show user-friendly messages
                if (!handled)
                {
                    _logger?.LogWarning("Exception handler service not available for viewmodel initialization error");
                }
            }
            return;
        }

        Window? windowRef = null;
        IViewModelDialog<TEntity>? viewModelRef = null;

        // Define the handler in the method scope so it is accessible for both += and -=
        async void ViewModelClosedHandler(object? sender, EventArgs e)
        {
            try
            {
                if (viewModelRef != null)
                    viewModelRef.Closed -= ViewModelClosedHandler;

                if (Application.Current?.Windows[0]?.Page?.Navigation is INavigation navigation)
                {
                    await navigation.PopModalAsync();
                }
                else
                {
                    _logger?.LogWarning("Cannot close modal dialog - Navigation is not available");
                }

                viewModelRef?.Dispose();
                viewModelRef = null;

                windowRef?.Dispose();
                windowRef = null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in ViewModelClosedHandler");
            }
        }

        try
        {
            windowRef = window;
            viewModelRef = viewmodel;

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
                    _logger?.LogError(handlerEx, "Exception handler failed while processing viewmodel initialization error");
                }

                // Suppress exception if it was successfully handled or if handler is not available
                // This prevents app crashes while still allowing the handler to show user-friendly messages
                if (!handled)
                {
                    _logger?.LogWarning("Exception handler service not available for viewmodel initialization error");
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
                _logger?.LogError(handlerEx, "Exception handler failed while processing dialog creation error");
            }

            // Suppress exception if it was successfully handled or if handler is not available
            // This prevents app crashes while still allowing the handler to show user-friendly messages
            if (!handled)
            {
                _logger?.LogWarning("Exception handler service not available for dialog creation error");
            }
        }
    }
}
