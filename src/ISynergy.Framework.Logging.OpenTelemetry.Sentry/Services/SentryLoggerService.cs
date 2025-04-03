using ISynergy.Framework.Logging.Base;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Services;

public class SentryLoggerService : BaseLogger
{
    public SentryLoggerService(ILoggerFactory loggerFactory)
        : base("Sentry Logger", loggerFactory)
    {
    }

    public override void Flush()
    {
        try
        {
            // Perform OpenTelemetry-specific flush operations
            SentrySdk.Flush();
            base.Flush();
        }
        catch (OperationCanceledException)
        {
            // Silently ignore task cancellations during shutdown
        }
        catch (Exception ex)
        {
            // Log to debug output only, since regular logging might be unavailable
            System.Diagnostics.Debug.WriteLine($"Error flushing Sentry logger: {ex.Message}");
        }
    }

    // Override Dispose to handle OpenTelemetry-specific cleanup
    public override void Dispose()
    {
        try
        {
            // Perform any OpenTelemetry-specific cleanup
            base.Dispose();
        }
        catch (OperationCanceledException)
        {
            // Silently ignore task cancellations during disposal
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error disposing Sentry logger: {ex.Message}");
        }
    }
}
