using ISynergy.Framework.EventSourcing.Enumerations;
using ISynergy.Framework.EventSourcing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="EventRecord"/>.
/// </summary>
/// <remarks>
/// The table is partitioned by list on <c>TenantId</c>, so the partition key is part of the
/// composite primary key <c>(EventId, TenantId)</c>. All secondary indexes are tenant-prefixed
/// so they stay usable by queries that filter on the current tenant.
/// </remarks>
internal sealed class EventRecordConfiguration : IEntityTypeConfiguration<EventRecord>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EventRecord> builder)
    {
        builder.ToTable("EventStore");

        // The partition key (TenantId) must be part of the primary key for a
        // PARTITION BY LIST (TenantId) table.
        builder.HasKey(e => new { e.EventId, e.TenantId });

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

        builder.Property(e => e.RecordType)
            .HasConversion<short>()
            .IsRequired()
            .HasDefaultValue(EventRecordTypes.DomainEvent);

        // Enforce optimistic concurrency at DB level: no two events can share
        // the same (TenantId, AggregateId, AggregateVersion) tuple.
        builder.HasIndex(e => new { e.TenantId, e.AggregateId, e.AggregateVersion })
            .IsUnique()
            .HasDatabaseName("UX_EventStore_TenantId_AggregateId_Version");

        // Primary lookup: all events for a specific aggregate within a tenant.
        builder.HasIndex(e => new { e.TenantId, e.AggregateId })
            .HasDatabaseName("IX_EventStore_TenantId_AggregateId");

        // Supports GetEventsByTypeAsync and GetEventsSinceAsync.
        builder.HasIndex(e => new { e.TenantId, e.EventType, e.Timestamp })
            .HasDatabaseName("IX_EventStore_TenantId_EventType_Timestamp");

        builder.HasIndex(e => new { e.TenantId, e.Timestamp })
            .HasDatabaseName("IX_EventStore_TenantId_Timestamp");

        // Supports filtering the event log by row kind (e.g. domain events only).
        builder.HasIndex(e => new { e.TenantId, e.RecordType, e.Timestamp })
            .HasDatabaseName("IX_EventStore_TenantId_RecordType_Timestamp");
    }
}
