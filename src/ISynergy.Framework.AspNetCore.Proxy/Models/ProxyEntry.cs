using Microsoft.AspNetCore.Http;

namespace ISynergy.Framework.AspNetCore.Proxy.Models;

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
    /// Endpoints of swagger documentation.
    /// </summary>
    /// <value>The swagger endpoint.</value>
    public List<SwaggerEndpoint> SwaggerEndpoints { get; set; }

    /// <summary>
    /// List of allowed HttpMethod.
    /// </summary>
    /// <value>The allowed methods.</value>
    public List<string> AllowedMethods { get; set; }
}
