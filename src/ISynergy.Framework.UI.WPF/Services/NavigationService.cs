using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using View = ISynergy.Framework.UI.Controls.View;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// WPF implementation of the navigation service.
/// Handles WPF-specific window/frame access, view creation, and UI thread dispatching.
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
        ILocalSettings localSettings,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NavigationService> logger)
        : base(exceptionHandlerService, commonServices, localSettings, serviceScopeFactory, logger)
    {
    }

    /// <summary>
    /// WPF clears the backstack changed event handlers before modal navigation.
    /// </summary>
    protected override bool ClearBackStackOnModalNavigation => true;

    /// <summary>
    /// Gets the navigation Frame from the WPF MainWindow's visual tree.
    /// </summary>
    protected override object? GetNavigationFrame()
    {
        if (Application.Current.MainWindow?.Content is DependencyObject dependencyObject)
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
        return Application.Current.MainWindow?.Content;
    }

    /// <summary>
    /// Sets the MainWindow content directly (used for modal navigation).
    /// </summary>
    protected override void SetWindowContent(IView page)
    {
        Application.Current.MainWindow.Content = page;
    }

    /// <summary>
    /// Creates an <see cref="IView"/> from a ViewModel type using WPF-specific view resolution.
    /// </summary>
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
    /// Monitors a blade view for loading by subscribing to the WPF Loaded event.
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
    /// Dispatches an action on the WPF Dispatcher.
    /// </summary>
    protected override void DispatchOnUIThread(Action action)
    {
        if (Application.Current?.Dispatcher is not null)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action();
            else
                Application.Current.Dispatcher.Invoke(action);
        }
    }
}
