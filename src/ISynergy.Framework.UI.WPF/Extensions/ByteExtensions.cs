using ISynergy.Framework.Core.Extensions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISynergy.Framework.UI.Extensions;

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
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.BeginInit();
        bitmap.StreamSource = _self.ToMemoryStream();
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}
