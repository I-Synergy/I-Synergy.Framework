using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class Window.
/// Implements the <see cref="Window" />
/// Implements the <see cref="Mvvm.Abstractions.IWindow" />
/// </summary>
/// <seealso cref="Window" />
/// <seealso cref="Mvvm.Abstractions.IWindow" />
[Bindable(true)]
public partial class Window : System.Windows.Window, IWindow
{
    private IViewModel? _viewModel;

    /// <summary>
    /// Gets or sets the viewmodel and data context for a window.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel? ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            DataContext = _viewModel;
        }
    }

    /// <summary>
    /// Gets the name of the descendant from.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="name">The name.</param>
    /// <returns>FrameworkElement.</returns>
    private static FrameworkElement? GetDescendantFromName(DependencyObject parent, string name)
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);

        if (count < 1)
            return null;

        for (var i = 0; i < count; i++)
        {
            if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
            {
                if (frameworkElement.Name == name)
                {
                    return frameworkElement;
                }

                if (GetDescendantFromName(frameworkElement, name) is FrameworkElement descendant)
                    return descendant;
            }
        }

        return null;
    }

    /// <summary>
    /// show as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>

    public Task<bool> ShowAsync<TEntity>()
    {
        var result = ShowDialog();

        if ((result.HasValue && result.Value) || (DataContext is IViewModelDialog<TEntity> dataContext && !dataContext.IsCancelled))
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task CloseAsync()
    {
        Close();
        return Task.CompletedTask;
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
        }

        // free native resources if there are any.
    }
    #endregion
}
