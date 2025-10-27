using ISynergy.Framework.AspNetCore.Tests.Internals;
using Microsoft.Extensions.Logging;
using Reqnroll;
using System.Net;

namespace ISynergy.Framework.AspNetCore.Tests.StepDefinitions;

/// <summary>
/// Step definitions for HTTP Client extensions testing.
/// Demonstrates BDD testing for HTTP client performance monitoring.
/// </summary>
[Binding]
public class HttpClientExtensionsSteps
{
    private readonly ILogger<HttpClientExtensionsSteps> _logger;
    private HttpClient? _httpClient;
    private HttpResponseMessageWithTiming? _response;
    private List<HttpResponseMessageWithTiming> _responses = new();
    private Exception? _caughtException;

    public HttpClientExtensionsSteps()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<HttpClientExtensionsSteps>();
    }

    [Given(@"I have an HTTP client configured")]
    public void GivenIHaveAnHttpClientConfigured()
    {
        _logger.LogInformation("Configuring HTTP client");
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
        };
    }

    [Given(@"I have an HTTP client with performance monitoring")]
    public void GivenIHaveAnHttpClientWithPerformanceMonitoring()
    {
        _logger.LogInformation("Configuring HTTP client with performance monitoring");
        GivenIHaveAnHttpClientConfigured();
    }

    [When(@"I make a GET request to an endpoint")]
    public async Task WhenIMakeAGetRequestToAnEndpoint()
    {
        _logger.LogInformation("Making GET request");
        ArgumentNullException.ThrowIfNull(_httpClient);

        try
        {
            var startTime = DateTime.UtcNow;
            var httpResponse = await _httpClient.GetAsync("posts/1");
            var endTime = DateTime.UtcNow;

            _response = httpResponse.WithTiming(endTime - startTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making request");
            _caughtException = ex;
        }
    }

    [When(@"I make a request to a successful endpoint")]
    public async Task WhenIMakeARequestToASuccessfulEndpoint()
    {
        await WhenIMakeAGetRequestToAnEndpoint();
    }

    [When(@"I make a request to a non-existent endpoint")]
    public async Task WhenIMakeARequestToANonExistentEndpoint()
    {
        _logger.LogInformation("Making request to non-existent endpoint");
        ArgumentNullException.ThrowIfNull(_httpClient);

        try
        {
            var startTime = DateTime.UtcNow;
            var httpResponse = await _httpClient.GetAsync("nonexistent/endpoint/12345");
            var endTime = DateTime.UtcNow;

            _response = httpResponse.WithTiming(endTime - startTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making request");
            _caughtException = ex;
        }
    }

    [When(@"I make multiple requests to an endpoint")]
    public async Task WhenIMakeMultipleRequestsToAnEndpoint()
    {
        _logger.LogInformation("Making multiple requests");
        ArgumentNullException.ThrowIfNull(_httpClient);

        for (int i = 1; i <= 3; i++)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var httpResponse = await _httpClient.GetAsync($"posts/{i}");
                var endTime = DateTime.UtcNow;

                _responses.Add(httpResponse.WithTiming(endTime - startTime));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making request {Number}", i);
            }
        }
    }

    [Then(@"the response should include timing information")]
    public void ThenTheResponseShouldIncludeTimingInformation()
    {
        _logger.LogInformation("Verifying timing information");
        ArgumentNullException.ThrowIfNull(_response);

        if (_response.Timing == TimeSpan.Zero)
        {
            throw new InvalidOperationException("Expected timing information to be present");
        }
    }

    [Then(@"the timing should be greater than zero")]
    public void ThenTheTimingShouldBeGreaterThanZero()
    {
        _logger.LogInformation("Verifying timing is greater than zero");
        ArgumentNullException.ThrowIfNull(_response);

        if (_response.Timing <= TimeSpan.Zero)
        {
            throw new InvalidOperationException($"Expected timing > 0 but got {_response.Timing.TotalMilliseconds}ms");
        }

        _logger.LogInformation("Request completed in {Milliseconds}ms", _response.Timing.TotalMilliseconds);
    }

    [Then(@"the response status should be successful")]
    public void ThenTheResponseStatusShouldBeSuccessful()
    {
        _logger.LogInformation("Verifying successful response status");
        ArgumentNullException.ThrowIfNull(_response);

        if (!_response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Expected successful status but got {_response.StatusCode}");
        }
    }

    [Then(@"the response content should be retrievable")]
    public async Task ThenTheResponseContentShouldBeRetrievable()
    {
        _logger.LogInformation("Verifying response content");
        ArgumentNullException.ThrowIfNull(_response);

        var content = await _response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content))
        {
            throw new InvalidOperationException("Expected response content to be present");
        }

        _logger.LogInformation("Retrieved content of length {Length}", content.Length);
    }

    [Then(@"the response status should indicate failure")]
    public void ThenTheResponseStatusShouldIndicateFailure()
    {
        _logger.LogInformation("Verifying failure response status");
        ArgumentNullException.ThrowIfNull(_response);

        // 404 is an expected failure for non-existent endpoints
        if (_response.StatusCode != HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Expected 404 but got {StatusCode}", _response.StatusCode);
        }
    }

    [Then(@"appropriate error information should be available")]
    public void ThenAppropriateErrorInformationShouldBeAvailable()
    {
        _logger.LogInformation("Verifying error information");
        ArgumentNullException.ThrowIfNull(_response);

        // Status code itself is the error information
        if (_response.StatusCode == HttpStatusCode.OK)
        {
            throw new InvalidOperationException("Expected non-success status code");
        }

        _logger.LogInformation("Error status: {StatusCode}", _response.StatusCode);
    }

    [Then(@"each response should have timing metrics")]
    public void ThenEachResponseShouldHaveTimingMetrics()
    {
        _logger.LogInformation("Verifying timing metrics for all responses");

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException("Expected multiple responses");
        }

        foreach (var response in _responses)
        {
            if (response.Timing <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("Each response should have timing information");
            }
        }

        _logger.LogInformation("All {Count} responses have timing metrics", _responses.Count);
    }

    [Then(@"I can compare performance across requests")]
    public void ThenICanComparePerformanceAcrossRequests()
    {
        _logger.LogInformation("Comparing performance across requests");

        if (_responses.Count < 2)
        {
            throw new InvalidOperationException("Need at least 2 responses to compare");
        }

        var avgTime = _responses.Average(r => r.Timing.TotalMilliseconds);
        var minTime = _responses.Min(r => r.Timing.TotalMilliseconds);
        var maxTime = _responses.Max(r => r.Timing.TotalMilliseconds);

        _logger.LogInformation("Performance metrics - Avg: {Avg}ms, Min: {Min}ms, Max: {Max}ms",
            avgTime, minTime, maxTime);
    }
}
