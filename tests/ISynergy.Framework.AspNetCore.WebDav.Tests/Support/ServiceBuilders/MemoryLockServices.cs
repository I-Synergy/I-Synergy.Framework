using System;
using ISynergy.Framework.AspNetCore.WebDav.Locking.InMemory;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders
{
    public class MemoryLockServices : ILockServices
    {
        public MemoryLockServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.AddLogging();
            serviceCollection.AddScoped<ISystemClock, TestSystemClock>();
            serviceCollection.Configure<InMemoryLockManagerOptions>(opt =>
            {
                opt.Rounding = new DefaultLockTimeRounding(DefaultLockTimeRoundingMode.OneHundredMilliseconds);
            });
            serviceCollection.AddTransient<ILockCleanupTask, LockCleanupTask>();
            serviceCollection.AddTransient<ILockManager, InMemoryLockManager>();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddDebug(LogLevel.Trace);
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
