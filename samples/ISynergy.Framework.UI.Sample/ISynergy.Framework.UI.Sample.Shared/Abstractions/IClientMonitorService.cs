using System.Threading.Tasks;

namespace ISynergy.Framework.UI.Sample.Abstractions.Services
{
    /// <summary>
    /// Interface IClientMonitorService
    /// </summary>
    public interface IClientMonitorService
    {
        /// <summary>
        /// Connects the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        Task ConnectAsync(string token);
        /// <summary>
        /// Disconnects the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task DisconnectAsync();
    }
}
