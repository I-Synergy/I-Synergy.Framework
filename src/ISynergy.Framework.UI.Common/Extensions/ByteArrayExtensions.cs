#if WINDOWS_UWP
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#else
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#endif

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Class ByteArrayExtensions.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts the byte array to an image source.
        /// </summary>
        /// <param name="_self">The image.</param>
        /// <returns>ImageSource.</returns>

        public static ImageSource ToImageSource(this byte[] _self)
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(_self.ToMemoryStream().AsRandomAccessStream());
            return bitmap;
        }
    }
}
