using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ISynergy.Framework.UI.Extensions;

public static class BitmapExtensions
{
    /// <summary>
    /// Converts a bitmap byte array to any image format (as byte array).
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="quality"></param>
    /// <param name="imageFormat"></param>
    /// <returns></returns>
    public static async Task<byte[]> ToImageBytesAsync(this byte[] bitmap, uint quality, ImageFormats imageFormat)
    {
        Argument.IsNotNullOrEmptyArray(bitmap);

        using var inputStream = new InMemoryRandomAccessStream();
        using var outputStream = new InMemoryRandomAccessStream();

        await inputStream.WriteAsync(bitmap.AsBuffer());
        inputStream.Seek(0);

        var decoder = await BitmapDecoder.CreateAsync(inputStream);
        var pixelData = await decoder.GetPixelDataAsync();

        var encoderId = GetEncoderId(imageFormat);
        var encoder = await BitmapEncoder.CreateAsync(encoderId, outputStream);

        var propertySet = new BitmapPropertySet();
        var qualityValue = new BitmapTypedValue(
            quality / 100.0, Windows.Foundation.PropertyType.Single);
        propertySet.Add("ImageQuality", qualityValue);

        encoder.SetPixelData(
            decoder.BitmapPixelFormat,
            decoder.BitmapAlphaMode,
            decoder.PixelWidth,
            decoder.PixelHeight,
            decoder.DpiX,
            decoder.DpiY,
            pixelData.DetachPixelData());

        await encoder.FlushAsync();

        var bytes = new byte[outputStream.Size];
        await outputStream.ReadAsync(bytes.AsBuffer(), (uint)outputStream.Size, InputStreamOptions.None);

        return bytes;
    }

    private static Guid GetEncoderId(ImageFormats format) => format switch
    {
        ImageFormats.jpg => BitmapEncoder.JpegEncoderId,
        ImageFormats.png => BitmapEncoder.PngEncoderId,
        ImageFormats.bmp => BitmapEncoder.BmpEncoderId,
        ImageFormats.gif => BitmapEncoder.GifEncoderId,
        ImageFormats.tiff => BitmapEncoder.TiffEncoderId,
        _ => BitmapEncoder.PngEncoderId
    };
}
