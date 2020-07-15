using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.Framework.Windows.Helpers
{
    /// <summary>
    /// Class ImageHelper.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// image from clipboard image as an asynchronous operation.
        /// </summary>
        /// <returns>BitmapSource.</returns>
        public static async Task<BitmapSource> ImageFromClipboardImageAsync()
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
        /// PNG byte array from clipboard image as an asynchronous operation.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        public static async Task<byte[]> PngByteArrayFromClipboardImageAsync()
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
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream);
                    encoder.SetSoftwareBitmap(softwareBitmap);

                    try
                    {
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

        /// <summary>
        /// Converts the color of the integer2.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>Color.</returns>
        public static Color ConvertInteger2Color(int color)
        {
            var bytes = BitConverter.GetBytes(color);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        /// <summary>
        /// Converts the color2 integer.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>System.Int32.</returns>
        public static int ConvertColor2Integer(Color color)
        {
            var bytes = new byte[] { 255, color.R, color.G, color.B };

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Converts the byte array2 image source asynchronous.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>ImageSource.</returns>
        public static ImageSource ConvertByteArray2ImageSourceAsync(byte[] image)
        {
            ImageSource result = null;

            if (image != null && image.Length > 0)
            {
                var stream = new MemoryStream();
                stream.Write(image, 0, image.Length);
                stream.Position = 0;

                var bitmap = new BitmapImage();
                bitmap.SetSource(stream.AsRandomAccessStream());
                result = bitmap;
            }

            return result;
        }

        /// <summary>
        /// convert image source2 byte array as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>System.Byte[].</returns>
        public static async Task<byte[]> ConvertImageSource2ByteArrayAsync(Uri uri)
        {
            byte[] result = null;

            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                result = new byte[readStream.Length];
                await readStream.ReadAsync(result, 0, result.Length);
            }

            return result;
        }
    }
}
