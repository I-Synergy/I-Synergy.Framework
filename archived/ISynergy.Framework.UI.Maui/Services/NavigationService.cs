using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : INavigationService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger _logger;

    private readonly bool _animated = true;

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
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    public Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel =>
        NavigateAsync(default(TViewModel)!, parameter, backNavigation);

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
    /// Navigates to the viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        if (NavigationExtensions.CreatePage<TViewModel>(_scopedContextService, viewModel, parameter) is { } view && view is Page page)
        {
            var result = Microsoft.Maui.Controls.Application.Current.MainPage.GetNavigation();

            if (result.Navigation.NavigationStack.Contains(page))
            {
                for (int i = result.Navigation.NavigationStack.Count - 1; i >= 0; i--)
                {
                    if (result.Navigation.NavigationStack[i].Equals(page))
                        break;

                    if (!backNavigation)
                        await result.Navigation.PopAsync(_animated);
                }
            }
            else
            {
                page.Parent = null;
                await result.Navigation.PushAsync(page, _animated);
            }

            await view.ViewModel.InitializeAsync();

            OnBackStackChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Navigates to the modal viewmodel with parameters.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public async Task NavigateModalAsync<TViewModel>(object parameter = null)
         where TViewModel : class, IViewModel
    {
        if (!Microsoft.Maui.Controls.Application.Current.Dispatcher.IsDispatchRequired &&
            NavigationExtensions.CreatePage<TViewModel>(_scopedContextService, parameter) is { } view && view is Page page)
        {
            // Added this nullification of handler becuase of some issues with TabbedPage.
            // When tabbed page is set as main page and then modal page is opened, then after closing and reopening the page, the tabs are not visible.
            page.Handler = null;
            Microsoft.Maui.Controls.Application.Current.MainPage = page;
            await view.ViewModel.InitializeAsync();
        }
        else
            await Microsoft.Maui.Controls.Application.Current.Dispatcher.DispatchAsync(async () => await NavigateModalAsync<TViewModel>(parameter));
    }

    #region NotImplemented
    [Obsolete("Not supported!", true)]
    public Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel)
        where TView : IView =>
        throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView => throw new NotImplementedException();

    [Obsolete("Not supported!", true)]
    public Task GoForwardAsync() => throw new NotImplementedException();
    #endregion
}
