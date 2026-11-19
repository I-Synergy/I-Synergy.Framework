using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// MAUI implementation of the navigation service.
/// </summary>
/// <remarks>
/// Navigation follows the WinUI and WPF model: the shell owns a content host (the MAUI
/// equivalent of a <c>Frame</c>) and navigation replaces the host's content while
/// <see cref="BaseNavigationService"/> owns the backstack. MAUI pages cannot be children of a
/// layout, so <see cref="INavigationHost"/> reparents each page's content instead of the page.
/// Modal navigation still uses MAUI's native modal stack.
/// </remarks>
public partial class NavigationService : BaseNavigationService
{
    private const string DefaultPageNotAvailable = "Main page is not available.";
    private const bool Animated = true;

    /// <summary>
    /// Modals pushed onto the native stack, with the scope that owns their view model.
    /// </summary>
    private readonly Stack<ModalPage> _modals = new();

    private sealed record ModalPage(Page Page, IServiceScope Scope);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    public NavigationService(
        IExceptionHandlerService exceptionHandlerService,
        ICommonServices commonServices,
        ILocalSettings localSettings,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NavigationService> logger)
        : base(exceptionHandlerService, commonServices, localSettings, serviceScopeFactory, logger)
    {
        _logger.LogTrace("NavigationService instance created for MAUI");
    }

    // -----------------------------------------------------------------------
    // Override: CanGoBack — the shell backstack plus any open native modal
    // -----------------------------------------------------------------------
    public override bool CanGoBack =>
        _backStack.Count > 0 ||
        (GetMainPage() is Page page && page.Navigation.ModalStack.Count > 0);

    // -----------------------------------------------------------------------
    // Abstract: navigation frame — the shell's content host
    // -----------------------------------------------------------------------
    protected override object? GetNavigationFrame() => NavigationHostRegistry.Resolve();

    protected override object? GetFrameContent(object frame) => ((INavigationHost)frame).CurrentPage;

    protected override void SetFrameContent(object frame, IView page) => ((INavigationHost)frame).SetContent(page);

    // -----------------------------------------------------------------------
    // Abstract: Window content accessors
    // -----------------------------------------------------------------------
    protected override object? GetWindowContent()
        => (Application.Current?.Windows is { Count: > 0 } windows ? windows[0].Page : null);

    protected override void SetWindowContent(IView page)
    {
        if (Application.Current?.Windows.Count > 0)
            Application.Current.Windows[0].Page = (Page)page;
    }

