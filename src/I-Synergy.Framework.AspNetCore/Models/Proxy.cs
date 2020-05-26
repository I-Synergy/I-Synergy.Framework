using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ISynergy.Framework.AspNetCore.Models
{
    /// <summary>
    /// Proxy class with mapping properties.
    /// </summary>
    public class Proxy
    {
        /// <summary>
        /// Path request message.
        /// </summary>
        public List<PathString> SourcePaths { get; set; }

        /// <summary>
        /// Uri of proxied destination.
        /// </summary>
        public Uri DestinationUri { get; set; }

        /// <summary>
        /// Description of destination endpoint.
        /// </summary>
        public string SwaggerDescription { get; set; }

        /// <summary>
        /// Endpoint of swagger documentation.
        /// </summary>
        public Uri SwaggerEndpoint { get; set; }

        /// <summary>
        /// List of allowed HttpMethod.
        /// </summary>
        public List<string> AllowedMethods { get; set; }
    }
}
