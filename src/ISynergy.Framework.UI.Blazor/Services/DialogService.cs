using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Components.Layout;
using ISynergy.Framework.UI.Messages;
using ISynergy.Framework.UI.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using IDialogService = ISynergy.Framework.Mvvm.Abstractions.Services.IDialogService;
using MessageService = ISynergy.Framework.Core.Services.MessageService;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger<DialogService> _logger;

    public DialogService(
        IScopedContextService scopedContextService,
        ILogger<DialogService> logger)
    {
        _scopedContextService = scopedContextService;
        _logger = logger;
    }

    public async Task<MessageBoxResult> ShowInformationAsync(string message, string title = "")
    {
        var request = new MessageBoxRequest(message, title);
        MessageService.Default.Send(new ShowInformationMessage(request));

        //var dialogService = _scopedContextService.GetRequiredService<Microsoft.FluentUI.AspNetCore.Components.IDialogService>();
        //var dialogResult = await dialogService.ShowInfoAsync(message, title, LanguageService.Default.GetString("OK"));

        //if (dialogResult is not null)
        //{
        //    var result = await dialogResult.Result;

        //    if (result.Cancelled)
        //        return MessageBoxResult.Cancel;

        //    return MessageBoxResult.OK;
        //}

        return MessageBoxResult.None;
    }

    public async Task<MessageBoxResult> ShowWarningAsync(string message, string title = "")
    {
        var request = new MessageBoxRequest(message, title);
        MessageService.Default.Send(new ShowWarningMessage(request));

        //var dialogService = _scopedContextService.GetRequiredService<Microsoft.FluentUI.AspNetCore.Components.IDialogService>();
        //var dialogResult = await dialogService.ShowWarningAsync(message, title, LanguageService.Default.GetString("OK"));

        //if (dialogResult is not null)
        //{
        //    var result = await dialogResult.Result;

        //    if (result.Cancelled)
        //        return MessageBoxResult.Cancel;

        //    return MessageBoxResult.OK;
        //}

        return MessageBoxResult.None;
    }

    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "")
    {
        return ShowErrorAsync(error.Message);
    }

    public async Task<MessageBoxResult> ShowErrorAsync(string message, string title = "")
    {
        var request = new MessageBoxRequest(message, title);
        MessageService.Default.Send(new ShowErrorMessage(request));

        //var dialogService = _scopedContextService.GetRequiredService<Microsoft.FluentUI.AspNetCore.Components.IDialogService>();
        //var dialogResult = await dialogService.ShowErrorAsync(message, title, LanguageService.Default.GetString("OK"));

        //if (dialogResult is not null)
        //{
        //    var result = await dialogResult.Result;

        //    if (result.Cancelled)
        //        return MessageBoxResult.Cancel;

        //    return MessageBoxResult.OK;
        //}

        return MessageBoxResult.None;
    }

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

    public async Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default)
    {
        try
        {
            var parameters = new DialogParameters<MessageBoxContent>
            {
                Content = new MessageBoxContent
                {
                    Title = title,
                    MarkupMessage = new MarkupString(message)
                },
                ShowTitle = !string.IsNullOrEmpty(title),
                ShowDismiss = false
            };

            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    parameters.PrimaryAction = LanguageService.Default.GetString("OK");
                    parameters.SecondaryAction = LanguageService.Default.GetString("Cancel");
                    break;
                case MessageBoxButtons.YesNoCancel:
                    parameters.PrimaryAction = LanguageService.Default.GetString("Yes");
                    parameters.SecondaryAction = LanguageService.Default.GetString("No");
                    parameters.ShowDismiss = true;
                    break;
                case MessageBoxButtons.YesNo:
                    parameters.PrimaryAction = LanguageService.Default.GetString("Yes");
                    parameters.SecondaryAction = LanguageService.Default.GetString("No");
                    break;
                case MessageBoxButtons.OK:
                default:
                    parameters.PrimaryAction = LanguageService.Default.GetString("OK");
                    parameters.SecondaryAction = null;
                    break;
            }

            var dialogService = _scopedContextService.GetRequiredService<Microsoft.FluentUI.AspNetCore.Components.IDialogService>();
            var dialogReference = await dialogService.ShowMessageBoxAsync(parameters);

            if (dialogReference is not null)
            {
                var result = await dialogReference.Result;

                if (result is not null)
                {
                    switch (buttons)
                    {
                        case MessageBoxButtons.OKCancel:
                            if (result.Cancelled)
                                return MessageBoxResult.Cancel;

                            return MessageBoxResult.OK;
                        case MessageBoxButtons.YesNoCancel:
                        case MessageBoxButtons.YesNo:
                            if (result.Cancelled)
                                return MessageBoxResult.No;

                            return MessageBoxResult.Yes;
                        default:
                            return MessageBoxResult.OK;
                    }
                }
            }

            return MessageBoxResult.None;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing message dialog");
            return MessageBoxResult.None;
        }
    }

    private async Task<DialogResult?> OpenDialogAsync(DialogParameters<MessageBoxContent> parameters)
    {
        var dialogService = _scopedContextService.GetRequiredService<Microsoft.FluentUI.AspNetCore.Components.IDialogService>();
        var dialog = await dialogService.ShowMessageBoxAsync(parameters);
        return await dialog.Result;
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

        //if (dialog is Window window)
        //{
        //    try
        //    {
        //        window.ViewModel = viewmodel;

        //        window.PrimaryButtonCommand = viewmodel.SubmitCommand;
        //        window.SecondaryButtonCommand = viewmodel.CancelCommand;
        //        window.CloseButtonCommand = viewmodel.CancelCommand;

        //        try
        //        {
        //            window.PrimaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
        //            window.SecondaryButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
        //            window.CloseButtonStyle = (Microsoft.UI.Xaml.Style)Microsoft.UI.Xaml.Application.Current.Resources["DefaultDialogButtonStyle"];
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogWarning(ex, "Failed to set button styles");
        //        }

        //        EventHandler? viewModelClosedHandler = null;
        //        viewModelClosedHandler = async (sender, e) =>
        //        {
        //            try
        //            {
        //                if (sender is IViewModelDialog<TEntity> vm)
        //                    vm.Closed -= viewModelClosedHandler;

        //                window.ViewModel?.Dispose();
        //                await window.CloseAsync();
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error in viewmodel closed handler");
        //            }
        //        };

        //        viewmodel.Closed += viewModelClosedHandler;

        //        try
        //        {
        //            await viewmodel.InitializeAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error initializing viewmodel");
        //            viewmodel.Closed -= viewModelClosedHandler;
        //            return;
        //        }

        //        await OpenDialogAsync(window);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating dialog");
        //    }
        //}
    }

    //public async Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
    //{
    //    try
    //    {
    //        // Initialize the view model if needed
    //        if (!viewmodel.IsInitialized)
    //        {
    //            await viewmodel.InitializeAsync();
    //        }

    //        // Create dialog parameters
    //        var parameters = new DialogParameters
    //        {
    //            ShowTitle = true,
    //            Title = viewmodel.Title,
    //            PrimaryAction = "OK",
    //            SecondaryAction = "Cancel",
    //            ShowDismiss = true,
    //            // Pass the view model as a parameter
    //            ["ViewModel"] = viewmodel
    //        };

    //        // Find the actual component type if needed
    //        Type componentType = type;
    //        if (!typeof(Microsoft.AspNetCore.Components.ComponentBase).IsAssignableFrom(componentType))
    //        {
    //            // If type isn't a component, try to find a suitable dialog component
    //            componentType = FindDialogComponentType(type);
    //        }

    //        // Show the dialog
    //        var dialogReference = await _dialogService.ShowDialogAsync(componentType, parameters);

    //        // Wait for the dialog to close
    //        var result = await dialogReference.Result;

    //        // Cleanup view model
    //        viewmodel.Cleanup(true);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error showing dialog for type {WindowType} and view model {ViewModelType}",
    //            type.Name, viewmodel.GetType().Name);
    //        throw;
    //    }
    //}
}