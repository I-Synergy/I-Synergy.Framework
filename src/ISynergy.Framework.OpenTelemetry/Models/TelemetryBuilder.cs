using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;

namespace ISynergy.Framework.OpenTelemetry.Models;

public class TelemetryBuilder
{
    public OpenTelemetryBuilder OpenTelemetryBuilder { get; }
    public IConfiguration Configuration { get; }
    public IInfoService InfoService { get; }

    public TelemetryBuilder(OpenTelemetryBuilder openTelemetryBuilder, IConfiguration configuration, IInfoService infoService)
    {
        OpenTelemetryBuilder = openTelemetryBuilder;
        Configuration = configuration;
        InfoService = infoService;
    }
}
