using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

public interface ISelectionViewModel
{
    SelectionModes SelectionMode { get; }
    void SetSelectionMode(SelectionModes selectionMode);
    List<object> SelectedItems { get; }
    void SetItems(IEnumerable<object> e);
    void SetSelectedItems(IEnumerable<object> e);
    AsyncRelayCommand<List<object>> SelectCommand { get; }
}
