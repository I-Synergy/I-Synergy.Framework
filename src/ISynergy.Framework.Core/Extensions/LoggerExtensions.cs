using Microsoft.Extensions.Logging;

#pragma warning disable S3267 // foreach loop uses early-return; cannot be converted to LINQ without losing short-circuit semantics

namespace ISynergy.Framework.Core.Extensions;
public static class LoggerExtensions
{
    public static LogLevel CurrentLogLevel(this ILogger logger)
    {
        foreach (LogLevel logLevel in Enum.GetValues<LogLevel>()) // NOSONAR
        {
            if (logger.IsEnabled(logLevel))
                return logLevel;
        }

        return LogLevel.None;
    }
}
