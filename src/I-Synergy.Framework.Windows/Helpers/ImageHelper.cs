using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.Framework.Windows.Helpers
{
    public static class ImageHelper
    {
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

                    using (var memoryStream = new InMemoryRandomAccessStream())
                    {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream);
                        encoder.SetSoftwareBitmap(softwareBitmap);

                        try
                        {
                            await encoder.FlushAsync();
                        }
                        catch (Exception ex)
                        { 
                            return Array.Empty<byte>(); 
                        }

                        result = new byte[memoryStream.Size];
                        await memoryStream.ReadAsync(result.AsBuffer(), (uint)memoryStream.Size, InputStreamOptions.None);
                    }
                };
            }

            return result;
        }

        public static Color ConvertInteger2Color(int color)
        {
            var bytes = BitConverter.GetBytes(color);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static int ConvertColor2Integer(Color color)
        {
            var bytes = new byte[] { 255, color.R, color.G, color.B };

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

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
