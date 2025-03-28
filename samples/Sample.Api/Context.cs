using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;

namespace Sample.Api;

public class Context : IContext
{
    public IProfile Profile { get; set; }

    public TimeZoneInfo TimeZone { get; }

    public SoftwareEnvironments Environment { get; set; }

    public bool IsAuthenticated { get; }

    public string GatewayEndpoint { get; }
}
