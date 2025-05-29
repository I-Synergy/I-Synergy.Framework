using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

namespace ISynergy.Framework.UI.Extensions;

public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="scopedContextService"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static View CreatePage<TViewModel>(IScopedContextService scopedContextService, object? parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(scopedContextService, default(TViewModel)!, parameter);

    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="scopedContextService"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static View CreatePage<TViewModel>(IScopedContextService scopedContextService, TViewModel viewModel, object? parameter = null) where TViewModel : class, IViewModel
    {
        if (viewModel is null)
            viewModel = scopedContextService.GetRequiredService<TViewModel>();

        var view = viewModel.GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (viewType is not null && scopedContextService.GetRequiredService(viewType) is View resolvedPage)
        {
            if (viewModel is not null)
                resolvedPage.ViewModel = viewModel;

            if (resolvedPage.ViewModel is null)
                resolvedPage.ViewModel = scopedContextService.GetRequiredService<TViewModel>();

            if (parameter is not null && resolvedPage.ViewModel is not null)
                resolvedPage.ViewModel.Parameter = parameter;

            return resolvedPage;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {view}.");
    }

    /// <summary>
    /// Navigates to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="scopedContextService"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushViewModelAsync<TViewModel>(this INavigation navigation, IScopedContextService scopedContextService, TViewModel? viewModel = null, object? parameter = null)
        where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(scopedContextService, viewModel, parameter);

        if (!Microsoft.Maui.Controls.Application.Current.Windows[0].GetType().Name.Equals(page.GetType().Name))
            await navigation.PushAsync((Page)page, true);
    }

    /// <summary>
    /// Modal navigation to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="scopedContextService"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushModalViewModelAsync<TViewModel>(this INavigation navigation, IScopedContextService scopedContextService, TViewModel? viewModel = null, object? parameter = null) where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(scopedContextService, viewModel, parameter);

        if (!Microsoft.Maui.Controls.Application.Current.MainPage.GetType().Name.Equals(page.GetType().Name))
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
            return (Microsoft.Maui.Controls.Application.Current.MainPage.Navigation, null);
    }
}
