using ISynergy.Framework.EventSourcing.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="EventArchiveIndex"/>.
/// </summary>
internal sealed class EventArchiveIndexConfiguration : IEntityTypeConfiguration<EventArchiveIndex>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EventArchiveIndex> builder)
    {
        builder.ToTable("EventArchiveIndex");
        builder.HasKey(e => e.IndexId);

        builder.Property(e => e.StreamType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.BlobPath)
            .IsRequired()
            .HasMaxLength(1024);

        // Primary lookup: all archive entries for a specific aggregate within a tenant.
        builder.HasIndex(e => new { e.TenantId, e.StreamId, e.StreamType })
            .HasDatabaseName("IX_EventArchiveIndex_TenantId_StreamId_StreamType");

        // Supports time-range audit queries per tenant.
        builder.HasIndex(e => new { e.TenantId, e.ArchivedAt })
            .HasDatabaseName("IX_EventArchiveIndex_TenantId_ArchivedAt");
    }
}
