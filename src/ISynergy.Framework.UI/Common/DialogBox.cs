using System;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI.Common
{
    /// <summary>
    /// Class DialogBox.
    /// </summary>
    public static class DialogBox
    {
        /// <summary>
        /// show as an asynchronous operation.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="ok">The ok.</param>
        public static Task ShowAsync(Result result, string ok = "Ok") =>
            ShowAsync(result.Message, result.Description, ok);

        /// <summary>
        /// show as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        /// <param name="ok">The ok.</param>
        /// <param name="cancel">The cancel.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static async Task<bool> ShowAsync(string title, string content, string ok = "Ok", string cancel = null)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = ok
            };
            if (cancel != null)
            {
                dialog.SecondaryButtonText = cancel;
            }
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
