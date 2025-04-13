using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Logging.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Sample.Api.Data;
using Sample.Api.Entities;
using System.Reflection;

namespace Sample.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var mainAssembly = Assembly.GetAssembly(typeof(Program));
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly!);

        builder.Logging.AddOpenTelemetryLogging(
            infoService,
            builder.Configuration,
                logger =>
                {
                    //if (!string.IsNullOrEmpty(builder.Configuration[ApplicationInsightsConnectionString]))
                    //{
                    //    logger.AddAzureMonitorLogExporter(options =>
                    //    {
                    //        options.ConnectionString = builder.Configuration[ApplicationInsightsConnectionString];
                    //    });
                    //}
                });

        builder.Host.ConfigureOpenTelemetryLogging(infoService,
            tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();

                tracing.AddSqlClientInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.RecordException = true;
                });

                tracing.AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.SetDbStatementForStoredProcedure = true;
                });

                //if (!string.IsNullOrEmpty(builder.Configuration[ApplicationInsightsConnectionString]))
                //{
                //    // Azure Monitor tracing
                //    tracing.AddAzureMonitorTraceExporter(options =>
                //    {
                //        options.ConnectionString = builder.Configuration[ApplicationInsightsConnectionString];
                //    });
                //}
            },
            metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();

                // ASP.NET Core metrics
                metrics.AddMeter("Microsoft.AspNetCore.Hosting");
                metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");

                // EF Core metrics
                metrics.AddMeter("Microsoft.EntityFrameworkCore");

                //if (!string.IsNullOrEmpty(builder.Configuration[ApplicationInsightsConnectionString]))
                //{
                //    // Azure Monitor metrics
                //    metrics.AddAzureMonitorMetricExporter(options =>
                //    {
                //        options.ConnectionString = builder.Configuration[ApplicationInsightsConnectionString];
                //    });
                //}
            },
            logger =>
            {
            });

        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        // Add services to the container.
        builder.Services.AddControllerWithDefaultJsonSerialization();

        builder.Services.AddDbContext<TestDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.UseOpenApi();
        app.UseSwaggerUi();

        // Seed the database
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            for (int i = 1; i <= 1000; i++)
            {
                dbContext.TestEntities.Add(new TestEntity
                {
                    Name = $"Test Entity {i}",
                    Description = $"Description for entity {i}",
                    CreatedDate = DateTime.UtcNow
                });
            }
            dbContext.SaveChanges();
        }

        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        app.Run();
    }
}
