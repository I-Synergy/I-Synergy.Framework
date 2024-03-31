using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Logging.ApplicationInsights.Options;
using ISynergy.Framework.Logging.Base;
using ISynergy.Framework.Logging.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;

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
    /// <param name="telemetryInitializer"></param>
    /// <param name="options">The options.</param>
    public Logger(IContext context, IInfoService infoService, ITelemetryInitializer telemetryInitializer, IOptions<ApplicationInsightsOptions> options)
        : base("Application Insights Logger")
    {
        _context = context;
        _infoService = infoService;
        _applicationInsightsOptions = options.Value;

        var config = TelemetryConfiguration.CreateDefault();
        config.ConnectionString = _applicationInsightsOptions.ConnectionString;
        config.TelemetryInitializers.Add(telemetryInitializer);

        _client = new TelemetryClient(config);
        _client.Context.User.UserAgent = infoService.ProductName;
        _client.Context.Component.Version = infoService.ProductVersion.ToString();
        _client.Context.Session.Id = Guid.NewGuid().ToString();
        _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Flush();
    }

    /// <summary>
    /// Flushes this instance.
    /// </summary>
    public void Flush()
    {
        _client.Flush();

        // Explicitly call Flush() followed by Delay, as required in console apps.
        // This ensures that even if the application terminates, telemetry is sent to the back end.
        Task.Delay(5000).Wait();
    }

    public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        base.Log(logLevel, eventId, state, exception, formatter);

        var message = formatter(state, exception);

        if (!string.IsNullOrEmpty(message))
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Critical:
                    _client.TrackException(new ExceptionTelemetry { Exception = exception, Message = message });
                    break;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    if (message.EndsWith("ViewModel"))
                        _client.TrackPageView(message.Replace("ViewModel", ""));
                    else if (message.EndsWith("View"))
                        _client.TrackPageView(message.Replace("View", ""));
                    else if (message.EndsWith("Window"))
                        _client.TrackPageView(message.Replace("Window", ""));
                    else
                        _client.TrackTrace(message);

                    break;
                default:
                    _client.TrackEvent(message);
                    break;
            }
        }
    }
}
