using ISynergy.Framework.Core.Extensions;
using System;
using System.IO;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

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
        if (_self is not null && _self != Array.Empty<byte>())
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(_self.ToMemoryStream().AsRandomAccessStream());
            return bitmap;
        }
        
        return null;
    }
}
