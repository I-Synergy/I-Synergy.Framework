using ISynergy.Framework.Logging.Base;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Services;

public class OpenTelemetryLoggerService : BaseLogger
{
    public OpenTelemetryLoggerService(ILoggerFactory loggerFactory)
        : base("OpenTelemetry Logger", loggerFactory)
    {
    }
}