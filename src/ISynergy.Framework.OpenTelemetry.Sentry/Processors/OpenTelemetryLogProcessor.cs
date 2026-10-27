using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;

#pragma warning disable S3267 // foreach loop uses side-effects; cannot be simplified to LINQ

namespace ISynergy.Framework.OpenTelemetry.Sentry.Processors;
/// <summary>
/// Processor to convert OpenTelemetry logs to Sentry events
/// </summary>
internal class OpenTelemetryLogProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        if (data is null)
            return;

        // Convert log records to Sentry events
        if (data.LogLevel >= LogLevel.Error)
        {
            try
            {
                // Create and capture a Sentry event from the log record
                var sentryEvent = new SentryEvent(data.Exception)
                {
                    Level = GetSentryLevel(data.LogLevel),
                    Message = new SentryMessage { Message = data.Body },
                    Logger = data.CategoryName
                };

                // Add attributes as tags
                foreach (var attr in data.Attributes.EnsureNotNull()) // NOSONAR
                {
                    if (attr.Value is not null)
                        sentryEvent.SetTag(attr.Key, attr.Value.ToString()!);
                }

                SentrySdk.CaptureEvent(sentryEvent);
            }
            catch (Exception ex)
            {
                // Sentry serialization can fail on types that System.Text.Json doesn't support
                // (e.g. KeyValuePair<string, bool> in exception data or log attributes).
                // Don't let a serialization failure crash the OpenTelemetry log pipeline.
                System.Diagnostics.Debug.WriteLine($"Sentry log processor failed: {ex.Message}");
            }
        }

        base.OnEnd(data);
    }

    private static SentryLevel GetSentryLevel(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => SentryLevel.Debug,
        LogLevel.Debug => SentryLevel.Debug,
        LogLevel.Information => SentryLevel.Info,
        LogLevel.Warning => SentryLevel.Warning,
        LogLevel.Error => SentryLevel.Error,
        LogLevel.Critical => SentryLevel.Fatal,
        _ => SentryLevel.Info
    };
}
