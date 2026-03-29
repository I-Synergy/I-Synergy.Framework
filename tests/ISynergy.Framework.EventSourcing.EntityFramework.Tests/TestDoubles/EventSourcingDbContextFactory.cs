using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EventSourcing.EntityFramework.TestDoubles;

/// <summary>
/// Creates isolated <see cref="EventSourcingDbContext"/> instances backed by the
/// EF Core in-memory provider. Each call produces a fresh database.
/// </summary>
internal static class EventSourcingDbContextFactory
{
    public static EventSourcingDbContext Create(TestTenantService tenantService)
    {
        var options = new DbContextOptionsBuilder<EventSourcingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new EventSourcingDbContext(options, tenantService);
    }
}
