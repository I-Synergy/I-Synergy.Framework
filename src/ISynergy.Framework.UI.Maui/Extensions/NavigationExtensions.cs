using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

namespace ISynergy.Framework.UI.Extensions;

public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static IView CreatePage<TViewModel>(IServiceProvider serviceProvider, object parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(serviceProvider, default(TViewModel), parameter);

    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static IView CreatePage<TViewModel>(IServiceProvider serviceProvider, TViewModel viewModel = null, object parameter = null) where TViewModel : class, IViewModel
    {
        var page = MauiAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(typeof(TViewModel).GetRelatedView()));

        if (page is null)
            throw new Exception($"Page not found: {typeof(TViewModel).GetRelatedView()}.");

        if (serviceProvider.GetRequiredService(page) is IView resolvedPage)
        {
            if (resolvedPage.ViewModel is null)
            {
                resolvedPage.ViewModel = serviceProvider.GetRequiredService<TViewModel>();
                resolvedPage.ViewModel.Parameter = parameter;
            }

            return resolvedPage;
        }

        throw new InvalidNavigationException($"Cannot create or navigate to page: {typeof(TViewModel).GetRelatedView()}.");
    }

    /// <summary>
    /// Navigates to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushViewModelAsync<TViewModel>(this INavigation navigation, IServiceProvider serviceProvider, TViewModel viewModel = null, object parameter = null)
        where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(serviceProvider, viewModel, parameter);

        if (!Application.Current.MainPage.GetType().Name.Equals(page.GetType().Name))
            await navigation.PushAsync((Page)page, true);
    }

    /// <summary>
    /// Modal navigation to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static async Task PushModalViewModelAsync<TViewModel>(this INavigation navigation, IServiceProvider serviceProvider, TViewModel viewModel = null, object parameter = null) where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(serviceProvider, viewModel, parameter);

        if (!Application.Current.MainPage.GetType().Name.Equals(page.GetType().Name))
            await navigation.PushModalAsync((Page)page, true);
    }

    ///// <summary>
    ///// Navigates to the viewmodel.
    ///// </summary>
    ///// <typeparam name="TViewModel"></typeparam>
    ///// <param name="shell"></param>
    ///// <param name="serviceProvider"></param>
    ///// <param name="viewModel"></param>
    ///// <param name="parameter"></param>
    ///// <returns></returns>
    //public static async Task GoToViewModelAsync<TViewModel>(this Shell shell, IServiceProvider serviceProvider, TViewModel viewModel, object parameter = null)
    //    where TViewModel : class, IViewModel
    //{
    //    if (Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is IDisposable disposable)
    //        disposable.Dispose();

    //    var page = CreatePage<TViewModel>(serviceProvider, viewModel, parameter);

    //    if (!shell.CurrentPage.GetType().Name.Equals(page.GetType().Name))
    //        await shell.GoToAsync($"//{page.GetType().Name}");
    //}
}
