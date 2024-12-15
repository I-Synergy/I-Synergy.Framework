using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.DependencyInjection;
using View = ISynergy.Framework.UI.Controls.View;

namespace ISynergy.Framework.UI.Extensions;

public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="scopedContextService"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static View CreatePage<TViewModel>(IScopedContextService scopedContextService, object parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(scopedContextService, default(TViewModel), parameter);

    /// <summary>
    /// Creates or gets Page from ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="scopedContextService"></param>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static View CreatePage<TViewModel>(IScopedContextService scopedContextService, TViewModel viewModel, object parameter = null) where TViewModel : class, IViewModel
    {
        if (viewModel is null)
            viewModel = scopedContextService.ServiceProvider.GetRequiredService<TViewModel>();

        var view = viewModel.GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (scopedContextService.ServiceProvider.GetRequiredService(viewType) is View resolvedPage)
        {
            if (viewModel is not null)
                resolvedPage.ViewModel = viewModel;

            if (resolvedPage.ViewModel is null)
                resolvedPage.ViewModel = scopedContextService.ServiceProvider.GetRequiredService<TViewModel>();

            if (parameter is not null && resolvedPage.ViewModel is not null)
                resolvedPage.ViewModel.Parameter = parameter;

            return resolvedPage;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {view}.");
    }
}
