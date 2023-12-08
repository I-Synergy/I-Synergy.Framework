using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class View.
/// Implements the <see cref="IView" />
/// </summary>
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
}
