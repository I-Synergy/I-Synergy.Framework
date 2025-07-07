using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Aspire.Proxy.Models;
internal class ProxyEntryLifecycleHook(
    DistributedApplicationExecutionContext executionContext,
    ResourceNotificationService resourceNotificationService,
    ResourceLoggerService resourceLoggerService) : IDistributedApplicationLifecycleHook, IAsyncDisposable
{
    private WebApplication _app;

    public async Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        if (executionContext.IsPublishMode)
        {
            return;
        }

        var proxyEntry = appModel.Resources.OfType<ProxyEntry>().SingleOrDefault();

        if (proxyEntry is null)
        {
            return;
        }

        await resourceNotificationService.PublishUpdateAsync(proxyEntry, s => s with
        {
            ResourceType = "Yarp",
            State = "Starting"
        });

        // We don't want to create proxies for yarp resources so remove them
        var bindings = proxyEntry.Annotations.OfType<EndpointAnnotation>().ToList();

        foreach (var b in bindings)
        {
            proxyEntry.Annotations.Remove(b);
            proxyEntry.Endpoints.Add(b);
        }
    }

    public async Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        if (executionContext.IsPublishMode)
        {
            return;
        }

        var proxyEntry = appModel.Resources.OfType<ProxyEntry>().SingleOrDefault();

        if (proxyEntry is null)
        {
            return;
        }

        var builder = WebApplication.CreateSlimBuilder();

        builder.Logging.ClearProviders();

        builder.Logging.AddProvider(new ResourceLoggerProvider(resourceLoggerService.GetLogger(proxyEntry.Name)));

        // Convert environment variables into configuration
        if (proxyEntry.TryGetEnvironmentVariables(out var envAnnotations))
        {
            var context = new EnvironmentCallbackContext(executionContext, cancellationToken: cancellationToken);

            foreach (var cb in envAnnotations)
            {
                await cb.Callback(context);
            }

            var dict = new Dictionary<string, string>();
            foreach (var (k, v) in context.EnvironmentVariables)
            {
                var val = v switch
                {
                    string s => s,
                    IValueProvider vp => await vp.GetValueAsync(context.CancellationToken),
                    _ => throw new NotSupportedException()
                };

                if (val is not null)
                {
                    dict[k.Replace("__", ":")] = val;
                }
            }

            builder.Configuration.AddInMemoryCollection(dict);
        }

        builder.Services.AddServiceDiscovery();

        var proxyBuilder = builder.Services.AddReverseProxy();

        if (proxyEntry.RouteConfigs.Count > 0)
        {
            proxyBuilder.LoadFromMemory(proxyEntry.RouteConfigs.Values.ToList(), proxyEntry.ClusterConfigs.Values.ToList());
        }

        if (proxyEntry.ConfigurationSectionName is not null)
        {
            proxyBuilder.LoadFromConfig(builder.Configuration.GetSection(proxyEntry.ConfigurationSectionName));
        }

        proxyBuilder.AddServiceDiscoveryDestinationResolver();

        _app = builder.Build();

        if (proxyEntry.Endpoints.Count == 0)
        {
            _app.Urls.Add($"http://127.0.0.1:0");
        }
        else
        {
            foreach (var ep in proxyEntry.Endpoints)
            {
                var scheme = ep.UriScheme ?? "http";

                if (ep.Port is null)
                {
                    _app.Urls.Add($"{scheme}://127.0.0.1:0");
                }
                else
                {
                    _app.Urls.Add($"{scheme}://localhost:{ep.Port}");
                }
            }
        }

        _app.MapReverseProxy();

        await _app.StartAsync(cancellationToken);

        var urls = _app.Services.GetRequiredService<IServer>().Features.GetRequiredFeature<IServerAddressesFeature>().Addresses;

        await resourceNotificationService.PublishUpdateAsync(proxyEntry, s => s with
        {
            State = "Running",
            Urls = [.. urls.Select(u => new UrlSnapshot(u, u, IsInternal: false))]
        });
    }

    public ValueTask DisposeAsync()
    {
        return _app?.DisposeAsync() ?? default;
    }

    private class ResourceLoggerProvider(ILogger logger) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ResourceLogger(logger);
        }

        public void Dispose()
        {
        }

        private class ResourceLogger(ILogger logger) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                return logger.BeginScope(state);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logger.IsEnabled(logLevel);
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }
}