using ISynergy.Framework.EventSourcing.Abstractions.Aggregates;
using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Sample.EventSourcing.Api.Domain;
using System.Text.Json;

namespace Sample.EventSourcing.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapPost("/", PlaceOrderAsync)
            .WithName("PlaceOrder")
            .WithSummary("Place a new order");

        group.MapGet("/", ListOrdersAsync)
            .WithName("ListOrders")
            .WithSummary("List recent orders (based on OrderPlaced events)");

        group.MapGet("/{id:guid}", GetOrderAsync)
            .WithName("GetOrder")
            .WithSummary("Get current order state by replaying its event stream");

        group.MapGet("/{id:guid}/events", GetOrderEventsAsync)
            .WithName("GetOrderEvents")
            .WithSummary("Get the raw event stream for an order");

        group.MapPut("/{id:guid}", EditOrderAsync)
            .WithName("EditOrder")
            .WithSummary("Update customer name and total of a pending order");

        group.MapPost("/{id:guid}/ship", ShipOrderAsync)
            .WithName("ShipOrder")
            .WithSummary("Ship a pending order");

        group.MapPost("/{id:guid}/cancel", CancelOrderAsync)
            .WithName("CancelOrder")
            .WithSummary("Cancel a pending order");

        return app;
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> PlaceOrderAsync(
        PlaceOrderRequest request,
        IAggregateRepository<Order, Guid> repository,
        CancellationToken cancellationToken)
    {
        var orderId = Guid.NewGuid();
        var order = new Order();
        order.Place(orderId, request.CustomerName, request.Total);

        await repository.SaveAsync(order, cancellationToken);

        return Results.Created($"/api/orders/{orderId}", new { orderId, version = order.Version });
    }

    private static readonly JsonSerializerOptions s_caseInsensitive = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static async Task<IResult> ListOrdersAsync(
        IEventStore store,
        EventSourcingDbContext dbContext,
        CancellationToken cancellationToken)
    {
        // Build order summaries without loading full aggregates to avoid N+1 queries.
        var summaries = new Dictionary<Guid, object>();

        // 1. Populate from snapshots — a single query covers all archived orders and any
        //    order that has been snapshotted.  The tenant query filter ensures isolation.
        var snapshots = await dbContext.Snapshots
            .Select(s => new { s.AggregateId, s.Data })
            .ToListAsync(cancellationToken);

        foreach (var snap in snapshots)
        {
            try
            {
                var data = JsonSerializer.Deserialize<OrderSnapshot>(snap.Data, s_caseInsensitive);
                if (data is not null)
                    summaries[snap.AggregateId] = new
                    {
                        orderId      = data.Id,
                        customerName = data.CustomerName,
                        total        = data.Total,
                        placedAt     = data.PlacedAt
                    };
            }
            catch (JsonException) { /* skip malformed snapshot data */ }
        }

        // 2. Hot orders not yet snapshotted — read from their OrderPlaced event (single query).
        var placedTypeName = typeof(OrderPlaced).FullName!;
        var hotEvents = await store.GetEventsByTypeAsync(placedTypeName, cancellationToken: cancellationToken);

        foreach (var ev in hotEvents)
        {
            if (summaries.ContainsKey(ev.AggregateId)) continue;

            try
            {
                var placed = JsonSerializer.Deserialize<OrderPlaced>(ev.Data, s_caseInsensitive);
                if (placed is not null)
                    summaries[ev.AggregateId] = new
                    {
                        orderId      = placed.OrderId,
                        customerName = placed.CustomerName,
                        total        = placed.Total,
                        placedAt     = ev.Timestamp
                    };
            }
            catch (JsonException) { /* skip malformed event data */ }
        }

        return Results.Ok(summaries.Values);
    }

    private static async Task<IResult> GetOrderAsync(
        Guid id,
        IAggregateRepository<Order, Guid> repository,
        CancellationToken cancellationToken)
    {
        var order = await repository.LoadAsync(id, cancellationToken);
        if (order is null)
            return Results.NotFound(new { message = $"Order {id} not found." });

        return Results.Ok(ToResponse(order));
    }

    private static async Task<IResult> GetOrderEventsAsync(
        Guid id,
        IEventStore store,
        CancellationToken cancellationToken)
    {
        var events = await store.GetEventsForAggregateAsync("Order", id, cancellationToken: cancellationToken);
        return Results.Ok(events.Select(e => new
        {
            e.EventId,
            e.EventType,
            e.AggregateVersion,
            e.Timestamp,
            e.UserId,
            Data = JsonSerializer.Deserialize<JsonElement>(e.Data)
        }));
    }

    private static async Task<IResult> EditOrderAsync(
        Guid id,
        EditOrderRequest request,
        IAggregateRepository<Order, Guid> repository,
        CancellationToken cancellationToken)
    {
        var order = await repository.LoadAsync(id, cancellationToken);
        if (order is null)
            return Results.NotFound(new { message = $"Order {id} not found." });

        try
        {
            order.UpdateDetails(request.CustomerName, request.Total);
            await repository.SaveAsync(order, cancellationToken);
            return Results.Ok(ToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }

    private static async Task<IResult> ShipOrderAsync(
        Guid id,
        ShipOrderRequest request,
        IAggregateRepository<Order, Guid> repository,
        CancellationToken cancellationToken)
    {
        var order = await repository.LoadAsync(id, cancellationToken);
        if (order is null)
            return Results.NotFound(new { message = $"Order {id} not found." });

        try
        {
            order.Ship(request.TrackingNumber);
            await repository.SaveAsync(order, cancellationToken);
            return Results.Ok(ToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }

    private static async Task<IResult> CancelOrderAsync(
        Guid id,
        CancelOrderRequest request,
        IAggregateRepository<Order, Guid> repository,
        CancellationToken cancellationToken)
    {
        var order = await repository.LoadAsync(id, cancellationToken);
        if (order is null)
            return Results.NotFound(new { message = $"Order {id} not found." });

        try
        {
            order.Cancel(request.Reason);
            await repository.SaveAsync(order, cancellationToken);
            return Results.Ok(ToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static object ToResponse(Order order) => new
    {
        orderId = order.Id,
        customerName = order.CustomerName,
        total = order.Total,
        status = order.Status.ToString(),
        trackingNumber = order.TrackingNumber,
        cancellationReason = order.CancellationReason,
        version = order.Version
    };

    // ── Request records ───────────────────────────────────────────────────────

    private record PlaceOrderRequest(string CustomerName, decimal Total);
    private record EditOrderRequest(string CustomerName, decimal Total);
    private record ShipOrderRequest(string TrackingNumber);
    private record CancelOrderRequest(string Reason);
}
