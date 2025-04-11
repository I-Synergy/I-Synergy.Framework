using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.EntityFramework.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISynergy.Framework.EntityFramework.Tests.Entities;

public class TestEntity : BaseEntity
{
    [Identity]
    public int Id { get; set; }
    public decimal TestDecimal { get; set; }
    public string? Name { get; set; }
}

public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100);
    }
}
