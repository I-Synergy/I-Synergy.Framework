using Microsoft.Extensions.Logging;
using Sentry;

namespace ISynergy.Framework.Logging.Extensions
{
    public static class LogLevelExtensions
    {
        public static SentryLevel ToSentryLevel(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return SentryLevel.Debug;
                case LogLevel.Information:
                    return SentryLevel.Info;
                case LogLevel.Warning:
                    return SentryLevel.Warning;
                case LogLevel.Error:
                    return SentryLevel.Error;
                case LogLevel.Critical:
                    return SentryLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}
