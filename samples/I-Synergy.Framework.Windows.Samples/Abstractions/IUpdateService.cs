using System.Threading.Tasks;

namespace ISynergy.Framework.Windows.Samples.Abstractions.Services
{
    /// <summary>
    /// Interface IUpdateService
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// Checks for update asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CheckForUpdateAsync();
        /// <summary>
        /// Downloads the and install update asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task DownloadAndInstallUpdateAsync();
    }
}
