using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.UI.Extensions;

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
        => CreatePage<TViewModel>(default(TViewModel), parameter);

    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static View CreatePage<TViewModel>(TViewModel viewModel, object parameter = null) where TViewModel : class, IViewModel
    {
        var context = ServiceLocator.Default.GetInstance<IContext>();
        
        if (viewModel is null && context.ScopedServices.ServiceProvider.GetRequiredService(typeof(TViewModel)) is TViewModel resolvedViewModel)
            viewModel = resolvedViewModel;

        viewModel.Parameter = parameter;

        var page = WindowsAppBuilderExtensions.ViewTypes.SingleOrDefault(q => q.Name.Equals(viewModel.GetViewFullName()));

        if (page is null)
            throw new Exception($"Page not found: {viewModel.GetViewFullName()}.");

        if (ServiceLocator.Default.GetInstance(page) is View resolvedPage)
        {
            resolvedPage.ViewModel = viewModel;
            return resolvedPage;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {viewModel.GetViewFullName()}.");
    }
}
