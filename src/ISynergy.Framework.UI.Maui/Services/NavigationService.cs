using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : INavigationService
{
    private const string DefaultPageNotAvailable = "Main page is not available.";

    private readonly ILocalSettings _localSettings;
    private readonly IExceptionHandlerService _exceptionHandlerService;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private const bool Animated = true;

    /// <summary>
    /// Scope that owns the current modal page's ViewModel.
    /// Disposed on next modal navigation or service disposal.
    /// </summary>
    private IServiceScope? _modalScope;

    /// <summary>
    /// Tracks scopes for pages pushed onto the navigation stack.
    /// When a page is GC'd or cleaned up via <see cref="CleanupPage"/>,
    /// its scope is disposed.
    /// </summary>
    private static readonly ConditionalWeakTable<Page, IServiceScope> _pageScopes = new();

    public event EventHandler? BackStackChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="localSettings"></param>
    /// <param name="exceptionHandlerService"></param>
    /// <param name="serviceScopeFactory">Factory for creating scoped service providers per navigation.</param>
    /// <param name="logger"></param>
    public NavigationService(
        ILocalSettings localSettings,
        IExceptionHandlerService exceptionHandlerService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NavigationService> logger)
    {
        _logger = logger;
        _logger.LogTrace("NavigationService instance created with ID: {InstanceId}", Guid.NewGuid());

        _localSettings = localSettings ?? throw new ArgumentNullException(nameof(localSettings));
        _exceptionHandlerService = exceptionHandlerService ?? throw new ArgumentNullException(nameof(exceptionHandlerService));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    /// <summary>
    /// Handles the <see cref="E:BackStackChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public virtual void OnBackStackChanged(EventArgs e) => BackStackChanged?.Invoke(this, e);

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// </summary>
    /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
    private static Page? GetMainPage() =>
        Application.Current?.Windows.FirstOrDefault()?.Page;

    public bool CanGoBack =>
        GetMainPage() is Page page && (page.GetNavigation().Navigation.NavigationStack.Count > 0 || page.Navigation.ModalStack.Count > 0);


    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel =>
        NavigateAsync<TViewModel>(null, parameter, backNavigation);

    /// <summary>
    /// Navigates to the viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public async Task NavigateAsync<TViewModel>(TViewModel? viewModel, object? parameter = null, bool backNavigation = false) // NOSONAR
        where TViewModel : class, IViewModel
    {
        // Create a scoped provider so ViewModels are released when the page
        // leaves the navigation stack, rather than being tracked by the root container.
        var scope = _serviceScopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        if (NavigationExtensions.CreatePage<TViewModel>(scopedProvider, viewModel, parameter) is { } view && view is Page page)
        {
            // Track the scope for later cleanup
            _pageScopes.Add(page, scope);

            if (CultureInfo.GetCultureInfo(_localSettings.Language).TextInfo.IsRightToLeft)
                page.FlowDirection = FlowDirection.RightToLeft;

            var mainPage = GetMainPage();

            if (mainPage is null)
                throw new InvalidOperationException(DefaultPageNotAvailable);

            var result = mainPage.GetNavigation();

            if (result.Navigation.NavigationStack.Contains(page))
            {
                for (int i = result.Navigation.NavigationStack.Count - 1; i >= 0; i--)
                {
                    if (result.Navigation.NavigationStack[i].Equals(page))
                        break;

                    if (!backNavigation)
                        await result.Navigation.PopAsync(Animated);
                }
            }
            else
            {
                page.Parent = null;
                await result.Navigation.PushAsync(page, Animated);
            }

            if (view is not null && view.ViewModel is not null) // NOSONAR
            {
                try
                {
                    await view.ViewModel.InitializeAsync();
                }
                catch (Exception ex)
                {
                    // Use exception handler service instead of just logging
                    try
                    {
                        _exceptionHandlerService?.HandleException(ex);
                    }
                    catch
                    {
                        // If we can't get the exception handler service, log to debug
                        System.Diagnostics.Debug.WriteLine("Error initializing ViewModel: {0}", ex.Message);
                    }
                }
            }

            OnBackStackChanged(EventArgs.Empty);
        }
        else
        {
            // Page creation failed — dispose the unused scope
            scope.Dispose();
        }
    }

    /// <summary>
    /// Navigates to the modal viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public async Task NavigateModalAsync<TViewModel>(object? parameter = null) // NOSONAR
        where TViewModel : class, IViewModel
    {
        if (Application.Current is not null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                // Dispose previous modal scope and create a fresh one.
                // This mirrors the BaseNavigationService pattern used by WinUI/WPF.
                _modalScope?.Dispose();
                _modalScope = _serviceScopeFactory.CreateScope();
                var scopedProvider = _modalScope.ServiceProvider;

                if (NavigationExtensions.CreatePage<TViewModel>(scopedProvider, parameter: parameter) is { } view && view is Page page)
                {
                    // Added this nullification of handler because of some issues with TabbedPage.
                    // When tabbed page is set as main page and then modal page is opened, then after closing and reopening the page, the tabs are not visible.
                    page.Handler = null;

                    var mainPage = GetMainPage();

                    if (mainPage is null)
                        throw new InvalidOperationException(DefaultPageNotAvailable);

                    // Capture the old root page BEFORE replacement
                    var oldPage = Application.Current.Windows[0].Page;

                    // Assign the new page
                    Application.Current.Windows[0].Page = page;

                    // Clean up the old page graph to release event subscriptions and native resources
                    CleanupPage(oldPage);

                    if (view is not null && view.ViewModel is not null) // NOSONAR
                    {
                        try
                        {
                            await view.ViewModel.InitializeAsync();
                        }
                        catch (Exception ex)
                        {
                            // Use exception handler service instead of just logging
                            try
                            {
                                _exceptionHandlerService?.HandleException(ex);
                            }
                            catch
                            {
                                // If we can't get the exception handler service, log to debug
                                System.Diagnostics.Debug.WriteLine($"Error initializing ViewModel: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    // Page creation failed — dispose the unused scope
                    _modalScope?.Dispose();
                    _modalScope = null;
                }
            });
        }
    }

    public async Task CleanBackStackAsync(bool _ = false)
    {
        var mainPage = GetMainPage();

        if (mainPage is null)
            throw new InvalidOperationException("Main page is not available.");

        await mainPage.GetNavigation().Navigation.PopToRootAsync(Animated);

        OnBackStackChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Goes the back.
    /// </summary>
    public async Task GoBackAsync()
    {
        var mainPage = GetMainPage();

        if (mainPage is null)
            throw new InvalidOperationException("Main page is not available.");

        if (CanGoBack)
        {
            if (mainPage.Navigation.ModalStack.Count > 0)
                await mainPage.Navigation.PopModalAsync(Animated);
            else
                await mainPage.GetNavigation().Navigation.PopAsync(Animated);
        }

        OnBackStackChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Cleans up a page's ViewModel, DI scope, BindingContext, and native handler to
    /// release event subscriptions and native resources when the root page
    /// is replaced.
    /// </summary>
    private static void CleanupPage(Page? page)
    {
        if (page is null)
            return;

        // 1. Dispose the ViewModel first — this unsubscribes from singleton service
        //    events like NavigationService.BackStackChanged (BaseShellViewModel.Dispose)
        //    and MessengerService (ViewModel.Dispose).
        if (page is IView view && view.ViewModel is IDisposable disposableViewModel)
        {
            disposableViewModel.Dispose();
        }

        // 2. Clear BindingContext directly on the Page to break the reference chain.
        //    We use BindingContext = null (not IView.ViewModel setter) because
        //    ShellView.ViewModel setter calls SetBinding(...) which would fail
        //    on a detached page.
        page.BindingContext = null;

        // 3. Disconnect the handler to release native platform views.
        //    Mirrors the existing pattern at the TabbedPage workaround above
        //    and LoadingView.cs cleanup.
        page.Handler = null;

        // 4. Dispose the page's DI scope if one was created during navigation.
        //    This releases scoped services (ViewModel, etc.) that were resolved
        //    from a child scope rather than the root container.
        if (_pageScopes.TryGetValue(page, out var pageScope))
        {
            pageScope.Dispose();
            _pageScopes.Remove(page);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing,
    /// or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _modalScope?.Dispose();
        _modalScope = null;

        GC.SuppressFinalize(this);
    }

    #region NotImplemented
    [Obsolete("Not supported!", true)] // NOSONAR
    public Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false) // NOSONAR
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)] // NOSONAR
    public Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false) // NOSONAR
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)] // NOSONAR
    public Task GoForwardAsync() => throw new NotImplementedException(); // NOSONAR
    #endregion
}
