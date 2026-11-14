using Azure.Core;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.ApplicationInsights.Extensions;
using ISynergy.Framework.OpenTelemetry.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenTelemetry;

namespace ISynergy.Framework.OpenTelemetry.Exporters.Tests;

/// <summary>
/// Tests for identity-based credential wiring in <see cref="ApplicationInsightsTelemetryExtensions"/>.
/// </summary>
[TestClass]
public class ApplicationInsightsTelemetryExtensionsTests
{
    private const string ConnectionString =
        "InstrumentationKey=00000000-0000-0000-0000-000000000000;" +
        "IngestionEndpoint=https://test.in.applicationinsights.azure.com/;" +
        "LiveEndpoint=https://test.livediagnostics.monitor.azure.com/";

    [TestMethod]
    public void AddApplicationInsightsExporter_UsesSuppliedCredential()
    {
        var credential = new FakeTokenCredential();
        AzureMonitorExporterOptions? captured = null;
        var builder = CreateBuilder(new() { ["Telemetry:ConnectionString"] = ConnectionString });

        var result = builder.AddApplicationInsightsExporter(options => captured = options, credential);

        Assert.AreSame(builder, result);
        Assert.IsNotNull(captured);
        Assert.AreSame(credential, captured.Credential);
    }

    [TestMethod]
    public void AddApplicationInsightsExporter_CreatesManagedIdentityCredential_WhenConfiguredWithClientId()
    {
        AzureMonitorExporterOptions? captured = null;
        var builder = CreateBuilder(new()
        {
            ["Telemetry:ConnectionString"] = ConnectionString,
            ["Telemetry:UseManagedIdentity"] = "true",
            ["Telemetry:ManagedIdentityClientId"] = Guid.NewGuid().ToString()
        });

        builder.AddApplicationInsightsExporter(options => captured = options);

        Assert.IsNotNull(captured);
        Assert.IsInstanceOfType<ManagedIdentityCredential>(captured.Credential);
    }

    [TestMethod]
    public void AddApplicationInsightsExporter_CreatesDefaultCredential_WhenUseManagedIdentityHasNoClientId()
    {
        AzureMonitorExporterOptions? captured = null;
        var builder = CreateBuilder(new()
        {
            ["Telemetry:ConnectionString"] = ConnectionString,
            ["Telemetry:UseManagedIdentity"] = "true"
        });

        builder.AddApplicationInsightsExporter(options => captured = options);

        Assert.IsNotNull(captured);
        Assert.IsInstanceOfType<DefaultAzureCredential>(captured.Credential);
    }

    [TestMethod]
    public void AddApplicationInsightsExporter_LeavesCredentialNull_WhenIdentityIsNotConfigured()
    {
        AzureMonitorExporterOptions? captured = null;
        var builder = CreateBuilder(new() { ["Telemetry:ConnectionString"] = ConnectionString });

        builder.AddApplicationInsightsExporter(options => captured = options);

        Assert.IsNotNull(captured);
        Assert.IsNull(captured.Credential);
    }

    [TestMethod]
    public void AddApplicationInsightsExporter_UsesCredentialEvenWithoutConnectionString()
    {
        var credential = new FakeTokenCredential();
        AzureMonitorExporterOptions? captured = null;
        var builder = CreateBuilder(new());

        builder.AddApplicationInsightsExporter(options => captured = options, credential);

        Assert.IsNotNull(captured);
        Assert.AreSame(credential, captured.Credential);
    }

    private static TelemetryBuilder CreateBuilder(Dictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();
        var openTelemetryBuilder = services.AddOpenTelemetry();

        return new TelemetryBuilder(openTelemetryBuilder, configuration, Mock.Of<IInfoService>());
    }

    private sealed class FakeTokenCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => new("fake-token", DateTimeOffset.MaxValue);

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(new AccessToken("fake-token", DateTimeOffset.MaxValue));
    }
}
