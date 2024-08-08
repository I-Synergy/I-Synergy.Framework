using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

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
    public static View CreatePage<TViewModel>(IContext context, object parameter = null) where TViewModel : class, IViewModel
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
    public static View CreatePage<TViewModel>(IContext context, TViewModel viewModel, object parameter = null) where TViewModel : class, IViewModel
    {
        var view = typeof(TViewModel).GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (context.ScopedServices.ServiceProvider.GetRequiredService(viewType) is View resolvedPage)
        {
            if (viewModel is not null)
                resolvedPage.ViewModel = viewModel;

            if (resolvedPage.ViewModel is null)
                resolvedPage.ViewModel = context.ScopedServices.ServiceProvider.GetRequiredService<TViewModel>();

            if (parameter is not null && resolvedPage.ViewModel is not null)
                resolvedPage.ViewModel.Parameter = parameter;

            return resolvedPage;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {view}.");
    }
}
