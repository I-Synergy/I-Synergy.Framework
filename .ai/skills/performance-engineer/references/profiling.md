# Performance Profiling Tools

`dotnet-counters` / `dotnet-trace` / `dotnet-dump` / `dotnet-gcdump` setup, Application Insights wiring, and the request-duration middleware. Load this when a performance investigation starts.

## Performance Profiling Tools

### .NET Diagnostic Tools
```bash
# dotnet-counters (real-time metrics)
dotnet tool install --global dotnet-counters
dotnet-counters monitor --process-id <PID>

# dotnet-trace (performance tracing)
dotnet tool install --global dotnet-trace
dotnet-trace collect --process-id <PID>

# dotnet-dump (memory dumps)
dotnet tool install --global dotnet-dump
dotnet-dump collect --process-id <PID>

# dotnet-gcdump (GC analysis)
dotnet tool install --global dotnet-gcdump
dotnet-gcdump collect --process-id <PID>
```

### Application Insights
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Custom metrics
public class PerformanceMonitoringMiddleware(
    RequestDelegate next,
    TelemetryClient telemetryClient)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        await next(context);

        sw.Stop();

        telemetryClient.TrackMetric(
            "RequestDuration",
            sw.ElapsedMilliseconds,
            new Dictionary<string, string>
            {
                ["Endpoint"] = context.Request.Path,
                ["Method"] = context.Request.Method
            });
    }
}
```
