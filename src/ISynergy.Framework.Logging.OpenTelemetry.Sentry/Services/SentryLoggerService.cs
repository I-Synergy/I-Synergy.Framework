using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Logging.Base;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Services;

public class SentryLoggerService : BaseLogger
{
    private readonly IContext _context;

    public SentryLoggerService(ILoggerFactory loggerFactory)
        : base("Sentry Logger", loggerFactory)
    {
    }

    override public void Flush()
    {
        SentrySdk.Flush();
    }
}
