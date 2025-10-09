using ISynergy.Framework.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ISynergy.Framework.UI.Extensions.Tests;

[TestClass()]
public class BitmapExtensionsTests
{
    [TestMethod]
    public async Task ZeroByteBitmapToImageBytesTest()
    {
        // Arrange
        var bitmap = Array.Empty<byte>();
        var quality = 90u;
        var format = ImageFormats.jpg;

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => bitmap.ToImageBytesAsync(quality, format));
    }

    [TestMethod]
    public async Task ToJpegImageBytesTest()
    {
        // Arrange
        var bitmap = await CreateTestBitmapAsync(10, 10);
        var quality = 90u;
        var format = ImageFormats.jpg;

        // Act
        var result = await bitmap.ToImageBytesAsync(quality, format);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
        Assert.IsInstanceOfType(result, typeof(byte[]));
    }

    [TestMethod]
    public async Task ToPngImageBytesTest()
    {
        // Arrange
        var bitmap = await CreateTestBitmapAsync(10, 10);
        var quality = 100u;
        var format = ImageFormats.png;

        // Act
        var result = await bitmap.ToImageBytesAsync(quality, format);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
        Assert.IsInstanceOfType(result, typeof(byte[]));
    }

    private static async Task<byte[]> CreateTestBitmapAsync(uint width, uint height)
    {
        using var stream = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

        byte[] pixels = new byte[width * height * 4];
        encoder.SetPixelData(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            width,
            height,
            96,
            96,
            pixels);

        await encoder.FlushAsync();

        var bytes = new byte[stream.Size];
        await stream.ReadAsync(bytes.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);

        return bytes;
    }
}