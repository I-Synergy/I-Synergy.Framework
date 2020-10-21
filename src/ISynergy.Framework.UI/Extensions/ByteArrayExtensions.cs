using ISynergy.Framework.Core.Extensions;
using Windows.UI.Xaml.Media;

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
        public static ImageSource ToImageSource(this byte[] _self) =>
            new ImageSource(_self.ToMemoryStream());
    }
}
