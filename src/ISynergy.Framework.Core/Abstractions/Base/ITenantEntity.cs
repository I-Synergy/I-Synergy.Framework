namespace ISynergy.Framework.Core.Abstractions.Base;

public interface ITenantEntity : IEntity
{
    public Guid TenantId { get; set; }
}
