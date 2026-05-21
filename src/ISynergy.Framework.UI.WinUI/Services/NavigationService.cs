using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;
using View = ISynergy.Framework.UI.Controls.View;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// WinUI implementation of the navigation service.
/// Handles WinUI-specific window/frame access, view creation, and UI thread dispatching.
/// </summary>
/// <seealso cref="BaseNavigationService" />
public class NavigationService : BaseNavigationService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    public NavigationService(
        IExceptionHandlerService exceptionHandlerService,
        ICommonServices commonServices,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NavigationService> logger)
        : base(exceptionHandlerService, commonServices, serviceScopeFactory, logger)
    {
    }

    /// <summary>
    /// Gets the navigation Frame from the WinUI MainWindow's visual tree.
    /// </summary>
    protected override object? GetNavigationFrame()
    {
        if (Application.MainWindow?.Content is DependencyObject dependencyObject)
            return dependencyObject.FindDescendant<Frame>();
        return null;
    }

    /// <summary>
    /// Gets the current content of the specified Frame.
    /// </summary>
    protected override object? GetFrameContent(object frame)
    {
        return ((Frame)frame).Content;
    }

    /// <summary>
    /// Sets the content of the specified Frame.
    /// </summary>
    protected override void SetFrameContent(object frame, IView page)
    {
        ((Frame)frame).Content = page;
    }

    /// <summary>
    /// Gets the MainWindow's current content directly.
    /// </summary>
    protected override object? GetWindowContent()
    {
        return Application.MainWindow?.Content;
    }

    /// <summary>
    /// Sets the MainWindow content directly (used for modal navigation).
    /// </summary>
    protected override void SetWindowContent(IView page)
    {
        Application.MainWindow?.Content = (Microsoft.UI.Xaml.UIElement)page;
    }

    /// <summary>
    /// Creates an <see cref="IView"/> from a ViewModel type using WinUI-specific view resolution.
    /// </summary>
    [RequiresUnreferencedCode(
        "The view-type resolution uses GetRelatedViewType() which scans loaded assemblies. " +
        "This is not trim-safe. Ensure view/ViewModel types are preserved via a trimmer root descriptor or source-generated registration.")]
    protected override IView CreateView(IServiceProvider serviceProvider, Type viewModelType, IViewModel? viewModel, object? parameter)
    {
        if (viewModel is null)
            viewModel = (IViewModel)serviceProvider.GetRequiredService(viewModelType);

        var viewName = viewModelType.GetRelatedView();
        var viewType = viewName.GetRelatedViewType();

        if (viewType is not null && serviceProvider.GetRequiredService(viewType) is View resolvedPage)
        {
            if (viewModel is not null)
                resolvedPage.ViewModel = viewModel;

            if (resolvedPage.ViewModel is null)
                resolvedPage.ViewModel = (IViewModel)serviceProvider.GetRequiredService(viewModelType);

            if (parameter is not null && resolvedPage.ViewModel is not null)
                resolvedPage.ViewModel.Parameter = parameter;

            return resolvedPage;
        }

        throw new FileNotFoundException($"Cannot create or navigate to page: {viewName}.");
    }

    /// <summary>
    /// Monitors a blade view for loading by subscribing to the WinUI Loaded event.
    /// </summary>
    protected override async Task MonitorViewLoadAsync(IView view, int timeoutMs = 500)
    {
        if (view is View platformView && !platformView.IsLoaded)
        {
            var tcs = new TaskCompletionSource<bool>();

            void OnViewLoaded(object sender, RoutedEventArgs e)
            {
                platformView.Loaded -= OnViewLoaded;
                tcs.TrySetResult(true);
            }

            platformView.Loaded += OnViewLoaded;

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));

            if (completedTask != tcs.Task)
                platformView.Loaded -= OnViewLoaded;
        }
    }

    /// <summary>
    /// Dispatches an action on the WinUI DispatcherQueue.
    /// </summary>
    protected override void DispatchOnUIThread(Action action)
    {
        var queue = Application.MainWindow?.DispatcherQueue
            ?? Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        queue?.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
            new Microsoft.UI.Dispatching.DispatcherQueueHandler(action));
    }

    /// <summary>
    /// Gets a navigation blade for the specified ViewModel.
    /// WinUI-specific: checks for <see cref="ISelectionViewModel"/> and resolves a <see cref="SelectionView"/> when applicable.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "GetRelatedViewType() and GetRequiredService(Type) use reflection/runtime-type lookup. All view and ViewModel types are preserved through DI registration.")]
    protected override async Task<IView> GetNavigationBladeAsync(IViewModel viewModel)
    {
        var viewType = default(Type);
        var view = viewModel.GetRelatedView();

        if (viewModel is ISelectionViewModel)
            viewType = typeof(SelectionView);
        else
            viewType = view.GetRelatedViewType();

        using var scope = _serviceScopeFactory.CreateScope();
        if (viewType is not null && scope.ServiceProvider.GetRequiredService(viewType) is IView resolvedPage)
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
}
