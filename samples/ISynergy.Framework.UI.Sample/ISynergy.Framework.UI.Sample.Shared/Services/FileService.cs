using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Services;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class FileService.
    /// </summary>
    public class FileService : BaseFileService, IFileService
    {
        public Task<FileResult> BrowseFileAsync(string filefilter, ulong maxfilesize)
        {
            throw new NotImplementedException();
        }

        public Task<object> SaveFileAsync(string filefilter, string filename)
        {
            throw new NotImplementedException();
        }
    }
}
