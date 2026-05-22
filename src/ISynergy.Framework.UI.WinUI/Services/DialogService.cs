using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;

namespace ISynergy.Framework.UI.Services;

public class DialogService : BaseDialogService, IDialogService, IDisposable
{
    private const string DefaultDialogButtonStyleKey = "DefaultDialogButtonStyle";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILanguageService _languageService;
    private readonly ILogger<DialogService> _logger;

    private Window? _activeDialog = null;
    private readonly SemaphoreSlim _dialogSemaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="scopeFactory"></param>
    /// <param name="languageService"></param>
    /// <param name="logger"></param>
    public DialogService(
        IServiceScopeFactory scopeFactory,
        ILanguageService languageService,
        ILogger<DialogService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogTrace("DialogService instance created with ID: {InstanceId}", Guid.NewGuid());

        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
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
    /// <typeparam name="TWindow"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "GetRequiredService(Type) is called with typeof() arguments known at compile time via generic type parameters. Types are preserved through DI registration.")]
    public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>()
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
    {
        var scope = _scopeFactory.CreateScope();
        try
        {
            if (scope.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
                scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
            {
                viewmodel.Closed += (_, _) => scope.Dispose();
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
            _logger.LogError(ex, "Error showing dialog for {WindowType} and {ViewModelType}", typeof(TWindow).Name, typeof(TViewModel).Name);
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
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "GetRequiredService(Type) is called with typeof() arguments known at compile time via generic type parameters. Types are preserved through DI registration.")]
    public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>(TEntity e)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
    {
        var scope = _scopeFactory.CreateScope();
        try
        {
            if (scope.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
                scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
            {
                viewmodel.SetSelectedItem(e);
                viewmodel.Closed += (_, _) => scope.Dispose();
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
            _logger.LogError(ex, "Error showing dialog for {WindowType} and {ViewModelType} with entity", typeof(TWindow).Name, typeof(TViewModel).Name);
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
    /// This overload resolves the dialog window using <c>window.GetType()</c> which is not AOT-safe.
    /// Prefer the strongly-typed overload <c>ShowDialogAsync&lt;TWindow, TViewModel, TEntity&gt;()</c>
    /// when the concrete window type is known at compile time.
    /// </remarks>
    [RequiresUnreferencedCode(
        "GetType() on the window instance is not trim-safe. Use ShowDialogAsync<TWindow,TViewModel,TEntity>() for AOT-safe dialog display.")]
    public async Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
    {
        Argument.IsNotNull(window);
        Argument.IsNotNull(viewmodel);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            if (scope.ServiceProvider.GetRequiredService(window.GetType()) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
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
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "GetRequiredService(Type) is called with a runtime Type argument. Callers must ensure the window type is preserved. This overload is preserved for backwards compatibility.")]
    public async Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
    {
        Argument.IsNotNull(type);
        Argument.IsNotNull(viewmodel);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            if (scope.ServiceProvider.GetRequiredService(type) is Window dialog)
                await CreateDialogAsync(dialog, viewmodel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing dialog for {WindowType}", type.Name);
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

                        window.ViewModel?.Dispose();
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


