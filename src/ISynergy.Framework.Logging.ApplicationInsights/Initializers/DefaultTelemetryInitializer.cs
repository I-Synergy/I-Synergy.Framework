using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Logging.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace ISynergy.Framework.Logging.Initializers;

public class DefaultTelemetryInitializer : ITelemetryInitializer
{
    /// <summary>
    /// The context
    /// </summary>
    private readonly IContext _context;

    /// <summary>
    /// The information service
    /// </summary>
    private readonly IInfoService _infoService;

    public DefaultTelemetryInitializer(IContext context, IInfoService infoService)
    {
        _context = context;
        _infoService = infoService;
    }

    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is RequestTelemetry requestTelemetry)
            requestTelemetry.Properties.AddDefaultProperties(requestTelemetry.Context, _context, _infoService);

        if (telemetry is ExceptionTelemetry exceptionTelemetry)
            exceptionTelemetry.Properties.AddDefaultProperties(exceptionTelemetry.Context, _context, _infoService);

        if (telemetry is DependencyTelemetry dependencyTelemetry)
            dependencyTelemetry.Properties.AddDefaultProperties(dependencyTelemetry.Context, _context, _infoService);

        if (telemetry is EventTelemetry eventTelemetry)
            eventTelemetry.Properties.AddDefaultProperties(eventTelemetry.Context, _context, _infoService);

        if (telemetry is MetricTelemetry metricTelemetry)
            metricTelemetry.Properties.AddDefaultProperties(metricTelemetry.Context, _context, _infoService);

        if (telemetry is TraceTelemetry traceTelemetry)
            traceTelemetry.Properties.AddDefaultProperties(traceTelemetry.Context, _context, _infoService);

        if (telemetry is PageViewTelemetry pageViewTelemetry)
            pageViewTelemetry.Properties.AddDefaultProperties(pageViewTelemetry.Context, _context, _infoService);

        if (telemetry is ISupportProperties supportProperties && !supportProperties.Properties.ContainsKey("client-ip"))
        {
            var clientIPAddress = telemetry.Context.Location.Ip;
            supportProperties.Properties.Add("client-ip", clientIPAddress);
        }
    }
}
