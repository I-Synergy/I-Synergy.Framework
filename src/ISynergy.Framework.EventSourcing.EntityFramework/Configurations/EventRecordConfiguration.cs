using ISynergy.Framework.EventSourcing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="EventRecord"/>.
/// </summary>
internal sealed class EventRecordConfiguration : IEntityTypeConfiguration<EventRecord>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EventRecord> builder)
    {
        builder.ToTable("EventStore");
        builder.HasKey(e => e.EventId);

        builder.Property(e => e.AggregateType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(e => e.Data)
            .IsRequired();

        builder.Property(e => e.Metadata);

        builder.Property(e => e.UserId)
            .HasMaxLength(256);

        // Primary lookup: all events for a specific aggregate within a tenant.
        builder.HasIndex(e => new { e.AggregateId, e.TenantId })
            .HasDatabaseName("IX_EventStore_AggregateId_TenantId");

        // Enforce optimistic concurrency at DB level: no two events can share
        // the same (AggregateId, AggregateVersion) pair.
        builder.HasIndex(e => new { e.AggregateId, e.AggregateVersion })
            .IsUnique()
            .HasDatabaseName("UX_EventStore_AggregateId_Version");

        // Supports GetEventsByTypeAsync and GetEventsSinceAsync.
        builder.HasIndex(e => new { e.EventType, e.Timestamp })
            .HasDatabaseName("IX_EventStore_EventType_Timestamp");

        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_EventStore_Timestamp");
    }
}
