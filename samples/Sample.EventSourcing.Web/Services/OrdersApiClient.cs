using System.Net.Http.Json;
using System.Text.Json;

namespace Sample.EventSourcing.Web.Services;

/// <summary>
/// Typed HTTP client for the Event Sourcing API.
/// The base URL is resolved via Aspire service discovery ("api").
/// All requests carry the configured <see cref="TenantId"/> as X-Tenant-Id header.
/// </summary>
public sealed class OrdersApiClient(HttpClient http)
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// The tenant ID sent with every request.
    /// Change this to switch tenants and observe data isolation.
    /// </summary>
    public Guid TenantId { get; set; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public async Task<List<OrderSummary>> GetOrdersAsync(CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Get, "/api/orders");
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<OrderSummary>>(s_jsonOptions, ct) ?? [];
    }

    public async Task<OrderDetail?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Get, $"/api/orders/{orderId}");
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    public async Task<List<EventEntry>> GetOrderEventsAsync(Guid orderId, CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Get, $"/api/orders/{orderId}/events");
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<EventEntry>>(s_jsonOptions, ct) ?? [];
    }

    public async Task<Guid> PlaceOrderAsync(string customerName, decimal total, CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Post, "/api/orders");
        request.Content = JsonContent.Create(new { customerName, total });
        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PlaceOrderResult>(s_jsonOptions, ct);
        return result!.OrderId;
    }

    public async Task<OrderDetail?> EditOrderAsync(Guid orderId, string customerName, decimal total, CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Put, $"/api/orders/{orderId}");
        request.Content = JsonContent.Create(new { customerName, total });
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    public async Task<OrderDetail?> ShipOrderAsync(Guid orderId, string trackingNumber, CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Post, $"/api/orders/{orderId}/ship");
        request.Content = JsonContent.Create(new { trackingNumber });
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    public async Task<OrderDetail?> CancelOrderAsync(Guid orderId, string reason, CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Post, $"/api/orders/{orderId}/cancel");
        request.Content = JsonContent.Create(new { reason });
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetail>(s_jsonOptions, ct);
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("X-Tenant-Id", TenantId.ToString());
        return request;
    }
}

public record OrderSummary(Guid OrderId, string? CustomerName, decimal? Total, DateTimeOffset PlacedAt);
public record OrderDetail(Guid OrderId, string CustomerName, decimal Total, string Status, string? TrackingNumber, string? CancellationReason, long Version);
public record EventEntry(Guid EventId, string EventType, long AggregateVersion, DateTimeOffset Timestamp, string? UserId);
public record PlaceOrderResult(Guid OrderId, long Version);
