using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
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
public class NavigationService : BaseNavigationService
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
        => Application.Current?.Windows.FirstOrDefault()?.Page;

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

            // Detach any existing handler so MAUI reinitializes the native view tree for
            // recycled singleton pages.
            page.Handler = null;

            var mainPage = GetMainPage();

            if (mainPage is null)
            {
                scope.Dispose();
                throw new InvalidOperationException(DefaultPageNotAvailable);
            }

            if (mainPage is Shell or NavigationPage || GetNavigationFrame() is not null)
            {
                // The root owns a content host (or is a Shell/NavigationPage), so a modal can be
                // pushed on top of it and the shell stays alive underneath.
                await mainPage.Navigation.PushModalAsync(page, Animated);
                _modals.Push(new ModalPage(page, scope));
            }
            else
            {
                // Initial setup or bare-page root (e.g., LoadingView): replace the window root.
                var oldPage = Application.Current.Windows[0].Page;
                Application.Current.Windows[0].Page = page;

                CleanupPageResources(oldPage);
                scope.Dispose();
            }

            if (view.ViewModel is not null)
            {
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
        });
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
    [Obsolete("Not supported!", true)]
    public new Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
        => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public new Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
        => throw new NotImplementedException();

    // -----------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------

    private static Page? GetMainPage() =>
        Application.Current?.Windows.FirstOrDefault()?.Page;

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
