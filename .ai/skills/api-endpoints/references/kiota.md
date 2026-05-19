# Kiota Client Generation

## Naming Convention

**Single-app:** `{ApplicationName}.Clients.Api`
**Multi-app:** `{ApplicationName}.Clients.{AppName}` (swap `App` → `Clients`)

| Consuming App | → | Kiota Client |
|---------------|---|--------------|
| `App.Api` | → | `Clients.Api` |
| `App.Business` | → | `Clients.Business` |
| `App.Business.Web` | → | `Clients.Business.Web` |

## Client Project .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Http" />
    <PackageReference Include="Microsoft.Kiota.Bundle" />
  </ItemGroup>
</Project>
```

`Microsoft.Kiota.Bundle` is a metapackage covering `Abstractions`, `HttpClientLibrary`, and all serializers (JSON, Text, Form, Multipart). Always use the bundle, not individual packages.

## Project Structure

```
{ApplicationName}.Clients.Api/
├── {ApplicationName}.Clients.Api.csproj
├── ApiClient.cs               # Kiota-generated entry point
├── Api/                        # Kiota-generated request builders
├── Models/                     # Kiota-generated models
├── Extensions/                 # DI registration (hand-written)
│   └── ServiceCollectionExtensions.cs
└── kiota-lock.json             # Generation lock file
```

All source files in the client project are committed: `Api/`, `Models/`, `ApiClient.cs`, `kiota-lock.json`, `Extensions/`, `.csproj`.

## Kiota Tool Version

Pin the Kiota CLI version in `.config/dotnet-tools.json`:

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "microsoft.openapi.kiota": {
      "version": "1.31.1",
      "commands": ["kiota"],
      "rollForward": false
    }
  }
}
```

Setup commands:
```bash
dotnet new tool-manifest          # Skip if .config/dotnet-tools.json exists
dotnet tool install Microsoft.OpenApi.Kiota
dotnet tool restore               # On fresh checkout
```

## DI Registration

```csharp
// File: {ApplicationName}.Clients.Api/Extensions/ServiceCollectionExtensions.cs

namespace {ApplicationName}.Clients.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticationProvider, AnonymousAuthenticationProvider>();

        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        });

        services.AddScoped<ApiClient>(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ApiClient");
            var authProvider = sp.GetRequiredService<IAuthenticationProvider>();
            var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
            return new ApiClient(adapter);
        });

        return services;
    }
}
```

For authenticated APIs, replace `AnonymousAuthenticationProvider` with:
```csharp
// Bearer token
services.AddSingleton<IAuthenticationProvider>(sp =>
    new BaseBearerTokenAuthenticationProvider(new TokenProvider(token)));

// API key
services.AddSingleton<IAuthenticationProvider>(
    new ApiKeyAuthenticationProvider(apiKey, "X-Api-Key", KeyLocation.Header));
```

## Using the Generated Client

```csharp
public sealed class GetEntityListQueryHandler(
    ApiClient client
) : IQueryHandler<GetEntityListQuery, GetEntityListResponse>
{
    public async Task<GetEntityListResponse> HandleAsync(
        GetEntityListQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await client.Api.V1.Entities.GetAsync(config =>
        {
            config.QueryParameters.Page = query.PageIndex;
            config.QueryParameters.PageSize = query.PageSize;
        }, cancellationToken);

        return new GetEntityListResponse(
            result?.Select(e => /* map to model */).ToList() ?? []);
    }
}
```

## Key Rules

- Kiota client per application, not per API project
- `Microsoft.Kiota.Bundle` — single metapackage, not individual packages
- `dotnet-tools.json` — pinned version with `rollForward: false`
- MSBuild `OpenAPI` target in the API project regenerates the client on every build
- Client source files (`Api/`, `Models/`, `ApiClient.cs`, `kiota-lock.json`) are committed
- Use Aspire service discovery URL (`https+http://api`) for the client base address
- Never use raw `HttpClient` to call an internal API
