using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls;

[Lifetime(Lifetimes.Singleton)]
public abstract class View : ContentPage, IView
{
    private IServiceScope? _viewModelScope;

    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel ViewModel
    {
        get
        {
            if (BindingContext is IViewModel viewModel)
                return viewModel;

            // Return null instead of throwing. BindingContext can legitimately be null
            // or non-IViewModel when a Singleton page is reused after CleanupPage cleared
            // its BindingContext during a prior modal navigation. Callers (e.g.
            // CreatePage, CleanupPage) handle null gracefully via null checks.
            return null!;
        }
        set
        {
            BindingContext = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    protected View()
    {
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="viewModelType"></param>
    protected View(Type viewModelType) // NOSONAR
    : this()
    {
        Argument.IsNotNull(viewModelType);
        _viewModelScope = IPlatformApplication.Current!.Services.CreateScope();
        ViewModel = (_viewModelScope.ServiceProvider.GetRequiredService(viewModelType) as IViewModel)!;
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="viewModel"></param>
    protected View(IViewModel viewModel)
    : this()
    {
        Argument.IsNotNull(viewModel);
        ViewModel = viewModel;
    }

    /// <summary>
    /// When the page appears, automatically inherits the <see cref="FlowDirection"/>
    /// from the application's main <see cref="Microsoft.Maui.Controls.Window"/>.
    /// This ensures all runtime-created pages (dialogs, modal navigation, etc.)
    /// respect the RTL/LTR setting without per-view configuration.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Application.Current?.Windows.Count > 0)
        {
            var mainWindow = Application.Current.Windows[0];
            if (mainWindow.FlowDirection != FlowDirection.MatchParent)
            {
                FlowDirection = mainWindow.FlowDirection;
            }
        }
    }

    /// <summary>
    /// Called when navigation to this page completes. Ensures the ViewModel's
    /// <see cref="IViewModel.InitializeAsync"/> is called before
    /// <see cref="IViewModel.OnNavigatedTo"/> — matching the lifecycle used
    /// by the shared navigation infrastructure.
    /// This covers Shell tab pages created via <c>DataTemplate</c> where the
    /// programmatic navigation service is not involved.
    /// </summary>
    /// <param name="args">Navigation event arguments.</param>
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (ViewModel is { IsDisposed: false } vm)
        {
            try
            {
                if (!vm.IsInitialized)
                    await vm.InitializeAsync();

                vm.OnNavigatedTo();
            }
            catch (Exception)
            {
                // Suppress — the exception handler service is not available
                // from within the View. The NavigationService path calls
                // InitializeAsync with its own exception handling.
            }
        }
    }

    #region IDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _viewModelScope?.Dispose();
            _viewModelScope = null;

            // Views are Singletons — only disposed at app shutdown.
            // Do NOT clear BindingContext or Content here. Setting
            // BindingContext to null propagates through the entire visual
            // tree, triggering bindings (e.g. Image.Source) that try to
            // load resources asynchronously. Those async operations use
            // MauiContext.WrappedServiceProvider, which is already disposed
            // at shutdown, causing ObjectDisposedException.
            //
            // MAUI handles all native cleanup during teardown. The ViewModel
            // lifecycle is managed by its own DI scope. We just let go.
        }
    }
    #endregion
}
