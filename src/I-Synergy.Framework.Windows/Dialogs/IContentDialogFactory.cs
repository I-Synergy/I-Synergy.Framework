using System;

namespace ISynergy.Framework.Windows.Dialogs
{
    /// <summary>
    /// Interface responsible for creating content dialogs.
    /// </summary>
    public interface IContentDialogFactory
    {
        /// <summary>
        /// Creates a <see cref="IContentDialog" /> of specified type.
        /// </summary>
        /// <param name="dialogType">Type of the dialog.</param>
        /// <returns>IContentDialog.</returns>
        IContentDialog Create(Type dialogType);
    }
}
