using System.Threading.Tasks;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;

#if NETFX_CORE
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
#endif

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ClipboardService.
    /// </summary>
    public class ClipboardService : IClipboardService
    {
#if NETFX_CORE
        /// <summary>
        /// Gets the bitmap source from clipboard asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public async Task<object> GetBitmapSourceFromClipboardAsync()
        {
            var dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                var imageReceived = await dataPackageView.GetBitmapAsync();

                if (imageReceived != null)
                {
                    using var imageStream = await imageReceived.OpenReadAsync();
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(imageStream);
                    return bitmapImage;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the byte array from clipboard image asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public async Task<byte[]> GetByteArrayFromClipboardImageAsync(ImageFormats format)
        {
            byte[] result = null;

            var dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Bitmap) && await dataPackageView.GetBitmapAsync() is RandomAccessStreamReference imageReceived)
            {
                using (var imageStream = await imageReceived.OpenReadAsync())
                {
                    var decoder = await BitmapDecoder.CreateAsync(imageStream);
                    var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    using var memoryStream = new InMemoryRandomAccessStream();

                    BitmapEncoder encoder = null;

                    switch (format)
                    {
                        case ImageFormats.bmp:
                            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, memoryStream);
                            break;
                        case ImageFormats.gif:
                            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.GifEncoderId, memoryStream);
                            break;
                        case ImageFormats.jpg:
                            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, memoryStream);
                            break;
                        case ImageFormats.jpgXr:
                            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegXREncoderId, memoryStream);
                            break;
                        case ImageFormats.png:
                            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream);
                            break;
                        case ImageFormats.tiff:
                            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.TiffEncoderId, memoryStream);
                            break;
                        case ImageFormats.heif:
                            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.HeifEncoderId, memoryStream);
                            break;
                    }

                    try
                    {
                        if (encoder is null)
                            return Array.Empty<byte>();

                        encoder.SetSoftwareBitmap(softwareBitmap);

                        await encoder.FlushAsync();
                    }
                    catch (Exception)
                    {
                        return Array.Empty<byte>();
                    }

                    result = new byte[memoryStream.Size];
                    await memoryStream.ReadAsync(result.AsBuffer(), (uint)memoryStream.Size, InputStreamOptions.None);
                };
            }

            return result;
        }
#else
        /// <summary>
        /// Gets the bitmap source from clipboard asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<object> GetBitmapSourceFromClipboardAsync() =>
            throw new System.NotImplementedException();

        /// <summary>
        /// Gets the byte array from clipboard image asynchronous.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<byte[]> GetByteArrayFromClipboardImageAsync(ImageFormats format) =>
            throw new System.NotImplementedException();
#endif
    }
}
