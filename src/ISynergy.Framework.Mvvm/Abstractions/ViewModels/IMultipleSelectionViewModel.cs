using ISynergy.Framework.Mvvm.Commands;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Marker interface for a selection ViewModel that always operates in Multiple mode.
/// Used by the navigation service to resolve the corresponding multiple-selection view.
/// </summary>
public interface IMultipleSelectionViewModel
{
    List<object>? SelectedItem { get; }
    void SetItems(IEnumerable<object> items);
    void SetSelectedItem(IEnumerable<object>? items);
    AsyncRelayCommand<List<object>> SelectCommand { get; }
}
