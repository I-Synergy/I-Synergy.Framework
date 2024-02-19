using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Logging.Base;
using ISynergy.Framework.Logging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Logging.Services;

public class Logger : BaseLogger
{
    /// <summary>
    /// The context
    /// </summary>
    private readonly IContext _context;

    /// <summary>
    /// The information service
    /// </summary>
    private readonly IInfoService _infoService;

    /// <summary>
    /// The application center options
    /// </summary>
    private readonly SentryOptions _sentryOptions;

    /// <summary>
    /// Logging client for Application Insights.
    /// </summary>
    private readonly ISentryClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="infoService">The information service.</param>
    /// <param name="options">The options.</param>
    public Logger(IContext context, IInfoService infoService, IOptions<SentryOptions> options)
        : base("Sentry Logger")
    {
        _context = context;
        _infoService = infoService;
        _sentryOptions = options.Value;

        _client = new SentryClient(_sentryOptions);

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Flush();
    }

    /// <summary>
    /// Sets profile in logging context.
    /// </summary>
    private void SetUserProfile()
    {
        if (_context.IsAuthenticated && _context.Profile is IProfile profile)
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

    /// <summary>
    /// Flushes this instance.
    /// </summary>
    public void Flush()
    {
        SetUserProfile();

        _client.FlushAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets the metrics.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
    private Dictionary<string, object> GetMetrics()
    {
        var metrics = new Dictionary<string, object>();

        if (_context.IsAuthenticated && _context.Profile is IProfile profile)
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

    public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        base.Log(logLevel, eventId, state, exception, formatter);

        var message = formatter(state, exception);

        SetUserProfile();

        var metrics = GetMetrics();

        if (!string.IsNullOrEmpty(message) || exception != null)
        {
            if (logLevel == LogLevel.Error)
            {
                _client.CaptureException(exception);
            }
            else
            {
                metrics.Add("LogLevel", logLevel.ToLogLevelString());

                var data = new SentryEvent
                {
                    Message = message,
                    Level = logLevel.ToSentryLevel(),
                };

                data.SetExtras(metrics);

                _client.CaptureEvent(data);
            }
        }
    }
}
