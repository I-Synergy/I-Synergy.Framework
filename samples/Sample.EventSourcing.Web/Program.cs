using Microsoft.Extensions.ServiceDiscovery;
using Sample.EventSourcing.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Blazor Server ─────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Service discovery (resolves "api" to Aspire-injected URL) ─────────────────
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(h => h.AddServiceDiscovery());

// ── Typed HTTP client for the Orders API ─────────────────────────────────────
// "http+https://eventsourcing-api" is resolved by Aspire service discovery to the API's actual URL.
builder.Services.AddHttpClient<OrdersApiClient>(client =>
    client.BaseAddress = new Uri("http+https://eventsourcing-api"));

// ── Health checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapHealthChecks("/health");

app.MapRazorComponents<Sample.EventSourcing.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
