using ISynergy.Framework.Core.Serializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

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

                var response = new
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Info = report.Entries.Select(e => new
                    {
                        Key = e.Key,
                        Description = e.Value.Description,
                        Status = e.Value.Status.ToString(),
                        Duration = e.Value.Duration,
                        Data = e.Value.Data
                    })
                };

                await JsonSerializer.SerializeAsync(
                    context.Response.Body,
                    response,
                    DefaultJsonSerializers.Web);
            }
        });
    }
}