using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using Windows.ApplicationModel.DataTransfer;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ClipboardService.
/// </summary>
public class ClipboardService : IClipboardService
{
    private readonly ILogger _logger;

    public ClipboardService(ILogger<ClipboardService> logger)
    {
        _logger = logger;
        _logger.LogDebug($"ClipboardService instance created with ID: {Guid.NewGuid()}");
    }

    /// <summary>
    /// Gets image (bytes and content type) from clipboard.
    /// </summary>
    /// <returns></returns>
#if WINDOWS
    public async Task<ImageResult> GetImageFromClipboardAsync()
    {
        var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

        if (dataPackageView.Contains(StandardDataFormats.Bitmap) &&
            await dataPackageView.GetBitmapAsync() is { } imageReceived)
        {
            using var imageStream = await imageReceived.OpenReadAsync();
            var image = await imageStream.AsStreamForRead().ToByteArrayAsync();

            if (imageStream.ContentType == "image/bmp")
                return new ImageResult(image.ToImageBytes(100, ImageFormat.Png), "image/png");
            return new ImageResult(image, imageStream.ContentType);
        }

        return null;
    }
#else
    public Task<ImageResult> GetImageFromClipboardAsync()
    {
        throw new NotImplementedException();
    }
#endif
}
