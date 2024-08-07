using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class View.
/// Implements the <see cref="Page" />
/// Implements the <see cref="IView" />
/// </summary>
/// <seealso cref="Page" />
/// <seealso cref="IView" />
[Bindable(true)]
public abstract partial class View : Page, IView
{
    private IViewModel _viewModel;

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
            DataContext = _viewModel;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    protected View()
        : this(null)
    {
    }

    protected View(IViewModel viewModel)
    {
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
            // free managed resources
            ViewModel?.Dispose();
            ViewModel = null;
        }

        // free native resources if there are any.
    }
    #endregion
}
