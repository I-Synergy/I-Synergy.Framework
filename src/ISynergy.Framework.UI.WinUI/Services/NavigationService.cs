using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            if (_backStackChanged is null ||
                !_backStackChanged.GetInvocationList().Contains(value))
            {
                _backStackChanged += value;
            }
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
        if (handlers != null)
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
    private readonly Stack<object> _backStack = new Stack<object>();

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// </summary>
    /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
    public bool CanGoBack => _backStack.Count > 0 ? true : false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="scopedContextService"></param>
    /// <param name="loggerFactory"></param>
    public NavigationService(
        IScopedContextService scopedContextService,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NavigationService>();
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
    /// get navigation blade as an asynchronous operation.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <returns>IView.</returns>
    /// <exception cref="ArgumentException">Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure? - viewModel</exception>
    /// <exception cref="ArgumentException">Instance could not be created from {viewModelKey}</exception>
    private async Task<IView> GetNavigationBladeAsync(IViewModel viewModel)
    {
        var viewType = default(Type);
        var view = viewModel.GetRelatedView();

        if (viewModel is ISelectionViewModel selectionBlade)
            viewType = typeof(SelectionView);
        else
            viewType = view.GetRelatedViewType();

        if (viewType is not null && _scopedContextService.ServiceProvider.GetRequiredService(viewType) is View resolvedPage)
        {
            resolvedPage.ViewModel = viewModel;

            if (!viewModel.IsInitialized)
                await viewModel.InitializeAsync();

            return resolvedPage;
        }

        throw new InvalidOperationException($"Instance create or navigate to page: {view}.");
    }

    /// <summary>
    /// open blade as an asynchronous operation.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task.</returns>
    public async Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel)
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
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

            var view = await GetNavigationBladeAsync(bladeVm);

            if (!owner.Blades.Any(a => a.GetType().FullName!.Equals(view.GetType().FullName)))
            {
                foreach (var blade in owner.Blades.EnsureNotNull())
                {
                    blade.IsEnabled = false;
                }

                owner.Blades.Add(view);
            }

            owner.IsPaneVisible = true;
        }
    }

    /// <summary>
    /// Opens blade with a custom defined view.
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <param name="owner"></param>
    /// <param name="viewmodel"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel)
        where TView : IView
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
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

            if (_scopedContextService.ServiceProvider.GetRequiredService(typeof(TView)) is View view)
            {
                view.ViewModel = viewmodel;

                if (!viewmodel.IsInitialized)
                    await viewmodel.InitializeAsync();

                if (!owner.Blades.Any(a => a.GetType().FullName!.Equals(view.GetType().FullName)))
                {
                    foreach (var blade in owner.Blades.EnsureNotNull())
                    {
                        blade.IsEnabled = false;
                    }

                    owner.Blades.Add(view);
                }

                owner.IsPaneVisible = true;
            }
            else
            {
                throw new KeyNotFoundException($"Page not found: {typeof(TView)}.");
            }
        }
    }

    /// <summary>
    /// Removes the blade asynchronous.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="viewmodel">The viewmodel.</param>
    /// <returns>Task.</returns>
    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
    {
        Argument.IsNotNull(owner);

        if (viewmodel is IViewModelBlade bladeVm)
        {
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
                        blade.IsEnabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Navigates to a specified viewmodel asynchronous.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel =>
        NavigateAsync(default(TViewModel)!, parameter, backNavigation);

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TView"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView =>
        NavigateAsync<TViewModel, TView>(default!, parameter, backNavigation);

    /// <summary>
    /// navigate as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="backNavigation"></param>
    /// <returns>Task&lt;IView&gt;.</returns>
    /// <exception cref="ArgumentException">Page not found: {viewmodel.GetType().FullName}. Did you forget to call NavigationService.Configure?</exception>
    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        if (Application.MainWindow is not null && Application.MainWindow.Content is DependencyObject dependencyObject &&
            dependencyObject.FindDescendant<Frame>() is { } frame &&
            NavigationExtensions.CreatePage<TViewModel>(_scopedContextService, viewModel, parameter) is { } page)
        {
            // Check if actual page is the same as destination page.
            if (frame.Content is View originalView)
            {
                if (originalView.GetType().Equals(page.GetType()))
                    return;

                if (!backNavigation && originalView.ViewModel is not null)
                    _backStack.Push(originalView.ViewModel);
            }

            frame.Content = page;

            if (page.ViewModel is not null && !page.ViewModel.IsInitialized)
                await page.ViewModel.InitializeAsync();

            OnBackStackChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Navigates viewmodel to a specified view.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TView"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="parameter"></param>
    /// <param name="backNavigation"></param>
    /// <returns></returns>
    public async Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
    {
        if (Application.MainWindow is not null && Application.MainWindow.Content is DependencyObject dependencyObject &&
            dependencyObject.FindDescendant<Frame>() is { } frame &&
            _scopedContextService.ServiceProvider.GetRequiredService(typeof(TView)) is View page)
        {
            if (viewModel is null && _scopedContextService.ServiceProvider.GetRequiredService(typeof(TViewModel)) is TViewModel resolvedViewModel)
                viewModel = resolvedViewModel;

            if (viewModel is not null && parameter is not null)
                viewModel.Parameter = parameter;

            page.ViewModel = viewModel;

            // Check if actual page is the same as destination page.
            if (frame.Content is View originalView)
            {
                if (originalView.GetType().Equals(page.GetType()))
                    return;

                if (!backNavigation && originalView.ViewModel is not null)
                    _backStack.Push(originalView.ViewModel);
            }

            frame.Content = page;

            if (page.ViewModel is not null && !page.ViewModel.IsInitialized)
                await page.ViewModel.InitializeAsync();

            OnBackStackChanged(EventArgs.Empty);
        }
    }

    public async Task NavigateModalAsync<TViewModel>(object? parameter = null)
        where TViewModel : class, IViewModel
    {
        // Unsubscribe old handlers before modal navigation
        _backStackChanged = null;

        if (Application.MainWindow is not null && NavigationExtensions.CreatePage<TViewModel>(_scopedContextService, parameter) is { } page)
        {
            Application.MainWindow.Content = page;

            if (!page.ViewModel!.IsInitialized)
                await page.ViewModel.InitializeAsync();
        }
    }

    public Task CleanBackStackAsync(bool suppressEvent = false)
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

        return Task.CompletedTask;
    }
}
