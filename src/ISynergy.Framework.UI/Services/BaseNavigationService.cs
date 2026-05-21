using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Abstract base class for platform-specific NavigationService implementations.
/// Contains all shared navigation logic and defines abstract methods for
/// platform-specific window/frame access, view creation, and UI threading operations.
/// </summary>
/// <seealso cref="INavigationService" />
public abstract class BaseNavigationService : INavigationService
{
    /// <summary>
    /// The scoped context service for resolving services.
    /// </summary>
    protected readonly IScopedContextService _scopedContextService;

    /// <summary>
    /// The exception handler service for handling exceptions during navigation.
    /// </summary>
    protected readonly IExceptionHandlerService _exceptionHandlerService;

    /// <summary>
    /// The common services for accessing shared application services.
    /// </summary>
    protected readonly ICommonServices _commonServices;

    /// <summary>
    /// The logger for diagnostic output.
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Backs the stack changed event field.
    /// Made protected to allow WPF subclass to clear on modal navigation (preserving existing behavior).
    /// </summary>
    protected EventHandler? _backStackChanged;

    /// <summary>
    /// A navigation frame binds a ViewModel to its owning child scope.
    /// When the frame is evicted, the scope is disposed, which cascades
    /// to the ViewModel and all scoped dependencies resolved within that frame.
    /// </summary>
    private sealed class NavigationFrame : IDisposable
    {
        public IServiceScope Scope { get; init; } = null!;
        public Type ViewModelType { get; init; } = null!;
        public object? Parameter { get; set; }

        public void Dispose() => Scope.Dispose();
    }

    /// <summary>
    /// Navigation backstack. Each entry holds a child scope that owns
    /// exactly one ViewModel instance (registered as scoped).
    /// </summary>
    private Stack<NavigationFrame> _backStack = new Stack<NavigationFrame>();

    /// <summary>
    /// The frame currently displayed in the navigation host.
    /// </summary>
    private NavigationFrame? _currentFrame;

    /// <summary>
    /// Maximum size for the backstack before disposing oldest entries.
    /// </summary>
    private const int MaxBackstackSize = 10;

    /// <summary>
    /// Gets the root service provider for creating child scopes.
    /// </summary>
    private IServiceProvider RootProvider => _scopedContextService.ServiceProvider;

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// </summary>
    /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
    public bool CanGoBack => _backStack.Count > 0;

    #region Abstract Platform-Specific Methods

    /// <summary>
    /// Gets the navigation Frame from the current window's visual tree.
    /// </summary>
    /// <returns>The platform-specific Frame object, or null if not available.</returns>
    protected abstract object? GetNavigationFrame();

    /// <summary>
    /// Gets the current content of the specified navigation frame.
    /// </summary>
    /// <param name="frame">The platform-specific Frame object.</param>
    /// <returns>The current frame content as an object, or null.</returns>
    protected abstract object? GetFrameContent(object frame);

    /// <summary>
    /// Sets the content of the specified navigation frame.
    /// </summary>
    /// <param name="frame">The platform-specific Frame object.</param>
    /// <param name="page">The view to set as frame content.</param>
    protected abstract void SetFrameContent(object frame, IView page);

    /// <summary>
    /// Gets the main window content directly (used for modal navigation).
    /// </summary>
    /// <returns>The current window content as an object, or null.</returns>
    protected abstract object? GetWindowContent();

    /// <summary>
    /// Sets the main window content directly (used for modal navigation).
    /// </summary>
    /// <param name="page">The view to set as window content.</param>
    protected abstract void SetWindowContent(IView page);

    /// <summary>
    /// Creates an <see cref="IView"/> instance for the specified ViewModel type,
    /// optionally associating an existing ViewModel instance and navigation parameter.
    /// </summary>
    /// <param name="viewModelType">The type of the ViewModel to create a view for.</param>
    /// <param name="viewModel">An existing ViewModel instance, or null to resolve from DI.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <returns>The created <see cref="IView"/> instance.</returns>
    protected abstract IView CreateView(IServiceProvider serviceProvider, Type viewModelType, IViewModel? viewModel, object? parameter);

