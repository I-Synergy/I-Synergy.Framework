using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.EntityFramework.Base;

namespace ISynergy.Framework.EntityFramework.Tests.Entities;

public class TestEntity : BaseEntity
{
    [Identity]
    public int Id { get; set; }
    public decimal TestDecimal { get; set; }
}
