using ISynergy.Framework.Core.Extensions;
using Windows.UI.Xaml.Media;

#if NETFX_CORE
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
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
#if NETFX_CORE
        public static ImageSource ToImageSource(this byte[] _self)
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(_self.ToMemoryStream().AsRandomAccessStream());
            return bitmap;
        }
#else
        public static ImageSource ToImageSource(this byte[] _self) =>
            new ImageSource(_self.ToMemoryStream());
#endif
    }
}
