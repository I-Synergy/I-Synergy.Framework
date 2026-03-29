var builder = DistributedApplication.CreateBuilder(args);

// ── Databases ─────────────────────────────────────────────────────────────────
// Session lifetime keeps containers ephemeral: they are removed when the AppHost
// stops so no Docker/Podman images or volumes accumulate between runs.

// Single PostgreSQL instance shared by all samples that need a database.
// pgAdmin is included for browser-based database management (Development only).
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Session)
    .WithPgAdmin(c => c.WithLifetime(ContainerLifetime.Session));

var eventsourcingDb = postgres.AddDatabase("eventsourcing");
var synchronizationDb = postgres.AddDatabase("synchronization-db");

// ── Event Sourcing ────────────────────────────────────────────────────────────
var eventSourcingApi = builder.AddProject<Projects.Sample_EventSourcing_Api>("eventsourcing-api")
    .WithReference(eventsourcingDb)
    .WaitFor(eventsourcingDb);

builder.AddProject<Projects.Sample_EventSourcing_Web>("eventsourcing-web")
    .WithReference(eventSourcingApi)
    .WaitFor(eventSourcingApi);

// ── Core API sample ───────────────────────────────────────────────────────────
// Demonstrates REST API patterns, OpenTelemetry, and InMemory EF Core.
builder.AddProject<Projects.Sample_Api>("api");

// ── Automation sample ─────────────────────────────────────────────────────────
// Demonstrates the ISynergy.Framework.Automations library.
builder.AddProject<Projects.Sample_Automation>("automation");

// ── Blazor UI sample ──────────────────────────────────────────────────────────
// Demonstrates the ISynergy.Framework.AspNetCore.Blazor + Fluent UI integration.
builder.AddProject<Projects.Sample_Blazor>("blazor");

// ── SignalR monitoring sample ─────────────────────────────────────────────────
// Demonstrates the ISynergy.Framework.AspNetCore.Monitoring SignalR integration.
builder.AddProject<Projects.Sample_Monitor_SignalR>("monitor-signalr");

// ── Synchronization sample ────────────────────────────────────────────────────
// Demonstrates the ISynergy.Framework.Synchronization library with PostgreSQL.
builder.AddProject<Projects.Sample_Synchronization>("synchronization")
    .WithReference(synchronizationDb)
    .WaitFor(synchronizationDb);

// ── Token Service sample ──────────────────────────────────────────────────────
// Demonstrates JWT and WOPI token generation via ISynergy.Framework.AspNetCore.Authentication.
builder.AddProject<Projects.Sample_TokenService>("token-service");

builder.Build().Run();
