namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IDownloadFileService
    /// </summary>
    public interface IDownloadFileService
    {
        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="file">The file.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Task.</returns>
        Task DownloadFileAsync(string folder, string filename, byte[] file);
    }
}
