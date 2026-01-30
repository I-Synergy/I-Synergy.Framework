using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Extensions;
using ISynergy.Framework.AspNetCore.WebDav.FileSystem.SQLite;
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
    public class SQLiteFileSystemServices : IFileSystemServices, IDisposable
    {
        private readonly ConcurrentBag<string> _tempDbRootPaths = new ConcurrentBag<string>();

        public SQLiteFileSystemServices()
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
                .AddScoped<IFileSystemFactory, SQLiteFileSystemFactory>(
                    sp =>
                    {
                        var tempRootPath = Path.Combine(
                            Path.GetTempPath(),
                            "webdavserver-sqlite-tests",
                            Guid.NewGuid().ToString("N"));
                        Directory.CreateDirectory(tempRootPath);
                        _tempDbRootPaths.Add(tempRootPath);

                        var opt = new SQLiteFileSystemOptions
                        {
                            RootPath = tempRootPath,
                        };
                        var pte = sp.GetRequiredService<IPathTraversalEngine>();
                        var psf = sp.GetService<IPropertyStoreFactory>();
                        var lm = sp.GetService<ILockManager>();

                        var fsf = new SQLiteFileSystemFactory(
                            new OptionsWrapper<SQLiteFileSystemOptions>(opt),
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
