using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.S3.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Storage.S3.Tests.Extensions;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    private static IConfiguration CreateValidConfig(string prefix = "") =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{prefix}S3StorageOptions:AccessKey"] = "fake-access-key",
                [$"{prefix}S3StorageOptions:SecretKey"] = "fake-secret-key",
                [$"{prefix}S3StorageOptions:ServiceUrl"] = "http://localhost:9999",
                [$"{prefix}S3StorageOptions:ForcePathStyle"] = "true"
            })
            .Build();

    // ------------------------------------------------------------------ //
    // Null guard tests
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddS3StorageIntegration_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        Assert.Throws<ArgumentNullException>(() =>
            services.AddS3StorageIntegration(CreateValidConfig()));
    }

    [TestMethod]
    public void AddS3StorageIntegration_NullConfiguration_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() =>
            services.AddS3StorageIntegration(null!));
    }

    // ------------------------------------------------------------------ //
    // Registration tests
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddS3StorageIntegration_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddS3StorageIntegration(CreateValidConfig());

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddS3StorageIntegration_RegistersIStorageServiceAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddS3StorageIntegration(CreateValidConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IStorageService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddS3StorageIntegration_CalledTwice_DoesNotDuplicate()
    {
        var services = new ServiceCollection();
        services.AddS3StorageIntegration(CreateValidConfig());
        services.AddS3StorageIntegration(CreateValidConfig());

        var count = services.Count(d => d.ServiceType == typeof(IStorageService));

        // TryAddSingleton: only one registration survives.
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public void AddS3StorageIntegration_WithPrefix_ReturnsSameServiceCollection()
    {
        var config = CreateValidConfig("MyPrefix:");
        var services = new ServiceCollection();

        var result = services.AddS3StorageIntegration(config, "MyPrefix:");

        Assert.AreSame(services, result);
    }
}
