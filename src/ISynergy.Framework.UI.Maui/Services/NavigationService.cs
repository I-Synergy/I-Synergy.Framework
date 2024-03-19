using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : INavigationService
{
    private readonly IContext _context;

    public event EventHandler BackStackChanged;

    /// <summary>
    /// Handles the <see cref="E:BackStackChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public virtual void OnBackStackChanged(EventArgs e) => BackStackChanged?.Invoke(this, e);

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// </summary>
    /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
    public bool CanGoBack => Application.Current.MainPage.Navigation.NavigationStack.Count > 1 ? true : false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="context"></param>
    public NavigationService(IContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="absolute"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel>(object parameter = null, bool absolute = false)
        where TViewModel : class, IViewModel =>
        NavigateAsync<TViewModel>(null, parameter, absolute);

    /// <summary>
    /// Navigates to the viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="absolute"></param>
    /// <returns></returns>
    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object parameter = null, bool absolute = false)
        where TViewModel : class, IViewModel 
    {
        if (NavigationExtensions.CreatePage<TViewModel>(_context, viewModel, parameter) is { } page)
        {
            if (absolute)
                Application.Current.MainPage = new NavigationPage((Page)page);
            else
            {
                if (Application.Current.MainPage is FlyoutPage flyoutPage)
                {
                    if (flyoutPage.Detail is NavigationPage navigationPage)
                    {
                        if (navigationPage.Navigation.NavigationStack.Contains((Page)page))
                        {
                            for (int i = navigationPage.Navigation.NavigationStack.Count - 1; i >= 0; i--)
                            {
                                if (navigationPage.Navigation.NavigationStack[i].Equals(page))
                                    break;

                                await navigationPage.PopAsync();
                            }
                        }
                        else
                        {
                            if (navigationPage.CurrentPage is null)
                                ((Page)page).Parent = null;

                            await navigationPage.PushAsync((Page)page, true);
                        }
                    }
                    else
                        flyoutPage.Detail = (Page)page;
                }
                else if (Application.Current.MainPage is NavigationPage navigationPage)
                    await navigationPage.PushAsync((Page)page, true);
                else
                    await Application.Current.MainPage.Navigation.PushAsync((Page)page, true);
            }

            if (!page.ViewModel.IsInitialized)
                await page.ViewModel.InitializeAsync();

            OnBackStackChanged(EventArgs.Empty);
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
        if (NavigationExtensions.CreatePage<TViewModel>(_context, parameter) is { } page)
        {
            if (absolute)
                Application.Current.MainPage = (Page)page;
            else
                await Application.Current.MainPage.Navigation.PushModalAsync((Page)page, true);

            if (!page.ViewModel.IsInitialized)
                await page.ViewModel.InitializeAsync();
        }
    }

    public async Task CleanBackStackAsync()
    {
        await Application.Current.MainPage.Navigation.PopToRootAsync();
        OnBackStackChanged(EventArgs.Empty);
    }
        

    /// <summary>
    /// Goes the back.
    /// </summary>
    public async Task GoBackAsync()
    {
        if (CanGoBack && Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
            await Application.Current.MainPage.Navigation.PopAsync();

        OnBackStackChanged(EventArgs.Empty);
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
    public Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object parameter = null, bool absolute = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task NavigateAsync<TViewModel, TView>(object parameter = null, bool absolute = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task GoForwardAsync() => throw new NotImplementedException();
    #endregion
}
