using Sample.Abstractions.Services;
using System.Threading.Tasks;

namespace Sample.Services
{
    /// <summary>
    /// Class ClientMonitorService.
    /// </summary>
    public class ClientMonitorService : IClientMonitorService
    {
        /// <summary>
        /// Connects the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task ConnectAsync(string token)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Disconnects the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task DisconnectAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
