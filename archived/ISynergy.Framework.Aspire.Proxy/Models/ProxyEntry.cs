using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Yarp.ReverseProxy.Configuration;

namespace ISynergy.Framework.Aspire.Proxy.Models;

/// <summary>
/// Proxy class with mapping properties.
/// </summary>
public class ProxyEntry(string name) : Resource(name), IResourceWithServiceDiscovery, IResourceWithEnvironment
{
    internal Dictionary<string, RouteConfig> RouteConfigs { get; } = [];
    internal Dictionary<string, ClusterConfig> ClusterConfigs { get; } = [];
    internal List<EndpointAnnotation> Endpoints { get; } = [];
    internal string ConfigurationSectionName { get; set; }
}
