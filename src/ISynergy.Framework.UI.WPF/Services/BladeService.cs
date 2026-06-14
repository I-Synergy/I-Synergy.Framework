using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// WPF implementation of <see cref="IBladeService"/>. Resolves blade views
/// from the same DI scope as the ViewModel.
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
        var viewName = bladeVm.GetRelatedView();
        var viewType = viewName.GetRelatedViewType();

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
