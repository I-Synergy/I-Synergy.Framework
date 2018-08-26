using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IUpdateService
    {
        Task<bool> CheckForUpdateAsync();

        Task DownloadAndInstallUpdateAsync();
    }
}