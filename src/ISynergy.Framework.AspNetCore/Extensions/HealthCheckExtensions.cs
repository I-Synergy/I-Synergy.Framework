using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.AspNetCore.Extensions;

/// <summary>
/// Extensions for configuring health checks with OpenTelemetry integration.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Maps health check endpoints with OpenTelemetry integration.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="path">The path to map health checks to.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder MapTelemetryHealthChecks(
        this IApplicationBuilder app,
        string path = "/health")
    {
        return app.UseHealthChecks(path, new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var response = new HealthCheckResponse(
                    Status: report.Status.ToString(),
                    Duration: report.TotalDuration,
                    Info: report.Entries.Select(e => new HealthCheckEntryResponse(
                        Key: e.Key,
                        Description: e.Value.Description,
                        Status: e.Value.Status.ToString(),
                        Duration: e.Value.Duration,
                        Data: e.Value.Data
                    )).ToList()
                );

                await JsonSerializer.SerializeAsync(
                    context.Response.Body,
                    response,
                    HealthCheckJsonContext.Default.HealthCheckResponse);
            }
        });
    }
}

/// <summary>
/// Represents the health check response payload.
/// </summary>
/// <param name="Status">The overall health status.</param>
/// <param name="Duration">The total duration of all health checks.</param>
/// <param name="Info">The individual health check entries.</param>
internal sealed record HealthCheckResponse(
    string Status,
    TimeSpan Duration,
    IEnumerable<HealthCheckEntryResponse> Info);

/// <summary>
/// Represents a single health check entry in the response payload.
/// </summary>
/// <param name="Key">The health check name.</param>
/// <param name="Description">An optional description of the health check.</param>
/// <param name="Status">The status of this individual health check.</param>
/// <param name="Duration">The duration of this individual health check.</param>
/// <param name="Data">Additional key-value data reported by the health check.</param>
internal sealed record HealthCheckEntryResponse(
    string Key,
    string? Description,
    string Status,
    TimeSpan Duration,
    IReadOnlyDictionary<string, object> Data);

/// <summary>
/// Source-generated JSON serializer context for health check response types.
/// </summary>
[JsonSerializable(typeof(HealthCheckResponse))]
[JsonSerializable(typeof(HealthCheckEntryResponse))]
[JsonSerializable(typeof(IEnumerable<HealthCheckEntryResponse>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, object>))]
internal sealed partial class HealthCheckJsonContext : JsonSerializerContext
{
}
