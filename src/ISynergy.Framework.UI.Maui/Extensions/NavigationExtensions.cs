using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

namespace ISynergy.Framework.UI.Extensions
{
    public static class NavigationExtensions
    {
        /// <summary>
        /// Resolves a page of type T (must inherit from Page) and pushes a new instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public static async Task<T> PushAsync<T>(this INavigation navigation) where T : Page, IView
        {
            var context = ServiceLocator.Default.GetInstance<IContext>();
            var resolvedPage = context.ScopedServices.ServiceProvider.GetRequiredService<T>();
            await navigation.PushAsync(resolvedPage, true);
            return resolvedPage;
        }

        public static async Task PushViewModelAsync<TViewModel>(this INavigation navigation, object parameter = null) where TViewModel : class, IViewModel
        {
            var context = ServiceLocator.Default.GetInstance<IContext>();
            var viewmodel = parameter is IViewModel instance ? instance : context.ScopedServices.ServiceProvider.GetRequiredService<TViewModel>();
            var page = MauiAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewmodel.GetViewFullName()));

            if (page is null)
                throw new Exception($"Page not found: {viewmodel.GetViewFullName()}.");

            if (context.ScopedServices.ServiceProvider.GetRequiredService(page) is Page resolvedPage)
                await navigation.PushAsync(resolvedPage, true);
        }

        /// <summary>
        /// Resolves a page of type T (must inherit from Page) and pushes a new instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T">The type of the page to be resolved</typeparam>
        /// <param name="navigation"></param>
        /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
        /// <returns></returns>
        public static async Task<T> PushAsync<T>(this INavigation navigation, params object[] parameters) where T : Page, IView
        {
            var context = ServiceLocator.Default.GetInstance<IContext>();
            var resolvedPage = ActivatorUtilities.CreateInstance<T>(context.ScopedServices.ServiceProvider, parameters);
            await navigation.PushAsync(resolvedPage, true);
            return resolvedPage;
        }

        /// <summary>
        /// Resolves a page of type T (must inherit from Page) and pushes a new modal instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public static async Task<T> PushModalAsync<T>(this INavigation navigation) where T : Page, IView
        {
            var context = ServiceLocator.Default.GetInstance<IContext>();
            var resolvedPage = context.ScopedServices.ServiceProvider.GetRequiredService<T>();
            await navigation.PushModalAsync(resolvedPage, true);
            return resolvedPage;
        }

        /// <summary>
        /// Resolves a page of type T (must inherit from Page) and pushes a new modal instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T">The type of the page to be resolved</typeparam>
        /// <param name="navigation"></param>
        /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
        /// <returns></returns>
        public static async Task<T> PushModalAsync<T>(this INavigation navigation, params object[] parameters) where T : Page, IView
        {
            var context = ServiceLocator.Default.GetInstance<IContext>();
            var resolvedPage = ActivatorUtilities.CreateInstance<T>(context.ScopedServices.ServiceProvider, parameters);
            await navigation.PushModalAsync(resolvedPage, true);
            return resolvedPage;
        }
    }
}
