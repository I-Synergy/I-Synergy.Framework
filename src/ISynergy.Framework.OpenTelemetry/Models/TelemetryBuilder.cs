using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

namespace ISynergy.Framework.OpenTelemetry.Models;

public class TelemetryBuilder
{
    public OpenTelemetryBuilder OpenTelemetryBuilder { get; }
    public HostBuilderContext HostBuilderContext { get; }
    public IInfoService InfoService { get; }

    public TelemetryBuilder(OpenTelemetryBuilder openTelemetryBuilder, HostBuilderContext hostBuilderContext, IInfoService infoService)
    {
        OpenTelemetryBuilder = openTelemetryBuilder;
        HostBuilderContext = hostBuilderContext;
        InfoService = infoService;
    }
}
