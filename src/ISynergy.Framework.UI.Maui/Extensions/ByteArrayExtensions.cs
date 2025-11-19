using ISynergy.Framework.Core.Extensions;

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
        return ImageSource.FromStream(() =>
        {
            if (_self.ToMemoryStream() is { } stream)
                return stream;

            return null;
        });
    }
}
