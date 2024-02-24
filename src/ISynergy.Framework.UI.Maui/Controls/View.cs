using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Controls;

public abstract class View : ContentPage, IView
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
            SetBinding(View.TitleProperty, new Binding(nameof(ViewModel.Title)));
        }
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    protected View()
    {
        Loaded += new WeakEventHandler<EventArgs>(View_Loaded).Handler;
    }

    /// <summary>
    /// Initializes a new instance of the view class.
    /// </summary>
    /// <param name="context"></param>
    protected View(IContext context)
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
    protected View(IContext context, Type viewModelType)
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
    protected View(IContext context, IViewModel viewModel)
    : this(context)
    {
        Argument.IsNotNull(viewModel);
        ViewModel = viewModel;
    }

    /// <summary>
    /// Handles event when loaded.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void View_Loaded(object sender, EventArgs e)
    {
        if (ViewModel is not null)
            await ViewModel.InitializeAsync();
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
