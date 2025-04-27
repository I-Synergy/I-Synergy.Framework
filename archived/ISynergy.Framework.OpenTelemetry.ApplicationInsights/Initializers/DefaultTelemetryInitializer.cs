using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace ISynergy.Framework.OpenTelemetry.Initializers;

public class DefaultTelemetryInitializer : ITelemetryInitializer
{
    private readonly IInfoService _infoService;
    private readonly IContext _context;

    public DefaultTelemetryInitializer(IInfoService infoService, IContext context)
    {
        _infoService = infoService;
        _context = context;
    }

    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is RequestTelemetry requestTelemetry)
            requestTelemetry.Properties.AddDefaultProperties(requestTelemetry.Context, _infoService, _context);

        if (telemetry is ExceptionTelemetry exceptionTelemetry)
            exceptionTelemetry.Properties.AddDefaultProperties(exceptionTelemetry.Context, _infoService, _context);

        if (telemetry is DependencyTelemetry dependencyTelemetry)
            dependencyTelemetry.Properties.AddDefaultProperties(dependencyTelemetry.Context, _infoService, _context);

        if (telemetry is EventTelemetry eventTelemetry)
            eventTelemetry.Properties.AddDefaultProperties(eventTelemetry.Context, _infoService, _context);

        if (telemetry is MetricTelemetry metricTelemetry)
            metricTelemetry.Properties.AddDefaultProperties(metricTelemetry.Context, _infoService, _context);

        if (telemetry is TraceTelemetry traceTelemetry)
            traceTelemetry.Properties.AddDefaultProperties(traceTelemetry.Context, _infoService, _context);

        if (telemetry is PageViewTelemetry pageViewTelemetry)
            pageViewTelemetry.Properties.AddDefaultProperties(pageViewTelemetry.Context, _infoService, _context);

        if (telemetry is ISupportProperties supportProperties && !supportProperties.Properties.ContainsKey("client-ip"))
        {
            var clientIPAddress = telemetry.Context.Location.Ip;
            supportProperties.Properties.Add("client-ip", clientIPAddress);
        }
    }
}
