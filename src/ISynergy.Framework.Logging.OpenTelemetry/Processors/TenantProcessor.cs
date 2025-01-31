using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Logging.Extensions;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace ISynergy.Framework.Logging.Processors;

public class TenantProcessor : BaseProcessor<LogRecord>
{
    private readonly IInfoService _infoService;
    private readonly IContext _context;

    public TenantProcessor(IContext context, IInfoService infoService)
    {
        _context = context;
        _infoService = infoService;
    }

    public override void OnEnd(LogRecord data)
    {
        var attributes = new Dictionary<string, object>();
        attributes.AddDefaultProperties(_infoService, _context);
        attributes.AddRange(data.Attributes);
        data.Attributes = attributes.ToList().AsReadOnly();
        base.OnEnd(data);
    }
}
