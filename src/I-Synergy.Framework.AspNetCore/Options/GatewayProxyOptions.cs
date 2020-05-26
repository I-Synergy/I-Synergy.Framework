using System.Collections.Generic;
using ISynergy.Framework.AspNetCore.Models;

namespace ISynergy.Framework.AspNetCore.Options
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
        public List<Proxy> GatewayProxies { get; set; }
    }
}
