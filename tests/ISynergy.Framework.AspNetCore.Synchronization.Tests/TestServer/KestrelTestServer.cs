﻿using ISynergy.Framework.AspNetCore.Synchronization.Extensions;
using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using ISynergy.Framework.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Synchronization.Tests.TestServer
{
    public delegate Task ResponseDelegate(string serviceUri);

    /// <summary>
    /// This is a test server for Kestrell
    /// Actually we can use Microsoft.AspNetCore.TestHost
    /// But I can't manage to find a way to perform through Fiddler
    /// </summary>
    public class KestrelTestServer : IDisposable
    {
        IWebHostBuilder builder;
        private bool useFiddler;
        IWebHost host;

        public KestrelTestServer(WebServerOrchestrator webServerOrchestrator, bool useFidller = false)
        {
            var versionService = new VersionService(Assembly.GetAssembly(typeof(KestrelTestServer)));

            var hostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://127.0.0.1:0/")
                .ConfigureServices(services =>
                {
                    services.AddDistributedMemoryCache();
                    services.AddSession(options =>
                    {
                        // Set a long timeout for easy testing.
                        options.IdleTimeout = TimeSpan.FromDays(10);
                        options.Cookie.HttpOnly = true;
                    });

                    // add a SqlSyncProvider acting as the server hub
                    services.AddSyncServer(versionService, webServerOrchestrator);

                });
            this.builder = hostBuilder;
            this.useFiddler = useFidller;
        }


        public string Run()
        {
            // Create server web proxy
            var serverHandler = new RequestDelegate(async context =>
            {
                var webServerOrchestrator = context.RequestServices.GetService(typeof(WebServerOrchestrator)) as WebServerOrchestrator;
                await webServerOrchestrator.HandleRequestAsync(context);
            });


            this.builder.Configure(app =>
            {
                app.UseSession();
                app.Run(async context => await serverHandler(context));
            });

            var fiddler = useFiddler ? ".fiddler" : "";

            this.host = this.builder.Build();
            this.host.Start();
            var serviceUrl = $"http://localhost{fiddler}:{this.host.GetPort()}/";

            return serviceUrl;
        }

        public async void Dispose()
        {
            await this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async Task Dispose(bool cleanup)
        {
            if (this.host is not null)
            {
                await this.host.StopAsync();
                this.host.Dispose();
            }
        }

        internal Task StopAsync() => this.Dispose(true);
    }

    public static class IWebHostPortExtensions
    {
        public static string GetHost(this IWebHost host, bool isHttps = false)
        {
            return host.GetUri(isHttps).Host;
        }

        public static int GetPort(this IWebHost host)
        {
            return host.GetPorts().First();
        }

        public static int GetPort(this IWebHost host, string scheme)
        {
            return host.GetUris()
                .Where(u => u.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Port)
                .First();
        }

        public static IEnumerable<int> GetPorts(this IWebHost host)
        {
            return host.GetUris()
                .Select(u => u.Port);
        }

        public static IEnumerable<Uri> GetUris(this IWebHost host)
        {
            return host.ServerFeatures.Get<IServerAddressesFeature>().Addresses
                .Select(a => new Uri(a));
        }

        public static Uri GetUri(this IWebHost host, bool isHttps = false)
        {
            var uri = host.GetUris().First();

            if (isHttps && uri.Scheme == "http")
            {
                var uriBuilder = new UriBuilder(uri)
                {
                    Scheme = "https",
                };

                if (uri.Port == 80)
                    uriBuilder.Port = 443;

                return uriBuilder.Uri;
            }

            return uri;
        }
    }
}
