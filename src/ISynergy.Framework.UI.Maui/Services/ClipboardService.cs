using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;

#if WINDOWS
using ISynergy.Framework.Core.Extensions;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#elif ANDROID
using Android.Content;
using Android.Graphics;
using AndroidApplication = Android.App.Application;
#elif IOS || MACCATALYST
using Foundation;
using UIKit;
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

            // WinRT can report a composite list, e.g. "image/jpeg,image/jpe,image/jpg".
            var contentType = imageStream.ContentType;
            var separatorIndex = contentType.IndexOf(',');
            if (separatorIndex > 0)
            {
                contentType = contentType[..separatorIndex].Trim();
            }

            if (contentType == "image/bmp")
            {
                var pngBytes = await ConvertBmpToPngAsync(imageBytes);
                return ImageResult.Success(pngBytes, "image/png");
            }

            return ImageResult.Success(imageBytes, contentType);
        }
        return null;
    }

    private static async Task<byte[]> ConvertBmpToPngAsync(byte[] bmpBytes)
    {
        using var inputStream = new InMemoryRandomAccessStream();
        using (var writer = new DataWriter(inputStream.GetOutputStreamAt(0)))
        {
            writer.WriteBytes(bmpBytes);
            await writer.StoreAsync();
            await writer.FlushAsync();
            writer.DetachStream();
        }

        inputStream.Seek(0);

        var decoder = await BitmapDecoder.CreateAsync(inputStream);
        using var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        using var outputStream = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outputStream);
        encoder.SetSoftwareBitmap(softwareBitmap);
        await encoder.FlushAsync();
        outputStream.Seek(0);

        var pngBytes = new byte[outputStream.Size];
        using var reader = new DataReader(outputStream.GetInputStreamAt(0));
        await reader.LoadAsync((uint)outputStream.Size);
        reader.ReadBytes(pngBytes);
        reader.DetachStream();

        return pngBytes;
    }
#elif ANDROID
    public Task<ImageResult?> GetImageFromClipboardAsync()
    {
        try
        {
            var context = AndroidApplication.Context;

            if (context.GetSystemService(Context.ClipboardService) is not ClipboardManager clipboard ||
                !clipboard.HasPrimaryClip ||
                clipboard.PrimaryClip is not { ItemCount: > 0 } clip ||
                clip.GetItemAt(0)?.Uri is not { } uri)
            {
                return Task.FromResult<ImageResult?>(null);
            }

            using var input = context.ContentResolver?.OpenInputStream(uri);
            if (input is null)
            {
                return Task.FromResult<ImageResult?>(null);
            }

            using var bitmap = BitmapFactory.DecodeStream(input);
            if (bitmap is null)
            {
                return Task.FromResult<ImageResult?>(null);
            }

            using var output = new MemoryStream();
            if (!bitmap.Compress(Bitmap.CompressFormat.Png!, 100, output))
            {
                return Task.FromResult<ImageResult?>(null);
            }

            return Task.FromResult<ImageResult?>(ImageResult.Success(output.ToArray(), "image/png"));
        }
        catch (Exception)
        {
            // Clipboard content can be revoked by the owning app between read and decode.
            return Task.FromResult<ImageResult?>(null);
        }
    }
#elif IOS || MACCATALYST
    public Task<ImageResult?> GetImageFromClipboardAsync()
    {
        var pasteboard = UIPasteboard.General;
        if (!pasteboard.HasImages || pasteboard.Image is not { } image)
        {
            return Task.FromResult<ImageResult?>(null);
        }

        using var pngData = image.AsPNG();
        if (pngData is null)
        {
            return Task.FromResult<ImageResult?>(null);
        }

        return Task.FromResult<ImageResult?>(ImageResult.Success(pngData.ToArray(), "image/png"));
    }
#else
    public Task<ImageResult?> GetImageFromClipboardAsync()
    {
        return Task.FromResult<ImageResult?>(null);
    }
#endif
}
