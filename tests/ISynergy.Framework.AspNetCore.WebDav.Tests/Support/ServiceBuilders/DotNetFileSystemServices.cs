using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Extensions;
using ISynergy.Framework.AspNetCore.WebDav.FileSystem.DotNet;
using ISynergy.Framework.AspNetCore.WebDav.Locking.InMemory;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;
using ISynergy.Framework.AspNetCore.WebDav.Storage.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders
{
    public class DotNetFileSystemServices : IFileSystemServices, IDisposable
    {
        private readonly ConcurrentBag<string> _tempDbRootPaths = new ConcurrentBag<string>();

        public DotNetFileSystemServices()
        {
            var serviceCollection = new ServiceCollection()
                .AddOptions()
                .AddLogging()
                .Configure<InMemoryLockManagerOptions>(opt =>
                {
                    opt.Rounding = new DefaultLockTimeRounding(DefaultLockTimeRoundingMode.OneHundredMilliseconds);
                })
                .AddScoped<ILockManager, InMemoryLockManager>()
                .AddScoped<IDeadPropertyFactory, DeadPropertyFactory>()
                .AddScoped<IWebDavContext>(sp => new TestHost(sp, new Uri("http://localhost/")))
                .AddScoped<IFileSystemFactory>(
                    sp =>
                    {
                        var tempRootPath = Path.Combine(
                            Path.GetTempPath(),
                            "webdavserver-dotnet-tests",
                            Guid.NewGuid().ToString("N"));
                        Directory.CreateDirectory(tempRootPath);
                        _tempDbRootPaths.Add(tempRootPath);

                        var opt = new DotNetFileSystemOptions
                        {
                            RootPath = tempRootPath,
                        };
                        var pte = sp.GetRequiredService<IPathTraversalEngine>();
                        var psf = sp.GetService<IPropertyStoreFactory>();
                        var lm = sp.GetService<ILockManager>();

                        var fsf = new DotNetFileSystemFactory(
                            new OptionsWrapper<DotNetFileSystemOptions>(opt),
                            pte,
                            psf,
                            lm);
                        return fsf;
                    })
                .AddSingleton<IPropertyStoreFactory, InMemoryPropertyStoreFactory>()
                .AddWebDav();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddDebug(LogLevel.Trace);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            foreach (var tempDbRootPath in _tempDbRootPaths.Where(Directory.Exists))
            {
                Directory.Delete(tempDbRootPath, true);
            }
        }
    }
}
