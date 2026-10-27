# OAuth2 Client Flow

The cached client-credentials token service and the delegating handler that attaches the bearer token. Load this when an external API needs machine-to-machine authentication.

## OAuth2 Client Flow

```csharp
// File: {ApplicationName}.Infrastructure.ExternalServices/OAuth2TokenService.cs
namespace {ApplicationName}.Infrastructure.ExternalServices;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public interface IOAuth2TokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public class OAuth2TokenService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<OAuth2TokenService> logger
) : IOAuth2TokenService
{
    private string? _cachedToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Return cached token if valid
        if (!string.IsNullOrEmpty(_cachedToken) && DateTimeOffset.UtcNow < _tokenExpiry)
        {
            logger.LogDebug("Using cached OAuth2 token");
            return _cachedToken;
        }

        logger.LogDebug("Requesting new OAuth2 token");

        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = configuration["OAuth:ClientId"]!,
            ["client_secret"] = configuration["OAuth:ClientSecret"]!,
            ["scope"] = configuration["OAuth:Scope"]!
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/token")
        {
            Content = new FormUrlEncodedContent(tokenRequest)
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(
            cancellationToken: cancellationToken);

        if (tokenResponse is null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            throw new InvalidOperationException("Failed to obtain OAuth2 token");

        _cachedToken = tokenResponse.AccessToken;
        _tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Refresh 1 minute early

        logger.LogInformation("Successfully obtained OAuth2 token, expires in {ExpiresIn}s", tokenResponse.ExpiresIn);

        return _cachedToken;
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );
}

// Add authentication to API client
builder.Services.AddHttpClient<IExternalApiClient, ExternalApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalApi:BaseUrl"]!);
})
.AddHttpMessageHandler<OAuth2DelegatingHandler>();

// OAuth2 delegating handler
public class OAuth2DelegatingHandler(
    IOAuth2TokenService tokenService
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
```
