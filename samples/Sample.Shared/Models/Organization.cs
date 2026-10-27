using ISynergy.Framework.Core.Abstractions;

namespace Sample.Models;

public sealed class Organization : IOrganization
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
}
