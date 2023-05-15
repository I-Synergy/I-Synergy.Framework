using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface ISelectionViewModel
    {
        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        /// <value>The selection mode.</value>
        SelectionModes SelectionMode { get; set; }

        /// <summary>
        /// Gets or sets the selected items.    
        /// </summary>
        List<object> SelectedItems { get; set; }
    }
}
