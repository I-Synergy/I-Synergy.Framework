using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using MessageBoxButtons = ISynergy.Framework.Mvvm.Enumerations.MessageBoxButtons;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = ISynergy.Framework.Mvvm.Enumerations.MessageBoxResult;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class abstracting the interaction between view models and views when it comes to
/// opening dialogs using the MVVM pattern in UWP applications.
/// </summary>
public class DialogService : BaseDialogService, IDialogService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILanguageService _languageService;
    private readonly ILogger _logger;

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
        _logger = logger;
        _scopeFactory = scopeFactory;
        _languageService = languageService;
    }

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string? title = "") =>
        ShowMessageAsync(error.Message, !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButtons.OK);

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
    public Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default) // NOSONAR - default value required by interface
    {
        var button = System.Windows.MessageBoxButton.OK;

        switch (buttons)
        {
            case MessageBoxButtons.OKCancel:
                button = System.Windows.MessageBoxButton.OKCancel;
                break;
            case MessageBoxButtons.YesNoCancel:
                button = System.Windows.MessageBoxButton.YesNoCancel;
                break;
            case MessageBoxButtons.YesNo:
                button = System.Windows.MessageBoxButton.YesNo;
                break;
        }

        var dialog = MessageBox.Show(
            Application.Current.MainWindow,
            message,
            title,
            button,
            MessageBoxImage.Information,
            System.Windows.MessageBoxResult.Cancel);

        var result = dialog switch
        {
            System.Windows.MessageBoxResult.None => MessageBoxResult.None,
            System.Windows.MessageBoxResult.OK => MessageBoxResult.OK,
            System.Windows.MessageBoxResult.Cancel => MessageBoxResult.Cancel,
            System.Windows.MessageBoxResult.Yes => MessageBoxResult.Yes,
            System.Windows.MessageBoxResult.No => MessageBoxResult.No,
            _ => MessageBoxResult.None,
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Shows the dialog asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="window">The window, already resolved from the caller's DI scope together
    /// with the <paramref name="viewmodel"/>. Both share the same scope so they are disposed
    /// together when the dialog closes — no additional scope is created here.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public async Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
    {
        if (window is Window dialog)
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
        // Create the Window via ActivatorUtilities so it is NOT tracked by a DI
        // scope. The ViewModel was resolved from the caller's scope and must not
        // be paired with a Window from a second scope. The Window lifecycle is
        // managed explicitly by CreateDialogAsync.
        using var tempScope = _scopeFactory.CreateScope();
        var dialog = ActivatorUtilities.CreateInstance(tempScope.ServiceProvider, type) as Window;

        if (dialog is not null)
            await CreateDialogAsync(dialog, viewmodel);
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
        var scope = _scopeFactory.CreateScope();
        try
        {
            if (scope.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
                scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
            {
                initialization?.Invoke((TViewModel)viewmodel);
                viewmodel.SetSelectedItem(e);

                var windowTypeName = typeof(TWindow).Name;
                var viewModelTypeName = typeof(TViewModel).Name;

                EventHandler<SubmitEventArgs<TEntity>>? onSubmittedHandler = null;
                onSubmittedHandler = (_, args) =>
                    _ = HandleDialogSubmittedAsync(args.Result, submittedEventHandler, windowTypeName, viewModelTypeName)
                        .SafeFireAndForget();
                viewmodel.Submitted += onSubmittedHandler;

                EventHandler? onCancelledHandler = null;
                if (cancelledEventHandler is not null)
                {
                    onCancelledHandler = (_, _) =>
                        _ = HandleDialogCancelledAsync(cancelledEventHandler, windowTypeName, viewModelTypeName)
                            .SafeFireAndForget();
                    viewmodel.Cancelled += onCancelledHandler;
                }

                viewmodel.Closed += (_, _) =>
                {
                    viewmodel.Submitted -= onSubmittedHandler;
                    if (onCancelledHandler is not null) viewmodel.Cancelled -= onCancelledHandler;

                    if (closedEventHandler is not null)
                        _ = HandleDialogClosedAsync(closedEventHandler, windowTypeName, viewModelTypeName)
                            .SafeFireAndForget();

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

    /// <inheritdoc />
    public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
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

                var windowTypeName = typeof(TWindow).Name;
                var viewModelTypeName = typeof(TViewModel).Name;

                EventHandler<SubmitEventArgs<TEntity>>? onSubmittedHandler = null;
                onSubmittedHandler = (_, args) =>
                    _ = HandleDialogSubmittedAsync(args.Result, submittedEventHandler, windowTypeName, viewModelTypeName)
                        .SafeFireAndForget();
                viewmodel.Submitted += onSubmittedHandler;

                EventHandler? onCancelledHandler = null;
                if (cancelledEventHandler is not null)
                {
                    onCancelledHandler = (_, _) =>
                        _ = HandleDialogCancelledAsync(cancelledEventHandler, windowTypeName, viewModelTypeName)
                            .SafeFireAndForget();
                    viewmodel.Cancelled += onCancelledHandler;
                }

                viewmodel.Closed += (_, _) =>
                {
                    viewmodel.Submitted -= onSubmittedHandler;
                    if (onCancelledHandler is not null) viewmodel.Cancelled -= onCancelledHandler;

                    if (closedEventHandler is not null)
                        _ = HandleDialogClosedAsync(closedEventHandler, windowTypeName, viewModelTypeName)
                            .SafeFireAndForget();

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

    public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
    {
        if (dialog is Window window)
        {
            window.ViewModel = viewmodel;
            window.Owner = Application.Current.MainWindow;

            void ViewModelClosedHandler(object? sender, EventArgs e)
            {
                if (sender is IViewModelDialog<TEntity> vm)
                    vm.Closed -= ViewModelClosedHandler;

                window.Close();
            }

            viewmodel.Closed += ViewModelClosedHandler;

            try
            {
                await viewmodel.InitializeAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing viewmodel");
                viewmodel.Closed -= ViewModelClosedHandler;
                throw;
            }

            await window.ShowAsync<TEntity>();
        }
    }

    /// <summary>
    /// Handles the dialog Submitted event. Invokes the caller's submit callback
    /// and logs any exception.
    /// </summary>
    private async Task HandleDialogSubmittedAsync<TEntity>(
        TEntity result,
        Func<TEntity, Task> submittedEventHandler,
        string windowTypeName,
        string viewModelTypeName)
    {
        try
        {
            await submittedEventHandler(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Submit callback failed for {WindowType}/{ViewModelType}", windowTypeName, viewModelTypeName);
        }
    }

    /// <summary>
    /// Handles the dialog Cancelled event. Invokes the caller's cancelled callback
    /// and logs any exception.
    /// </summary>
    private async Task HandleDialogCancelledAsync(
        Func<Task> cancelledEventHandler,
        string windowTypeName,
        string viewModelTypeName)
    {
        try
        {
            await cancelledEventHandler();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cancel callback failed for {WindowType}/{ViewModelType}", windowTypeName, viewModelTypeName);
        }
    }

    /// <summary>
    /// Handles the dialog Closed event. Invokes the caller's closed callback
    /// and logs any exception. The scope has already been disposed by the
    /// synchronous <see cref="ShowDialogAsync{TWindow,TViewModel,TEntity}(TEntity, Func{TEntity,Task}, Func{Task}?, Func{Task}?, Action{TViewModel}?)"/> handler.
    /// </summary>
    private async Task HandleDialogClosedAsync(
        Func<Task> closedEventHandler,
        string windowTypeName,
        string viewModelTypeName)
    {
        try
        {
            await closedEventHandler();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Close callback failed for {WindowType}/{ViewModelType}", windowTypeName, viewModelTypeName);
        }
    }
}
