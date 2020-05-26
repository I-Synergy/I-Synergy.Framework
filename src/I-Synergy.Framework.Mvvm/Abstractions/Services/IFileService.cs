using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public class FileResult
    {
        public int FileTypeId { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; }
        public Uri Url { get; set; }
    }

    public interface IFileService : IFileSupport
    {
        Task<FileResult> BrowseFileAsync(string filefilter, ulong maxfilesize);
        Task<object> SaveFileAsync(string filefilter, string filename);
    }
}
