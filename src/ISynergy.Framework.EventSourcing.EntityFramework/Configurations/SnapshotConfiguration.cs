using ISynergy.Framework.EventSourcing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="Snapshot"/>.
/// </summary>
/// <remarks>
/// One snapshot is kept per aggregate instance per tenant.
/// The composite primary key <c>(AggregateId, TenantId)</c> enforces this invariant.
/// </remarks>
internal sealed class SnapshotConfiguration : IEntityTypeConfiguration<Snapshot>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Snapshot> builder)
    {
        builder.ToTable("Snapshots");

        // One snapshot per aggregate per tenant.
        builder.HasKey(s => new { s.AggregateId, s.TenantId });

        builder.Property(s => s.AggregateType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(s => s.Data)
            .IsRequired();
    }
}
