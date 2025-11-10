using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class View.
/// Implements the <see cref="IView" />
/// </summary>
/// <seealso cref="IView" />
[Bindable(true)]
public abstract partial class View : Page, IView
{
    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel ViewModel
    {
        get
        {
            if (DataContext is IViewModel viewModel)
                return viewModel;

            throw new InvalidOperationException("The BindingContext is not of type IViewModel.");
        }
        set
        {
            DataContext = value;
        }
    }

    #region IDisposable
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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
        }

        // free native resources if there are any.
    }
    #endregion
}