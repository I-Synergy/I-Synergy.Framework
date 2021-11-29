using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.Proxy.Models
{
    /// <summary>
    /// Proxy class with mapping properties.
    /// </summary>
    public class ProxyEntry
    {
        /// <summary>
        /// Path request message.
        /// </summary>
        /// <value>The source paths.</value>
        public List<PathString> SourcePaths { get; set; }

        /// <summary>
        /// Uri of proxied destination.
        /// </summary>
        /// <value>The destination URI.</value>
        public Uri DestinationUri { get; set; }

        /// <summary>
        /// Description of destination endpoint.
        /// </summary>
        /// <value>The swagger description.</value>
        public string SwaggerDescription { get; set; }

        /// <summary>
        /// Endpoint of swagger documentation.
        /// </summary>
        /// <value>The swagger endpoint.</value>
        public Uri SwaggerEndpoint { get; set; }

        /// <summary>
        /// List of allowed HttpMethod.
        /// </summary>
        /// <value>The allowed methods.</value>
        public List<string> AllowedMethods { get; set; }
    }
}
