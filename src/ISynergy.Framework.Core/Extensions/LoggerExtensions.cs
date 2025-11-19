using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Core.Extensions;
public static class LoggerExtensions
{
    public static LogLevel CurrentLogLevel(this ILogger logger)
    {
        foreach (LogLevel logLevel in Enum.GetValues(typeof(LogLevel)))
        {
            if (logger.IsEnabled(logLevel))
                return logLevel;
        }

        return LogLevel.None;
    }
}
