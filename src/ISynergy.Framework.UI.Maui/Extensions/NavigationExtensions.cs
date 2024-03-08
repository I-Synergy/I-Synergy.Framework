using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

namespace ISynergy.Framework.UI.Extensions;

public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="context"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static IView CreatePage<TViewModel>(IContext context, object parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(context, default(TViewModel), parameter);

    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="context"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static IView CreatePage<TViewModel>(IContext context, TViewModel viewModel = null, object parameter = null) where TViewModel : class, IViewModel
    {
        var view = typeof(TViewModel).GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (context.ScopedServices.ServiceProvider.GetRequiredService(viewType) is IView resolvedPage)
        {
            if (resolvedPage.ViewModel is null)
            {
                if (viewModel is not null)
                    resolvedPage.ViewModel = viewModel;
                else
                    resolvedPage.ViewModel = context.ScopedServices.ServiceProvider.GetRequiredService<TViewModel>();

                resolvedPage.ViewModel.Parameter = parameter;
            }

            return resolvedPage;
        }

        throw new InvalidNavigationException($"Cannot create or navigate to page: {view}.");
    }

    /// <summary>
    /// Navigates to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="context"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushViewModelAsync<TViewModel>(this INavigation navigation, IContext context, TViewModel viewModel = null, object parameter = null)
        where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(context, viewModel, parameter);

        if (!Application.Current.MainPage.GetType().Name.Equals(page.GetType().Name))
            await navigation.PushAsync((Page)page, true);
    }

    /// <summary>
    /// Modal navigation to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="context"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushModalViewModelAsync<TViewModel>(this INavigation navigation, IContext context, TViewModel viewModel = null, object parameter = null) where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(context, viewModel, parameter);

        if (!Application.Current.MainPage.GetType().Name.Equals(page.GetType().Name))
            await navigation.PushModalAsync((Page)page, true);
    }
}
