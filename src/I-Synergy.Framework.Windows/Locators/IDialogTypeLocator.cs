using ISynergy.ViewModels.Base;
using System;

namespace ISynergy.Dialogs
{
    /// <summary>
    /// Interface responsible for finding a dialog type matching a view model.
    /// </summary>
    public interface IDialogTypeLocator
    {
        /// <summary>
        /// Locates a dialog type based on the specified view model.
        /// </summary>
        Type Locate(IViewModel viewModel);
    }
}
