using System.IO.Compression;

#if !NET35 && !NET40
namespace ISynergy.Framework.Mathematics.IO.NumPy;

public static partial class NpzFormat
{
    private const CompressionLevel DEFAULT_COMPRESSION = CompressionLevel.Fastest;

    /// <summary>
    ///     Saves the specified arrays to an array of bytes.
    /// </summary>
    /// <param name="arrays">The arrays to be saved to the array of bytes.</param>
    /// <param name="compression">The compression level to use when compressing the array.</param>
    /// <returns>A byte array containig the saved arrays.</returns>
    public static byte[] Save(Dictionary<string, Array> arrays, CompressionLevel compression = DEFAULT_COMPRESSION)
    {
        using (var stream = new MemoryStream())
        {
            Save(arrays, stream, compression, true);
            return stream.ToArray();
        }
    }

    /// <summary>
    ///     Saves the specified array to an array of bytes.
    /// </summary>
    /// <param name="array">The array to be saved to the array of bytes.</param>
    /// <param name="compression">The compression level to use when compressing the array.</param>
    /// <returns>A byte array containig the saved array.</returns>
    public static byte[] Save(Array array, CompressionLevel compression = DEFAULT_COMPRESSION)
    {
        using (var stream = new MemoryStream())
        {
            Save(array, stream, compression, true);
            return stream.ToArray();
        }
    }
    /// <summary>
    ///     Saves the specified arrays to a file in the disk.
    /// </summary>
    /// <param name="arrays">The arrays to be saved to disk.</param>
    /// <param name="path">The disk path under which the file will be saved.</param>
    /// <param name="compression">The compression level to use when compressing the array.</param>
    public static void Save(Dictionary<string, Array> arrays, string path,
        CompressionLevel compression = DEFAULT_COMPRESSION)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            Save(arrays, stream, compression);
        }
    }

    /// <summary>
    ///     Saves the specified array to a file in the disk.
    /// </summary>
    /// <param name="array">The array to be saved to disk.</param>
    /// <param name="path">The disk path under which the file will be saved.</param>
    /// <param name="compression">The compression level to use when compressing the array.</param>
    public static void Save(Array array, string path, CompressionLevel compression = DEFAULT_COMPRESSION)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            Save(array, stream, compression);
        }
    }
    /// <summary>
    ///     Saves the specified arrays to a file in the disk.
    /// </summary>
    /// <param name="arrays">The arrays to be saved to disk.</param>
    /// <param name="stream">The stream to which the file will be saved.</param>
    /// <param name="compression">The compression level to use when compressing the array.</param>
    /// <param name="leaveOpen">True to leave the stream opened after the file is saved; false otherwise.</param>
    public static void Save(Dictionary<string, Array> arrays, Stream stream,
        CompressionLevel compression = DEFAULT_COMPRESSION, bool leaveOpen = false)
    {
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            foreach (var p in arrays)
            {
                var entry = zip.CreateEntry(p.Key, compression);
                NpyFormat.Save(p.Value, entry.Open());
            }
        }
    }

    /// <summary>
    ///     Saves the specified array to a stream.
    /// </summary>
    /// <param name="array">The array to be saved to disk.</param>
    /// <param name="stream">The stream to which the file will be saved.</param>
    /// <param name="compression">The compression level to use when compressing the array.</param>
    /// <param name="leaveOpen">True to leave the stream opened after the file is saved; false otherwise.</param>
    public static void Save(Array array, Stream stream, CompressionLevel compression = DEFAULT_COMPRESSION,
        bool leaveOpen = false)
    {
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen))
        {
            var entry = zip.CreateEntry("arr_0");
            NpyFormat.Save(array, entry.Open());
        }
    }
}
#endif