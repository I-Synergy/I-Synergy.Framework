using Azure.Core;
using Azure.Storage.Blobs;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.Azure.Extensions;
using ISynergy.Framework.Storage.Azure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Storage.Azure;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions.AddAzureStorageIntegration"/>.
/// </summary>
[TestClass]
public class AzureStorageRegistrationTests
{
    [TestMethod]
    public void AddAzureStorageIntegration_RegistersStorageServiceFromConfiguration()
    {
        var services = new ServiceCollection();
        services.AddAzureStorageIntegration(BuildConfiguration("UseDevelopmentStorage=true"));

        using var provider = services.BuildServiceProvider();
        var storage = provider.GetRequiredService<IStorageService>();

        Assert.IsInstanceOfType<AzureStorageService>(storage);
    }

    [TestMethod]
    public void AddAzureStorageIntegration_RegistersSuppliedCredential()
    {
        var credential = new FakeTokenCredential();
        var services = new ServiceCollection();
        services.AddAzureStorageIntegration(BuildConfiguration("UseDevelopmentStorage=true"), credential: credential);

        using var provider = services.BuildServiceProvider();

        Assert.AreSame(credential, provider.GetRequiredService<TokenCredential>());
    }

    [TestMethod]
    public void AddAzureStorageIntegration_Resolves_WhenAnAspireBlobClientIsRegistered()
    {
        var blobServiceClient = new BlobServiceClient(new Uri("https://injected.blob.core.windows.net"));
        var services = new ServiceCollection();
        services.AddSingleton(blobServiceClient);
        services.AddAzureStorageIntegration(BuildConfiguration("UseDevelopmentStorage=true"));

        using var provider = services.BuildServiceProvider();
        var storage = provider.GetRequiredService<IStorageService>();

        Assert.IsInstanceOfType<AzureStorageService>(storage);
    }

    [TestMethod]
    public void AddAzureStorageIntegration_Throws_WhenConfigurationIsNull()
    {
        var services = new ServiceCollection();

        Assert.ThrowsExactly<ArgumentNullException>(
            () => services.AddAzureStorageIntegration(configuration: null!));
    }

    private static IConfiguration BuildConfiguration(string connectionString)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureStorageOptions:ConnectionString"] = connectionString
            })
            .Build();

    private sealed class FakeTokenCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => new("fake-token", DateTimeOffset.MaxValue);

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(new AccessToken("fake-token", DateTimeOffset.MaxValue));
    }
}
