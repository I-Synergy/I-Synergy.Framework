using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using ISynergy.Framework.Aspire.Proxy.Models;

namespace ISynergy.Framework.Aspire.Proxy.Extensions;

public static class DistributedApplicationExtensions
{
    public static IResourceBuilder<ProxyEntry> AddProxy(this IDistributedApplicationBuilder builder, string name)
    {
        var yarp = builder.Resources.OfType<ProxyEntry>().SingleOrDefault();

        if (yarp is not null)
        {
            // You only need one yarp resource per application
            throw new InvalidOperationException("A yarp resource has already been added to this application");
        }

        builder.Services.TryAddLifecycleHook<ProxyEntryLifecycleHook>();

        var resource = new ProxyEntry(name);
        return builder.AddResource(resource).ExcludeFromManifest();

        // REVIEW: YARP resource type?
        //.WithManifestPublishingCallback(context =>
        // {
        //     context.Writer.WriteString("type", "yarp.v0");

        //     context.Writer.WriteStartObject("routes");
        //     // REVIEW: Make this less YARP specific
        //     foreach (var r in resource.RouteConfigs.Values)
        //     {
        //         context.Writer.WriteStartObject(r.RouteId);

        //         context.Writer.WriteStartObject("match");
        //         context.Writer.WriteString("path", r.Match.Path);

        //         if (r.Match.Hosts is not null)
        //         {
        //             context.Writer.WriteStartArray("hosts");
        //             foreach (var h in r.Match.Hosts)
        //             {
        //                 context.Writer.WriteStringValue(h);
        //             }
        //             context.Writer.WriteEndArray();
        //         }
        //         context.Writer.WriteEndObject();
        //         context.Writer.WriteString("destination", r.ClusterId);
        //         context.Writer.WriteEndObject();
        //     }
        //     context.Writer.WriteEndObject();
        // });
    }
}

