using Azure.Core;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;
using Microsoft.Extensions.Configuration;

namespace ISynergy.Framework.OpenTelemetry.ApplicationInsights.Extensions;
/// <summary>
/// Extension methods for adding Application Insights exporter to telemetry.
/// </summary>
public static class ApplicationInsightsTelemetryExtensions
{
    /// <summary>
    /// Adds the Application Insights exporter to the telemetry pipeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Identity-based authentication is preferred. A credential is used when one is supplied to this
    /// method or when <c>Telemetry:UseManagedIdentity</c> is <c>true</c> (optionally with
    /// <c>Telemetry:ManagedIdentityClientId</c> for user-assigned identities). The
    /// <c>Telemetry:ConnectionString</c> is still required as the resource locator, including the
    /// ingestion endpoint; identity replaces the connection string's instrumentation key as the
    /// authentication mechanism rather than the connection string itself.
    /// </para>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> <see cref="DefaultAzureCredential"/> probes multiple
    /// credential providers at runtime using reflection and carries
    /// <c>[RequiresUnreferencedCode]</c>. Applications targeting
    /// <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should pass an explicit credential such as
    /// <see cref="ManagedIdentityCredential"/> or set <c>Telemetry:ManagedIdentityClientId</c>.
    /// </para>
    /// </remarks>
    /// <param name="telemetryBuilder">The telemetry builder.</param>
    /// <param name="telemetryExporterOptions">Optional callback to further configure the exporter.</param>
    /// <param name="credential">
    /// Optional credential used for identity-based authentication. Takes precedence over
    /// <c>Telemetry:UseManagedIdentity</c>.
    /// </param>
    /// <returns>The telemetry builder for chaining.</returns>
    public static TelemetryBuilder AddApplicationInsightsExporter(
        this TelemetryBuilder telemetryBuilder,
        Action<AzureMonitorExporterOptions>? telemetryExporterOptions = null,
        TokenCredential? credential = null)
    {
        Argument.IsNotNull(telemetryBuilder);

        // IConfiguration.Bind(object) uses reflection over the target's property graph, which
        // NativeAOT trimming silently breaks (properties are left at their defaults instead of
        // throwing) -- most importantly ConnectionString, which then reads as null/empty below
        // and disables this exporter entirely with no error anywhere. Read each value explicitly
        // instead. Transport/Diagnostics/Retry/RetryPolicy are complex Azure SDK types that
        // IConfiguration.Bind could never populate from flat JSON config either way, so they
        // aren't included here -- they keep whatever AzureMonitorExporterOptions defaults to.
        var options = new AzureMonitorExporterOptions();
        var telemetrySection = telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection);
        options.ConnectionString = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.ConnectionString), options.ConnectionString);
        options.SamplingRatio = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.SamplingRatio), options.SamplingRatio);
        options.TracesPerSecond = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.TracesPerSecond), options.TracesPerSecond);
        options.Version = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.Version), options.Version);
        options.StorageDirectory = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.StorageDirectory), options.StorageDirectory);
        options.DisableOfflineStorage = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.DisableOfflineStorage), options.DisableOfflineStorage);
        options.EnableLiveMetrics = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnableLiveMetrics), options.EnableLiveMetrics);
        options.EnableTraceBasedLogsSampler = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnableTraceBasedLogsSampler), options.EnableTraceBasedLogsSampler);
        options.EnableStandardMetrics = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnableStandardMetrics), options.EnableStandardMetrics);
        options.EnablePerformanceCounters = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnablePerformanceCounters), options.EnablePerformanceCounters);
        options.Credential = credential ?? CreateConfiguredCredential(telemetrySection);

        telemetryExporterOptions?.Invoke(options);

        // The connection string is the resource locator (ingestion endpoint and, historically, the
        // instrumentation key); identity changes the authentication mechanism, not the locator.
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            return telemetryBuilder;

        // Configure all telemetry types with the same options
        telemetryBuilder
            .OpenTelemetryBuilder
                .WithTracing(builder => builder
                    .AddAzureMonitorTraceExporter(target => options.Map(target)))
                .WithMetrics(builder => builder
                    .AddAzureMonitorMetricExporter(target => options.Map(target)))
                .WithLogging(builder => builder
                    .AddAzureMonitorLogExporter(target => options.Map(target)));

        return telemetryBuilder;
    }

    /// <summary>
    /// Creates an identity credential when <c>Telemetry:UseManagedIdentity</c> is enabled.
    /// </summary>
    /// <param name="telemetrySection">The telemetry configuration section.</param>
    /// <returns>
    /// The configured credential, or <c>null</c> when identity-based authentication is not enabled.
    /// </returns>
    private static TokenCredential? CreateConfiguredCredential(IConfigurationSection telemetrySection)
    {
        if (!telemetrySection.GetValue("UseManagedIdentity", false))
            return null;

        var managedIdentityClientId = telemetrySection.GetValue<string?>("ManagedIdentityClientId", null);

        return string.IsNullOrWhiteSpace(managedIdentityClientId)
            ? new DefaultAzureCredential()
            : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(managedIdentityClientId));
    }

    /// <summary>
    /// Copies all public settable properties from <paramref name="source"/> to <paramref name="target"/>.
    /// </summary>
    /// <param name="source">The source options to copy from.</param>
    /// <param name="target">The target options to copy to.</param>
    /// <remarks>
    /// Uses explicit property assignments instead of reflection to ensure AOT (Ahead-of-Time) compilation
    /// compatibility. Reflection-based property copying is blocked under AOT trimming.
    /// </remarks>
    private static void Map(this AzureMonitorExporterOptions source, AzureMonitorExporterOptions target)
    {
        target.ConnectionString = source.ConnectionString;
        target.Credential = source.Credential;
        target.SamplingRatio = source.SamplingRatio;
        target.TracesPerSecond = source.TracesPerSecond;
        target.Version = source.Version;
        target.StorageDirectory = source.StorageDirectory;
        target.DisableOfflineStorage = source.DisableOfflineStorage;
        target.EnableLiveMetrics = source.EnableLiveMetrics;
        target.EnableStandardMetrics = source.EnableStandardMetrics;
        target.EnablePerformanceCounters = source.EnablePerformanceCounters;
        target.EnableTraceBasedLogsSampler = source.EnableTraceBasedLogsSampler;
        target.Transport = source.Transport;
        target.RetryPolicy = source.RetryPolicy;
    }
}
