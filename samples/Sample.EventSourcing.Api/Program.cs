using System.Text;
using ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.EventSourcing.EntityFramework;
using ISynergy.Framework.EventSourcing.EntityFramework.Extensions;
using ISynergy.Framework.EventSourcing.Storage.Azure.Extensions;
using ISynergy.Framework.EventSourcing.Storage.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sample.EventSourcing.Api.Domain;
using Sample.EventSourcing.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// ── Authentication / authorization ───────────────────────────────────────────
// The API trusts ONLY authenticated principals. Tenant and user identity are resolved from the
// authenticated JWT's claims (see AddMultiTenancyIntegration below) — never from a request header.
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:SymmetricKeySecret"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Jwt:SymmetricKeySecret is not configured. Source it from a secret store before running this " +
        "sample — for example 'dotnet user-secrets set \"Jwt:SymmetricKeySecret\" \"<32+ char key>\"' " +
        "or the Jwt__SymmetricKeySecret environment variable.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Every endpoint requires an authenticated caller unless it explicitly opts out (e.g. /health).
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

// ── Tenant resolution ────────────────────────────────────────────────────────
// Resolves the tenant and user from the authenticated principal's claims and stores them in the
// ambient TenantContext. Fails closed when an authenticated principal carries no tenant claim.
builder.Services.AddMultiTenancyIntegration(options =>
{
    options.TenantIdClaimType = "tenant";
    options.UsernameClaimType = "username";
});

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

// ── CORS (for the Blazor frontend) ────────────────────────────────────────────
// Only the explicitly configured origins may call this API — no wildcard origin.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

// Resolves the tenant from the authenticated principal's claims (must run after UseAuthentication).
app.UseMultiTenancyMiddleware();

app.MapHealthChecks("/health").AllowAnonymous();

// ── Endpoints ─────────────────────────────────────────────────────────────────
app.MapOrderEndpoints();
app.MapArchiveEndpoints();

await app.RunAsync();
