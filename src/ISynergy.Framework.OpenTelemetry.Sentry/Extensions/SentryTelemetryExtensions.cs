using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;
using ISynergy.Framework.OpenTelemetry.Sentry.Converters;
using ISynergy.Framework.OpenTelemetry.Sentry.Processors;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using Sentry.OpenTelemetry;

namespace ISynergy.Framework.OpenTelemetry.Sentry.Extensions;

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
    public static TelemetryBuilder AddSentryExporter(
        this TelemetryBuilder telemetryBuilder,
        Action<SentryOptions>? telemetryExporterOptions = null)
    {
        Argument.IsNotNull(telemetryBuilder);

        // IConfiguration.Bind(object) uses reflection over the target's property graph, which
        // NativeAOT trimming silently breaks (properties are left at their defaults instead of
        // throwing) -- most importantly Dsn, which then reads as null/empty below and disables
        // Sentry entirely with no error anywhere. Read each value explicitly instead.
        var options = new SentryOptions();
        var telemetrySection = telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection);
        options.Dsn = telemetrySection.GetValue<string?>(nameof(SentryOptions.Dsn), options.Dsn);
        options.Environment = telemetrySection.GetValue<string?>(nameof(SentryOptions.Environment), options.Environment);
        options.Release = telemetrySection.GetValue<string?>(nameof(SentryOptions.Release), options.Release);
        options.SampleRate = telemetrySection.GetValue<float?>(nameof(SentryOptions.SampleRate), options.SampleRate);
        options.TracesSampleRate = telemetrySection.GetValue<double?>(nameof(SentryOptions.TracesSampleRate), options.TracesSampleRate);
        options.MaxBreadcrumbs = telemetrySection.GetValue(nameof(SentryOptions.MaxBreadcrumbs), options.MaxBreadcrumbs);
        options.Debug = telemetrySection.GetValue(nameof(SentryOptions.Debug), options.Debug);
        options.AttachStacktrace = telemetrySection.GetValue(nameof(SentryOptions.AttachStacktrace), options.AttachStacktrace);
        options.SendDefaultPii = telemetrySection.GetValue(nameof(SentryOptions.SendDefaultPii), options.SendDefaultPii);
        options.IsGlobalModeEnabled = telemetrySection.GetValue(nameof(SentryOptions.IsGlobalModeEnabled), options.IsGlobalModeEnabled);
        options.DeduplicateMode = telemetrySection.GetValue(nameof(SentryOptions.DeduplicateMode), options.DeduplicateMode);
        options.MaxQueueItems = telemetrySection.GetValue(nameof(SentryOptions.MaxQueueItems), options.MaxQueueItems);
        options.ShutdownTimeout = telemetrySection.GetValue(nameof(SentryOptions.ShutdownTimeout), options.ShutdownTimeout);

        telemetryExporterOptions?.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Dsn))
            return telemetryBuilder;

        SentrySdk.Init(sentryOptions =>
        {
            options.Map(sentryOptions);

            // Register a custom JSON converter for KeyValuePair<,>. System.Text.Json cannot
            // serialize KeyValuePair natively (its constructor parameter names are null).
            // On iOS, the device context collected by Sentry contains KeyValuePair<string, bool>
            // values that would otherwise cause a NotSupportedException during serialization.
            //
            // This goes through Sentry's own registration point rather than
            // JsonSerializerOptions.Default: that instance is created read-only, so
            // Converters.Add on it throws InvalidOperationException ("...is read-only or has
            // already been used in serialization or deserialization") and faults every host
            // startup that configures a DSN.
            sentryOptions.AddJsonConverter(new KeyValuePairJsonConverterFactory());

            sentryOptions.UseOtlp();
        });

        telemetryBuilder.OpenTelemetryBuilder.WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder.AddSentryOtlpExporter(options.Dsn!);
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