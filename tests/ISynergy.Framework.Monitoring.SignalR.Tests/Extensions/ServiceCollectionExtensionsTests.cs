using ISynergy.Framework.Monitoring.Abstractions.Services;
using ISynergy.Framework.Monitoring.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Monitoring.Extensions.Tests;

[TestClass()]
public class ServiceCollectionExtensionsTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public ServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();
        _configuration = new ConfigurationBuilder().Build();

        _services.AddMonitorSignalR<object>(_configuration);
        _serviceProvider = _services.BuildServiceProvider();
    }

    [TestMethod()]
    public void AddMonitorSignalRTest()
    {
        Assert.IsNotNull(_serviceProvider.GetRequiredService<IMonitorService<object>>());
        Assert.IsNotNull(_serviceProvider.GetRequiredService<MonitorHub>());
    }
}