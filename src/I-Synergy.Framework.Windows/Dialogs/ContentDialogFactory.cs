using System;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Dialogs
{
    /// <summary>
    /// Class responsible for creating content dialogs using reflection.
    /// </summary>
    public class ContentDialogFactory : IContentDialogFactory
    {
        /// <summary>
        /// Creates a <see cref="ContentDialog" /> of specified type using
        /// <see cref="Activator.CreateInstance(Type)"/>.
        /// </summary>
        public IContentDialog Create(Type dialogType)
        {
            Argument.IsNotNull(nameof(dialogType), dialogType);

            var instance = Activator.CreateInstance(dialogType);

            // Is instance of type IContentDialog?
            if (instance is IContentDialog customContentDialog) return customContentDialog;

            // Is instance of type ContentDialog?
            if (instance is ContentDialog contentDialog) return new ContentDialogWrapper(contentDialog);

            throw new ArgumentException($"Only dialogs of type {typeof(ContentDialog)} or {typeof(IContentDialog)} are supported.");
        }
    }
}
