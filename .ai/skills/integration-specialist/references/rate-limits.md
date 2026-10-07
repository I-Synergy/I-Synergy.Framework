# Rate Limit Handling

Honouring `429 Too Many Requests`, using `Retry-After`, and falling back to exponential backoff. Load this when an external API throttles the integration.

## Rate Limit Handling

```csharp
public class RateLimitedApiClient(
    HttpClient httpClient,
    ILogger<RateLimitedApiClient> logger
) : IExternalApiClient
{
    public async Task<ExternalData> GetDataAsync(
        string dataId,
        CancellationToken cancellationToken = default)
    {
        var maxRetries = 3;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var response = await httpClient.GetAsync($"/data/{dataId}", cancellationToken);

                // Handle rate limit (429 Too Many Requests)
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);

                    logger.LogWarning(
                        "Rate limited by external API. Retrying after {RetryAfter}",
                        retryAfter);

                    await Task.Delay(retryAfter, cancellationToken);
                    retryCount++;
                    continue;
                }

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<ExternalData>(
                    cancellationToken: cancellationToken);

                return data ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (HttpRequestException ex) when (retryCount < maxRetries - 1)
            {
                logger.LogWarning(
                    ex,
                    "Request failed, retrying ({RetryCount}/{MaxRetries})",
                    retryCount + 1, maxRetries);

                retryCount++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)), cancellationToken);
            }
        }

        throw new ExternalApiException("Failed to fetch data after max retries");
    }
}
```
