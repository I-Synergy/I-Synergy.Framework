using Azure.Storage.Blobs;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using ISynergy.Framework.EventSourcing.Storage.Azure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Storage.Azure.Tests.Extensions;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    // ------------------------------------------------------------------ //
    // Registration tests
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddAzureEventArchiveStorage_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(new BlobServiceClient("UseDevelopmentStorage=true"));

        var result = services.AddAzureEventArchiveStorage();

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddAzureEventArchiveStorage_RegistersIEventArchiveStorageAsScoped()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(new BlobServiceClient("UseDevelopmentStorage=true"));

        services.AddAzureEventArchiveStorage();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEventArchiveStorage));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddAzureEventArchiveStorage_ResolvesIEventArchiveStorage()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(new BlobServiceClient("UseDevelopmentStorage=true"));
        services.AddAzureEventArchiveStorage();

        var scope = services.BuildServiceProvider().CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<IEventArchiveStorage>();

        Assert.IsNotNull(storage);
        Assert.IsInstanceOfType<IEventArchiveStorage>(storage);
    }
}
