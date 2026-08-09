using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Models;
using ISynergy.Framework.OpenTelemetry.Sentry.Converters;
using ISynergy.Framework.OpenTelemetry.Sentry.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenTelemetry;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.OpenTelemetry.Exporters.Tests;

/// <summary>
/// Regression tests for <see cref="SentryTelemetryExtensions.AddSentryExporter"/>.
/// </summary>
/// <remarks>
/// Registering the KeyValuePair converter on <see cref="JsonSerializerOptions.Default"/> threw
/// <see cref="InvalidOperationException"/> ("This JsonSerializerOptions instance is read-only or
/// has already been used in serialization or deserialization") on every host startup that had a
/// DSN configured, taking down both the application and the build-time OpenAPI document
/// generation that boots the host.
/// </remarks>
[TestClass]
public class SentryTelemetryExtensionsTests
{
    private const string TestDsn = "https://test@sentry.io/1";

    private static TelemetryBuilder CreateBuilder(string? dsn)
    {
        var data = new Dictionary<string, string?>();

        if (dsn is not null)
            data["Telemetry:Dsn"] = dsn;

        var source = new MemoryConfigurationSource { InitialData = data };
        var configuration = new ConfigurationRoot([new MemoryConfigurationProvider(source)]);

        var services = new ServiceCollection();

        return new TelemetryBuilder(services.AddOpenTelemetry(), configuration, new Mock<IInfoService>().Object);
    }

    /// <summary>
    /// SentrySdk is a process-wide singleton, so it must not leak between tests.
    /// </summary>
    [TestCleanup]
    public void Cleanup() => SentrySdk.Close();

    [TestMethod]
    public void AddSentryExporter_WithConfiguredDsn_DoesNotThrow()
    {
        var builder = CreateBuilder(TestDsn);

        var result = builder.AddSentryExporter();

        Assert.AreSame(builder, result, "The builder should be returned for chaining.");
    }

    [TestMethod]
    public void AddSentryExporter_CalledTwice_DoesNotThrow()
    {
        // Tooling such as dotnet-getdocument builds the host more than once in a
        // single process, so registration must tolerate repeat calls.
        var builder = CreateBuilder(TestDsn);

        builder.AddSentryExporter();
        builder.AddSentryExporter();
    }

    [TestMethod]
    public void AddSentryExporter_WithoutDsn_ReturnsBuilderWithoutInitialising()
    {
        var builder = CreateBuilder(dsn: null);

        var result = builder.AddSentryExporter();

        Assert.AreSame(builder, result);
        Assert.IsFalse(SentrySdk.IsEnabled, "Sentry must stay disabled when no DSN is configured.");
    }

    [TestMethod]
    public void AddSentryExporter_WithNullBuilder_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ((TelemetryBuilder)null!).AddSentryExporter());
    }

    /// <summary>
    /// Guards the reason the original implementation could never work, so the
    /// <c>JsonSerializerOptions.Default.Converters.Add(...)</c> approach is not reintroduced.
    /// </summary>
    [TestMethod]
    public void JsonSerializerOptionsDefault_IsReadOnly_AndRejectsConverterRegistration()
    {
        Assert.IsTrue(JsonSerializerOptions.Default.IsReadOnly);

        Assert.ThrowsExactly<InvalidOperationException>(
            () => JsonSerializerOptions.Default.Converters.Add(new KeyValuePairJsonConverterFactory()));
    }

    /// <summary>
    /// The converter exists to stop Sentry's iOS device context (which contains
    /// <c>KeyValuePair&lt;string, bool&gt;</c>) from failing serialization, so verify it still does that.
    /// </summary>
    [TestMethod]
    public void KeyValuePairJsonConverterFactory_SerializesKeyValuePair()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new KeyValuePairJsonConverterFactory());

        var json = JsonSerializer.Serialize(new KeyValuePair<string, bool>("simulator", true), options);

        Assert.IsTrue(json.Contains("simulator"), $"Key missing from payload: {json}");
        Assert.IsTrue(json.Contains("true"), $"Value missing from payload: {json}");
    }

    [TestMethod]
    public void KeyValuePairJsonConverterFactory_CanConvert_OnlyKeyValuePairTypes()
    {
        var factory = new KeyValuePairJsonConverterFactory();

        Assert.IsTrue(factory.CanConvert(typeof(KeyValuePair<string, bool>)));
        Assert.IsTrue(factory.CanConvert(typeof(KeyValuePair<string, string>)));
        Assert.IsFalse(factory.CanConvert(typeof(string)));
        Assert.IsFalse(factory.CanConvert(typeof(Dictionary<string, bool>)));
    }
}
