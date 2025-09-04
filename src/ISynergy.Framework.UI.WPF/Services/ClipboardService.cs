using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Converters;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Service for handling clipboard actions.
/// </summary>
public class ClipboardService : IClipboardService
{
    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct BITMAPFILEHEADER
    {
        public static readonly short BM = 0x4d42;

        // BM
        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    /// <summary>
    /// Gets the bitmap source from clipboard asynchronous.
    /// </summary>
    /// <returns>Task&lt;System.Object&gt;.</returns>
    public Task<object?> GetBitmapSourceFromClipboardAsync()
    {
        if ((Clipboard.GetData("DeviceIndependentBitmap") is MemoryStream memoryStream))
        {
            var buffer = new byte[Convert.ToInt32(memoryStream.Length - 1) + 1];
            memoryStream.Read(buffer, 0, buffer.Length);

            BITMAPINFOHEADER iInfoHeader = BinaryConverter.FromByteArray<BITMAPINFOHEADER>(buffer);

            var fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
            var infoHeaderSize = iInfoHeader.biSize;
            var fileSize = fileHeaderSize + iInfoHeader.biSize + iInfoHeader.biSizeImage;

            var header = new BITMAPFILEHEADER
            {
                bfType = BITMAPFILEHEADER.BM,
                bfSize = fileSize,
                bfReserved1 = 0,
                bfReserved2 = 0,
                bfOffBits = fileHeaderSize + infoHeaderSize + iInfoHeader.biClrUsed * 4
            };

            var bytes = BinaryConverter.ToByteArray<BITMAPFILEHEADER>(header);

            var stream = new MemoryStream();
            stream.Write(bytes, 0, fileHeaderSize);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);

            return Task.FromResult<object?>(BitmapFrame.Create(stream));
        }

        return Task.FromResult<object?>(null);
    }

    public async Task<ImageResult?> GetImageFromClipboardAsync()
    {
        if (await GetBitmapSourceFromClipboardAsync() is ImageSource imageSource)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return new ImageResult(stream.ToArray(), "image/png");
            }
        }

        return null;
    }
}
