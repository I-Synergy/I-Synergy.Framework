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

    private static async Task<IResult> ListOrdersAsync(
        IAggregateRepository<Order, Guid> repository,
        IEventStore store,
        EventSourcingDbContext dbContext,
        CancellationToken cancellationToken)
    {
        // Discover order IDs from two sources so archived orders remain visible:
        // 1. Hot events still in PostgreSQL (OrderPlaced event type)
        // 2. EventArchiveIndex rows for StreamType == "Order" (events moved to cold storage)
        var typeName = typeof(OrderPlaced).FullName!;

        var hotIds = (await store.GetEventsByTypeAsync(typeName, cancellationToken: cancellationToken))
            .Select(e => e.AggregateId)
            .ToHashSet();

        var archivedIds = await dbContext.ArchiveIndexes
            .Where(a => a.StreamType == "Order")
            .Select(a => a.StreamId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var allIds = hotIds.Union(archivedIds).ToList();

        var orders = new List<object>();
        foreach (var id in allIds)
        {
            var order = await repository.LoadAsync(id, cancellationToken);
            if (order is null) continue;

            orders.Add(new
            {
                orderId      = order.Id,
                customerName = order.CustomerName,
                total        = order.Total,
                placedAt     = order.PlacedAt
            });
        }

        return Results.Ok(orders);
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
