using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public class FileResult
    {
        public int FileType_Id { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; }
    }

    public interface IFileService : IFileSupport
    {
        Task<FileResult> BrowseFileAsync(string filefilter, ulong maxfilesize, string maxfilesizestring);
        Task<object> SaveFileAsync(string filefilter, string filename);
        [Obsolete]Task<bool> SaveFileAsync(string filename);
    }
}