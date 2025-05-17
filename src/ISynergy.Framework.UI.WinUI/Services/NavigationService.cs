using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
using View = ISynergy.Framework.UI.Controls.View;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class NavigationService.
/// Implements the <see cref="INavigationService" />
/// </summary>
/// <seealso cref="INavigationService" />
public class NavigationService : INavigationService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger _logger;

    private EventHandler? _backStackChanged;

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

    /// <summary>
    /// Navigation backstack.
    /// </summary>
    private Stack<object> _backStack = new Stack<object>();

    /// <summary>
    /// Maximum size for the backstack before cleaning up old ViewModels
    /// </summary>
    private const int MaxBackstackSize = 10;

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// </summary>
    /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
    public bool CanGoBack => _backStack.Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="scopedContextService"></param>
    /// <param name="logger"></param>
    public NavigationService(
        IScopedContextService scopedContextService,
        ILogger<NavigationService> logger)
    {
        _logger = logger;
        _logger.LogTrace($"NavigationService instance created with ID: {Guid.NewGuid()}");

        _scopedContextService = scopedContextService;
    }

    /// <summary>
    /// Goes the back.
    /// </summary>
    public Task GoBackAsync()
    {
        if (CanGoBack && _backStack.Pop() is IViewModel viewModel)
            return NavigateAsync(viewModel, backNavigation: true);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cleans up ViewModels in the backstack beyond a certain threshold.
    /// </summary>
    public void CleanBackStack(bool suppressEvent = false)
    {
        _backStack.Clear();

        if (!suppressEvent)
        {
            try
            {
                OnBackStackChanged(EventArgs.Empty);
            }
            catch (ObjectDisposedException)
            {
                // Clear all handlers if we get a disposal exception
                _backStackChanged = null;
            }
        }
    }

    /// <summary>
    /// Checks if navigation away from the current ViewModel is allowed
    /// </summary>
    private async Task<bool> CanNavigateAwayFromAsync(IViewModel viewModel)
    {
        if (!HasRunningCommands(viewModel))
            return true;

        // Ask user if they want to cancel operations
        var result = await _scopedContextService.GetRequiredService<IDialogService>().ShowMessageAsync(
            "Operations in Progress",
            "There are operations in progress. Do you want to cancel them and navigate away?",
            MessageBoxButtons.YesNo);

        if (result == MessageBoxResult.No)
            return false;

        // Cancel commands
        viewModel.CancelAllCommands();
        return true;
    }

    /// <summary>
    /// Checks if a ViewModel has any running commands
    /// </summary>
    private bool HasRunningCommands(IViewModel viewModel)
    {
        if (viewModel == null) return false;

        // Get all properties that are IAsyncRelayCommand
        var commandProperties = viewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(IAsyncRelayCommand).IsAssignableFrom(p.PropertyType));

        foreach (var property in commandProperties)
        {
            if (property.GetValue(viewModel) is IAsyncRelayCommand command && command.IsRunning)
                return true;
        }

        return false;
    }

    /// <summary>
    /// get navigation blade as an asynchronous operation.
    /// </summary>
    private async Task<IView> GetNavigationBladeAsync(IViewModel viewModel)
    {
        var viewType = default(Type);
        var view = viewModel.GetRelatedView();

        if (viewModel is ISelectionViewModel)
            viewType = typeof(SelectionView);
        else
            viewType = view.GetRelatedViewType();

        if (viewType is not null && _scopedContextService.GetRequiredService(viewType) is View resolvedPage)
        {
            resolvedPage.ViewModel = viewModel;

            // Create a task to track when the view is loaded
            var viewLoadedTcs = new TaskCompletionSource<bool>();

            void OnViewLoaded(object sender, RoutedEventArgs e)
            {
                ((View)sender).Loaded -= OnViewLoaded;
                viewLoadedTcs.TrySetResult(true);
            }

            if (!resolvedPage.IsLoaded)
                resolvedPage.Loaded += OnViewLoaded;

            // Initialize the ViewModel first
            if (!viewModel.IsInitialized)
                await viewModel.InitializeAsync();

            // If the view is not yet loaded, wait for it (with a reasonable timeout)
            if (!resolvedPage.IsLoaded)
            {
                var timeoutTask = Task.Delay(500);
                var completedTask = await Task.WhenAny(viewLoadedTcs.Task, timeoutTask);

                // Cleanup the event handler if we timed out
                if (completedTask == timeoutTask)
                    resolvedPage.Loaded -= OnViewLoaded;
            }

            // Call OnNavigatedTo for the blade ViewModel
            viewModel.OnNavigatedTo();

            return resolvedPage;
        }

        throw new InvalidOperationException($"Instance create or navigate to page: {view}.");
    }

    /// <summary>
    /// Common blade setup logic
    /// </summary>
    private async Task<IView?> SetupBladeAsync(IViewModelBladeView owner, IViewModelBlade bladeVm, Func<Task<IView?>> getViewFunc)
    {
        // Check if owner ViewModel can navigate away (if it has running commands)
        if (owner is IViewModel ownerVm && HasRunningCommands(ownerVm))
        {
            bool canNavigate = await CanNavigateAwayFromAsync(ownerVm);

            // Navigation was cancelled
            if (!canNavigate)
                return null;

            // Notify owner it's being partially navigated from
            ownerVm.OnNavigatedFrom();
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

        // Get the view
        var view = await getViewFunc();

        return view;
    }

    /// <summary>
    /// Common logic for adding a blade to the owner
    /// </summary>
    private void AddBladeToOwner(IViewModelBladeView owner, IView view)
    {
        if (!owner.Blades.Any(a => a.GetType().FullName!.Equals(view.GetType().FullName)))
        {
            foreach (var blade in owner.Blades.EnsureNotNull())
            {
                // Notify existing blades they're being backgrounded
                if (blade.ViewModel is IViewModel existingVm)
                    existingVm.OnNavigatedFrom();

                blade.IsEnabled = false;
            }

            owner.Blades.Add(view);
        }

        owner.IsPaneVisible = true;
    }

    /// <summary>
    /// open blade as an asynchronous operation.
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
    /// Opens blade with a custom defined view.
    /// </summary>
    public async Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel)
        where TView : IView
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
            var view = await SetupBladeAsync(owner, bladeVm, async () =>
            {
                if (_scopedContextService.GetRequiredService(typeof(TView)) is View view)
                {
                    view.ViewModel = viewmodel;

                    // Create a task to track when the view is loaded
                    var viewLoadedTcs = new TaskCompletionSource<bool>();

                    void OnViewLoaded(object sender, RoutedEventArgs e)
                    {
                        ((View)sender).Loaded -= OnViewLoaded;
                        viewLoadedTcs.TrySetResult(true);
                    }

                    if (!view.IsLoaded)
                        view.Loaded += OnViewLoaded;

                    // Initialize the ViewModel
                    if (!viewmodel.IsInitialized)
                        await viewmodel.InitializeAsync();

                    // If the view is not yet loaded, wait for it (with a reasonable timeout)
                    if (!view.IsLoaded)
                    {
                        var timeoutTask = Task.Delay(500);
                        var completedTask = await Task.WhenAny(viewLoadedTcs.Task, timeoutTask);

                        // Cleanup the event handler if we timed out
                        if (completedTask == timeoutTask)
                            view.Loaded -= OnViewLoaded;
                    }

                    // Call OnNavigatedTo for the blade ViewModel
                    viewmodel.OnNavigatedTo();

                    return view;
                }

                throw new KeyNotFoundException($"Page not found: {typeof(TView)}.");
            });

            if (view != null)
                AddBladeToOwner(owner, view);
        }
    }

    /// <summary>
    /// Removes the blade asynchronous.
    /// </summary>
    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
            // Notify the blade ViewModel it's being navigated from
            viewmodel.OnNavigatedFrom();

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
                            activeVm.OnNavigatedTo();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    public Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel =>
        NavigateAsync(default(TViewModel)!, parameter, backNavigation);

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    public Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView =>
        NavigateAsync<TViewModel, TView>(default!, parameter, backNavigation);

    /// <summary>
    /// Common navigation logic for handling current ViewModel
    /// </summary>
    private async Task<bool> HandleCurrentViewModelAsync(IViewModel? currentViewModel, bool backNavigation)
    {
        if (currentViewModel == null)
            return true;

        bool canNavigate = await CanNavigateAwayFromAsync(currentViewModel);

        if (!canNavigate)
            return false;

        // Notify current ViewModel it's being navigated from
        currentViewModel.OnNavigatedFrom();

        // Perform partial cleanup
        currentViewModel.Cleanup(isClosing: false);

        // Add to backstack if not a back navigation
        if (!backNavigation)
        {
            _backStack.Push(currentViewModel);
            OnBackStackChanged(EventArgs.Empty);
        }

        return true;
    }

    /// <summary>
    /// Common logic for initializing a view and its ViewModel
    /// </summary>
    private async Task InitializeViewAndViewModel(View page, bool backNavigation)
    {
        // Initialize if needed and notify ViewModel it's being navigated to
        if (page.ViewModel is not null && !page.ViewModel.IsInitialized)
        {
            // Dispatch initialization to another thread to allow the UI to continue rendering
            await Task.Delay(50); // Small delay to let the UI render

            await page.ViewModel.InitializeAsync();

            // Post another small delay to ensure data binding has a chance to update
            await Task.Delay(50);

            page.ViewModel.OnNavigatedTo();
        }
    }

    /// <summary>
    /// navigate as an asynchronous operation.
    /// </summary>
    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        if (Application.MainWindow is not null &&
            Application.MainWindow.Content is DependencyObject dependencyObject &&
            dependencyObject.FindDescendant<Frame>() is { } frame)
        {
            // Try to reuse the current ViewModel if it matches the requested type
            IViewModel? currentViewModel = null;

            if (frame.Content is View originalView)
            {
                currentViewModel = originalView.ViewModel as IViewModel;

                // If we're navigating to the same view type that's already shown, just update parameters
                if (currentViewModel is TViewModel existingVm && viewModel is null)
                {
                    viewModel = existingVm;

                    if (parameter is not null)
                        viewModel.Parameter = parameter;

                    // No need to navigate - we're already showing this view with this viewmodel
                    viewModel.OnNavigatedTo();
                    return;
                }
            }

            if (viewModel is null)
            {
                if (currentViewModel is TViewModel existingVm)
                    viewModel = existingVm;
                else
                    viewModel = _scopedContextService.GetRequiredService<TViewModel>();
            }

            if (viewModel is not null && parameter is not null)
                viewModel.Parameter = parameter;

            // Handle current ViewModel
            if (!await HandleCurrentViewModelAsync(currentViewModel, backNavigation))
                return;

            if (viewModel is not null)
            {
                // Use the extension method to create the page
                var page = NavigationExtensions.CreatePage<TViewModel>(_scopedContextService, viewModel, parameter);

                // Set frame content
                frame.Content = page;

                // Initialize the ViewModel if needed
                if (!viewModel.IsInitialized)
                    await viewModel.InitializeAsync();

                viewModel.OnNavigatedTo();
            }
            else
            {
                throw new InvalidOperationException($"Could not resolve ViewModel {typeof(TViewModel).Name}");
            }
        }
    }

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    public async Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
    {
        if (Application.MainWindow is not null &&
            Application.MainWindow.Content is DependencyObject dependencyObject &&
            dependencyObject.FindDescendant<Frame>() is { } frame &&
            _scopedContextService.GetRequiredService(typeof(TView)) is View page)
        {
            // Try to reuse the current ViewModel if it matches the requested type
            IViewModel? currentViewModel = null;
            if (frame.Content is View originalView)
                currentViewModel = originalView.ViewModel as IViewModel;

            if (viewModel is null)
            {
                if (currentViewModel is TViewModel existingVm)
                    viewModel = existingVm;
                else if (_scopedContextService.GetRequiredService(typeof(TViewModel)) is TViewModel resolvedViewModel)
                    viewModel = resolvedViewModel;
            }

            if (viewModel is not null && parameter is not null)
                viewModel.Parameter = parameter;

            // Handle current ViewModel
            if (!await HandleCurrentViewModelAsync(currentViewModel, backNavigation))
                return;

            // Set ViewModel before setting frame content
            page.ViewModel = viewModel;
            frame.Content = page;

            if (viewModel is not null)
            {
                // Initialize the ViewModel if needed
                if (!viewModel.IsInitialized)
                    await viewModel.InitializeAsync();

                viewModel.OnNavigatedTo();
            }
        }
    }

    public async Task NavigateModalAsync<TViewModel>(object? parameter = null)
        where TViewModel : class, IViewModel
    {
        // Unsubscribe old handlers before modal navigation
        _backStackChanged = null;

        if (Application.MainWindow is not null)
        {
            // Try to see if we already have an instance of this ViewModel
            IViewModel? existingViewModel = null;
            TViewModel viewModel;

            if (Application.MainWindow.Content is View originalView)
                existingViewModel = originalView.ViewModel as IViewModel;

            // Reuse existing ViewModel if it matches the requested type
            if (existingViewModel is TViewModel existingVm)
                viewModel = existingVm;
            else
                viewModel = _scopedContextService.GetRequiredService<TViewModel>();

            if (parameter is not null)
                viewModel.Parameter = parameter;

            if (NavigationExtensions.CreatePage<TViewModel>(_scopedContextService, parameter) is { } page)
            {
                page.ViewModel = viewModel;
                Application.MainWindow.Content = page;

                if (!viewModel.IsInitialized)
                    await viewModel.InitializeAsync();

                viewModel.OnNavigatedTo();
            }
            else
            {
                throw new InvalidOperationException($"Could not resolve view for ViewModel type {typeof(TViewModel).Name}");
            }
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Dispose all ViewModels in the backstack
        foreach (var item in _backStack)
        {
            if (item is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing ViewModel during NavigationService disposal");
                }
            }
        }

        _backStack.Clear();
        _backStackChanged = null;

        GC.SuppressFinalize(this);
    }
}

