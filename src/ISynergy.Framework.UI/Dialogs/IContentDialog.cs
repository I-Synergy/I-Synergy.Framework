using Windows.Foundation;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Interface describing a content dialog.
    /// </summary>
    /// <remarks>This interface is intended for use when custom content dialogs, i.e. dialogs not inheriting
    /// from <see cref="ContentDialog" />, should be shown.</remarks>
    public interface IContentDialog
    {
        /// <summary>
        /// Gets or sets the data context for a <see cref="FrameworkElement" /> when it participates
        /// in data binding.
        /// </summary>
        /// <value>The data context.</value>
        object DataContext { get; set; }

        /// <summary>
        /// Begins an asynchronous operation to show the dialog.
        /// </summary>
        /// <returns>An asynchronous operation showing the dialog. When complete, returns a
        /// <see cref="ContentDialogResult" />.</returns>
        IAsyncOperation<ContentDialogResult> ShowAsync();
    }
}
