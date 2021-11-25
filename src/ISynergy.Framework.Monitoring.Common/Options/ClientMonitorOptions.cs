using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ISynergy.Framework.Monitoring.Client.SignalR")]
namespace ISynergy.Framework.Monitoring.Common.Options
{
    /// <summary>
    /// Client monitor options.
    /// </summary>
    internal class ClientMonitorOptions
    {
        /// <summary>
        /// Endpoint url.
        /// </summary>
        public string EndpointUrl { get; set; }
    }
}
