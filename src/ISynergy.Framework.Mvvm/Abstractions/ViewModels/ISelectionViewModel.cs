using System.Collections;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Non-generic projection of a selection ViewModel. XAML compiled bindings cannot name generic
/// types, so templates and views bind against this interface instead of the generic class.
/// Implementations expose the members explicitly.
/// </summary>
public interface ISelectionViewModel : IViewModel
{
    /// <summary>
    /// Gets the items offered for selection.
    /// </summary>
    IEnumerable? Items { get; }

    /// <summary>
    /// Gets the current selection.
    /// </summary>
    object? SelectedItem { get; }

    /// <summary>
    /// Gets the command that validates and submits the selection.
    /// </summary>
    ICommand? SelectCommand { get; }

    /// <summary>
    /// Gets the command that re-runs the item query.
    /// </summary>
    ICommand? RefreshCommand { get; }
}
