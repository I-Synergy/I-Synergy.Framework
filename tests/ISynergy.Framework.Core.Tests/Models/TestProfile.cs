using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Test double for IProfile used in unit tests.
/// </summary>
public sealed class TestProfile : IProfile
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CultureCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public List<string> Features { get; set; } = [];
    public DateTimeOffset Expiration { get; set; } = DateTimeOffset.UtcNow.AddHours(1);
    public required Token Token { get; set; }

    public List<IOrganization> Organizations { get; set; } = [];

    IReadOnlyList<IOrganization> IProfile.Organizations => Organizations.AsReadOnly();
}
