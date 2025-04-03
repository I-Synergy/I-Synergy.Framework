using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace ISynergy.Framework.Core.Abstractions;

/// <summary>
/// Interface IContext
/// </summary>
public interface IContext
{
    IProfile? Profile { get; set; }
    TimeZoneInfo? TimeZone { get; }
    NumberFormatInfo? NumberFormat { get; }
    CultureInfo? Culture { get; }
    SoftwareEnvironments Environment { get; set; }
    bool IsAuthenticated { get; }
    string? GatewayEndpoint { get; }
}
