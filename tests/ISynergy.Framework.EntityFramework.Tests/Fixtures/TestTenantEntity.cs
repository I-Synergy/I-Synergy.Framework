using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.EntityFramework.Base;

namespace ISynergy.Framework.EntityFramework.Tests.Fixtures;
internal class TestTenantEntity : BaseTenantEntity
{
    [Identity]
    public int Id { get; set; }
    public decimal TestDecimal { get; set; }
}
