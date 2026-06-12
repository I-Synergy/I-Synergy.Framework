using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
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
    protected override async Task SetupAndOpenBladeAsync(
        IViewModelBladeView owner,
        IViewModelBlade bladeVm,
        IServiceProvider serviceProvider)
    {
        var viewType = ResolveViewType(bladeVm);

        if (viewType is null || serviceProvider.GetRequiredService(viewType) is not IView view)
            throw new InvalidOperationException($"Cannot resolve view for blade: {bladeVm.GetType().Name}");

        view.ViewModel = bladeVm;

        PrepareOwnerForBlade(owner, bladeVm);

        // Wait for the view to finish loading
        var loadTask = MonitorViewLoadAsync(view);

        if (!bladeVm.IsInitialized)
            await bladeVm.InitializeAsync();

        await loadTask;

        SafeOnNavigatedTo(bladeVm);
        AddBladeToOwner(owner, view);
    }

    /// <summary>
    /// Resolves the view type for a blade ViewModel. Selection ViewModels get
    /// their known selection views; others use reflection-based naming convention.
    /// </summary>
    private static Type? ResolveViewType(IViewModel bladeVm)
    {
        if (bladeVm is ISingleSelectionViewModel)
            return typeof(SingleSelectionView);

        if (bladeVm is IMultipleSelectionViewModel)
            return typeof(MultipleSelectionView);

        var viewName = bladeVm.GetRelatedView();
        return viewName.GetRelatedViewType();
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

            await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
        }
    }
}
