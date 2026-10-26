using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Services.Base;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// MAUI implementation of <see cref="IBladeService"/>. It is adaptive: on tablet and desktop
/// (where an attached <see cref="IBladePaneHost"/> exists) blades are added to the owner and
/// rendered as an overlay pane by <see cref="ISynergy.Framework.UI.Controls.BladePane"/>; on phone — or when no pane host is
/// present in the current page — a blade is pushed as a native modal page.
/// </summary>
public class BladeService : BaseBladeService
{
    private const int DefaultLoadTimeoutMs = 500;

    private readonly ILogger<BladeService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BladeService"/> class.
    /// </summary>
    /// <param name="scopeFactory">The DI scope factory used to create blade scopes.</param>
    /// <param name="exceptionHandlerService">The exception handler service.</param>
    /// <param name="logger">The logger.</param>
    public BladeService(
        IServiceScopeFactory scopeFactory,
        IExceptionHandlerService exceptionHandlerService,
        ILogger<BladeService> logger)
        : base(scopeFactory, exceptionHandlerService, logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        // Wait for the view to finish loading.
        var loadTask = MonitorViewLoadAsync(view);

        if (!bladeVm.IsInitialized)
            await bladeVm.InitializeAsync();

        await loadTask;

        SafeOnNavigatedTo(bladeVm);

        var hostAttached = ResolveBladePaneHost()?.IsHostAttached ?? false;

        // Overlay pane on tablet/desktop when a pane host is present in the current page.
        if (DeviceInfo.Idiom != DeviceIdiom.Phone && hostAttached)
        {
            AddBladeToOwner(owner, view);
            return;
        }

        // Phone, or no pane in the current page: push a native modal page.
        if (view is not Page page)
        {
            _logger.LogWarning(
                "Blade '{ViewModelType}' view '{ViewType}' is not a Page; falling back to the overlay pane host.",
                bladeVm.GetType().Name, view.GetType().Name);

            AddBladeToOwner(owner, view);
            return;
        }

        var rootPage = Application.Current?.Windows.FirstOrDefault()?.Page;

        if (rootPage is null)
        {
            _logger.LogWarning(
                "No root page is available to present blade '{ViewModelType}' modally; falling back to the overlay pane host.",
                bladeVm.GetType().Name);

            AddBladeToOwner(owner, view);
            return;
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(() => rootPage.Navigation.PushModalAsync(page, true));

            // Remember the page so the blade's Closed event pops it. The blade VM's own Closed
            // event (wired in BaseBladeService) drives scope disposal; this simply closes the modal.
            // This handler is intentionally not detached: the blade VM belongs to a DI scope that is
            // disposed when the blade closes, so the subscription is collected together with it.
            bladeVm.Closed += (_, _) => _ = PopModalAsync(rootPage, page);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to present blade '{ViewModelType}' modally; falling back to the overlay pane host.",
                bladeVm.GetType().Name);

            AddBladeToOwner(owner, view);
        }
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

    /// <summary>
    /// Pops the modal page hosting a blade. Only pops when <paramref name="modalPage"/> is actually on
    /// top of the modal stack, so closing an older blade never pops a newer page. Runs on the UI
    /// thread and never throws.
    /// </summary>
    private async Task PopModalAsync(Page rootPage, Page modalPage)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (rootPage.Navigation.ModalStack.Count > 0 && ReferenceEquals(rootPage.Navigation.ModalStack[^1], modalPage))
                    await rootPage.Navigation.PopModalAsync(true);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pop the modal page hosting a blade.");
        }
    }

    private static IBladePaneHost? ResolveBladePaneHost() =>
        IPlatformApplication.Current?.Services?.GetService<IBladePaneHost>();

    /// <inheritdoc />
    protected override IViewModelBladeView ResolveBladeHost(IViewModelBladeView owner, BladeHostTargets hostTarget) =>
        hostTarget == BladeHostTargets.Shell
            ? IPlatformApplication.Current?.Services?.GetService<BladeHostViewModel>() ?? owner
            : owner;

    /// <inheritdoc />
    protected override void DetachBladeView(IView view)
    {
        // Blade views are singletons (Controls.View): clear the binding context so a queued binding
        // update cannot write into the view model that the blade scope is about to dispose.
        if (view is Page page)
            page.BindingContext = null;
    }

    /// <inheritdoc />
    protected override async Task MonitorViewLoadAsync(IView view, int timeoutMs = DefaultLoadTimeoutMs)
    {
        if (view is VisualElement element && !element.IsLoaded)
        {
            var tcs = new TaskCompletionSource<bool>();

            void OnLoaded(object? sender, EventArgs e)
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
