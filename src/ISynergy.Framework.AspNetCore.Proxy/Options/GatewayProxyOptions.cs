using ISynergy.Framework.AspNetCore.Proxy.Models;
using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.Proxy.Options
{
    /// <summary>
    /// Options for gateway proxies.
    /// </summary>
    public class GatewayProxyOptions
    {
        /// <summary>
        /// List of <see cref="Proxy" />.
        /// </summary>
        /// <value>The gateway proxies.</value>
        public List<ProxyEntry> ProxyEntries { get; set; }
    }
}
