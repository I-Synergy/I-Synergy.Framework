using System.Threading.Tasks;
using ISynergy.Framework.Windows.Samples.Abstractions.Services;

namespace ISynergy.Framework.Windows.Samples.Services
{
    /// <summary>
    /// Class ClientMonitorService.
    /// </summary>
    public class ClientMonitorService : IClientMonitorService
    {
        public Task ConnectAsync(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task DisconnectAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
