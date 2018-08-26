using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public enum StorageTypes
    {
        Document,
        Image,
        Group,
        MainGroup,
        Template
    }

    public interface ICloudStorageService
    {
        Task<string> UploadFileAsync(byte[] file, int fileTypeId, string filename, StorageTypes type, CancellationToken cancellationToken = default(CancellationToken));
        Task<byte[]> DownloadFileAsync(string filename, StorageTypes type, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> UpdateFileAsync(byte[] file, int fileTypeId, string filename, StorageTypes type, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> RemoveFileAsync(string filename, StorageTypes type, CancellationToken cancellationToken = default(CancellationToken));
    }
}
