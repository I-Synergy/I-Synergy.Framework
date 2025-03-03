using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using IThemeService = ISynergy.Framework.Mvvm.Abstractions.Services.IThemeService;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly ILanguageService _languageService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IThemeService _themeService;
    private readonly IContext _context;
    private Window _activeDialog = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="languageService">The language service.</param>
    /// <param name="themeService"></param>
    public DialogService(
        IContext context,
        IServiceProvider serviceProvider,
        ILanguageService languageService,
        IThemeService themeService)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _languageService = languageService;
        _themeService = themeService;
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

        switch (_themeService.Style.Theme)
        {
            case Core.Enumerations.Themes.Light:
                dialog.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
                break;
            case Core.Enumerations.Themes.Dark:
                dialog.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Dark;
                break;
            default:
                dialog.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Default;
                break;
        }

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
        var scope = _serviceProvider.CreateScope();
        var viewmodel = (IViewModelDialog<TEntity>)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel));

        if (scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
        {
            dialog.Unloaded += (sender, e) => scope.Dispose();
            await CreateDialogAsync(dialog, viewmodel);
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
        var scope = _serviceProvider.CreateScope();
        var viewmodel = (IViewModelDialog<TEntity>)_context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel));

        await viewmodel.SetSelectedItemAsync(e);

        if (scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
        {
            dialog.Unloaded += (sender, e) => scope.Dispose();
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
        var scope = _serviceProvider.CreateScope();

        if (scope.ServiceProvider.GetRequiredService(window.GetType()) is Window dialog)
        {
            dialog.Unloaded += (sender, e) => scope.Dispose();
            await CreateDialogAsync(dialog, viewmodel);
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
        var scope = _serviceProvider.CreateScope();

        if (scope.ServiceProvider.GetRequiredService(type) is Window dialog)
        {
            dialog.Unloaded += (sender, e) => scope.Dispose();
            await CreateDialogAsync(dialog, viewmodel);
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
        if (dialog is Window window)
        {
            if (Application.Current is BaseApplication baseApplication)
                window.XamlRoot = baseApplication.MainWindow.Content.XamlRoot;

            switch (_themeService.Style.Theme)
            {
                case Core.Enumerations.Themes.Light:
                    window.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
                    break;
                case Core.Enumerations.Themes.Dark:
                    window.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Dark;
                    break;
                default:
                    window.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Default;
                    break;
            }

            window.ViewModel = viewmodel;

            window.PrimaryButtonCommand = viewmodel.SubmitCommand;
            window.SecondaryButtonCommand = viewmodel.CancelCommand;
            window.CloseButtonCommand = viewmodel.CancelCommand;

            window.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
            window.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];
            window.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Application.Current.Resources["DefaultDialogButtonStyle"];

            void ViewModelClosedHandler(object sender, EventArgs e)
            {
                viewmodel.Closed -= ViewModelClosedHandler;

                window.ViewModel?.Dispose();
                window.ViewModel = null;

                window.Close();
            };

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
            _activeDialog.Dispose();
        }

        _activeDialog = dialog;

        return await _activeDialog.ShowAsync().AsTask();
    }
}
