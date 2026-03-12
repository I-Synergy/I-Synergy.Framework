using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Provides navigation helper methods for MAUI pages created from ViewModels.
/// </summary>
public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets a view from a ViewModel type using runtime type resolution.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to resolve a view for.</typeparam>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The resolved <see cref="IView"/> instance.</returns>
    /// <exception cref="InvalidNavigationException">Thrown when the view cannot be resolved for the ViewModel.</exception>
    /// <remarks>
    /// The view-type resolution uses <c>GetRelatedViewType()</c> which scans loaded assemblies.
    /// This is not compatible with MAUI iOS/Android linker trimming without a source-generated view map.
    /// Ensure view/ViewModel types are preserved via a linker configuration file or source-generated registration.
    /// </remarks>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "This is not trim-safe. Ensure view/ViewModel types are preserved via linker configuration or source-generated registration.")]
    public static IView CreatePage<TViewModel>(object? parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(default(TViewModel), parameter);

    /// <summary>
    /// Creates or gets a view from a ViewModel instance using runtime type resolution.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to resolve a view for.</typeparam>
    /// <param name="viewModel">
    /// The ViewModel instance to associate with the view. When <see langword="null"/>,
    /// the ViewModel is resolved from the service container.
    /// </param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The resolved <see cref="IView"/> instance.</returns>
    /// <exception cref="InvalidNavigationException">Thrown when the view cannot be resolved for the ViewModel.</exception>
    /// <remarks>
    /// The view-type lookup calls <c>GetRelatedViewType()</c> which performs an assembly scan.
    /// This is not compatible with MAUI iOS/Android linker trimming without a source-generated view map.
    /// </remarks>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "This is not trim-safe. Ensure view/ViewModel types are preserved via linker configuration or source-generated registration.")]
    public static IView CreatePage<TViewModel>(TViewModel? viewModel = null, object? parameter = null) where TViewModel : class, IViewModel
    {
        var view = typeof(TViewModel).GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (viewType is null)
            throw new InvalidNavigationException($"Cannot determine view type for ViewModel: {typeof(TViewModel).Name}.");

        var resolvedService = ServiceLocator.Default.GetRequiredService(viewType);
        if (resolvedService is IView resolvedPage)
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
    /// <typeparam name="TViewModel">The ViewModel type to navigate to.</typeparam>
    /// <param name="navigation">The MAUI navigation instance.</param>
    /// <param name="viewModel">An optional ViewModel instance. When <see langword="null"/>, resolved from DI.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "This is not trim-safe. Ensure view/ViewModel types are preserved via linker configuration or source-generated registration.")]
    public static async Task PushViewModelAsync<TViewModel>(this INavigation navigation, TViewModel? viewModel = null, object? parameter = null)
        where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(viewModel, parameter);

        // Use direct type equality instead of name comparison to avoid fragile string comparison.
        if (Application.Current?.Windows[0] is not null && Application.Current.Windows[0].GetType() != page.GetType())
            await navigation.PushAsync((Page)page, true);
    }

    /// <summary>
    /// Modal navigation to the viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to navigate to.</typeparam>
    /// <param name="navigation">The MAUI navigation instance.</param>
    /// <param name="viewModel">An optional ViewModel instance. When <see langword="null"/>, resolved from DI.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>A task representing the asynchronous modal navigation operation.</returns>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "This is not trim-safe. Ensure view/ViewModel types are preserved via linker configuration or source-generated registration.")]
    public static async Task PushModalViewModelAsync<TViewModel>(this INavigation navigation, TViewModel? viewModel = null, object? parameter = null) where TViewModel : class, IViewModel
    {
        var page = CreatePage<TViewModel>(viewModel, parameter);
        var currentApp = Application.Current;

        if (currentApp is not null && currentApp.Windows.Count > 0)
        {
            var currentPage = currentApp.Windows[0].Page;
            // Use direct type equality instead of name comparison to avoid fragile string comparison.
            if (currentPage is not null && page is not null && currentPage.GetType() != page.GetType())
                await navigation.PushModalAsync((Page)page, true);
        }
    }

    public static (INavigation Navigation, NavigationPage?) GetNavigation(this Page page)
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
        {
            var currentApp = Application.Current;
            var mainPage = currentApp?.Windows.Count > 0 ? currentApp.Windows[0].Page : null;
            return (mainPage?.Navigation ?? throw new InvalidNavigationException("No navigation context available"), null);
        }
    }
}
