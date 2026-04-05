using Amazon.Runtime;
using Amazon.S3;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using ISynergy.Framework.EventSourcing.Storage.S3.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Storage.S3.Tests.Extensions;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    private static void RegisterFakeClient(IServiceCollection services)
    {
        var credentials = new BasicAWSCredentials("fake-key", "fake-secret");
        var config = new AmazonS3Config { ServiceURL = "http://localhost:9999", ForcePathStyle = true };
        services.AddSingleton(new AmazonS3Client(credentials, config));
    }

    // ------------------------------------------------------------------ //
    // Null / empty guard tests
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddS3EventArchiveStorage_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        Assert.Throws<ArgumentNullException>(() =>
            services.AddS3EventArchiveStorage("archive"));
    }

    [TestMethod]
    public void AddS3EventArchiveStorage_EmptyBucketNamePrefix_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() =>
            services.AddS3EventArchiveStorage(""));
    }

    // ------------------------------------------------------------------ //
    // Registration tests
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddS3EventArchiveStorage_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        RegisterFakeClient(services);

        var result = services.AddS3EventArchiveStorage("archive");

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddS3EventArchiveStorage_RegistersIEventArchiveStorageAsScoped()
    {
        var services = new ServiceCollection();
        RegisterFakeClient(services);
        services.AddS3EventArchiveStorage("archive");

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEventArchiveStorage));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddS3EventArchiveStorage_ResolvesIEventArchiveStorage()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        RegisterFakeClient(services);
        services.AddS3EventArchiveStorage("archive");

        var scope = services.BuildServiceProvider().CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<IEventArchiveStorage>();

        Assert.IsNotNull(storage);
        Assert.IsInstanceOfType<IEventArchiveStorage>(storage);
    }
}
