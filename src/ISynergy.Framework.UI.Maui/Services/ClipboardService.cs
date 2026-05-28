using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;

#if WINDOWS
using ISynergy.Framework.Core.Extensions;
using SkiaSharp;
#endif

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ClipboardService.
/// </summary>
public class ClipboardService : IClipboardService
{
#if WINDOWS
    public async Task<ImageResult?> GetImageFromClipboardAsync()
    {
        var dataPackageView = global::Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

        if (dataPackageView.Contains(global::Windows.ApplicationModel.DataTransfer.StandardDataFormats.Bitmap) &&
            await dataPackageView.GetBitmapAsync() is { } imageReceived)
        {
            using var imageStream = await imageReceived.OpenReadAsync();
            var imageBytes = await imageStream.AsStreamForRead().ToByteArrayAsync();

            if (imageStream.ContentType == "image/bmp")
            {
                var pngBytes = ConvertBmpToPng(imageBytes);
                return new ImageResult(pngBytes, "image/png");
            }

            return new ImageResult(imageBytes, imageStream.ContentType);
        }
        return null;
    }

    private static byte[] ConvertBmpToPng(byte[] bmpBytes)
    {
        using var input = new MemoryStream(bmpBytes);
        using var skBitmap = SKBitmap.Decode(input);
        using var output = new MemoryStream();
        using (var skImage = SKImage.FromBitmap(skBitmap))
        {
            var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
            data.SaveTo(output);
        }
        return output.ToArray();
    }
#else
    public Task<ImageResult?> GetImageFromClipboardAsync()
    {
        return Task.FromResult<ImageResult?>(null);
    }
#endif
}
