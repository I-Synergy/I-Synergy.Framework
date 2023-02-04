using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Windows.Storage;

#if WINDOWS10_0_18362_0_OR_GREATER && !HAS_UNO
using Windows.System;
#else
using System.Diagnostics;
#endif

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class DownloadFileService.
    /// </summary>
    public class DownloadFileService : IDownloadFileService
    {
        /// <summary>
        /// The file service
        /// </summary>
        private readonly IFileService<FileResult> _fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileService"/> class.
        /// </summary>
        /// <param name="fileservice">The fileservice.</param>
        public DownloadFileService(IFileService<FileResult> fileservice)
        {
            _fileService = fileservice;
        }

        /// <summary>
        /// download file as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="filename">The filename.</param>
        public async Task DownloadFileAsync(string filename, byte[] file)
        {
            if (await _fileService.SaveFileAsync(filename, file) is FileResult savedFile)
            {
#if WINDOWS10_0_18362_0_OR_GREATER && !HAS_UNO
                var storageFile = await StorageFile.GetFileFromPathAsync(savedFile.FilePath);
                await Launcher.LaunchFileAsync(storageFile);
#else
                Process.Start(savedFile.FilePath);
#endif
            }
        }
    }
}
