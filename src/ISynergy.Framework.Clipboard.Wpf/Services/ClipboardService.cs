using ISynergy.Framework.Clipboard.Abstractions.Services;
using ISynergy.Framework.Clipboard.Wpf;
using ISynergy.Framework.Clipboard.Wpf.Converters;
using ISynergy.Framework.Core.Enumerations;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISynergy.Framework.Clipboard.Services
{
    /// <summary>
    /// Class ClipboardService.
    /// </summary>
    internal class ClipboardService : IClipboardService
    {
        /// <summary>
        /// Gets the bitmap source from clipboard asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> GetBitmapSourceFromClipboardAsync()
        {
            if (System.Windows.Clipboard.GetData("DeviceIndependentBitmap") is MemoryStream iMemoryStream)
            {
                byte[] dibBuffer = new byte[Convert.ToInt32(iMemoryStream.Length - 1) + 1];
                iMemoryStream.Read(dibBuffer, 0, dibBuffer.Length);

                BITMAPINFOHEADER iInfoHeader = BinaryStructConverter.FromByteArray<BITMAPINFOHEADER>(dibBuffer);

                int fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
                int infoHeaderSize = iInfoHeader.biSize;
                int fileSize = fileHeaderSize + iInfoHeader.biSize + iInfoHeader.biSizeImage;

                BITMAPFILEHEADER iFileHeader = new BITMAPFILEHEADER
                {
                    bfType = BITMAPFILEHEADER.BM,
                    bfSize = fileSize,
                    bfReserved1 = 0,
                    bfReserved2 = 0,
                    bfOffBits = fileHeaderSize + infoHeaderSize + iInfoHeader.biClrUsed * 4
                };

                byte[] fileHeaderBytes = BinaryStructConverter.ToByteArray<BITMAPFILEHEADER>(iFileHeader);

                MemoryStream iBitmapStream = new MemoryStream();
                iBitmapStream.Write(fileHeaderBytes, 0, fileHeaderSize);
                iBitmapStream.Write(dibBuffer, 0, dibBuffer.Length);
                iBitmapStream.Seek(0, SeekOrigin.Begin);

                return Task.FromResult<object>(BitmapFrame.Create(iBitmapStream));
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Gets the byte array from clipboard image asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public async Task<byte[]> GetByteArrayFromClipboardImageAsync(ImageFormats format)
        {
            if(await GetBitmapSourceFromClipboardAsync() is ImageSource imageSource)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));

                using (MemoryStream imageStream = new MemoryStream())
                {
                    encoder.Save(imageStream);
                    return imageStream.ToArray();
                }
            }

            return Array.Empty<byte>();
        }
    }
}
