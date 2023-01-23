using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface ISelectionViewModel
    /// </summary>
    public interface ISelectionViewModel
    {
        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        /// <value>The selection mode.</value>
        SelectionModes SelectionMode { get; set; }
        List<object> SelectedItems { get; set; }
    }
}