    /// <summary>
    /// Subscribes to the platform-specific Loaded event (if not already loaded)
    /// and returns a task that completes when the view is loaded or the timeout expires.
    /// Must be called before ViewModel initialization to ensure the subscription
    /// is in place before any InitializeAsync-triggered loading occurs.
    /// </summary>
    /// <param name="view">The view to monitor.</param>
    /// <param name="timeoutMs">The timeout in milliseconds.</param>
    /// <returns>A task that completes when the view is loaded or the timeout expires.</returns>
    protected abstract Task MonitorViewLoadAsync(IView view, int timeoutMs = 500);

    /// <summary>
    /// Dispatches the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to dispatch.</param>
    protected abstract void DispatchOnUIThread(Action action);

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNavigationService"/> class.
    /// </summary>
    /// <param name="scopedContextService">The scoped context service.</param>
    /// <param name="exceptionHandlerService">The exception handler service.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    protected BaseNavigationService(
        IScopedContextService scopedContextService,
        IExceptionHandlerService exceptionHandlerService,
        ICommonServices commonServices,
        ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogTrace("BaseNavigationService instance created with ID: {InstanceId}", Guid.NewGuid());

        _exceptionHandlerService = exceptionHandlerService ?? throw new ArgumentNullException(nameof(exceptionHandlerService));
        _scopedContextService = scopedContextService ?? throw new ArgumentNullException(nameof(scopedContextService));
        _commonServices = commonServices ?? throw new ArgumentNullException(nameof(commonServices));
    }

    #endregion

    #region BackStack Event

    public event EventHandler? BackStackChanged
    {
        add
        {
            // Only add if not already subscribed
            if (_backStackChanged is null || !_backStackChanged.GetInvocationList().Contains(value))
                _backStackChanged += value;
        }
        remove
        {
            _backStackChanged -= value;
        }
    }

    /// <summary>
    /// Handles the <see cref="E:BackStackChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public virtual void OnBackStackChanged(EventArgs e)
    {
        // Create a copy to avoid modification during iteration
        var handlers = _backStackChanged?.GetInvocationList();
        if (handlers is not null)
        {
            foreach (EventHandler handler in handlers)
            {
                try
                {
                    handler(this, e);
                }
                catch (ObjectDisposedException)
                {
                    // Remove disposed subscribers
                    _backStackChanged -= handler;
                }
            }
        }
    }

    #endregion

    #region ViewModel Lifecycle Helpers

    /// <summary>
    /// Safely calls OnNavigatedTo with exception handling.
    /// </summary>
    protected void SafeOnNavigatedTo(IViewModel viewModel)
    {
        try
        {
            viewModel.OnNavigatedTo();
        }
        catch (Exception ex)
        {
            _exceptionHandlerService.HandleException(ex);
        }
    }

    /// <summary>
    /// Safely calls OnNavigatedFrom with exception handling.
    /// </summary>
    protected void SafeOnNavigatedFrom(IViewModel viewModel)
    {
        try
        {
            viewModel.OnNavigatedFrom();
        }
        catch (Exception ex)
        {
            _exceptionHandlerService.HandleException(ex);
        }
    }

    #endregion

    #region Command Checking

    /// <summary>
    /// Checks if a ViewModel has any running commands.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="ICommandProvider"/> when available (AOT-safe).
    /// Falls back to property reflection when the ViewModel does not implement <see cref="ICommandProvider"/>.
    /// The reflection fallback is decorated with <see cref="RequiresUnreferencedCodeAttribute"/>
    /// to surface a warning at call sites when publishing with trimming enabled.
    /// </remarks>
    private bool HasRunningCommands(IViewModel viewModel)
    {
        if (viewModel is null)
            return false;

        // AOT-safe path: ViewModel explicitly exposes its commands via ICommandProvider.
        if (viewModel is ICommandProvider provider)
            return provider.GetCommands().OfType<IAsyncRelayCommand>().Any(c => c.IsRunning);

        // Reflection fallback — not AOT-safe, but preserved for backwards compatibility.
        return HasRunningCommandsViaReflection(viewModel);
    }

    /// <summary>
    /// Reflection-based fallback for <see cref="HasRunningCommands"/> used when the ViewModel
    /// does not implement <see cref="ICommandProvider"/>.
    /// Not compatible with NativeAOT or aggressive trimming.
    /// </summary>
    private static bool HasRunningCommandsViaReflection(IViewModel viewModel)
    {
        var commandProperties = viewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(IAsyncRelayCommand).IsAssignableFrom(p.PropertyType));

        foreach (var property in commandProperties)
        {
            if (property.GetValue(viewModel) is IAsyncRelayCommand command && command.IsRunning)
                return true;
        }

        return false;
    }

    #endregion

    #region Navigation Helpers

    /// <summary>
    /// Handles the current navigation frame before navigating away.
    /// <list type="bullet">
    /// <item><description>Forward navigation: pushes the current frame onto the backstack, trims if over capacity.</description></item>
    /// <item><description>Back navigation: disposes the current frame (scope + ViewModel + scoped dependencies).</description></item>
    /// </list>
    /// </summary>
    private void HandleCurrentFrame(bool backNavigation)
    {
        if (_currentFrame is null)
            return;

        // Notify the current ViewModel it's being navigated away from
        if (_currentFrame.Scope.ServiceProvider.GetService(_currentFrame.ViewModelType) is IViewModel currentViewModel)
        {
            currentViewModel.CancelAllCommands();
            SafeOnNavigatedFrom(currentViewModel);
            currentViewModel.Cleanup(isClosing: false);
        }

        if (!backNavigation)
        {
            // Store the current frame on the backstack
            _backStack.Push(_currentFrame);
            OnBackStackChanged(EventArgs.Empty);

            // Trim backstack if over capacity — dispose oldest frames
            while (_backStack.Count > MaxBackstackSize)
            {
                var oldest = _backStack.ToArray().Last();
                _backStack = new Stack<NavigationFrame>(_backStack.Where(f => f != oldest).Reverse());
                try { oldest.Dispose(); }
                catch (Exception ex) { _logger.LogError(ex, "Error disposing oldest backstack frame"); }
            }
        }
        else
        {
            // Back navigation: the current frame is being permanently evicted
            try { _currentFrame.Dispose(); }
            catch (Exception ex) { _logger.LogError(ex, "Error disposing frame during back navigation"); }
        }

        _currentFrame = null;
    }

    /// <summary>
    /// Common logic for initializing a view and its ViewModel.
    /// Resolves M-02: actually called from navigation paths instead of being dead code.
    /// Resolves M-03: uses _exceptionHandlerService field instead of ServiceLocator.
    /// When the ViewModel is already initialized, still calls SafeOnNavigatedTo
    /// to preserve the original behavior of notifying the ViewModel on every navigation.
    /// </summary>
    private async Task InitializeViewAndViewModel(IView page, bool backNavigation)
    {
        if (page.ViewModel is not null)
        {
            if (!page.ViewModel.IsInitialized)
            {
                try
                {
                    // Wait for the view to finish loading before initializing the ViewModel.
                    // This ensures InitializeAsync runs on the UI thread, avoiding
                    // COMExceptions from data-bound ObservableCollection changes.
                    await MonitorViewLoadAsync(page);

                    await page.ViewModel.InitializeAsync();

                    page.ViewModel.OnNavigatedTo();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initializing ViewModel for {ViewModelType}", page.ViewModel.GetType().Name);

                    bool handled = false;
                    try
                    {
                        _exceptionHandlerService.HandleException(ex);
                        handled = true;
                    }
                    catch (Exception handlerEx)
                    {
                        // Log failure of exception handler but suppress original exception to prevent app crash
                        _logger.LogError(handlerEx, "Exception handler failed while processing navigation initialization error");
                    }

                    // Suppress exception if it was successfully handled or if handler is not available
                    // This prevents app crashes while still allowing the handler to show user-friendly messages
                    if (!handled)
                    {
                        _logger.LogWarning("Exception handler service not available for navigation initialization error");
                    }
                }
            }
            else
            {
                // ViewModel already initialized, still notify it's being navigated to
                SafeOnNavigatedTo(page.ViewModel);
            }
        }
    }

    #endregion

    #region Blade Navigation

    /// <summary>
    /// Gets a navigation blade for the specified ViewModel.
    /// Platform-specific subclasses can override to customize view type resolution.
    /// </summary>
    protected virtual async Task<IView> GetNavigationBladeAsync(IViewModel viewModel)
    {
        var view = viewModel.GetRelatedView();
        var viewType = view.GetRelatedViewType();

        if (viewType is not null && _scopedContextService.GetRequiredService(viewType) is IView resolvedPage)
        {
            resolvedPage.ViewModel = viewModel;

            // Subscribe to Loaded event BEFORE initialization (preserves original order)
            var loadTask = MonitorViewLoadAsync(resolvedPage);

            // Initialize the ViewModel first
            if (!viewModel.IsInitialized)
                await viewModel.InitializeAsync();

            // Wait for the view to finish loading (with timeout)
            await loadTask;

            // Call OnNavigatedTo for the blade ViewModel
            SafeOnNavigatedTo(viewModel);

            return resolvedPage;
        }

        throw new InvalidOperationException($"Instance create or navigate to page: {view}.");
    }

    /// <summary>
    /// Common blade setup logic.
    /// </summary>
    private async Task<IView?> SetupBladeAsync(IViewModelBladeView owner, IViewModelBlade bladeVm, Func<Task<IView?>> getViewFunc)
    {
        // Check if owner ViewModel can navigate away (if it has running commands)
        if (owner is IViewModel ownerVm && HasRunningCommands(ownerVm))
        {
            ownerVm.CancelAllCommands();
            SafeOnNavigatedFrom(ownerVm);
        }

        bladeVm.Owner = owner;

        void Viewmodel_Closed(object? sender, EventArgs e)
        {
            if (sender is IViewModelBlade viewModel)
            {
                viewModel.Closed -= Viewmodel_Closed;
                RemoveBlade(viewModel.Owner, viewModel);
            }
        }

        bladeVm.Closed += Viewmodel_Closed;

        try
        {
            // Get the view
            var view = await getViewFunc();

            // If view creation failed (returned null), cleanup the handler
            if (view == null)
            {
                bladeVm.Closed -= Viewmodel_Closed;
            }

            return view;
        }
        catch
        {
            // If view creation threw an exception, cleanup the handler
            bladeVm.Closed -= Viewmodel_Closed;
            throw;
        }
    }

    /// <summary>
    /// Common logic for adding a blade to the owner.
    /// </summary>
    private void AddBladeToOwner(IViewModelBladeView owner, IView view)
    {
        if (owner.Blades is not null && !owner.Blades.Any(a => a.GetType().FullName!.Equals(view.GetType().FullName)))
        {
            foreach (var blade in owner.Blades.EnsureNotNull())
            {
                // Notify existing blades they're being backgrounded
                if (blade.ViewModel is IViewModel existingVm)
                    SafeOnNavigatedFrom(existingVm);

                blade.IsEnabled = false;
            }

            owner.Blades.Add(view);
        }

        owner.IsPaneVisible = true;
    }

    /// <summary>
    /// Opens a blade as an asynchronous operation.
    /// </summary>
    public async Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel)
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
            var view = await SetupBladeAsync(owner, bladeVm, async () => await GetNavigationBladeAsync(bladeVm));

            if (view != null)
                AddBladeToOwner(owner, view);
        }
    }

    /// <summary>
    /// Opens a blade with a custom defined view.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "GetRequiredService(typeof(TView)) is called with a compile-time generic type argument. The type is preserved through DI registration.")]
    public async Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel)
        where TView : IView
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
            var view = await SetupBladeAsync(owner, bladeVm, async () =>
            {
                if (_scopedContextService.GetRequiredService(typeof(TView)) is IView view)
                {
                    view.ViewModel = viewmodel;

                    // Subscribe to Loaded event BEFORE initialization (preserves original order)
                    var loadTask = MonitorViewLoadAsync(view);

                    // Initialize the ViewModel
                    if (!viewmodel.IsInitialized)
                        await viewmodel.InitializeAsync();

                    // Wait for the view to finish loading (with timeout)
                    await loadTask;

                    // Call OnNavigatedTo for the blade ViewModel
                    SafeOnNavigatedTo(viewmodel);

                    return view;
                }

                throw new KeyNotFoundException($"Page not found: {typeof(TView)}.");
            });

            if (view != null)
                AddBladeToOwner(owner, view);
        }
    }

    /// <summary>
    /// Removes a blade.
    /// </summary>
    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
            // Notify the blade ViewModel it's being navigated from
            SafeOnNavigatedFrom(viewmodel);

            if (owner.Blades is not null)
            {
                var bladeToRemove = owner.Blades
                    .FirstOrDefault(q =>
                        q.ViewModel == bladeVm &&
                        ((IViewModelBlade)q.ViewModel).Owner == bladeVm.Owner);

                if (bladeToRemove is not null && owner.Blades.Remove(bladeToRemove))
                {
                    if (owner.Blades.Count < 1)
                        owner.IsPaneVisible = false;
                    else if (owner.Blades[owner.Blades.Count - 1] is { } blade)
                    {
                        blade.IsEnabled = true;

                        // Notify the now-active blade it's being navigated to
                        if (blade.ViewModel is IViewModel activeVm)
                            SafeOnNavigatedTo(activeVm);
                    }
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Searches the backstack for a frame whose ViewModel type matches <typeparamref name="TViewModel"/>.
    /// If found, the frame is removed from the backstack and returned.
    /// The ViewModel is already cached in the frame's scope (scoped registration).
    /// </summary>
    private bool TryRemoveFromBackStack<TViewModel>([NotNullWhen(true)] out NavigationFrame? frame)
        where TViewModel : class, IViewModel
    {
        var match = _backStack.FirstOrDefault(f => f.ViewModelType == typeof(TViewModel));
        if (match is not null)
        {
            _backStack = new Stack<NavigationFrame>(_backStack.Where(f => f != match).Reverse());
            frame = match;
            return true;
        }

        frame = null;
        return false;
    }

    #region Back Navigation

    /// <summary>
    /// Goes back to the previous ViewModel in the navigation stack.
    /// </summary>
    public async Task GoBackAsync()
    {
        if (!CanGoBack)
            return;

        // Dispose the current frame — ViewModel + scoped dependencies disposed
        HandleCurrentFrame(backNavigation: true);

        // Pop the previous frame from the backstack
        var previousFrame = _backStack.Pop();
        _currentFrame = previousFrame;

        if (!CanGoBack)
            OnBackStackChanged(EventArgs.Empty);

        // ViewModel is cached in the frame's scope; force reinitialization
        var viewModel = (IViewModel)previousFrame.Scope.ServiceProvider.GetRequiredService(previousFrame.ViewModelType);
        viewModel.IsInitialized = false;

        // Show the previous frame
        var frame = GetNavigationFrame();
        if (frame is not null)
        {
            var page = CreateView(previousFrame.Scope.ServiceProvider, previousFrame.ViewModelType, viewModel, previousFrame.Parameter);
            SetFrameContent(frame, page);
            await InitializeViewAndViewModel(page, backNavigation: true);
        }
    }

    /// <summary>
    /// Cleans up ViewModels in the backstack.
    /// </summary>
    public Task CleanBackStackAsync(bool suppressEvent = false)
    {
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

        return Task.CompletedTask;
    }

    #endregion

    #region Navigation

    /// <summary>
    /// Navigates to a specified ViewModel as an asynchronous operation.
    /// </summary>
    public Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel =>
        NavigateAsync(default(TViewModel)!, parameter, backNavigation);

    /// <summary>
    /// Navigates to a specified ViewModel as an asynchronous operation.
    /// </summary>
    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        var hostFrame = GetNavigationFrame();
        if (hostFrame is null)
            return;

        // Check if the requested type is already displayed
        if (_currentFrame is not null && _currentFrame.ViewModelType == typeof(TViewModel) && viewModel is null)
        {
            var currentVm = _currentFrame.Scope.ServiceProvider.GetRequiredService<TViewModel>();
            if (parameter is not null)
                currentVm.Parameter = parameter;
            SafeOnNavigatedTo(currentVm);
            return;
        }

        NavigationFrame? newFrame;

        if (viewModel is not null)
        {
            // Caller provided a specific ViewModel instance — wrap it in a new scope
            var scope = RootProvider.CreateScope();
            // The provided ViewModel is already resolved; register it in the new scope would be complex.
            // Instead, create a new scope and resolve a fresh instance.
            scope.ServiceProvider.GetRequiredService<TViewModel>();
            newFrame = new NavigationFrame { Scope = scope, ViewModelType = typeof(TViewModel), Parameter = parameter };
        }
        else if (TryRemoveFromBackStack<TViewModel>(out var backStackFrame))
        {
            // Reuse existing frame from backstack (ViewModel cached in its scope)
            backStackFrame.Parameter = parameter;
            newFrame = backStackFrame;
        }
        else
        {
            // Create a fresh child scope and resolve the ViewModel from it
            var scope = RootProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<TViewModel>();
            newFrame = new NavigationFrame { Scope = scope, ViewModelType = typeof(TViewModel), Parameter = parameter };
        }

        if (parameter is not null)
            newFrame.Parameter = parameter;

        // Evict the current frame (push to backstack for forward nav, dispose for back nav)
        HandleCurrentFrame(backNavigation);

        // Activate the new frame
        _currentFrame = newFrame;
        var resolvedVm = newFrame.Scope.ServiceProvider.GetRequiredService<TViewModel>();
        if (parameter is not null)
            resolvedVm.Parameter = parameter;

        var page = CreateView(newFrame.Scope.ServiceProvider, typeof(TViewModel), resolvedVm, parameter);
        SetFrameContent(hostFrame, page);
        await InitializeViewAndViewModel(page, backNavigation);
    }

    /// <summary>
    /// Navigates to a specified ViewModel with a specified view type.
    /// </summary>
    public Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView =>
        NavigateAsync<TViewModel, TView>(default!, parameter, backNavigation);

    /// <summary>
    /// Navigates to a specified ViewModel with a specified view type.
    /// </summary>
    public async Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
    {
        var hostFrame = GetNavigationFrame();
        if (hostFrame is null)
            return;

        if (_scopedContextService.GetRequiredService(typeof(TView)) is not IView page)
            return;

        NavigationFrame? newFrame;

        if (viewModel is not null)
        {
            var scope = RootProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<TViewModel>();
            newFrame = new NavigationFrame { Scope = scope, ViewModelType = typeof(TViewModel), Parameter = parameter };
        }
        else if (TryRemoveFromBackStack<TViewModel>(out var backStackFrame))
        {
            backStackFrame.Parameter = parameter;
            newFrame = backStackFrame;
        }
        else
        {
            var scope = RootProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<TViewModel>();
            newFrame = new NavigationFrame { Scope = scope, ViewModelType = typeof(TViewModel), Parameter = parameter };
        }

        if (parameter is not null)
            newFrame.Parameter = parameter;

        HandleCurrentFrame(backNavigation);

        _currentFrame = newFrame;
        var resolvedVm = newFrame.Scope.ServiceProvider.GetRequiredService<TViewModel>();
        if (parameter is not null)
            resolvedVm.Parameter = parameter;

        page.ViewModel = resolvedVm;
        SetFrameContent(hostFrame, page);
        await InitializeViewAndViewModel(page, backNavigation);
    }

    #endregion

    #region Modal Navigation

    /// <summary>
    /// When overridden in a derived class, indicates that the backstack changed event
    /// handlers should be cleared before modal navigation.
    /// WPF overrides to return <c>true</c> to preserve existing behavior.
    /// </summary>
    protected virtual bool ClearBackStackOnModalNavigation => false;

    /// <summary>
    /// Navigates modally to a specified ViewModel.
    /// </summary>
    public async Task NavigateModalAsync<TViewModel>(object? parameter = null)
        where TViewModel : class, IViewModel
    {
        // WPF clears backstack handlers before modal navigation
        if (ClearBackStackOnModalNavigation)
            _backStackChanged = null;

        // Try to see if we already have an instance of this ViewModel
        IViewModel? existingViewModel = null;
        TViewModel viewModel;

        if (GetWindowContent() is IView originalView)
            existingViewModel = originalView.ViewModel;

        // Reuse existing ViewModel if it matches the requested type
        if (existingViewModel is TViewModel existingVm)
            viewModel = existingVm;
        else
            viewModel = _scopedContextService.GetRequiredService<TViewModel>();

        if (parameter is not null)
            viewModel.Parameter = parameter;

        if (CreateView(_scopedContextService.ServiceProvider, typeof(TViewModel), viewModel, parameter) is { } page)
        {
            page.ViewModel = viewModel;
            SetWindowContent(page);

            // Initialize the ViewModel if needed
            await InitializeViewAndViewModel(page, backNavigation: false);
        }
        else
        {
            throw new InvalidOperationException($"Could not resolve view for ViewModel type {typeof(TViewModel).Name}");
        }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Dispose the current frame (scope + ViewModel + scoped dependencies)
        try { _currentFrame?.Dispose(); }
        catch (Exception ex) { _logger.LogError(ex, "Error disposing current frame during shutdown"); }
        _currentFrame = null;

        // Dispose all backstack frames
        foreach (var frame in _backStack)
        {
            try { frame.Dispose(); }
            catch (Exception ex) { _logger.LogError(ex, "Error disposing backstack frame during shutdown"); }
        }

        _backStack.Clear();
        _backStackChanged = null;

        GC.SuppressFinalize(this);
    }

    #endregion
}
