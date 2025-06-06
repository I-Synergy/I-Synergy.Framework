using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Components.Layout;

[Bindable(true)]
[Lifetime(Lifetimes.Scoped)]
public partial class Window : FluentDialog, IWindow
{
    private IViewModel? _viewModel;

    /// <summary>
    /// Gets or sets the viewmodel and data context for a window.
    /// </summary>
    /// <value>The data context.</value>
    public IViewModel? ViewModel
    {
        get => _viewModel;
        set => _viewModel = value;
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
