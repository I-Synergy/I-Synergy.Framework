using Microsoft.AspNetCore.Mvc;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly ILogger _logger;
    public TelemetryController(ILogger<TelemetryController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IResult Get()
    {
        _logger.LogInformation("Information: TelemetryController.Get called");
        _logger.LogWarning("Warning: TelemetryController.Get called");
        LogError();
        LogCritical();
        _logger.LogTrace("Trace: TelemetryController.Get called");
        _logger.LogDebug("Debug: TelemetryController.Get called");

        return Results.Ok();
    }

    private void LogError() =>
        _logger.LogError(new NotImplementedException("Get"), "Error: TelemetryController.Get called");

    private void LogCritical() =>
        _logger.LogCritical(new InvalidOperationException("Get"), "Critical: TelemetryController.Get called");
}
