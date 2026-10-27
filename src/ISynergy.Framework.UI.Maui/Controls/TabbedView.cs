using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls;

[Lifetime(Lifetimes.Singleton)]
public abstract class TabbedView : TabbedPage, IView
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

            return default!;
        }
        set
        {
            BindingContext = value;
            SetBinding(View.TitleProperty, new Binding(nameof(value.Title)));
        }
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    protected TabbedView()
    {
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="viewModelType"></param>
    protected TabbedView(Type viewModelType)
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
    protected TabbedView(IViewModel viewModel)
    : this()
    {
        Argument.IsNotNull(viewModel);
        ViewModel = viewModel;
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

            // The ViewModel lifecycle is managed by its own DI scope.
            // Views are Singletons — only disposed at app shutdown.
            // Do NOT dispose the ViewModel here; the DI container owns
            // its lifetime.
        }

        // free native resources if there are any.
    }
    #endregion
}
