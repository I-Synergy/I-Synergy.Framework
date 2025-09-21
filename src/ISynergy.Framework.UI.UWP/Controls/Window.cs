using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class Window.
/// Implements the <see cref="Mvvm.Abstractions.IWindow" />
/// </summary>
/// <seealso cref="Mvvm.Abstractions.IWindow" />
[Bindable(true)]
public class Window : ContentDialog, IWindow
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Window()
    {
        CornerRadius = new CornerRadius(8);
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public Task CloseAsync()
    {
        Hide();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets or sets the viewmodel and data context for a window.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel? ViewModel
    {
        get
        {
            if (DataContext is IViewModel viewModel)
                return viewModel;

            return null;
        }
        set
        {
            DataContext = value;
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
                    return frameworkElement;

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
    public async Task<bool> ShowAsync<TEntity>()
    {
        switch (await ShowAsync())
        {
            case ContentDialogResult.Primary:
                return true;
            case ContentDialogResult.Secondary:
                return false;
            default:
                if (ViewModel is IViewModelDialog<TEntity> dataContext && !dataContext.IsCancelled)
                {
                    return true;
                }

                return false;
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
