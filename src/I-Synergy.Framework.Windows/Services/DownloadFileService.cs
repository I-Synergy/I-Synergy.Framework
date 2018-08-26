using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace ISynergy.Services
{
    public class DownloadFileService : IDownloadFileService
    {
        protected readonly IFileService FileService;

        public DownloadFileService(IFileService fileservice)
        {
            FileService = fileservice;
        }

        public async Task DownloadFileAsync(byte[] file, string filename, string filefilter)
        {
            StorageFile savedfile = (StorageFile)await FileService.SaveFileAsync(filefilter, filename);
            await FileIO.WriteBytesAsync(savedfile, file);
            await Launcher.LaunchFileAsync(savedfile);
        }
    }
}
