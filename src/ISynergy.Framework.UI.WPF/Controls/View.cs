using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class View.
/// Implements the <see cref="Page" />
/// Implements the <see cref="IView" />
/// </summary>
/// <seealso cref="Page" />
/// <seealso cref="IView" />
[Bindable(true)]
[Lifetime(Lifetimes.Singleton)]
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

            // Return null instead of throwing — DataContext can be null after cleanup.
            // Callers handle null gracefully via null checks.
            return null!;
        }
        set
        {
            DataContext = value;
            SetBinding(View.TitleProperty, new Binding(nameof(value.Title)));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    protected View()
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
            // The ViewModel lifecycle is managed by the DI scope that created it.
            // Do NOT dispose it here — the scope handles that. Do NOT set
            // ViewModel/DataContext to null — it propagates through the visual tree
            // and can trigger operations on elements whose services are already disposed.
        }
    }
    #endregion
}
