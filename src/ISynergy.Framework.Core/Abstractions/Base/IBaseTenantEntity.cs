namespace ISynergy.Framework.Core.Abstractions.Base;

public interface IBaseTenantEntity : IBaseEntity
{
    public Guid TenantId { get; set; }
}
