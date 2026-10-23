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

// ── Azure Blob Storage (Azurite emulator for local development) ───────────────
var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var blobs = storage.AddBlobs("blobs");

// ── Event Sourcing ────────────────────────────────────────────────────────────
var eventSourcingApi = builder.AddDotnetProject("eventsourcing-api", @"../Sample.EventSourcing.Api/Sample.EventSourcing.Api.csproj")
    .WithReference(eventsourcingDb)
    .WaitFor(eventsourcingDb)
    .WithReference(blobs)
    .WaitFor(blobs);

builder.AddDotnetProject("eventsourcing-web", @"../Sample.EventSourcing.Web/Sample.EventSourcing.Web.csproj")
    .WithReference(eventSourcingApi)
    .WaitFor(eventSourcingApi);

// ── Core API sample ───────────────────────────────────────────────────────────
// Demonstrates REST API patterns, OpenTelemetry, and InMemory EF Core.
builder.AddDotnetProject("api", @"../Sample.Api/Sample.Api.csproj");

// ── Automation sample ─────────────────────────────────────────────────────────
// Demonstrates the ISynergy.Framework.Automations library.
builder.AddDotnetProject("automation", @"../Sample.Automation/Sample.Automation.csproj");

// ── Blazor UI sample ──────────────────────────────────────────────────────────
// Demonstrates the ISynergy.Framework.AspNetCore.Blazor + Fluent UI integration.
builder.AddDotnetProject("blazor", @"../Sample.Blazor/Sample.Blazor.csproj");

// ── SignalR monitoring sample ─────────────────────────────────────────────────
// Demonstrates the ISynergy.Framework.AspNetCore.Monitoring SignalR integration.
builder.AddDotnetProject("monitor-signalr", @"../Sample.Monitor.SignalR/Sample.Monitor.SignalR.csproj");

var sample_app = builder.AddMauiProject("sample-app", @"../../Samples/Sample.Maui/Sample.Maui.csproj");

sample_app.AddWindowsDevice("sample-app-windows")
    .WithEnvironment("Environment", "Staging")
    .WithArgs("-p:WindowsPackageType=None")
    .WithExplicitStart();

await builder.Build().RunAsync();
