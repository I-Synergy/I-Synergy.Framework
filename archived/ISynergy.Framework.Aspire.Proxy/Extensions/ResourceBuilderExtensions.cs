using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using ISynergy.Framework.Aspire.Proxy.Models;
using Yarp.ReverseProxy.Configuration;

namespace ISynergy.Framework.Aspire.Proxy.Extensions;
public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<ProxyEntry> LoadFromConfiguration(this IResourceBuilder<ProxyEntry> builder, string sectionName)
    {
        builder.Resource.ConfigurationSectionName = sectionName;
        return builder;
    }

    public static IResourceBuilder<ProxyEntry> Route(this IResourceBuilder<ProxyEntry> builder, string routeId, IResourceBuilder<IResourceWithServiceDiscovery> target, string path = null, string[] hosts = null, bool preservePath = false)
    {
        builder.Resource.RouteConfigs[routeId] = new()
        {
            RouteId = routeId,
            ClusterId = target.Resource.Name,
            Match = new()
            {
                Path = path,
                Hosts = hosts
            },
            Transforms =
            [
                preservePath || path is null
                ? []
                : new Dictionary<string, string>{ ["PathRemovePrefix"] = path }
            ]
        };

        if (builder.Resource.ClusterConfigs.ContainsKey(target.Resource.Name))
        {
            return builder;
        }

        builder.Resource.ClusterConfigs[target.Resource.Name] = new()
        {
            ClusterId = target.Resource.Name,
            Destinations = new Dictionary<string, DestinationConfig>
            {
                [target.Resource.Name] = new() { Address = $"http://{target.Resource.Name}" }
            }
        };

        builder.WithReference(target);

        return builder;
    }
}