using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Imaging;

namespace ISynergy.Framework.UI.Extensions.Tests;

[TestClass()]
public class BitmapExtensionsTests
{
    [TestMethod]
    public void ZeroByteBitmapToImageBytesTest()
    {
        // Arrange
        var bitmap = Array.Empty<byte>();
        var quality = 90L;
        var format = ImageFormat.Jpeg;

        // Assert
        Assert.ThrowsException<ArgumentException>(() => bitmap.ToImageBytes(quality, format));
    }

    [TestMethod]
    public void ToJpegImageBytesTest()
    {
        // Arrange
        using var bitmap = new Bitmap(10, 10);// small 10x10 bitmap
        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Bmp);
        var bitmapBytes = ms.ToArray();

        var quality = 90L;
        var format = ImageFormat.Jpeg;

        // Act
        var result = bitmapBytes.ToImageBytes(quality, format);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(454, bitmapBytes.Length);
        Assert.AreEqual(632, result.Length);
        Assert.IsInstanceOfType(result, typeof(byte[]));
    }

    [TestMethod]
    public void ToPngImageBytesTest()
    {
        // Arrange
        using var bitmap = new Bitmap(10, 10); // small 10x10 bitmap
        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Bmp);
        var bitmapBytes = ms.ToArray();

        var quality = 100L;
        var format = ImageFormat.Png;

        // Act
        var result = bitmapBytes.ToImageBytes(quality, format);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(454, bitmapBytes.Length);
        //Assert.AreEqual(148, result.Length);
        Assert.IsInstanceOfType(result, typeof(byte[]));
    }
}