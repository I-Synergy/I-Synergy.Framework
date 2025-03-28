using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace ISynergy.Framework.Logging.Processors;
/// <summary>
/// Custom processor to send logs to Sentry
/// </summary>
internal class SentryLogProcessor : BaseProcessor<LogRecord>
{
    private readonly IContext _context;
    private readonly IInfoService _infoService;
    private readonly SentryOptions _options;

    public SentryLogProcessor(IContext context, IInfoService infoService, SentryOptions options)
    {
        _context = context;
        _infoService = infoService;
        _options = options;
    }

    public override void OnEnd(LogRecord data)
    {
        // Skip if not an error or higher
        if (data.LogLevel < LogLevel.Warning)
            return;

        SetUserProfile();
        var metrics = GetMetrics();

        var sentryEvent = new SentryEvent();

        if (data.Exception != null)
            sentryEvent = new SentryEvent(data.Exception);

        sentryEvent.Message = data.FormattedMessage;
        sentryEvent.Level = ConvertLogLevel(data.LogLevel);
        sentryEvent.SetExtras(metrics);

        SentrySdk.CaptureEvent(sentryEvent);
    }

    private SentryLevel ConvertLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Critical => SentryLevel.Fatal,
            LogLevel.Error => SentryLevel.Error,
            LogLevel.Warning => SentryLevel.Warning,
            LogLevel.Information => SentryLevel.Info,
            _ => SentryLevel.Debug
        };
    }

    /// <summary>
    /// Gets the metrics.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
    private Dictionary<string, object> GetMetrics()
    {
        var metrics = new Dictionary<string, object>();

        if (_context.IsAuthenticated && _context.Profile is { } profile)
        {
            metrics.Add(nameof(profile.Username), profile.Username);
            metrics.Add(nameof(profile.UserId), profile.UserId.ToString());
            metrics.Add(nameof(profile.AccountId), profile.AccountId.ToString());
            metrics.Add(nameof(profile.AccountDescription), profile.AccountDescription);
        }

        metrics.Add(nameof(_infoService.ProductName), _infoService.ProductName);
        metrics.Add(nameof(_infoService.ProductVersion), _infoService.ProductVersion.ToString());

        return metrics;
    }

    /// <summary>
    /// Sets profile in logging context.
    /// </summary>
    private void SetUserProfile()
    {
        if (_context.IsAuthenticated && _context.Profile is { } profile)
        {
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new SentryUser
                {
                    Username = profile.Username,
                    Id = profile.UserId.ToString(),
                    Other = new Dictionary<string, string>()
                    {
                        { "AccountId", profile.AccountId.ToString() },
                        { "AccountDescription", profile.AccountDescription }
                    }
                };
            });
        }
        else
        {
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = null;
            });
        }
    }
}