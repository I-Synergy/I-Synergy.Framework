using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.Storage;
using Windows.System;

namespace ISynergy.Framework.Windows.Samples.Services
{
    /// <summary>
    /// Class DownloadFileService.
    /// </summary>
    public class DownloadFileService : IDownloadFileService
    {
        /// <summary>
        /// The file service
        /// </summary>
        protected readonly IFileService FileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileService"/> class.
        /// </summary>
        /// <param name="fileservice">The fileservice.</param>
        public DownloadFileService(IFileService fileservice)
        {
            FileService = fileservice;
        }

        /// <summary>
        /// download file as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="filefilter">The filefilter.</param>
        public async Task DownloadFileAsync(byte[] file, string filename, string filefilter)
        {
            var savedfile = (StorageFile)await FileService.SaveFileAsync(filefilter, filename);
            await FileIO.WriteBytesAsync(savedfile, file);
            await Launcher.LaunchFileAsync(savedfile);
        }
    }
}
