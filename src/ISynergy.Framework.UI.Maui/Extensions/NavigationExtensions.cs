using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Maui.Controls;
using System.Reflection.Metadata;

namespace ISynergy.Framework.UI.Extensions
{
    public static class NavigationExtensions
    {
        /// <summary>
        /// Creates or gets Page from ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidNavigationException"></exception>
        public static Page CreatePage<TViewModel>(object parameter = null) where TViewModel : class, IViewModel
        {
            var context = ServiceLocator.Default.GetInstance<IContext>();
            var viewmodel = default(TViewModel);

            if (parameter is TViewModel instance)
            {
                viewmodel = instance;
            }
            else
            {
                viewmodel = context.ScopedServices.ServiceProvider.GetRequiredService<TViewModel>();
                viewmodel.Parameter = parameter;
            }

            var page = MauiAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewmodel.GetViewFullName()));

            if (page is null)
                throw new Exception($"Page not found: {viewmodel.GetViewFullName()}.");

            if (context.ScopedServices.ServiceProvider.GetRequiredService(page) is Page resolvedPage && resolvedPage is IView view)
            {
                view.ViewModel = viewmodel;
                return resolvedPage;
            }

            throw new InvalidNavigationException($"Cannot create or navigate to page: {viewmodel.GetViewFullName()}.");
        }

        /// <summary>
        /// Navigates to the viewmodel.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="navigation"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static Task PushViewModelAsync<TViewModel>(this INavigation navigation, object parameter = null) where TViewModel : class, IViewModel =>
            navigation.PushAsync(CreatePage<TViewModel>(parameter), true);

        /// <summary>
        /// Navigates to the viewmodel.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="navigation"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static Task PushModalViewModelAsync<TViewModel>(this INavigation navigation, object parameter = null) where TViewModel : class, IViewModel =>
            navigation.PushAsync(new NavigationPage(CreatePage<TViewModel>(parameter)), true);
    }
}
