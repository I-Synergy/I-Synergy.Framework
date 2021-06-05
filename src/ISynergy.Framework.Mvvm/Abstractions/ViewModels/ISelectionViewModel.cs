using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Mvvm.Enumerations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface ISelectionViewModel
    /// Implements the <see cref="IViewModelBlade" />
    /// </summary>
    /// <seealso cref="IViewModelBlade" />
    public interface ISelectionViewModel : IViewModelBlade
    {
        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        List<object> SelectedItem { get; set; }
        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        /// <value>The selection mode.</value>
        SelectionModes SelectionMode { get; set; }
    }
}
