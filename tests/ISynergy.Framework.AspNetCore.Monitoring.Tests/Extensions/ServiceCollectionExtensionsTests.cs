using ISynergy.Framework.AspNetCore.Monitoring.Extensions;
using ISynergy.Framework.AspNetCore.Monitoring.Hubs;
using ISynergy.Framework.Monitoring.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.AspNetCore.Monitoring.Tests.Extensions;

[TestClass()]
public class ServiceCollectionExtensionsTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceCollection _services;

    public ServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();
        _services.AddMonitorSignalR<object>();
        _serviceProvider = _services.BuildServiceProvider();
    }

    [TestMethod()]
    public void AddMonitorSignalRTest()
    {
        Assert.IsNotNull(_serviceProvider.GetRequiredService<IMonitorService<object>>());
        Assert.IsNotNull(_serviceProvider.GetRequiredService<MonitorHub>());
    }
}