    // -----------------------------------------------------------------------
    // Abstract: CreateView — MAUI-specific view creation with RTL support
    // -----------------------------------------------------------------------
    protected override IView CreateView(IServiceProvider serviceProvider, Type viewModelType, IViewModel? viewModel, object? parameter)
    {
        if (viewModel is null)
            viewModel = (IViewModel)serviceProvider.GetRequiredService(viewModelType);

        var viewName = viewModelType.GetRelatedView();
        var viewType = viewName.GetRelatedViewType();

        if (viewType is not null && serviceProvider.GetRequiredService(viewType) is IView resolvedView)
        {
            resolvedView.ViewModel = viewModel;

            if (parameter is not null && resolvedView.ViewModel is not null)
                resolvedView.ViewModel.Parameter = parameter;

            if (resolvedView is VisualElement visualElement &&
                CultureInfo.GetCultureInfo(_localSettings.Language).TextInfo.IsRightToLeft)
                visualElement.FlowDirection = FlowDirection.RightToLeft;

            return resolvedView;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {viewName}.");
    }

    // -----------------------------------------------------------------------
    // Abstract: MonitorViewLoadAsync — MAUI uses OnNavigatedTo, no Loaded event
    // -----------------------------------------------------------------------
    protected override Task MonitorViewLoadAsync(IView view, int timeoutMs = 500)
        => Task.CompletedTask;

    // -----------------------------------------------------------------------
    // Abstract: DispatchOnUIThread
    // -----------------------------------------------------------------------
    protected override void DispatchOnUIThread(Action action)
    {
        if (MainThread.IsMainThread)
            action();
        else
            MainThread.BeginInvokeOnMainThread(action);
    }

    // -----------------------------------------------------------------------
    // Override: GoBackAsync — pop a native modal first, then the shell backstack
    // -----------------------------------------------------------------------
    public override async Task GoBackAsync()
    {
        var mainPage = GetMainPage();

        if (mainPage is null)
            throw new InvalidOperationException(DefaultPageNotAvailable);

        if (mainPage.Navigation.ModalStack.Count > 0)
        {
            var poppedPage = mainPage.Navigation.ModalStack[^1];
            await mainPage.Navigation.PopModalAsync(Animated);

            if (_modals.Count > 0)
            {
                var modal = _modals.Pop();

                if (ReferenceEquals(modal.Page, poppedPage))
                {
                    try { modal.Scope.Dispose(); }
                    catch (Exception ex) { _logger.LogError(ex, "Error disposing the modal scope"); }
                }
            }

            CleanupPageResources(poppedPage);
            OnBackStackChanged(EventArgs.Empty);
            return;
        }

        await base.GoBackAsync();

        // Always announce the pop: shells derive their back-button state, breadcrumb and selection
        // from the stack, and the base only raises when the stack becomes empty.
        OnBackStackChanged(EventArgs.Empty);
    }

    // -----------------------------------------------------------------------
    // Override: NavigateModalAsync — native MAUI modal stack
    // -----------------------------------------------------------------------
    public override async Task NavigateModalAsync<TViewModel>(object? parameter = null)
    {
        if (Application.Current is null)
            return;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var scope = _serviceScopeFactory.CreateScope();

            if (NavigationExtensions.CreatePage<TViewModel>(scope.ServiceProvider, parameter: parameter) is not { } view ||
                view is not Page page)
            {
                scope.Dispose();
                return;
            }

            await PresentModalPageAsync(page, scope);

            // A shell is a root page, not a modal: PresentModalPageAsync has already installed it
            // as the window root and the shell initializes itself from its Loaded handler.
            if (page is not IShellView)
                await InitializeModalViewModelAsync(view);
        });
    }

    /// <summary>
    /// Presents the modal page by pushing it onto a host root, or by replacing a bare-page root.
    /// </summary>
    private async Task PresentModalPageAsync(Page page, IServiceScope scope)
    {
        var mainPage = GetMainPage();

        if (mainPage is null)
        {
            scope.Dispose();
            throw new InvalidOperationException(DefaultPageNotAvailable);
        }

        if (page is IShellView)
        {
            // MAUI's iOS/Mac Catalyst ShellFlyoutRenderer crashes while laying out a Shell that
            // is pushed as a modal (LayoutSidebar NullReferenceException). A shell is a root
            // surface, so install it as the window root instead of pushing it onto the modal
            // stack; this is also the path the application uses when rebuilding after a language
            // change.
            var window = Application.Current!.Windows[0];
            var oldPage = window.Page;

            if (ReferenceEquals(oldPage, page))
            {
                scope.Dispose();
                return;
            }

            // Detach any existing handler so MAUI reinitializes the native view tree for
            // recycled singleton pages.
            page.Handler = null;

            window.Page = page;
            await ReapplyWindowContentAsync(window);

            CleanupPageResources(oldPage);
            scope.Dispose();
        }
        else if (mainPage is Shell or NavigationPage || GetNavigationFrame() is not null)
        {
            // The root owns a content host (or is a Shell/NavigationPage), so a modal can be
            // pushed on top of it and the shell stays alive underneath.
            // Detach any existing handler so MAUI reinitializes the native view tree for
            // recycled singleton pages.
            page.Handler = null;

            await mainPage.Navigation.PushModalAsync(page, Animated);
            _modals.Push(new ModalPage(page, scope));
        }
        else
        {
            // Initial setup or bare-page root (e.g., LoadingView): replace the window root.
            var window = Application.Current!.Windows[0];
            var oldPage = window.Page;

            // Detach any existing handler so MAUI reinitializes the native view tree for
            // recycled singleton pages.
            page.Handler = null;

            window.Page = page;
            await ReapplyWindowContentAsync(window);

            CleanupPageResources(oldPage);
            scope.Dispose();
        }
    }

    /// <summary>
    /// Re-applies <see cref="Microsoft.Maui.IWindow.Content"/> to a window whose root page was
    /// just replaced, so the platform window hosts the new page.
    /// </summary>
    /// <remarks>
    /// MAUI 11 maps a window's root page under the <c>Content</c> key and updates that mapping from
    /// <c>Window.OnPropertyChanged("Page")</c>. On iOS and Mac Catalyst that automatic update does
    /// not attach the replacement page when the root is swapped after the window is live: the
    /// window keeps the previous page's view controller, and the new page stays detached (managed
    /// size -1x-1, its native tree never added — nothing is painted). Re-applying the mapping after
    /// the swap is what hands the new page's view controller to the window.
    /// </remarks>
    /// <param name="window">The window whose content mapping is re-applied.</param>
    /// <returns>A task that completes once the mapping has been re-applied.</returns>
    private static Task ReapplyWindowContentAsync(Microsoft.Maui.Controls.Window window)
    {
        if (window.Handler is null)
            return Task.CompletedTask;

        // The re-apply has to run on a later dispatcher turn: applying it inline races the tear-down
        // of the page that is being replaced, and the platform window keeps the old root.
        var completion = new TaskCompletionSource();

        window.Dispatcher.DispatchDelayed(TimeSpan.Zero, () =>
        {
            try
            {
                window.Handler?.UpdateValue(nameof(Microsoft.Maui.IWindow.Content));
            }
            finally
            {
                completion.TrySetResult();
            }
        });

        return completion.Task;
    }

    /// <summary>
    /// Initializes the modal view model, routing any failure through the exception handler.
    /// </summary>
    private async Task InitializeModalViewModelAsync(IView view)
    {
        if (view.ViewModel is null)
            return;

        try
        {
            await view.ViewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            try
            {
                _exceptionHandlerService.HandleException(ex);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing ViewModel: {ex.Message}");
            }
        }
    }

    // -----------------------------------------------------------------------
    // Override: Dispose
    // -----------------------------------------------------------------------
    public override void Dispose()
    {
        while (_modals.Count > 0)
        {
            try { _modals.Pop().Scope.Dispose(); }
            catch (Exception ex) { _logger.LogError(ex, "Error disposing a modal scope during shutdown"); }
        }

        base.Dispose();
    }

    // -----------------------------------------------------------------------
    // Obsolete: Typed-view navigation overloads (not supported in MAUI)
    // -----------------------------------------------------------------------

    // -----------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------

    private static Page? GetMainPage() =>
        (Application.Current?.Windows is { Count: > 0 } windows ? windows[0].Page : null);

    /// <summary>
    /// Clears MAUI-specific resources (BindingContext, Handler) on a popped modal page.
    /// Scope disposal is handled separately by the modal bookkeeping.
    /// </summary>
    private static void CleanupPageResources(Page? page)
    {
        if (page is null)
            return;

        bool isSingleton = page.GetType().GetCustomAttribute<LifetimeAttribute>()?.Lifetime == Lifetimes.Singleton;

        // Clear BindingContext to break the reference chain to the ViewModel.
        page.BindingContext = null;

        // Disconnect native handler. Skip for Singletons — they are reused.
        if (!isSingleton)
            page.Handler = null;
    }
}
