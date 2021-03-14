using ISynergy.Framework.Core.Extensions;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

#if NETFX_CORE
using System.IO;
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
#if NETFX_CORE
            bitmap.SetSource(_self.ToMemoryStream().AsRandomAccessStream());
#else
            bitmap.SetSource(_self.ToMemoryStream());
#endif
            return bitmap;
        }
    }
}
