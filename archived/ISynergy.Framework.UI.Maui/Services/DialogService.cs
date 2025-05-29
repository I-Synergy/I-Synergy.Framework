using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger<DialogService> _logger;

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
    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "") =>
        ShowMessageAsync(error.ToMessage(Environment.StackTrace), !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleError"), MessageBoxButtons.OK);

    /// <summary>
    /// Shows the error asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleError"), MessageBoxButtons.OK);

    /// <summary>
    /// Shows the information asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleInfo"), MessageBoxButtons.OK);

    /// <summary>
    /// Shows the warning asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <returns>Task&lt;MessageBoxResult&gt;.</returns>
    public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "") =>
        ShowMessageAsync(message, !string.IsNullOrEmpty(title) ? title : LanguageService.Default.GetString("TitleWarning"), MessageBoxButtons.OK);

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
                LanguageService.Default.GetString("TitleWelcome"), MessageBoxButtons.OK);
        }

        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
        {
            return ShowMessageAsync(string.Format(LanguageService.Default.GetString("Greeting_Morning"), name),
                LanguageService.Default.GetString("TitleWelcome"), MessageBoxButtons.OK);
        }
        if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
        {
            return ShowMessageAsync(string.Format(LanguageService.Default.GetString("Greeting_Afternoon"), name),
                LanguageService.Default.GetString("TitleWelcome"), MessageBoxButtons.OK);
        }
        return ShowMessageAsync(string.Format(LanguageService.Default.GetString("Greeting_Evening"), name),
            LanguageService.Default.GetString("TitleWelcome"), MessageBoxButtons.OK);
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
        switch (buttons)
        {
            case MessageBoxButtons.OKCancel:
                if (await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    LanguageService.Default.GetString("Ok"),
                    LanguageService.Default.GetString("Cancel")))
                    return MessageBoxResult.OK;
                return MessageBoxResult.Cancel;
            case MessageBoxButtons.YesNo:
                if (await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    LanguageService.Default.GetString("Yes"),
                    LanguageService.Default.GetString("No")))
                    return MessageBoxResult.Yes;
                return MessageBoxResult.No;
            default:
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    LanguageService.Default.GetString("Ok"));
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
        if (_scopedContextService.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
            _scopedContextService.GetRequiredService(typeof(TWindow)) is Window dialog)
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
        if (_scopedContextService.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
            _scopedContextService.GetRequiredService(typeof(TWindow)) is Window dialog)
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
        if (_scopedContextService.GetRequiredService(window.GetType()) is Window dialog)
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
        if (_scopedContextService.GetRequiredService(type) is Window dialog)
            await CreateDialogAsync(dialog, viewmodel);
    }

    /// <summary>
    /// Shows dialog as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="dialog"></param>
    /// <param name="viewmodel"></param>
    /// <returns></returns>
    public async Task CreateDialogAsync<TEntity>(IWindow dialog, IViewModelDialog<TEntity> viewmodel)
    {
        if (dialog is Window window)
        {
            window.ViewModel = viewmodel;

            async void ViewModelClosedHandler(object sender, EventArgs e)
            {
                viewmodel.Closed -= ViewModelClosedHandler;

                await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PopModalAsync();

                viewmodel.Dispose();
                viewmodel = null;

                window?.Dispose();
                window = null;
            }

            viewmodel.Closed += ViewModelClosedHandler;

            await viewmodel.InitializeAsync();

            await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushModalAsync(window, true);
        }
    }
}
