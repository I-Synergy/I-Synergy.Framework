using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.DependencyInjection;

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
        /// <exception cref="FileNotFoundException"></exception>
        public static View CreatePage<TViewModel>(object parameter = null) where TViewModel : class, IViewModel
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

            var page = WindowsAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewmodel.GetViewFullName()));

            if (page is null)
                throw new Exception($"Page not found: {viewmodel.GetViewFullName()}.");

            if (context.ScopedServices.ServiceProvider.GetRequiredService(page) is View resolvedPage)
            {
                resolvedPage.ViewModel = viewmodel;
                return resolvedPage;
            }

            throw new FileNotFoundException($"Cannot create or navigate to page: {viewmodel.GetViewFullName()}.");
        }
    }
}
