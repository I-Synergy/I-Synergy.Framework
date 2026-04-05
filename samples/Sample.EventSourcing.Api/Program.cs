using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.EventSourcing.EntityFramework;
using ISynergy.Framework.EventSourcing.EntityFramework.Extensions;
using ISynergy.Framework.EventSourcing.Storage.Azure.Extensions;
using ISynergy.Framework.EventSourcing.Storage.Extensions;
using Microsoft.EntityFrameworkCore;
using Sample.EventSourcing.Api.Domain;
using Sample.EventSourcing.Api.Endpoints;
using Sample.EventSourcing.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Tenant service ───────────────────────────────────────────────────────────
// DemoTenantService resolves tenant from X-Tenant-Id / X-User-Name request headers.
// In production, replace with ITenantService from ISynergy.Framework.AspNetCore.MultiTenancy
// which reads tenant from authenticated JWT claims.
builder.Services.AddSingleton<ITenantService, DemoTenantService>();

// ── Event Sourcing ───────────────────────────────────────────────────────────
// Connection string is injected by Aspire: ConnectionStrings__eventsourcing
var connectionString = builder.Configuration.GetConnectionString("eventsourcing")
    ?? throw new InvalidOperationException(
        "Connection string 'eventsourcing' not found. " +
        "Run this service through the Aspire AppHost (Sample.EventSourcing.AppHost).");

builder.Services
    .AddEventSourcingEntityFramework(o => o.UseNpgsql(connectionString))
    .AddAggregateRepository<Order, Guid>();

// ── Archive services (cold-tier blob storage) ─────────────────────────────────
// BlobServiceClient is injected by Aspire from the "blobs" resource.
builder.AddAzureBlobServiceClient("blobs");
builder.Services
    .AddAzureEventArchiveStorage()
    .AddEventArchiving(builder.Configuration);

// ── Service discovery ─────────────────────────────────────────────────────────
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(h => h.AddServiceDiscovery());

// ── Health checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

// ── OpenAPI / Swagger ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(c => c.Title = "Event Sourcing Sample API");

// ── CORS (for Blazor frontend) ────────────────────────────────────────────────
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Tenant resolution middleware ──────────────────────────────────────────────
// Reads X-Tenant-Id and X-User-Name headers and sets the ambient tenant context.
// The Blazor web app sends X-Tenant-Id to scope all operations to that tenant.
app.Use(async (ctx, next) =>
{
    var tenantService = ctx.RequestServices.GetRequiredService<ITenantService>();

    if (ctx.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader)
        && Guid.TryParse(tenantHeader, out var tenantId))
    {
        var userName = ctx.Request.Headers.TryGetValue("X-User-Name", out var userHeader)
            ? userHeader.ToString()
            : "anonymous";

        tenantService.SetTenant(tenantId, userName);
    }

    await next();
});

// ── DB schema ─────────────────────────────────────────────────────────────────
// EnsureCreated creates tables on first run. Replace with EF migrations for production.
using (var scope = app.Services.CreateScope())
{
    var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
    tenantService.SetTenant(Guid.Parse("00000000-0000-0000-0000-000000000001")); // system

    var context = scope.ServiceProvider.GetRequiredService<EventSourcingDbContext>();
    context.Database.EnsureCreated();
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.MapHealthChecks("/health");

// ── Endpoints ─────────────────────────────────────────────────────────────────
app.MapOrderEndpoints();
app.MapArchiveEndpoints();

app.Run();
