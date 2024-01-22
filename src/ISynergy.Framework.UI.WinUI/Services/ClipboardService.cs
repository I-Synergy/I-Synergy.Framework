using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System.Drawing.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ClipboardService.
/// </summary>
public class ClipboardService : IClipboardService
{
    /// <summary>
    /// Gets image (bytes and content type) from clipboard.
    /// </summary>
    /// <returns></returns>
    public async Task<ImageResult> GetImageFromClipboardAsync()
    {
        var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

        if (dataPackageView.Contains(StandardDataFormats.Bitmap) &&
            await dataPackageView.GetBitmapAsync() is RandomAccessStreamReference imageReceived)
        {
            using var imageStream = await imageReceived.OpenReadAsync();
            var image = await imageStream.AsStreamForRead().ToByteArrayAsync();

            if (imageStream.ContentType == "image/bmp")
                return new ImageResult(image.ToImageBytes(100, ImageFormat.Png), "image/png");
            else
                return new ImageResult(image, imageStream.ContentType);
        }

        return null;
    }
}
