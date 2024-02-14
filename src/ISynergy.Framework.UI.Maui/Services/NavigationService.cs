using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// </summary>
    /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
    public bool CanGoBack => Application.Current.MainPage.Navigation.NavigationStack.Count > 1 ? true : false;

    /// <summary>
    /// Gets a value indicating whether this instance can go forward.
    /// </summary>
    /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
    public bool CanGoForward => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="absolute"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel>(object parameter = null, bool absolute = false) where TViewModel : class, IViewModel
    {
        var url = typeof(TViewModel).Name;

        if (parameter is not null && parameter is Dictionary<string, string> parameters)
            url = QueryHelpers.AddQueryString(url, parameters);

        if (absolute)
            url = $"//{url}";

        return Shell.Current.GoToAsync(url);
    }

    /// <summary>
    /// Navigates to the viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="absolute"></param>
    /// <returns></returns>
    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object parameter = null, bool absolute = false) where TViewModel : class, IViewModel 
    {
        if (NavigationExtensions.CreatePage<TViewModel>(_serviceProvider, parameter) is IView page)
        {
            if (absolute)
                Application.Current.MainPage = new NavigationPage((Page)page);
            else
                await Application.Current.MainPage.Navigation.PushAsync((Page)page, true);

            if (!page.ViewModel.IsInitialized)
                await page.ViewModel.InitializeAsync();
        }
    }

    /// <summary>
    /// Navigates to the modal viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="absolute"></param>
    /// <returns></returns>
    public async Task NavigateModalAsync<TViewModel>(object parameter = null, bool absolute = false)
         where TViewModel : class, IViewModel
    {
        if (NavigationExtensions.CreatePage<TViewModel>(_serviceProvider, parameter) is IView page)
        {
            if (absolute)
                Application.Current.MainPage = (Page)page;
            else
                await Application.Current.MainPage.Navigation.PushModalAsync((Page)page, true);

            if (!page.ViewModel.IsInitialized)
                await page.ViewModel.InitializeAsync();
        }
    }

    public Task CleanBackStackAsync() =>
        Shell.Current.Navigation.PopToRootAsync();

    /// <summary>
    /// Goes the back.
    /// </summary>
    public async Task GoBackAsync()
    {
        if (CanGoBack)
            await Shell.Current.GoToAsync("..");
    }

    #region NotImplemented
    [Obsolete("Not supported!", true)]
    public Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel)
        where TView : IView =>
        throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object parameter = null, bool navigateBack = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task NavigateAsync<TViewModel, TView>(object parameter = null, bool navigateBack = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task GoForwardAsync() => throw new NotImplementedException();
    #endregion
}
