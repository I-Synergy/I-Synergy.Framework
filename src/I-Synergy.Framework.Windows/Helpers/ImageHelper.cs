using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.Helpers
{
    public class ImageHelper
    {
        public static async Task<BitmapSource> ImageFromClipboardAsync()
        {
            var dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                IRandomAccessStreamReference imageReceived = null;
                imageReceived = await dataPackageView.GetBitmapAsync();

                if (imageReceived != null)
                {
                    using (var imageStream = await imageReceived.OpenReadAsync())
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(imageStream);
                        return bitmapImage;
                    }
                }
            }

            return null;
        }

        public static Color ConvertInteger2Color(int color)
        {
            byte[] bytes = BitConverter.GetBytes(color);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static int ConvertColor2Integer(Color color)
        {
            byte[] bytes = new byte[] { 255, color.R, color.G, color.B };

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            
            return BitConverter.ToInt32(bytes, 0);
        }

        public static ImageSource ConvertByteArray2ImageSourceAsync(byte[] image)
        {
            ImageSource result = null;

            if (image != null && image.Length > 0)
            {
                MemoryStream stream = new MemoryStream();
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
