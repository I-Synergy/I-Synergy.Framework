using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// WinUI implementation of <see cref="IBladeService"/>. Resolves blade views
/// from the same DI scope as the ViewModel and supports selection view types.
/// </summary>
public class BladeService : BaseBladeService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BladeService"/> class.
    /// </summary>
    public BladeService(
        IServiceScopeFactory scopeFactory,
        IExceptionHandlerService exceptionHandlerService,
        ILogger<BladeService> logger)
        : base(scopeFactory, exceptionHandlerService, logger)
    {
    }

    /// <inheritdoc />
    protected override Type? ResolveViewType(IViewModel bladeVm)
    {
        if (bladeVm is ISingleSelectionViewModel)
            return typeof(SingleSelectionView);

        if (bladeVm is IMultipleSelectionViewModel)
            return typeof(MultipleSelectionView);

        var viewName = bladeVm.GetRelatedView();
        return viewName.GetRelatedViewType();
    }

    /// <inheritdoc />
    protected override IViewModelBladeView ResolveBladeHost(IViewModelBladeView owner, BladeHostTargets hostTarget) =>
        hostTarget == BladeHostTargets.Shell
            ? Application.Services.GetService<BladeHostViewModel>() ?? owner
            : owner;

    /// <inheritdoc />
    protected override void DetachBladeView(IView view)
    {
        // Blade views are singletons (Controls.View): clear the data context so a queued binding
        // update cannot write into the view model that the blade scope is about to dispose.
        if (view is FrameworkElement element)
            element.DataContext = null;
    }

    /// <inheritdoc />
    protected override async Task MonitorViewLoadAsync(IView view, int timeoutMs = 500)
    {
        if (view is FrameworkElement element && !element.IsLoaded)
        {
            var tcs = new TaskCompletionSource<bool>();

            void OnLoaded(object s, RoutedEventArgs e)
            {
                element.Loaded -= OnLoaded;
                tcs.TrySetResult(true);
            }

            element.Loaded += OnLoaded;

            try
            {
                await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
            }
            finally
            {
                element.Loaded -= OnLoaded;
            }
        }
    }
}
