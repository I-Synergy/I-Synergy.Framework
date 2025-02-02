using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Logging.Base;
using ISynergy.Framework.Logging.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;

namespace ISynergy.Framework.Logging.Services;

public class Logger : BaseLogger
{
    private readonly OpenTelemetryOptions _openTelemetryOptions;
    private readonly IInfoService _infoService;
    private readonly ILoggerFactory _loggerFactory;

    public Logger(IScopedContextService scopedContextService, IInfoService infoService, IOptions<OpenTelemetryOptions> options)
        : base("OpenTelemetry Logger")
    {
        _infoService = infoService;
        _openTelemetryOptions = options.Value;

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                options.AddOtlpExporter();
                options.AddConsoleExporter();
            });
        });

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Flush();
    }

    public void Flush()
    {
        _loggerFactory.Dispose();
        Thread.Sleep(1000);
    }

    private readonly Dictionary<string, object> _customAttributes = new();

    public void AddCustomAttribute(string key, object value)
    {
        _customAttributes[key] = value;
    }

    public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);
        if (string.IsNullOrEmpty(message)) return;

        var logger = _loggerFactory.CreateLogger(_name);

        using var scope = logger.BeginScope(_customAttributes);

        switch (logLevel)
        {
            case LogLevel.Error:
            case LogLevel.Critical:
                logger.LogError(exception, message);
                break;
            case LogLevel.Debug:
            case LogLevel.Trace:
                if (message.EndsWith("ViewModel") || message.EndsWith("View") || message.EndsWith("Window"))
                {
                    AddCustomAttribute("page.name", message.Replace("ViewModel", "")
                        .Replace("View", "").Replace("Window", ""));
                }
                logger.LogDebug(message);
                break;
            default:
                logger.LogInformation(message);
                break;
        }
    }
}