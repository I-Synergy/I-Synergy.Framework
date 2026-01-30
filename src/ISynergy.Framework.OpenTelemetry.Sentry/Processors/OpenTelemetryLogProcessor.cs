using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace ISynergy.Framework.OpenTelemetry.Processors;
/// <summary>
/// Processor to convert OpenTelemetry logs to Sentry events
/// </summary>
internal class OpenTelemetryLogProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        // Convert log records to Sentry events
        if (data.LogLevel >= LogLevel.Error)
        {
            // Create and capture a Sentry event from the log record
            var sentryEvent = new SentryEvent(data.Exception)
            {
                Level = GetSentryLevel(data.LogLevel),
                Message = new SentryMessage { Message = data.Body },
                Logger = data.CategoryName
            };

            // Add attributes as tags
            foreach (var attr in data.Attributes.EnsureNotNull())
            {
                if (attr.Value is not null)
                    sentryEvent.SetTag(attr.Key, attr.Value.ToString()!);
            }

            SentrySdk.CaptureEvent(sentryEvent);
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
