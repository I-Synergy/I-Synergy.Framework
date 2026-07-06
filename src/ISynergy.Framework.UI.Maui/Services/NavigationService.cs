using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
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
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : BaseNavigationService
{
    private const string DefaultPageNotAvailable = "Main page is not available.";
    private const bool Animated = true;

    /// <summary>
    /// Safety net: maps Page instances to their NavigationFrames so scopes
    /// can be cleaned up even when pages are popped by the platform back
    /// button (which bypasses <see cref="GoBackAsync"/>). Unlike the old
    /// <c>_pageScopes</c> table, this is secondary — the backstack is the
    /// primary scope owner. This table only defends against native stack
    /// manipulation.
    /// </summary>
    private static readonly ConditionalWeakTable<Page, NavigationFrame> _pageFrames = new();

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
    // Override: CanGoBack — checks MAUI's actual native navigation stacks
    // -----------------------------------------------------------------------
    public override bool CanGoBack =>
        GetMainPage() is Page page &&
        (page.GetNavigation().Navigation.NavigationStack.Count > 0 ||
         page.Navigation.ModalStack.Count > 0);

    // -----------------------------------------------------------------------
    // Abstract: GetNavigationFrame — not used (all navigation methods overridden)
    // -----------------------------------------------------------------------
    protected override object? GetNavigationFrame() => null;

    // -----------------------------------------------------------------------
    // Abstract: Frame content accessors — not used (MAUI push/pop model)
    // -----------------------------------------------------------------------
    protected override object? GetFrameContent(object frame)
        => throw new NotSupportedException("MAUI uses push/pop navigation, not frame content replacement.");

    protected override void SetFrameContent(object frame, IView page)
        => throw new NotSupportedException("MAUI uses push/pop navigation, not frame content replacement.");

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
    // Override: NavigateAsync<TViewModel> — MAUI push/pop model
    // -----------------------------------------------------------------------
    public override async Task NavigateAsync<TViewModel>(
        TViewModel viewModel, object? parameter = null, bool backNavigation = false)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        if (NavigationExtensions.CreatePage<TViewModel>(scopedProvider, viewModel, parameter) is { } view && view is Page page)
        {
            if (CultureInfo.GetCultureInfo(_localSettings.Language).TextInfo.IsRightToLeft)
                page.FlowDirection = FlowDirection.RightToLeft;

            var mainPage = GetMainPage();
            if (mainPage is null)
                throw new InvalidOperationException(DefaultPageNotAvailable);

            var result = mainPage.GetNavigation();

            if (result.Navigation.NavigationStack.Contains(page))
            {
                // Page already in the navigation stack — back-navigation scenario.
                // Pop intermediate pages and their corresponding NavigationFrames.
                var poppedPages = new List<Page>();
                for (int i = result.Navigation.NavigationStack.Count - 1; i >= 0; i--)
                {
                    if (result.Navigation.NavigationStack[i].Equals(page))
                        break;

                    if (!backNavigation)
                    {
                        var poppedPage = result.Navigation.NavigationStack[i];
                        await result.Navigation.PopAsync(Animated);
                        poppedPages.Add(poppedPage);
                    }
                }

                // Dispose frames for popped intermediate pages.
                // _currentFrame corresponds to the topmost page (first popped).
                // Remaining popped pages' frames are on _backStack.
                HandleCurrentFrame(backNavigation: true);

                // Pop and dispose additional frames for deeper intermediate pages.
                for (int j = 0; j < poppedPages.Count - 1; j++)
                {
                    if (_backStack.Count > 0)
                    {
                        var frame = _backStack.Pop();
                        frame.Dispose();
                    }
                }

                // Now _backStack top should be the target page's frame (or it was
                // already _currentFrame). If the target's frame is in _backStack,
                // restore it as _currentFrame.
                // Note: the target page itself was NOT popped — its frame must
                // survive in _backStack or be _currentFrame.
                // Since HandleCurrentFrame disposed _currentFrame and the extra
                // popped frames were removed from _backStack, we need to find
                // and restore the target's frame.
                // Actually, the target's frame remains in _backStack (it was never
                // popped from _backStack because it wasn't popped from MAUI stack).
                // We just need to ensure _currentFrame is set to it.
                // But _currentFrame was null after HandleCurrentFrame.
                // Find and restore the target's frame.
                if (TryRemoveFromBackStack<TViewModel>(out var targetFrame))
                {
                    _currentFrame = targetFrame;
                }

                // Dispose the scope created for this navigation. The target
                // page's existing frame (restored above) has its own preserved
                // scope — we don't need this one.
                scope.Dispose();

                // Clean up MAUI resources for popped pages
                foreach (var poppedPage in poppedPages)
                    CleanupPageResources(poppedPage);
            }
            else
            {
                // Forward navigation: create NavigationFrame, push to MAUI stack.
                NavigationFrame newFrame;

                if (viewModel is not null)
                {
                    // Caller provided a pre-constructed ViewModel — do NOT resolve
                    // from DI. The ViewModel may have constructor parameters (e.g.
                    // data models) that aren't registered in the container.
                    newFrame = new NavigationFrame
                    {
                        Scope = scope,
                        ViewModelType = typeof(TViewModel),
                        Parameter = parameter
                    };
                }
                else if (TryRemoveFromBackStack<TViewModel>(out var backStackFrame))
                {
                    // Reuse existing frame from backstack — ViewModel is already
                    // cached in the frame's scope from the original navigation.
                    backStackFrame.Parameter = parameter;
                    newFrame = backStackFrame;
                }
                else
                {
                    // Fresh frame — CreatePage below resolves the ViewModel into
                    // this scope when viewModel is null. We don't pre-resolve here
                    // because the ViewModel may have constructor parameters not
                    // registered in DI (e.g. data models passed via the thin
                    // overload through a factory or caller-provided instance).
                    newFrame = new NavigationFrame
                    {
                        Scope = scope,
                        ViewModelType = typeof(TViewModel),
                        Parameter = parameter
                    };
                }

                if (parameter is not null)
                    newFrame.Parameter = parameter;

                HandleCurrentFrame(backNavigation: false);
                _currentFrame = newFrame;

                // Register page→frame mapping so the safety net can clean up
                // if the platform back button pops this page without going
                // through GoBackAsync. Remove-then-add avoids duplicate-key
                // errors when the same singleton page is reused.
                _pageFrames.Remove(page);
                _pageFrames.Add(page, newFrame);

                page.Parent = null;
                await result.Navigation.PushAsync(page, Animated);
            }

            if (view is not null && view.ViewModel is not null)
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
                        System.Diagnostics.Debug.WriteLine("Error initializing ViewModel: {0}", ex.Message);
                    }
                }
            }

            OnBackStackChanged(EventArgs.Empty);
        }
        else
        {
            scope.Dispose();
        }
    }

    // -----------------------------------------------------------------------
    // Override: GoBackAsync
    // -----------------------------------------------------------------------
    public override async Task GoBackAsync()
    {
        var mainPage = GetMainPage();
        if (mainPage is null)
            throw new InvalidOperationException(DefaultPageNotAvailable);

        if (CanGoBack)
        {
            Page? poppedPage = null;

            if (mainPage.Navigation.ModalStack.Count > 0)
            {
                poppedPage = mainPage.Navigation.ModalStack[^1];
                await mainPage.Navigation.PopModalAsync(Animated);
            }
            else
            {
                var navigation = mainPage.GetNavigation().Navigation;
                if (navigation.NavigationStack.Count > 0)
                {
                    poppedPage = navigation.NavigationStack[^1];
                    await navigation.PopAsync(Animated);
                }
            }

            // Dispose current NavigationFrame (its scope owns the popped ViewModel)
            HandleCurrentFrame(backNavigation: true);

            // Restore previous frame from backstack
            if (_backStack.Count > 0)
            {
                _currentFrame = _backStack.Pop();
            }

            // Clean up MAUI-specific resources on the popped page
            CleanupPageResources(poppedPage);
        }

        OnBackStackChanged(EventArgs.Empty);
    }

    // -----------------------------------------------------------------------
    // Override: NavigateModalAsync
    // -----------------------------------------------------------------------
    public override async Task NavigateModalAsync<TViewModel>(object? parameter = null)
    {
        if (Application.Current is not null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var oldScope = _modalScope;
                _modalScope = _serviceScopeFactory.CreateScope();
                var scopedProvider = _modalScope.ServiceProvider;

                if (NavigationExtensions.CreatePage<TViewModel>(scopedProvider, parameter: parameter) is { } view && view is Page page)
                {
                    if (page is TabbedPage)
                        page.Handler = null;

                    var mainPage = GetMainPage();
                    if (mainPage is null)
                        throw new InvalidOperationException(DefaultPageNotAvailable);

                    var oldPage = Application.Current.Windows[0].Page;
                    Application.Current.Windows[0].Page = page;

                    CleanupPageResources(oldPage);
                    oldScope?.Dispose();

                    if (view is not null && view.ViewModel is not null)
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
                }
                else
                {
                    _modalScope?.Dispose();
                    _modalScope = null;
                    oldScope?.Dispose();
                }
            });
        }
    }

    // -----------------------------------------------------------------------
    // Override: CleanBackStackAsync
    // -----------------------------------------------------------------------
    public override async Task CleanBackStackAsync(bool suppressEvent = false)
    {
        var mainPage = GetMainPage();
        if (mainPage is null)
            throw new InvalidOperationException(DefaultPageNotAvailable);

        await mainPage.GetNavigation().Navigation.PopToRootAsync(Animated);

        foreach (var frame in _backStack)
        {
            try { frame.Dispose(); }
            catch (Exception ex) { _logger.LogError(ex, "Error disposing backstack frame during cleanup"); }
        }
        _backStack.Clear();

        if (!suppressEvent)
        {
            try
            {
                OnBackStackChanged(EventArgs.Empty);
            }
            catch (ObjectDisposedException)
            {
                _backStackChanged = null;
            }
        }
    }

    // -----------------------------------------------------------------------
    // Override: Dispose
    // -----------------------------------------------------------------------
    public override void Dispose()
    {
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
    /// Clears MAUI-specific resources (BindingContext, Handler) on a popped or
    /// replaced page. Scope disposal is handled separately by the
    /// NavigationFrame backstack.
    /// </summary>
    private static void CleanupPageResources(Page? page)
    {
        if (page is null)
            return;

        // Remove the page→frame mapping. If the frame hasn't been disposed
        // yet (e.g. the platform back button popped the page without going
        // through GoBackAsync), dispose it now.
        if (_pageFrames.TryGetValue(page, out var frame))
        {
            try { frame.Dispose(); }
            catch (Exception) { /* best-effort */ }
            _pageFrames.Remove(page);
        }

        bool isSingleton = page.GetType().GetCustomAttribute<LifetimeAttribute>()?.Lifetime == Lifetimes.Singleton;

        // Clear BindingContext to break the reference chain to the ViewModel.
        page.BindingContext = null;

        // Disconnect native handler. Skip for Singletons — they are reused.
        if (!isSingleton)
            page.Handler = null;
    }
}
