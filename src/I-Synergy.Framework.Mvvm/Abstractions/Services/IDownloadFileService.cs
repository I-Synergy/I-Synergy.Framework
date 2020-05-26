using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IDownloadFileService
    {
        Task DownloadFileAsync(byte[] file, string filename, string filefilter);
    }
}
