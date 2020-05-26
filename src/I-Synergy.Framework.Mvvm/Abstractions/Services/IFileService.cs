using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Class FileResult.
    /// </summary>
    public class FileResult
    {
        /// <summary>
        /// Gets or sets the file type identifier.
        /// </summary>
        /// <value>The file type identifier.</value>
        public int FileTypeId { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public byte[] File { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public Uri Url { get; set; }
    }

    /// <summary>
    /// Interface IFileService
    /// Implements the <see cref="IFileSupport" />
    /// </summary>
    /// <seealso cref="IFileSupport" />
    public interface IFileService : IFileSupport
    {
        /// <summary>
        /// Browses the file asynchronous.
        /// </summary>
        /// <param name="filefilter">The filefilter.</param>
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>Task&lt;FileResult&gt;.</returns>
        Task<FileResult> BrowseFileAsync(string filefilter, ulong maxfilesize);
        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <param name="filefilter">The filefilter.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        Task<object> SaveFileAsync(string filefilter, string filename);
    }
}
