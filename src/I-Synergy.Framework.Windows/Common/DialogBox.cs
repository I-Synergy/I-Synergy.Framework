using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Common
{
    public static class DialogBox
    {
        public static async Task ShowAsync(Result result, string ok = "Ok")
        {
            await ShowAsync(result.Message, result.Description, ok);
        }

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
