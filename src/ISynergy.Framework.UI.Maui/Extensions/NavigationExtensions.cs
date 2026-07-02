using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using System.Globalization;

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
    /// <summary>
    /// Creates or gets a view from a ViewModel instance using the root <see cref="ServiceLocator"/>.
    /// Prefer the overload that accepts an <see cref="IServiceProvider"/>
    /// when a scoped provider is available.
    /// </summary>
    public static IView CreatePage<TViewModel>(TViewModel? viewModel = null, object? parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(ServiceLocator.Default.ServiceProvider, viewModel, parameter);

    /// <summary>
    /// Creates or gets a view from a ViewModel instance using the specified
    /// <paramref name="serviceProvider"/> for all service resolution.
    /// Resolving from a scoped provider (created via <see cref="IServiceScopeFactory"/>)
    /// ensures ViewModels are properly released when the scope is disposed,
    /// rather than being permanently tracked by the root container.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to resolve a view for.</typeparam>
    /// <param name="serviceProvider">The service provider to resolve services from (scoped or root).</param>
    /// <param name="viewModel">
    /// An existing ViewModel instance, or <see langword="null"/> to resolve from the provider.
    /// </param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The resolved <see cref="IView"/> instance.</returns>
    public static IView CreatePage<TViewModel>(IServiceProvider serviceProvider, TViewModel? viewModel = null, object? parameter = null) where TViewModel : class, IViewModel
    {
        var view = typeof(TViewModel).GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (viewType is null)
            throw new InvalidNavigationException($"Cannot determine view type for ViewModel: {typeof(TViewModel).Name}.");

        var resolvedService = serviceProvider.GetRequiredService(viewType);

        if (resolvedService is IView resolvedView)
        {
            if (viewModel is not null)
                resolvedView.ViewModel = viewModel;

            if (resolvedView.ViewModel is null)
                resolvedView.ViewModel = (IViewModel)serviceProvider.GetRequiredService<TViewModel>();

            if (parameter is not null && resolvedView.ViewModel is not null)
                resolvedView.ViewModel.Parameter = parameter;

            var language = serviceProvider.GetRequiredService<ILocalSettings>().Language;

            if (resolvedView is VisualElement visualElement && CultureInfo.GetCultureInfo(language).TextInfo.IsRightToLeft)
                visualElement.FlowDirection = FlowDirection.RightToLeft;

            return resolvedView;
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
        else if (page is Shell shell)
        {
            // Shell wraps each ShellContent in a NavigationPage internally.
            // shell.CurrentPage returns the visible content page of the active section,
            // and its Navigation property is the same INavigation that PushAsync
            // operates on. Shell's renderer keeps the hamburger visible independently
            // of the navigation stack depth.
            if (shell.CurrentPage is not null)
                return (shell.CurrentPage.Navigation, shell.CurrentPage as NavigationPage);

            return (shell.Navigation, null);
        }
        else
        {
            var currentApp = Application.Current;
            var mainPage = currentApp?.Windows.Count > 0 ? currentApp.Windows[0].Page : null;
            return (mainPage?.Navigation ?? throw new InvalidNavigationException("No navigation context available"), null);
        }
    }
}
