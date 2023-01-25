using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Views;

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
            var resolvedPage = ServiceLocator.Default.GetInstance<T>();
            await navigation.PushAsync(resolvedPage);
            return resolvedPage;
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
            var resolvedPage = ActivatorUtilities.CreateInstance<T>(ServiceLocator.Default.GetServiceProvider(), parameters);
            await navigation.PushAsync(resolvedPage);
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
            var resolvedPage = ServiceLocator.Default.GetInstance<T>();
            await navigation.PushModalAsync(resolvedPage);
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
            var resolvedPage = ActivatorUtilities.CreateInstance<T>(ServiceLocator.Default.GetServiceProvider(), parameters);
            await navigation.PushModalAsync(resolvedPage);
            return resolvedPage;
        }

        public static void ReplaceMainWindow<T>(this Page? mainWindow)
            where T : IView
        {
            if (mainWindow is not null && ServiceLocator.Default.GetInstance<T>() is Page page)
                mainWindow = new NavigationPage(page);
            else
                throw new InvalidCastException($"Implementation of '{nameof(T)}' is not of type of Page.");
        }
    }
}
