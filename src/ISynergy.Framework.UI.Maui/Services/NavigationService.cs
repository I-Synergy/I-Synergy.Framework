using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Extensions;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : INavigationService
{
    private readonly bool _animated = true;

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
    public bool CanGoBack =>
        Application.Current.MainPage.GetNavigation().Navigation.NavigationStack.Count > 0 ||
        Application.Current.MainPage.Navigation.ModalStack.Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    public NavigationService()
    {
    }

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel>(object parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel =>
        NavigateAsync<TViewModel>(null, parameter, backNavigation);

    /// <summary>
    /// Navigates to the viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        if (NavigationExtensions.CreatePage<TViewModel>(viewModel, parameter) is { } view && view is Page page)
        {
            var result = Application.Current.MainPage.GetNavigation();

            if (result.Navigation.NavigationStack.Contains(page))
            {
                for (int i = result.Navigation.NavigationStack.Count - 1; i >= 0; i--)
                {
                    if (result.Navigation.NavigationStack[i].Equals(page))
                        break;

                    if (!backNavigation)
                        await result.Navigation.PopAsync(_animated);
                }
            }
            else
            {
                page.Parent = null;
                await result.Navigation.PushAsync(page, _animated);
            }

            await view.ViewModel.InitializeAsync();

            OnBackStackChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Navigates to the modal viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public async Task NavigateModalAsync<TViewModel>(object parameter = null)
         where TViewModel : class, IViewModel
    {
        if (!Application.Current.Dispatcher.IsDispatchRequired &&
            NavigationExtensions.CreatePage<TViewModel>(parameter) is { } view && view is Page page)
        {
            // Added this nullification of handler becuase of some issues with TabbedPage.
            // When tabbed page is set as main page and then modal page is opened, then after closing and reopening the page, the tabs are not visible.
            page.Handler = null;
            Application.Current.MainPage = page;
            await view.ViewModel.InitializeAsync();
        }
        else
            await Application.Current.Dispatcher.DispatchAsync(async () => await NavigateModalAsync<TViewModel>(parameter));
    }

    public async Task CleanBackStackAsync(bool suppressEvent = false)
    {
        await Application.Current.MainPage.GetNavigation().Navigation.PopToRootAsync(_animated);
        OnBackStackChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Goes the back.
    /// </summary>
    public async Task GoBackAsync()
    {
        if (CanGoBack)
        {
            if (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
                await Application.Current.MainPage.Navigation.PopModalAsync(_animated);
            else
                await Application.Current.MainPage.GetNavigation().Navigation.PopAsync(_animated);
        }

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
    public Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task NavigateAsync<TViewModel, TView>(object parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task GoForwardAsync() => throw new NotImplementedException();
    #endregion
}
