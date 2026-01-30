namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Stream extensions
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Convert stream to byte array.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this Stream stream)
    {
        using (var ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Convert stream to byte array asynchronous.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
    {
        using (var ms = new MemoryStream())
        {
            await stream.CopyToAsync(ms).ConfigureAwait(false);
            return ms.ToArray();
        }
    }
}
