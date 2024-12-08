using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel.DataTransfer;

namespace ISynergy.Framework.UI.Services;

public class ClipboardService : IClipboardService
{
    public async Task<ImageResult> GetImageFromClipboardAsync()
    {
        var dataPackageView = global::Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

        if (dataPackageView.Contains(StandardDataFormats.Bitmap) &&
            await dataPackageView.GetBitmapAsync() is { } imageReceived)
        {
            using var imageStream = await imageReceived.OpenReadAsync();
            var image = await imageStream.AsStreamForRead().ToByteArrayAsync();

            if (imageStream.ContentType == "image/bmp")
                return new ImageResult(image.ToImageBytes(100, System.Drawing.Imaging.ImageFormat.Png), "image/png");
            return new ImageResult(image, imageStream.ContentType);
        }

        return null;
    }
}
