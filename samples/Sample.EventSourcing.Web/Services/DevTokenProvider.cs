using System.Net.Http.Json;

namespace Sample.EventSourcing.Web.Services;

/// <summary>
/// Obtains a JWT bearer token for the active tenant from the API's Development-only
/// <c>POST /dev/token</c> endpoint and caches it until it nears expiry or the tenant changes.
/// </summary>
public sealed class DevTokenProvider(HttpClient http, TenantContext tenantContext)
{
    private static readonly TimeSpan s_expirySkew = TimeSpan.FromSeconds(60);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _token;
    private Guid _tokenTenant;
    private DateTimeOffset _expiresAt;

    /// <summary>
    /// Returns a valid bearer token for the current tenant, refreshing it when the cached
    /// token was issued for a different tenant or is close to expiry.
    /// </summary>
    public async Task<string> GetTokenAsync(CancellationToken ct = default)
    {
        var tenant = tenantContext.TenantId;

        if (_token is not null && _tokenTenant == tenant && DateTimeOffset.UtcNow < _expiresAt - s_expirySkew)
        {
            return _token;
        }

        await _gate.WaitAsync(ct);
        try
        {
            // Re-check: another caller may have refreshed while we waited.
            if (_token is not null && _tokenTenant == tenant && DateTimeOffset.UtcNow < _expiresAt - s_expirySkew)
            {
                return _token;
            }

            var response = await http.PostAsJsonAsync("/dev/token", new { tenantId = tenant, username = "demo-user" }, ct);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<DevTokenResponse>(ct)
                ?? throw new InvalidOperationException("The token endpoint returned an empty response.");

            _token = token.AccessToken;
            _tokenTenant = tenant;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
            return _token;
        }
        finally
        {
            _gate.Release();
        }
    }

    private sealed record DevTokenResponse(string AccessToken, string TokenType, int ExpiresIn);
}
