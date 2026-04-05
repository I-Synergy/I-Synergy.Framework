using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Sample.EventSourcing.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for triggering and querying the event archive.
/// </summary>
public static class ArchiveEndpoints
{
    /// <summary>Registers archive-related endpoints on <paramref name="app"/>.</summary>
    public static IEndpointRouteBuilder MapArchiveEndpoints(this IEndpointRouteBuilder app)
    {
        var env = app.ServiceProvider.GetRequiredService<IHostEnvironment>();

        // POST /api/archive/run  — only exposed in Development.
        // This endpoint triggers a global cross-tenant archive operation that permanently
        // deletes hot-tier events. In production, trigger via a scheduled background job
        // or an admin-only endpoint protected with proper authorization.
        if (env.IsDevelopment())
        {
            app.MapPost("/api/archive/run", async (
                IEventArchiver archiver,
                CancellationToken ct) =>
            {
                var result = await archiver.ArchiveOldEventsAsync(ct);
                return Results.Ok(result);
            })
            .WithName("RunArchive")
            .WithSummary("Trigger the global event archive job (Development only)")
            .WithDescription(
                "Archives events older than the configured RetentionDays to blob cold storage " +
                "across all tenants. Returns the number of streams and events archived.");
        }

        // GET /api/orders/{id}/full-history — cold + hot event history for an order
        app.MapGet("/api/orders/{id:guid}/full-history", async (
            Guid id,
            IEventArchiveReader reader,
            ITenantService tenantService,
            CancellationToken ct) =>
        {
            var events = await reader.GetFullHistoryAsync(
                tenantService.TenantId, "Order", id, ct);

            var result = events.Select(e => new
            {
                e.EventId,
                e.EventType,
                e.AggregateVersion,
                e.Timestamp,
                e.UserId
            });

            return Results.Ok(result);
        })
        .WithName("GetOrderFullHistory")
        .WithSummary("Get full (cold + hot) event history for an order")
        .WithDescription(
            "Returns all events for the order, combining archived cold-storage segments " +
            "with current hot-tier events, ordered by aggregate version.");

        return app;
    }
}
