using Flurl;
using Flurl.Http;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Comparers;
using ISynergy.Framework.Synchronization.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Files.Services
{
    public class FileSynchronizationService : IFileSynchronizationService
    {
        private readonly IFileSynchronizationOptions _options;
        private readonly FileComparer _comparer;
        
        private IEnumerable<FileInfoMetadata> _localFiles;
        private IEnumerable<FileInfoMetadata> _remoteFiles;

        private DirectoryInfo _localDirectory;
        private DirectoryInfo _remoteDirectory;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options"></param>
        public FileSynchronizationService(IFileSynchronizationOptions options)
        {
            _options = options;
            _comparer = new FileComparer();  
        }

        /// <summary>
        /// Synchronizes the set folder.
        /// </summary>
        /// <returns></returns>
        public async Task SynchronizeAsync()
        {
            _localDirectory = new DirectoryInfo(_options.SynchronizationFolderPath);
            _localFiles = _localDirectory
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Select(s => new FileInfoMetadata(_localDirectory.FullName, s))
                .ToList();

            _remoteDirectory = new DirectoryInfo(await new Url($"{_options.Host}{_options.SynchronizationFolderRoute}").GetStringAsync());
            _remoteFiles = await new Url($"{_options.Host}{_options.SynchronizationListRoute}").GetJsonAsync<List<FileInfoMetadata>>();

            await CopyFilesAsync();
            await DeleteFilesAsync();
        }

        /// <summary>
        /// Copies files that don't exist on the client side.
        /// </summary>
        /// <returns></returns>
        private async Task CopyFilesAsync()
        {
            foreach (var metadataFullName in _remoteFiles.Except(_localFiles, _comparer).Select(s => s.FullName))
            {
                var file = new FileInfo(metadataFullName.Replace(_remoteDirectory.FullName, _localDirectory.FullName));

                await new Url($"{_options.Host}{_options.SynchronizationDownloadRoute}")
                    .SetQueryParam(_options.SynchronizationDownloadParameter, metadataFullName)
                    .DownloadFileAsync(Path.GetDirectoryName(file.FullName));
            }
        }

        /// <summary>
        /// Deletes files that are not available on the remote side.
        /// </summary>
        /// <returns></returns>
        private Task DeleteFilesAsync()
        {
            foreach (var v in _localFiles.Except(_remoteFiles, _comparer))
                File.Delete(v.FullName);

            return Task.CompletedTask;
        }
    }
}
