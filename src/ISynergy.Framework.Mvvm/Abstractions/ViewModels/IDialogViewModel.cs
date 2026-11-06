using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Non-generic projection of a dialog ViewModel. XAML compiled bindings cannot name the generic
/// <see cref="IViewModelDialog{TModel}"/>, so the dialog templates bind against this interface.
/// </summary>
public interface IDialogViewModel : IViewModel
{
    /// <summary>
    /// Gets the current selection.
    /// </summary>
    object? SelectedItem { get; }

    /// <summary>
    /// Gets the command that validates and submits the dialog.
    /// </summary>
    ICommand? SubmitCommand { get; }
}
