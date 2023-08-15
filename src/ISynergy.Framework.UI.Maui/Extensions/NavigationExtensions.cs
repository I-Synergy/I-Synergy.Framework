using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;

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
        public static async Task<Page> CreatePage<TViewModel>(object parameter = null) where TViewModel : class, IViewModel
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

                await view.ViewModel.InitializeAsync();
                
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
        public static async Task PushViewModelAsync<TViewModel>(this INavigation navigation, object parameter = null) 
            where TViewModel : class, IViewModel
        {
            var page = await CreatePage<TViewModel>(parameter);
            await navigation.PushAsync(page, true);
        }


        /// <summary>
        /// Navigates to the viewmodel.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="navigation"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static async Task PushModalViewModelAsync<TViewModel>(this INavigation navigation, object parameter = null) where TViewModel : class, IViewModel 
        {
            var page = await CreatePage<TViewModel>(parameter);
            await navigation.PushModalAsync(page, true);
        }
    }
}
