using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Sample.EventSourcing.Web.Services;

/// <summary>
/// Typed HTTP client for the Event Sourcing API.
/// The base URL is resolved via Aspire service discovery ("api").
/// All requests carry a bearer token for the active tenant obtained from <see cref="DevTokenProvider"/>.
/// </summary>
public sealed class OrdersApiClient(HttpClient http, DevTokenProvider devTokenProvider)
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<OrderSummary>> GetOrdersAsync(CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Get, "/api/orders", ct);
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<OrderSummary>>(s_jsonOptions, ct) ?? [];
    }

    public async Task<OrderDetail?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Get, $"/api/orders/{orderId}", ct);
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    public async Task<List<EventEntry>> GetOrderEventsAsync(Guid orderId, CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Get, $"/api/orders/{orderId}/events", ct);
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<EventEntry>>(s_jsonOptions, ct) ?? [];
    }

    public async Task<Guid> PlaceOrderAsync(string customerName, decimal total, CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Post, "/api/orders", ct);
        request.Content = JsonContent.Create(new { customerName, total });
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PlaceOrderResult>(s_jsonOptions, ct);
        return result!.OrderId;
    }

    public async Task<OrderDetail?> EditOrderAsync(Guid orderId, string customerName, decimal total, CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Put, $"/api/orders/{orderId}", ct);
        request.Content = JsonContent.Create(new { customerName, total });
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    public async Task<OrderDetail?> ShipOrderAsync(Guid orderId, string trackingNumber, CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Post, $"/api/orders/{orderId}/ship", ct);
        request.Content = JsonContent.Create(new { trackingNumber });
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    public async Task<OrderDetail?> CancelOrderAsync(Guid orderId, string reason, CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Post, $"/api/orders/{orderId}/cancel", ct);
        request.Content = JsonContent.Create(new { reason });
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    public async Task<ArchiveRunResult?> RunArchiveAsync(CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Post, "/api/archive/run", ct);
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ArchiveRunResult>(s_jsonOptions, ct);
    }

    public async Task<List<EventEntry>> GetOrderFullHistoryAsync(Guid orderId, CancellationToken ct = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Get, $"/api/orders/{orderId}/full-history", ct);
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<EventEntry>>(s_jsonOptions, ct) ?? [];
    }

    private async Task<HttpRequestMessage> BuildRequestAsync(HttpMethod method, string url, CancellationToken ct)
    {
        var request = new HttpRequestMessage(method, url);
        var token = await devTokenProvider.GetTokenAsync(ct);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}

public record OrderSummary(Guid OrderId, string? CustomerName, decimal? Total, DateTimeOffset PlacedAt);
public record OrderDetail(Guid OrderId, string CustomerName, decimal Total, string Status, string? TrackingNumber, string? CancellationReason, long Version);
public record EventEntry(Guid EventId, string EventType, long AggregateVersion, DateTimeOffset Timestamp, string? UserId);
public record PlaceOrderResult(Guid OrderId, long Version);
public record ArchiveRunResult(int StreamsArchived, long EventsArchived, int Errors);
