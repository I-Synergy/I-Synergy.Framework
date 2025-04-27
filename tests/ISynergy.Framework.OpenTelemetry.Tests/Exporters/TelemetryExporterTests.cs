using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTelemetry;

namespace ISynergy.Framework.OpenTelemetry.Exporters.Tests;

[TestClass]
public class TelemetryExporterTests
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _services;
    private readonly OpenTelemetryBuilder _openTelemetryBuilder;

    public TelemetryExporterTests()
    {
        _configuration = CreateTestConfiguration();
        _services = new ServiceCollection();
        _openTelemetryBuilder = _services.AddOpenTelemetry();
    }

    private IConfiguration CreateTestConfiguration()
    {
        // Create configuration sources
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string?>
            {
                { "Telemetry:ConnectionString", "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://test.in.applicationinsights.azure.com/;LiveEndpoint=https://test.livediagnostics.monitor.azure.com/" },
                { "Telemetry:Dsn", "https://test@sentry.io/1" },
                { "Telemetry:OtlpEndpoint", $"http://localhost:18889" },
                { "Telemetry:OtlpProtocol", "Http" },
                { "Telemetry:OtlpHeaders", "Authorization=Bearer token123" },
                { "Telemetry:OtlpTimeoutMilliseconds", "10000" }
            }
        };

        var configurationSources = new List<IConfigurationSource> { configSource };

        // Create configuration provider
        var configurationProviders = new List<IConfigurationProvider>
        {
            new MemoryConfigurationProvider(configSource)
        };

        // Create and return the configuration root
        return new ConfigurationRoot(configurationProviders);
    }
}
