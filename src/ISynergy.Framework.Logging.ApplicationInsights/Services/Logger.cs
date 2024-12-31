using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Logging.ApplicationInsights.Options;
using ISynergy.Framework.Logging.Base;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ISynergy.Framework.Logging.Services;

public class Logger : BaseLogger
{
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
    /// <param name="scopedContextService"></param>
    /// <param name="options">The options.</param>
    public Logger(IScopedContextService scopedContextService, IOptions<ApplicationInsightsOptions> options)
        : base("Application Insights Logger")
    {
        _applicationInsightsOptions = options.Value;

        var config = TelemetryConfiguration.CreateDefault();
        config.ConnectionString = _applicationInsightsOptions.ConnectionString;
        config.TelemetryInitializers.Add(scopedContextService.GetService<ITelemetryInitializer>());

        var temporaryLogFolder = Path.Combine(
            Path.GetTempPath(),
            Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName),
            "Logging");

        if (!Directory.Exists(temporaryLogFolder))
            Directory.CreateDirectory(temporaryLogFolder);

        var serverTelemetryChannel = new ServerTelemetryChannel();
        serverTelemetryChannel.StorageFolder = temporaryLogFolder;
#if DEBUG
        serverTelemetryChannel.DeveloperMode = Debugger.IsAttached;
#endif
        serverTelemetryChannel.Initialize(config);
        config.TelemetryChannel = serverTelemetryChannel;

        _client = new TelemetryClient(config);
        _client.Context.User.UserAgent = InfoService.Default.ProductName;
        _client.Context.Component.Version = InfoService.Default.ProductVersion.ToString();
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
        System.Threading.Thread.Sleep(5000);
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
                    _client.TrackException(new ExceptionTelemetry(exception) { Message = message, });
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
