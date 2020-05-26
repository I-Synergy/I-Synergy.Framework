using System;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Windows.Locators
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
