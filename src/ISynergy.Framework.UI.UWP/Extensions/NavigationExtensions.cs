using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using System.Diagnostics.CodeAnalysis;
using View = ISynergy.Framework.UI.Controls.View;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Provides navigation helper methods for creating UWP pages from ViewModels.
/// </summary>
public static class NavigationExtensions
{
    /// <summary>
    /// Creates or gets a <see cref="View"/> from a ViewModel type using runtime type resolution.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to resolve a view for.</typeparam>
    /// <param name="scopedContextService">The scoped service provider.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The resolved <see cref="View"/> instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the view cannot be resolved for the ViewModel.</exception>
    /// <remarks>
    /// The view-type resolution uses <c>GetRelatedViewType()</c> which calls
    /// <c>AppDomain.CurrentDomain.GetAssemblies()</c>. Under UWP .NET Native,
    /// <c>AppDomain</c> is partially emulated and only returns assemblies the .NET Native linker
    /// has retained. Prefer source-generated view-type registration to avoid this limitation.
    /// </remarks>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "Under UWP .NET Native, assemblies not retained by the linker will not be discoverable. " +
        "Ensure view/ViewModel types are declared in Default.rd.xml or use source-generated registration.")]
    public static View CreatePage<TViewModel>(IScopedContextService scopedContextService, object? parameter = null) where TViewModel : class, IViewModel
        => CreatePage<TViewModel>(scopedContextService, default(TViewModel)!, parameter);

    /// <summary>
    /// Creates or gets a <see cref="View"/> from a ViewModel instance using runtime type resolution.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to resolve a view for.</typeparam>
    /// <param name="scopedContextService">The scoped service provider.</param>
    /// <param name="viewModel">
    /// The ViewModel instance to associate with the view. When <see langword="null"/>,
    /// the ViewModel is resolved from the service container.
    /// </param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The resolved <see cref="View"/> instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the view cannot be resolved for the ViewModel.</exception>
    /// <remarks>
    /// The view-type lookup calls <c>GetRelatedViewType()</c> which performs an assembly scan.
    /// Under UWP .NET Native, this requires all view/ViewModel assemblies to be declared in
    /// <c>Default.rd.xml</c>. Under standard trimming, ensure types are preserved via
    /// trimmer root descriptors or a source-generated registration.
    /// </remarks>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "Under UWP .NET Native, assemblies not retained by the linker will not be discoverable. " +
        "Ensure view/ViewModel types are declared in Default.rd.xml or use source-generated registration.")]
    public static View CreatePage<TViewModel>(IScopedContextService scopedContextService, TViewModel viewModel, object? parameter = null) where TViewModel : class, IViewModel
    {
        if (viewModel is null)
            viewModel = scopedContextService.GetRequiredService<TViewModel>();

        var view = viewModel.GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (viewType is not null && scopedContextService.GetRequiredService(viewType) is View resolvedPage)
        {
            if (viewModel is not null)
                resolvedPage.ViewModel = viewModel;

            if (resolvedPage.ViewModel is null)
                resolvedPage.ViewModel = scopedContextService.GetRequiredService<TViewModel>();

            if (parameter is not null && resolvedPage.ViewModel is not null)
                resolvedPage.ViewModel.Parameter = parameter;

            return resolvedPage;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {view}.");
    }
}
