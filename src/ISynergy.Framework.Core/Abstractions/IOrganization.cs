namespace ISynergy.Framework.Core.Abstractions;

public interface IOrganization
{
    Guid OrganizationId { get; set; }
    string Name { get; set; }
}
