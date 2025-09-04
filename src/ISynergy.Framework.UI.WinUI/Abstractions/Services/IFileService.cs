namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Interface IFileService
/// </summary>
public interface IFileService<T>
{
    /// <summary>
    /// Saves the file asynchronous.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="filename">The filename.</param>
    /// <param name="file">The file.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<T?> SaveFileAsync(string folder, string filename, byte[] file);

    /// <summary>
    /// Browses the file asynchronous.
    /// </summary>
    /// <param name="filefilter">The filefilter.</param>
    /// <param name="multiple"></param>
    /// <param name="maxFileSize">Maximum filesize, default 1Mb (1 * 1024 * 1024)</param>
    /// <returns>Task&lt;FileResult&gt;.</returns>
    Task<List<T>> BrowseFileAsync(string filefilter, bool multiple = false, long maxFileSize = 1 * 1024 * 1024);

    /// <summary>
    /// Browses the image asynchronous.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="maxFileSize">Maximum filesize, default 1Mb (1 * 1024 * 1024)</param>
    /// <returns>Task&lt;System.Byte[]&gt;.</returns>
    Task<byte[]?> BrowseImageAsync(string[] filter, long maxFileSize = 1 * 1024 * 1024);

    /// <summary>
    /// UWP implementation of OpenFile(), opening a file already stored in the app's local
    /// folder directory.
    /// storage.
    /// </summary>
    /// <param name="fileToOpen">relative filename of file to open</param>
    Task OpenFileAsync(string fileToOpen);
}
