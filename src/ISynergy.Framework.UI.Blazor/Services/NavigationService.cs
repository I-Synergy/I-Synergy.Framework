using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using IDialogService = Microsoft.FluentUI.AspNetCore.Components.IDialogService;

namespace ISynergy.Framework.UI.Services;

public class NavigationService : NavigationManager, INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDialogService _dialogService;

    // Dictionary to track blade panels by owner
    private readonly Dictionary<IViewModelBladeView, Dictionary<IViewModel, IDialogReference>> _bladeDialogs = new();

    // Navigation backstack
    private Stack<IViewModel> _backStack = new Stack<IViewModel>();

    // Maximum size for backstack
    private const int MaxBackstackSize = 10;

    public NavigationService(
        IServiceProvider serviceProvider,
        IDialogService dialogService)
    {
        _serviceProvider = serviceProvider;
        _dialogService = dialogService;
    }

    public bool CanGoBack => _backStack.Count > 0;

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

    protected virtual void OnBackStackChanged(EventArgs e)
    {
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

    public void CleanBackStack(bool suppressEvent = false)
    {
        while (_backStack.Count > 0)
        {
            var vm = _backStack.Pop();
            vm.Cleanup(true);
        }

        if (!suppressEvent)
        {
            OnBackStackChanged(EventArgs.Empty);
        }
    }

    public async Task GoBackAsync()
    {
        if (!CanGoBack)
            return;

        var previousViewModel = _backStack.Pop();

        // Navigate to the appropriate route
        string route = GetRouteForViewModel(previousViewModel);
        NavigateTo(route);

        // Notify the view model it's been navigated to
        previousViewModel.OnNavigatedTo();

        OnBackStackChanged(EventArgs.Empty);
    }

    public async Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel)
    {
        throw new NotImplementedException();

        //if (owner == null || viewmodel == null)
        //    return;

        //// Initialize the view model if needed
        //if (!viewmodel.IsInitialized)
        //{
        //    await viewmodel.InitializeAsync();
        //}

        //// If viewmodel is a blade viewmodel, set the owner
        //if (viewmodel is IViewModelBlade blade)
        //{
        //    blade.Owner = owner;
        //}

        //// Ensure the owner's blade collection is initialized
        //if (owner.Blades == null)
        //{
        //    owner.Blades = new ObservableCollection<IView>();
        //}

        //// Ensure the blade mapping for this owner exists
        //if (!_bladeDialogs.ContainsKey(owner))
        //{
        //    _bladeDialogs[owner] = new Dictionary<IViewModel, IDialogReference>();
        //}

        //// Find or create an appropriate blade view component for this viewmodel
        //Type bladeComponentType = FindViewComponentTypeForViewModel(viewmodel.GetType());

        //// Configure panel options
        //var panelParameters = new DialogParameters
        //{
        //    ShowTitle = true,
        //    Title = viewmodel.Title,
        //    Alignment = HorizontalAlignment.Right,
        //    ShowDismiss = true,
        //    PreventScroll = true,
        //    // Pass the viewmodel as a parameter to the panel component
        //    ["ViewModel"] = viewmodel
        //};

        //// Make the owner's pane visible
        //owner.IsPaneVisible = true;

        //// Show the panel using FluentUI's DialogService
        //var dialogReference = await _dialogService.ShowPanelAsync(bladeComponentType, panelParameters);

        //// Store the reference for later use
        //_bladeDialogs[owner][viewmodel] = dialogReference;

        //// Add a view to the blades collection for tracking
        //var view = CreateViewForViewModel(viewmodel);
        //if (view != null)
        //{
        //    owner.Blades.Add(view);
        //}

        //// Notify the viewmodel it's been navigated to
        //viewmodel.OnNavigatedTo();

        //// Handle dialog closure
        //var resultTask = dialogReference.Result;
        //resultTask.ContinueWith(task =>
        //{
        //    // Remove the blade and clean up when the dialog is closed
        //    if (_bladeDialogs.TryGetValue(owner, out var ownerBlades))
        //    {
        //        ownerBlades.Remove(viewmodel);

        //        // If this was the last blade, hide the pane
        //        if (ownerBlades.Count == 0)
        //        {
        //            owner.IsPaneVisible = false;
        //        }
        //    }

        //    // Remove from the UI collection
        //    var bladeToRemove = owner.Blades.FirstOrDefault(b => b.ViewModel == viewmodel);
        //    if (bladeToRemove != null)
        //    {
        //        owner.Blades.Remove(bladeToRemove);
        //    }

        //    // Cleanup the view model
        //    viewmodel.Cleanup(true);
        //});
    }

    public async Task OpenBladeAsync<TView>(IViewModelBladeView owner, IViewModel viewmodel) where TView : IView
    {
        throw new NotImplementedException();

        //if (owner == null || viewmodel == null)
        //    return;

        //// Initialize the view model if needed
        //if (!viewmodel.IsInitialized)
        //{
        //    await viewmodel.InitializeAsync();
        //}

        //// If viewmodel is a blade viewmodel, set the owner
        //if (viewmodel is IViewModelBlade blade)
        //{
        //    blade.Owner = owner;
        //}

        //// Ensure the owner's blade collection is initialized
        //if (owner.Blades == null)
        //{
        //    owner.Blades = new ObservableCollection<IView>();
        //}

        //// Ensure the blade mapping for this owner exists
        //if (!_bladeDialogs.ContainsKey(owner))
        //{
        //    _bladeDialogs[owner] = new Dictionary<IViewModel, IDialogReference>();
        //}

        //// Create the view instance
        //var view = CreateView<TView>(viewmodel);
        //if (view == null)
        //{
        //    throw new InvalidOperationException($"Failed to create view of type {typeof(TView).Name}");
        //}

        //// Find the corresponding component type for the view
        //Type componentType = typeof(TView);
        //if (!typeof(ComponentBase).IsAssignableFrom(componentType))
        //{
        //    // If TView is not a ComponentBase, try to find a suitable component
        //    componentType = FindComponentTypeForView<TView>();
        //    if (componentType == null)
        //    {
        //        throw new InvalidOperationException($"No suitable Blazor component found for view type {typeof(TView).Name}");
        //    }
        //}

        //// Configure panel options
        //var panelParameters = new DialogParameters
        //{
        //    ShowTitle = true,
        //    Title = viewmodel.Title,
        //    Alignment = HorizontalAlignment.Right,
        //    ShowDismiss = true,
        //    PreventScroll = true,
        //    // Pass the viewmodel as a parameter to the panel component
        //    ["ViewModel"] = viewmodel
        //};

        //// Make the owner's pane visible
        //owner.IsPaneVisible = true;

        //// Show the panel using FluentUI's DialogService
        //var dialogReference = await _dialogService.ShowPanelAsync(componentType, panelParameters);

        //// Store the reference for later use
        //_bladeDialogs[owner][viewmodel] = dialogReference;

        //// Add the view to the blades collection
        //owner.Blades.Add(view);

        //// Notify the viewmodel it's been navigated to
        //viewmodel.OnNavigatedTo();

        //// Handle dialog closure
        //var resultTask = dialogReference.Result;
        //resultTask.ContinueWith(task =>
        //{
        //    // Remove the blade and clean up when the dialog is closed
        //    if (_bladeDialogs.TryGetValue(owner, out var ownerBlades))
        //    {
        //        ownerBlades.Remove(viewmodel);

        //        // If this was the last blade, hide the pane
        //        if (ownerBlades.Count == 0)
        //        {
        //            owner.IsPaneVisible = false;
        //        }
        //    }

        //    // Remove from the UI collection
        //    var bladeToRemove = owner.Blades.FirstOrDefault(b => b.ViewModel == viewmodel);
        //    if (bladeToRemove != null)
        //    {
        //        owner.Blades.Remove(bladeToRemove);
        //    }

        //    // Cleanup the view model
        //    viewmodel.Cleanup(true);
        //});
    }

    public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
    {
        throw new NotImplementedException();

        //if (owner == null || viewmodel == null)
        //    return;

        //// Notify the viewmodel it's being navigated from
        //viewmodel.OnNavigatedFrom();

        //// Close the dialog if it exists
        //if (_bladeDialogs.TryGetValue(owner, out var ownerBlades) &&
        //    ownerBlades.TryGetValue(viewmodel, out var dialogReference))
        //{
        //    // Close the panel dialog
        //    dialogReference.CloseAsync();
        //    ownerBlades.Remove(viewmodel);

        //    // If this was the last blade, hide the pane
        //    if (ownerBlades.Count == 0)
        //    {
        //        owner.IsPaneVisible = false;
        //    }
        //}

        //// Remove from the UI collection
        //var bladeToRemove = owner.Blades.FirstOrDefault(b => b.ViewModel == viewmodel);
        //if (bladeToRemove != null)
        //{
        //    owner.Blades.Remove(bladeToRemove);
        //}

        //// Cleanup the view model
        //viewmodel.Cleanup(true);
    }

    public async Task NavigateAsync<TViewModel>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        // Get the current ViewModel for backstack handling
        IViewModel? currentViewModel = GetCurrentViewModel();

        if (currentViewModel != null && !backNavigation)
        {
            // Handle current ViewModel (cleanup and add to backstack)
            HandleCurrentViewModel(currentViewModel, backNavigation);
        }

        // Set parameter if provided
        if (parameter != null)
        {
            viewModel.Parameter = parameter;
        }

        // Initialize the ViewModel if needed
        if (!viewModel.IsInitialized)
        {
            await viewModel.InitializeAsync();
        }

        // Get route for the ViewModel
        string route = GetRouteForViewModel(viewModel);

        // Navigate to the route
        NavigateTo(route);

        // Notify the ViewModel it's being navigated to
        viewModel.OnNavigatedTo();
    }

    public async Task NavigateAsync<TViewModel, TView>(TViewModel viewModel, object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
    {
        // In Blazor, view selection is typically handled by routing
        // This method can be enhanced for specific scenarios
        await NavigateAsync(viewModel, parameter, backNavigation);
    }

    public async Task NavigateAsync<TViewModel>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
    {
        // Get the ViewModel instance from the service provider
        var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as TViewModel;
        if (viewModel == null)
        {
            throw new InvalidOperationException($"Failed to resolve ViewModel of type {typeof(TViewModel).Name}");
        }

        await NavigateAsync(viewModel, parameter, backNavigation);
    }

    public async Task NavigateAsync<TViewModel, TView>(object? parameter = null, bool backNavigation = false)
        where TViewModel : class, IViewModel
        where TView : IView
    {
        // Get the ViewModel instance from the service provider
        var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as TViewModel;
        if (viewModel == null)
        {
            throw new InvalidOperationException($"Failed to resolve ViewModel of type {typeof(TViewModel).Name}");
        }

        await NavigateAsync<TViewModel, TView>(viewModel, parameter, backNavigation);
    }

    public async Task NavigateModalAsync<TViewModel>(object? parameter = null)
        where TViewModel : class, IViewModel
    {
        throw new NotImplementedException();

        //// Get the ViewModel instance from the service provider
        //var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as TViewModel;
        //if (viewModel == null)
        //{
        //    throw new InvalidOperationException($"Failed to resolve ViewModel of type {typeof(TViewModel).Name}");
        //}

        //// Set parameter if provided
        //if (parameter != null)
        //{
        //    viewModel.Parameter = parameter;
        //}

        //// Initialize the ViewModel if needed
        //if (!viewModel.IsInitialized)
        //{
        //    await viewModel.InitializeAsync();
        //}

        //// Find the appropriate component type for this view model
        //Type componentType = FindViewComponentTypeForViewModel(viewModel.GetType());

        //// Configure dialog options
        //var parameters = new DialogParameters
        //{
        //    ShowTitle = true,
        //    Title = viewModel.Title,
        //    // Pass the viewmodel as a parameter to the component
        //    ["ViewModel"] = viewModel
        //};

        //// Show the dialog using FluentUI's DialogService
        //var dialogReference = await _dialogService.ShowDialogAsync(componentType, parameters);

        //// Notify the viewmodel it's been navigated to
        //viewModel.OnNavigatedTo();

        //// Wait for the dialog to close
        //await dialogReference.Result;

        //// Clean up when done
        //viewModel.Cleanup(true);
    }

    #region Helper Methods

    private Type? FindViewComponentTypeForViewModel(Type viewModelType)
    {
        // This is a simplified implementation that assumes components follow a naming convention
        // In a real application, you might have a more sophisticated registry or naming convention

        // Try to find a component with a matching name (e.g., "ProductViewModel" -> "ProductView")
        string viewModelName = viewModelType.Name;
        string componentName = viewModelName.Replace("ViewModel", "View");

        // Look for a component type matching the expected name
        // This assumes all components are in the same assembly or a known set of assemblies
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? componentType = assembly.GetTypes()
                .FirstOrDefault(t => t.Name == componentName &&
                                     typeof(ComponentBase).IsAssignableFrom(t));

            if (componentType != null)
                return componentType;
        }

        return null;
    }

    private Type? FindComponentTypeForView<TView>() where TView : IView
    {
        // Similar to FindViewComponentTypeForViewModel but based on view type
        string viewName = typeof(TView).Name;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? componentType = assembly.GetTypes()
                .FirstOrDefault(t => t.Name == viewName &&
                                     typeof(ComponentBase).IsAssignableFrom(t));

            if (componentType != null)
                return componentType;
        }

        return null;
    }

    //private IView? CreateViewForViewModel(IViewModel viewModel)
    //{
    //    // This is a simplified implementation
    //    // In a real application, you might use a factory or DI to create views

    //    // Look for a view type with a matching name
    //    string viewModelName = viewModel.GetType().Name;
    //    string viewName = viewModelName.Replace("ViewModel", "View");

    //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    //    {
    //        Type? viewType = assembly.GetTypes()
    //            .FirstOrDefault(t => t.Name == viewName &&
    //                               typeof(IView).IsAssignableFrom(t));

    //        if (viewType != null)
    //        {
    //            try
    //            {
    //                // Try to create the view
    //                IView? view = (IView?)Activator.CreateInstance(viewType);
    //                if (view != null)
    //                {
    //                    view.ViewModel = viewModel;
    //                    return view;
    //                }
    //            }
    //            catch
    //            {
    //                // If view creation fails, continue with the next approach
    //            }
    //        }
    //    }

    //    // If no specific view is found, see if we can get it from DI
    //    try
    //    {
    //        Type genericType = typeof(IView<>).MakeGenericType(viewModel.GetType());
    //        object? service = _serviceProvider.GetService(genericType);
    //        return service as IView;
    //    }
    //    catch
    //    {
    //        // If DI fails, return null
    //        return null;
    //    }
    //}

    private IView? CreateView<TView>(IViewModel viewModel) where TView : IView
    {
        try
        {
            // Try to get from DI first
            IView? view = (IView?)_serviceProvider.GetService(typeof(TView));

            // If DI fails, try to create an instance
            if (view == null)
            {
                view = (IView)Activator.CreateInstance(typeof(TView));
            }

            if (view != null)
            {
                view.ViewModel = viewModel;
                return view;
            }
        }
        catch
        {
            // If creation fails, return null
        }

        return null;
    }

    private string GetRouteForViewModel(IViewModel viewModel)
    {
        // This is a simplified implementation
        // In a real application, you would have a more sophisticated routing scheme

        string viewModelName = viewModel.GetType().Name;

        // Remove "ViewModel" suffix if present
        if (viewModelName.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase))
        {
            viewModelName = viewModelName.Substring(0, viewModelName.Length - 9);
        }

        // Convert to lowercase for route
        return viewModelName.ToLowerInvariant();
    }

    private IViewModel? GetCurrentViewModel()
    {
        // In a real application, you would have a way to track the current ViewModel
        // This might be through a state container, a service, or another mechanism
        return null;
    }

    private void HandleCurrentViewModel(IViewModel currentViewModel, bool backNavigation)
    {
        // Notify that we're navigating away
        currentViewModel.OnNavigatedFrom();

        // Perform partial cleanup
        currentViewModel.Cleanup(isClosing: false);

        if (!backNavigation)
        {
            // Add to backstack if not navigating back
            _backStack.Push(currentViewModel);

            OnBackStackChanged(EventArgs.Empty);

            // Limit backstack size
            while (_backStack.Count > MaxBackstackSize)
            {
                var oldestViewModel = _backStack.ToArray().Last();
                _backStack = new Stack<IViewModel>(_backStack.Where(vm => vm != oldestViewModel));

                // Clean up the removed ViewModel
                oldestViewModel.Cleanup(true);
            }
        }
    }

    #endregion
}
