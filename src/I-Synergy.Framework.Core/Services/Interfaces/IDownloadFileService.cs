using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IDownloadFileService
    {
        Task DownloadFileAsync(byte[] file, string filename, string filefilter);
    }
}
