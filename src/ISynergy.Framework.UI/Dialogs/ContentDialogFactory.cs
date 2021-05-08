using System;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;

#if (__UWP__ || HAS_UNO)
using Windows.UI.Xaml.Controls;
#elif (__WINUI__)
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Class responsible for creating content dialogs using reflection.
    /// </summary>
    public class ContentDialogFactory : IContentDialogFactory
    {
        /// <summary>
        /// Creates a <see cref="ContentDialog" /> of specified type using
        /// <see cref="TypeActivator.CreateInstance(Type)" />.
        /// </summary>
        /// <param name="dialogType">Type of the dialog.</param>
        /// <returns>IContentDialog.</returns>
        /// <exception cref="ArgumentException">Only dialogs of type {typeof(ContentDialog)} or {typeof(IContentDialog)} are supported.</exception>
        public IContentDialog Create(Type dialogType)
        {
            Argument.IsNotNull(nameof(dialogType), dialogType);

            var instance = TypeActivator.CreateInstance(dialogType);

            // Is instance of type IContentDialog?
            if (instance is IContentDialog customContentDialog) return customContentDialog;

            // Is instance of type ContentDialog?
            if (instance is ContentDialog contentDialog) return new ContentDialogWrapper(contentDialog);

            throw new ArgumentException($"Only dialogs of type {typeof(ContentDialog)} or {typeof(IContentDialog)} are supported.");
        }
    }
}
