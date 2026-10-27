using ISynergy.Framework.Mvvm.Commands;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Marker interface for a selection ViewModel that always operates in Single mode.
/// Used by the navigation service to resolve the corresponding single-selection view.
/// </summary>
public interface ISingleSelectionViewModel
{
    object? SelectedItem { get; }
    void SetItems(IEnumerable<object> items);
    void SetSelectedItem(object? item);
    AsyncRelayCommand<object> SelectCommand { get; }
}
