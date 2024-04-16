namespace ISynergy.Framework.Core.Abstractions.Base;

public interface IBaseTenantEntity : IEntityBase
{
    public Guid TenantId { get; set; }
}
