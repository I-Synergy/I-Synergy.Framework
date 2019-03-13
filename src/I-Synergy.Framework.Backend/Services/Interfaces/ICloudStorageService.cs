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
        Task<string> UploadFileAsync(byte[] file, int fileTypeId, string filename, StorageTypes type);
        Task<byte[]> DownloadFileAsync(string filename, StorageTypes type);
        Task<string> UpdateFileAsync(byte[] file, int fileTypeId, string filename, StorageTypes type);
        Task<bool> RemoveFileAsync(string filename, StorageTypes type);
    }
}
