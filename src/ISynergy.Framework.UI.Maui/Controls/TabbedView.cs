using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls;

[Singleton(true)]
public abstract class TabbedView : TabbedPage, IView
{
    private IViewModel _viewModel;

    protected IContext Context { get; private set; }

    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            BindingContext = _viewModel;
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
    /// <param name="context"></param>
    protected TabbedView(IContext context)
        : this()
    {
        Argument.IsNotNull(context);
        Context = context;
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="viewModelType"></param>
    protected TabbedView(IContext context, Type viewModelType)
    : this(context)
    {
        Argument.IsNotNull(viewModelType);
        ViewModel = context.ScopedServices.ServiceProvider.GetRequiredService(viewModelType) as IViewModel;
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="viewModel"></param>
    protected TabbedView(IContext context, IViewModel viewModel)
    : this(context)
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

    // NOTE: Leave out the finalizer altogether if this class doesn't
    // own unmanaged resources, but leave the other methods
    // exactly as they are.
    //~ObservableClass()
    //{
    //    // Finalizer calls Dispose(false)
    //    Dispose(false);
    //}

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            ViewModel?.Dispose();
            ViewModel = null;
        }

        // free native resources if there are any.
    }
    #endregion
}
