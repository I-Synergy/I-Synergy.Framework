using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Reflection;
using IDialogService = Microsoft.FluentUI.AspNetCore.Components.IDialogService;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : INavigationService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<NavigationService> _logger;

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
    private Stack<IViewModel> _backStack = new Stack<IViewModel>();

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
    /// <param name="dialogService"></param>
    /// <param name="logger"></param>
    public NavigationService(
        IScopedContextService scopedContextService,
        IDialogService dialogService,
        ILogger<NavigationService> logger)
    {
        _logger = logger;
        _logger.LogTrace($"NavigationService instance created with ID: {Guid.NewGuid()}");

        _scopedContextService = scopedContextService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Goes the back.
    /// </summary>
    public Task GoBackAsync()
    {
        // Keep track of whether we had items before popping
        bool hadItems = CanGoBack;

        if (CanGoBack && _backStack.Pop() is IViewModel viewModel)
        {
            // Set IsInitialized to false to force reinitialization
            viewModel.IsInitialized = false;

            // If this was the last item, explicitly raise the event
            if (hadItems && !CanGoBack)
            {
                OnBackStackChanged(EventArgs.Empty);
            }

            // Navigate back with the preserved ViewModel
            return NavigateAsync(viewModel, backNavigation: true);
        }

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
    /// open blade as an asynchronous operation.
    /// </summary>
    public async Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewModel)
    {
        Argument.IsNotNull(owner);

        if (viewModel is IViewModelBlade bladeVm)
        {
            var view = bladeVm.GetRelatedView();
            var viewType = view.GetRelatedViewType();

            var blade = await _dialogService.ShowPanelAsync(viewType, bladeVm, new DialogParameters<IViewModelBlade>()
            {
                Content = bladeVm,
                Alignment = HorizontalAlignment.Right,
                Title = bladeVm.Title
            });

            var result = await blade.Result;
        }
    }

    /// <summary>
    /// Opens blade with a custom defined view.
    /// </summary>
    public async Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewModel)
        where TView : IView
    {
        Argument.IsNotNull(owner);

        if (viewModel is IViewModelBlade bladeVm)
        {
            var blade = await _dialogService.ShowPanelAsync(typeof(TView), bladeVm, new DialogParameters<IViewModelBlade>()
            {
                Content = bladeVm,
                Alignment = HorizontalAlignment.Right,
                Title = bladeVm.Title
            });

            var result = await blade.Result;
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
        NavigateAsync(_scopedContextService.GetRequiredService<TViewModel>(), parameter, backNavigation);

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    public Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView =>
        NavigateAsync<TViewModel, TView>(_scopedContextService.GetRequiredService<TViewModel>(), parameter, backNavigation);

    /// <summary>
    /// Common navigation logic for handling current ViewModel
    /// </summary>
    private bool HandleCurrentViewModel(IViewModel? currentViewModel, bool backNavigation)
    {
        if (currentViewModel == null)
            return true;

        currentViewModel.CancelAllCommands();
        currentViewModel.OnNavigatedFrom();

        // Perform partial cleanup
        currentViewModel.Cleanup(isClosing: false);

        // Add to backstack if not a back navigation
        if (!backNavigation)
        {
            // Store the entire ViewModel instance
            _backStack.Push(currentViewModel);

            OnBackStackChanged(EventArgs.Empty);

            // If backstack exceeds maximum size, remove oldest entries
            while (_backStack.Count > MaxBackstackSize)
            {
                var oldestEntry = _backStack.ToArray().Last();
                _backStack = new Stack<IViewModel>(_backStack.Where(e => e != oldestEntry));

                // Dispose the oldest ViewModel if needed
                if (oldestEntry is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error disposing ViewModel during backstack cleanup");
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// navigate as an asynchronous operation.
    /// </summary>
    public Task NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        var view = viewModel.GetRelatedView();
        _scopedContextService.GetRequiredService<NavigationManager>().NavigateTo($"/{view.ToLower()}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    public Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
    {
        if (parameter is not null)
            viewModel.Parameter = parameter;

        _scopedContextService.GetRequiredService<NavigationManager>().NavigateTo(viewModel.GetRelatedView());
        return Task.CompletedTask;
    }

    public Task NavigateModalAsync<TViewModel>(object? parameter = null)
        where TViewModel : class, IViewModel
    {
        // Unsubscribe old handlers before modal navigation
        _backStackChanged = null;

        var viewModel = _scopedContextService.GetRequiredService<TViewModel>();

        if (parameter is not null)
            viewModel.Parameter = parameter;

        _scopedContextService.GetRequiredService<NavigationManager>().NavigateTo(viewModel.GetRelatedView(), new NavigationOptions { ForceLoad = true, ReplaceHistoryEntry = true });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Dispose all ViewModels in the backstack
        foreach (var viewModel in _backStack)
        {
            if (viewModel is IDisposable disposable)
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
