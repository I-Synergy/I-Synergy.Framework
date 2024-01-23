using System.Drawing;
using System.Drawing.Imaging;

namespace ISynergy.Framework.Core.Extensions;

public static class BitmapExtensions
{
    /// <summary>
    /// Converts a bitmap byte array to any image format (as byte array).
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="quality"></param>
    /// <param name="imageFormat"></param>
    /// <returns></returns>
    public static byte[] ToImageBytes(this byte[] bitmap, long quality, ImageFormat imageFormat)
    {
        var result = new MemoryStream();
        var encoder = GetEncoder(imageFormat);
        var encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

        if (encoder is null)
            throw new ArgumentException("Unknown image format type.");

        using var image = Image.FromStream(new MemoryStream(bitmap));
        image.Save(result, encoder, encoderParameters);

        return result.ToArray();
    }

    /// <summary>
    /// Gets Encoder from ImageFormat.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    private static ImageCodecInfo GetEncoder(ImageFormat format) =>
        ImageCodecInfo.GetImageDecoders().SingleOrDefault(codec => codec.FormatID == format.Guid);
}
