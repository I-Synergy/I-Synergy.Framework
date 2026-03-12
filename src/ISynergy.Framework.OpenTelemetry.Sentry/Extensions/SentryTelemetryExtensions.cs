using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;
using ISynergy.Framework.OpenTelemetry.Processors;
using OpenTelemetry.Logs;
using Sentry.OpenTelemetry;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.OpenTelemetry.Extensions;

/// <summary>
/// Extension methods for adding Sentry exporter to telemetry.
/// </summary>
public static class SentryTelemetryExtensions
{
    /// <summary>
    /// Adds the Sentry exporter to the telemetry pipeline.
    /// </summary>
    /// <param name="telemetryBuilder">The telemetry builder.</param>
    /// <param name="telemetryExporterOptions">Optional action to configure exporter options.</param>
    /// <returns>The telemetry builder for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> <c>SentrySdk.Init</c> and the Sentry SDK may use reflection
    /// internally and may not be fully AOT-annotated depending on the Sentry package version in use.
    /// Applications targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should verify the installed
    /// Sentry package version ships with trimmer-safe annotations, and suppress <c>IL2026</c> and <c>IL3050</c>
    /// warnings at the call site if required.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("SentrySdk.Init may use reflection internally. Verify AOT compatibility with the Sentry SDK version in use.")]
    [RequiresDynamicCode("Sentry SDK may require dynamic code generation.")]
    public static TelemetryBuilder AddSentryExporter(
        this TelemetryBuilder telemetryBuilder,
        Action<SentryOptions>? telemetryExporterOptions = null)
    {
        Argument.IsNotNull(telemetryBuilder);

        var options = new SentryOptions();
        telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

        telemetryExporterOptions?.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Dsn))
            return telemetryBuilder;

        SentrySdk.Init(sentryOptions =>
        {
            options.Map(sentryOptions);

            sentryOptions.UseOpenTelemetry();
        });

        telemetryBuilder.OpenTelemetryBuilder.WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder.AddSentry();
        });

        telemetryBuilder.OpenTelemetryBuilder.WithMetrics(meterProviderBuilder =>
        {
            // Sentry doesn't yet have direct OpenTelemetry metrics integration
            // Default implementation is empty and can be expanded when Sentry adds metrics support
        });

        telemetryBuilder.OpenTelemetryBuilder.WithLogging(loggerProviderBuilder =>
        {
            // Check if there's a specific extension method for OpenTelemetry logging
            // If not available, add a processor to convert logs to Sentry events
            loggerProviderBuilder.AddProcessor(new OpenTelemetryLogProcessor());
        });

        return telemetryBuilder;
    }

    /// <summary>
    /// Maps <see cref="SentryOptions"/> properties from <paramref name="source"/> to <paramref name="target"/>
    /// using explicit property assignments instead of reflection, making this method AOT-safe.
    /// </summary>
    /// <param name="source">The source options to copy from.</param>
    /// <param name="target">The target options to copy to.</param>
    private static void Map(this SentryOptions source, SentryOptions target)
    {
        target.Dsn = source.Dsn;
        target.Environment = source.Environment;
        target.Release = source.Release;
        target.SampleRate = source.SampleRate;
        target.TracesSampleRate = source.TracesSampleRate;
        target.MaxBreadcrumbs = source.MaxBreadcrumbs;
        target.Debug = source.Debug;
        target.AttachStacktrace = source.AttachStacktrace;
        target.SendDefaultPii = source.SendDefaultPii;
        target.IsGlobalModeEnabled = source.IsGlobalModeEnabled;
        target.DeduplicateMode = source.DeduplicateMode;
        target.MaxQueueItems = source.MaxQueueItems;
        target.ShutdownTimeout = source.ShutdownTimeout;
    }
}