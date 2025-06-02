using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.AspNetCore.Components;

namespace ISynergy.Framework.UI.Services;
public class NavigationService : INavigationService
{
    private readonly NavigationManager _navigationManager;

    public NavigationService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public bool CanGoBack => throw new NotImplementedException();

    public event EventHandler? BackStackChanged;

    public void CleanBackStack(bool suppressEvent = false)
    {
        throw new NotImplementedException();
    }

    public Task GoBackAsync()
    {
        throw new NotImplementedException();
    }

    public Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel)
    {
        throw new NotImplementedException();
    }

    public Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel) where TView : IView
    {
        throw new NotImplementedException();
    }

    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
    {
        throw new NotImplementedException();
    }

    Task INavigationService.NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter, bool backNavigation)
    {
        throw new NotImplementedException();
    }

    Task INavigationService.NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter, bool backNavigation)
    {
        throw new NotImplementedException();
    }

    Task INavigationService.NavigateAsync<TViewModel>(object? parameter, bool backNavigation)
    {
        throw new NotImplementedException();
    }

    Task INavigationService.NavigateAsync<TViewModel, TView>(object? parameter, bool backNavigation)
    {
        throw new NotImplementedException();
    }

    Task INavigationService.NavigateModalAsync<TViewModel>(object? parameter)
    {
        throw new NotImplementedException();
    }
}
