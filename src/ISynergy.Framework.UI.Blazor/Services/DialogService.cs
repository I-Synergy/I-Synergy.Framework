using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Services;

public class DialogService : IDialogService
{
    private readonly Microsoft.FluentUI.AspNetCore.Components.DialogService _dialogService;

    public DialogService(Microsoft.FluentUI.AspNetCore.Components.DialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public Task ShowDialogAsync<TWindow, TViewModel, TEntity>()
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
    {
        throw new NotImplementedException();
    }

    public Task ShowDialogAsync<TWindow, TViewModel, TEntity>(TEntity e)
        where TWindow : IWindow
        where TViewModel : IViewModelDialog<TEntity>
    {
        throw new NotImplementedException();
    }

    public Task ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
    {
        throw new NotImplementedException();
    }

    public Task ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
    {
        throw new NotImplementedException();
    }

    public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "")
    {
        throw new NotImplementedException();
    }

    public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "")
    {
        throw new NotImplementedException();
    }

    public Task ShowGreetingAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "")
    {
        throw new NotImplementedException();
    }

    public Task<MessageBoxResult> ShowMessageAsync(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, NotificationTypes notificationTypes = NotificationTypes.Default)
    {
        throw new NotImplementedException();
    }

    public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "")
    {
        throw new NotImplementedException();
    }
}
