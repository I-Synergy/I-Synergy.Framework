using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Models;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IFileService
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; set; }
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        string Filter { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add extension].
        /// </summary>
        /// <value><c>true</c> if [add extension]; otherwise, <c>false</c>.</value>
        bool AddExtension { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [check file exists].
        /// </summary>
        /// <value><c>true</c> if [check file exists]; otherwise, <c>false</c>.</value>
        bool CheckFileExists { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [check path exists].
        /// </summary>
        /// <value><c>true</c> if [check path exists]; otherwise, <c>false</c>.</value>
        bool CheckPathExists { get; set; }
        /// <summary>
        /// Gets or sets the index of the filter.
        /// </summary>
        /// <value>The index of the filter.</value>
        int FilterIndex { get; set; }
        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        string InitialDirectory { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [validate names].
        /// </summary>
        /// <value><c>true</c> if [validate names]; otherwise, <c>false</c>.</value>
        bool ValidateNames { get; set; }


        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<FileResult> SaveFileAsync(string filename, byte[] file);

        /// <summary>
        /// Browses the file asynchronous.
        /// </summary>
        /// <param name="filefilter">The filefilter.</param>
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>Task&lt;FileResult&gt;.</returns>
        Task<FileResult> BrowseFileAsync(string filefilter, long maxfilesize);

        /// <summary>
        /// Browses the image asynchronous.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        Task<byte[]> BrowseImageAsync(string[] filter, long maxfilesize = 0);

        /// <summary>
        /// UWP implementation of OpenFile(), opening a file already stored in the app's local
        /// folder directory.
        /// storage.
        /// </summary>
        /// <param name="fileToOpen">relative filename of file to open</param>
        Task OpenFileAsync(string fileToOpen);
    }
}
