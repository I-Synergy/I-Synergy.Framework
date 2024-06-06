using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Application = Microsoft.Maui.Controls.Application;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly ILanguageService _languageService;
    private readonly IContext _context;

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
        ShowMessageAsync(error.ToMessage(Environment.StackTrace), !string.IsNullOrEmpty(title) ? title : _languageService.GetString("TitleError"), MessageBoxButton.OK);

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

        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
        {
            return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Morning"), name),
                _languageService.GetString("TitleWelcome"), MessageBoxButton.OK);
        }
        if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
        {
            return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Afternoon"), name),
                _languageService.GetString("TitleWelcome"), MessageBoxButton.OK);
        }
        return ShowMessageAsync(string.Format(_languageService.GetString("Greeting_Evening"), name),
            _languageService.GetString("TitleWelcome"), MessageBoxButton.OK);
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
        switch (buttons)
        {
            case MessageBoxButton.OKCancel:
                if (await Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    _languageService.GetString("Ok"),
                    _languageService.GetString("Cancel")))
                    return MessageBoxResult.OK;
                return MessageBoxResult.Cancel;
            case MessageBoxButton.YesNo:
                if (await Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    _languageService.GetString("Yes"),
                    _languageService.GetString("No")))
                    return MessageBoxResult.Yes;
                return MessageBoxResult.No;
            default:
                await Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    _languageService.GetString("Ok"));
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
        if (_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
            _context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
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
        if (_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
            _context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
        {
            await viewmodel.SetSelectedItemAsync(e);
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
        if (_context.ScopedServices.ServiceProvider.GetRequiredService(window.GetType()) is Window dialog)
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
        if (_context.ScopedServices.ServiceProvider.GetRequiredService(type) is Window dialog)
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

                await Application.Current.MainPage.Navigation.PopModalAsync();

                viewmodel.Dispose();
                viewmodel = null;

                window?.Dispose();
                window = null;
            };

            viewmodel.Closed += ViewModelClosedHandler;

            await viewmodel.InitializeAsync();

            await Application.Current.MainPage.Navigation.PushModalAsync(window, true);
        }
    }
}
