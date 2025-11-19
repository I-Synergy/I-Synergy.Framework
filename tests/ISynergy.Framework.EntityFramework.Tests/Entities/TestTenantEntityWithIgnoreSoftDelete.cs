using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.EntityFramework.Attributes;
using ISynergy.Framework.EntityFramework.Base;

namespace ISynergy.Framework.EntityFramework.Tests.Entities;

[IgnoreSoftDelete]
public class TestTenantEntityWithIgnoreSoftDelete : BaseTenantEntity
{
    [Identity]
    public Guid Id { get; set; }
    public decimal TestDecimal { get; set; }
}