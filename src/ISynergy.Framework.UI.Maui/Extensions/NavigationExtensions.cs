using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

namespace ISynergy.Framework.UI.Extensions;

public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static IView CreatePage<TViewModel>(object parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(default(TViewModel), parameter);

    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static IView CreatePage<TViewModel>(TViewModel viewModel = null, object parameter = null) where TViewModel : class, IViewModel
    {
        var view = typeof(TViewModel).GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (ServiceLocator.Default.GetRequiredService(viewType) is IView resolvedPage)
        {
            if (viewModel is not null)
                resolvedPage.ViewModel = viewModel;

            if (resolvedPage.ViewModel is null)
                resolvedPage.ViewModel = ServiceLocator.Default.GetRequiredService<TViewModel>();

            if (parameter is not null && resolvedPage.ViewModel is not null)
                resolvedPage.ViewModel.Parameter = parameter;

            return resolvedPage;
        }

        throw new InvalidNavigationException($"Cannot create or navigate to page: {view}.");
    }

    /// <summary>
    /// Navigates to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushViewModelAsync<TViewModel>(this INavigation navigation, TViewModel viewModel = null, object parameter = null)
        where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(viewModel, parameter);

        if (!Application.Current.Windows[0].GetType().Name.Equals(page.GetType().Name))
            await navigation.PushAsync((Page)page, true);
    }

    /// <summary>
    /// Modal navigation to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushModalViewModelAsync<TViewModel>(this INavigation navigation, TViewModel viewModel = null, object parameter = null) where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(viewModel, parameter);

        if (!Application.Current.MainPage.GetType().Name.Equals(page.GetType().Name))
            await navigation.PushModalAsync((Page)page, true);
    }

    public static (INavigation Navigation, NavigationPage NavigationPage) GetNavigation(this Page page)
    {
        if (page is FlyoutPage flyoutPage)
        {
            if (flyoutPage.Detail is NavigationPage navigationPage)
                return (navigationPage.Navigation, navigationPage);

            return (flyoutPage.Navigation, null);
        }
        else if (page is NavigationPage navigationPage)
            return (navigationPage.Navigation, navigationPage);
        else
            return (Application.Current.MainPage.Navigation, null);
    }
}
