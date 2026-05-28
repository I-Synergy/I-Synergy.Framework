using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using View = ISynergy.Framework.UI.Controls.View;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Provides navigation helper methods for creating platform pages from ViewModels.
/// </summary>
public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets a <see cref="View"/> from a ViewModel type using runtime type resolution.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to resolve a view for.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The resolved <see cref="View"/> instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the view cannot be resolved for the ViewModel.</exception>
    /// <remarks>
    /// This overload calls <see cref="CreatePage{TViewModel}(IServiceProvider, TViewModel, object?)"/>
    /// with a <see langword="null"/> ViewModel; the ViewModel is resolved from the service container.
    /// </remarks>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "This is not trim-safe. Ensure view/ViewModel types are preserved via a trimmer root descriptor or source-generated registration.")]
    public static View CreatePage<TViewModel>(IServiceProvider serviceProvider, object? parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(serviceProvider, default(TViewModel)!, parameter);

    /// <summary>
    /// Creates or gets a <see cref="View"/> from a ViewModel instance using runtime type resolution.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to resolve a view for.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="viewModel">
    /// The ViewModel instance to associate with the view. When <see langword="null"/>,
    /// the ViewModel is resolved from the service container.
    /// </param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The resolved <see cref="View"/> instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the view cannot be resolved for the ViewModel.</exception>
    /// <remarks>
    /// The view-type lookup calls <c>GetRelatedViewType()</c> which performs an assembly scan.
    /// This is not compatible with NativeAOT or aggressive trimming without a source-generated view map.
    /// </remarks>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "This is not trim-safe. Ensure view/ViewModel types are preserved via a trimmer root descriptor or source-generated registration.")]
    public static View CreatePage<TViewModel>(IServiceProvider serviceProvider, TViewModel viewModel, object? parameter = null) where TViewModel : class, IViewModel
    {
        if (viewModel is null)
            viewModel = serviceProvider.GetRequiredService<TViewModel>();

        var view = viewModel.GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (viewType is not null && serviceProvider.GetRequiredService(viewType) is View resolvedPage)
        {
            if (viewModel is not null)
                resolvedPage.ViewModel = viewModel;

            if (resolvedPage.ViewModel is null)
                resolvedPage.ViewModel = serviceProvider.GetRequiredService<TViewModel>();

            if (parameter is not null && resolvedPage.ViewModel is not null)
                resolvedPage.ViewModel.Parameter = parameter;

            return resolvedPage;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {view}.");
    }
}
