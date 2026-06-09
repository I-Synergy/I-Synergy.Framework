using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Test double for IOrganization with a direct Name property.
/// </summary>
public sealed class TestOrganization : IOrganization
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
}
