using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Test double for IOrganization where Name maps to an Alias property,
/// mimicking the Keycloak-backed Organization pattern.
/// </summary>
public sealed class TestOrganizationWithAlias : IOrganization
{
    public Guid OrganizationId { get; set; }
    public string Alias { get; set; } = string.Empty;

    string IOrganization.Name
    {
        get => Alias;
        set => Alias = value;
    }
}
