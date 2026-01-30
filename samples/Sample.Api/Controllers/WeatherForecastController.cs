using Microsoft.AspNetCore.Mvc;
using Sample.Api.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ActivitySource _activitySource;
    private readonly Counter<long> _requestCounter;
    private readonly ILogger<WeatherForecastController> _logger;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        _activitySource = new ActivitySource("MyWebService");

        var meter = new Meter("MyWebService.Metrics");
        _requestCounter = meter.CreateCounter<long>("weather.requests", "Requests", "Number of weather requests");
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        // Create a span for this operation
        using var activity = _activitySource.StartActivity("GetWeatherForecasts");

        try
        {
            // Record a request metric
            _requestCounter.Add(1);

            // Add tags to the current activity
            activity?.SetTag("user.id", User.Identity?.Name ?? "anonymous");

            // Log the operation
            _logger.LogInformation("Weather forecast requested");

            // Perform the operation
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();

            // Mark the activity as successful
            activity?.SetStatus(ActivityStatusCode.Ok);

            return forecast;
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error retrieving weather forecast");

            // Mark the activity as failed with the error
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            throw;
        }
    }
}
