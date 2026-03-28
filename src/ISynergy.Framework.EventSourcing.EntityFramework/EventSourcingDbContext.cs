using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.EventSourcing.EntityFramework.Configurations;
using ISynergy.Framework.EventSourcing.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.EventSourcing.EntityFramework;

/// <summary>
/// EF Core <see cref="DbContext"/> that persists the event store tables
/// (<c>EventStore</c> and <c>Snapshots</c>) with automatic tenant query filtering.
/// </summary>
/// <remarks>
/// <para>
/// All read queries are automatically scoped to <see cref="ITenantService.TenantId"/> via a
/// global query filter, mirroring how <c>ApplyTenantFilters()</c> works in the framework's
/// main DbContext base. Write operations (via <see cref="Services.EventStore"/>) use an explicit
/// <c>tenantId</c> parameter and bypass the query filter.
/// </para>
/// <para>
/// This DbContext is designed for use with <c>AddDbContext</c> (scoped lifetime). Using
/// <c>AddDbContextPool</c> is not supported because the global query filter captures
/// <c>tenantService</c> by reference and EF Core pooling reuses model instances
/// across scopes, which would share the filter closure.
/// </para>
/// </remarks>
public class EventSourcingDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    /// <summary>
    /// Initializes a new instance of <see cref="EventSourcingDbContext"/>.
    /// </summary>
    /// <param name="options">EF Core context options.</param>
    /// <param name="tenantService">
    /// Provides the current tenant identifier used to scope all read queries.
    /// </param>
    [RequiresUnreferencedCode("EF Core isn't fully compatible with trimming. See https://aka.ms/efcore-docs-trimming for more details.")]
    [RequiresDynamicCode("EF Core isn't fully compatible with NativeAOT.")]
    public EventSourcingDbContext(DbContextOptions<EventSourcingDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    /// <summary>Gets the event log table.</summary>
    public DbSet<EventRecord> Events => Set<EventRecord>();

    /// <summary>Gets the snapshots table.</summary>
    public DbSet<Snapshot> Snapshots => Set<Snapshot>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new EventRecordConfiguration());
        modelBuilder.ApplyConfiguration(new SnapshotConfiguration());

        // Global tenant filter: all SELECT queries are automatically scoped to the current tenant.
        // The lambda captures _tenantService (not the TenantId value), so TenantId is re-evaluated
        // at each query execution — correct for scoped DbContext instances.
        modelBuilder.Entity<EventRecord>()
            .HasQueryFilter(e => e.TenantId == _tenantService.TenantId);

        modelBuilder.Entity<Snapshot>()
            .HasQueryFilter(s => s.TenantId == _tenantService.TenantId);
    }
}
