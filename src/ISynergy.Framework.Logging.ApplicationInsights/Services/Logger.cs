using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Logging.ApplicationInsights.Options;
using ISynergy.Framework.Logging.Base;
using ISynergy.Framework.Logging.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
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
    private readonly ApplicationInsightsOptions _applicationInsightsOptions;

    /// <summary>
    /// Telemetry client.
    /// </summary>
    private readonly TelemetryClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="infoService">The information service.</param>
    /// <param name="options">The options.</param>
    public Logger(IContext context, IInfoService infoService, IOptions<ApplicationInsightsOptions> options)
        : base("Application Insights Logger")
    {
        _context = context;
        _infoService = infoService;
        _applicationInsightsOptions = options.Value;

        var config = TelemetryConfiguration.CreateDefault();
        config.ConnectionString = _applicationInsightsOptions.ConnectionString;

        _client = new TelemetryClient(config);
        _client.Context.User.UserAgent = infoService.ProductName;
        _client.Context.Component.Version = infoService.ProductVersion.ToString();
        _client.Context.Session.Id = Guid.NewGuid().ToString();
        _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Flush();
    }

    /// <summary>
    /// Gets the metrics.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
    private Dictionary<string, string> GetMetrics()
    {
        var metrics = new Dictionary<string, string>();

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
            _client.Context.User.Id = profile.Username;
            _client.Context.User.AccountId = profile.AccountDescription;
        }
        else
        {
            _client.Context.User.Id = string.Empty;
            _client.Context.User.AccountId = string.Empty;
        }
    }

    /// <summary>
    /// Flushes this instance.
    /// </summary>
    public void Flush()
    {
        SetUserProfile();

        _client.Flush();

        // Explicitly call Flush() followed by Delay, as required in console apps.
        // This ensures that even if the application terminates, telemetry is sent to the back end.
        Task.Delay(5000).Wait();
    }

    public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        base.Log(logLevel, eventId, state, exception, formatter);

        var message = formatter(state, exception);

        SetUserProfile();

        var metrics = GetMetrics();

        if (!string.IsNullOrEmpty(message) || exception != null)
        {
            if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
            {
                _client.TrackException(new ExceptionTelemetry { Exception = exception, Message = message });
            }
            else if (logLevel == LogLevel.Trace)
            {
                metrics.Add("LogLevel", logLevel.ToLogLevelString());
                _client.TrackTrace(message, metrics);
            }
            else
            {
                metrics.Add("LogLevel", logLevel.ToLogLevelString());
                _client.TrackEvent(message, metrics, null);
            }
        }
    }
}
