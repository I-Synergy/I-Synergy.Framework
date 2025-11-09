using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Extensions;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : INavigationService
{
    private readonly bool _animated = true;

    public event EventHandler? BackStackChanged;

    /// <summary>
    /// Handles the <see cref="E:BackStackChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public virtual void OnBackStackChanged(EventArgs e) => BackStackChanged?.Invoke(this, e);

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// </summary>
    /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
    private static Page? GetMainPage() =>
        Application.Current?.Windows.FirstOrDefault()?.Page;

    public bool CanGoBack =>
        GetMainPage() is Page page && (page.GetNavigation().Navigation.NavigationStack.Count > 0 || page.Navigation.ModalStack.Count > 0);


    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
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
    public async Task NavigateAsync<TViewModel>(TViewModel? viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        if (NavigationExtensions.CreatePage<TViewModel>(viewModel, parameter) is { } view && view is Page page)
        {
            var mainPage = GetMainPage();

            if (mainPage is null)
                throw new InvalidOperationException("Main page is not available.");

            var result = mainPage.GetNavigation();

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

            if (view is not null && view.ViewModel is not null)
            {
                try
                {
                    await view.ViewModel.InitializeAsync();
                }
                catch (Exception ex)
                {
                    // Log error but don't throw - navigation should continue even if initialization fails
                    System.Diagnostics.Debug.WriteLine($"Error initializing ViewModel: {ex.Message}");
                }
            }

            OnBackStackChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Navigates to the modal viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public async Task NavigateModalAsync<TViewModel>(object? parameter = null)
        where TViewModel : class, IViewModel
    {
        if (Application.Current is not null)
        {
            if (!Application.Current.Dispatcher.IsDispatchRequired)
            {
                if (NavigationExtensions.CreatePage<TViewModel>(parameter) is { } view && view is Page page)
                {
                    // Added this nullification of handler because of some issues with TabbedPage.
                    // When tabbed page is set as main page and then modal page is opened, then after closing and reopening the page, the tabs are not visible.
                    page.Handler = null;

                    var mainPage = GetMainPage();

                    if (mainPage is null)
                        throw new InvalidOperationException("Main page is not available.");

                    Application.Current.Windows[0].Page = page;

                    if (view is not null && view.ViewModel is not null)
                    {
                        try
                        {
                            await view.ViewModel.InitializeAsync();
                        }
                        catch (Exception ex)
                        {
                            // Log error but don't throw - modal navigation should continue even if initialization fails
                            System.Diagnostics.Debug.WriteLine($"Error initializing ViewModel: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                await Application.Current.Dispatcher.DispatchAsync(async () => await NavigateModalAsync<TViewModel>(parameter));
            }
        }
    }

    public async Task CleanBackStackAsync(bool suppressEvent = false)
    {
        var mainPage = GetMainPage();

        if (mainPage is null)
            throw new InvalidOperationException("Main page is not available.");

        await mainPage.GetNavigation().Navigation.PopToRootAsync(_animated);

        OnBackStackChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Goes the back.
    /// </summary>
    public async Task GoBackAsync()
    {
        var mainPage = GetMainPage();

        if (mainPage is null)
            throw new InvalidOperationException("Main page is not available.");

        if (CanGoBack)
        {
            if (mainPage.Navigation.ModalStack.Count > 0)
                await mainPage.Navigation.PopModalAsync(_animated);
            else
                await mainPage.GetNavigation().Navigation.PopAsync(_animated);
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
    public Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task GoForwardAsync() => throw new NotImplementedException();
    #endregion
}
