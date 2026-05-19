---
name: api-endpoints
description: API endpoint creation, OpenAPI configuration, Kiota client generation, and API security hardening. Use when creating Minimal API endpoints, setting up API projects, generating Kiota clients, configuring OpenAPI/Swagger, or hardening API security (rate limiting, CORS, validation, HTTPS).
---

# API Endpoints Skill

Expert for creating and configuring .NET Minimal API endpoints with full OpenAPI, Kiota client generation, and security hardening.

## When to Use

- Creating a new API endpoint (CRUD, search, download, etc.)
- Setting up a new API project from scratch
- Generating or regenerating a Kiota client from an OpenAPI spec
- Adding OpenAPI configuration (transformers, document generation)
- Hardening API security (validation, rate limiting, CORS, HTTPS, auth)
- Auditing existing endpoints for compliance with patterns

## Workflows

### New API Endpoint

1. Read `references/endpoints.md` for the complete pattern
2. Create the endpoint class with `MapGroup`, version set, and auth
3. Use named static methods (never inline lambdas) with `[FromServices]`, `[FromBody]`, `[FromRoute]`, `[FromQuery]`
4. Return `Results<T1, T2>` with `TypedResults.*` factory methods
5. Declare all produces metadata: `.Produces<T>(statusCode)` for success + `.Produces(statusCode)` for errors
6. Add `.WithValidation<T>()` on POST/PUT routes + `.ProducesValidationProblem()`
7. Register the endpoint in Program.cs via `.Map{Entity}Endpoints("Bearer", "ApiKey")`

### New API Project

1. Configure `.csproj` with OpenAPI document generation properties and Kiota MSBuild target (see `references/openapi.md`)
2. Add `Microsoft.Extensions.ApiDescription.Server` package reference
3. Set up `Program.cs` with combined document + schema transformers in a single `AddOpenApi()` call (see `references/openapi.md`)
4. Configure security middleware in order: `UseHttpsRedirection` → `UseCors` → `UseRateLimiter` → `UseAuthentication` → `UseAuthorization` (see `references/security.md`)
5. Pin Kiota version in `.config/dotnet-tools.json` with `rollForward: false`

### New Kiota Client

1. Follow the naming convention: `{ApplicationName}.Clients.Api` (single-app) or `{ApplicationName}.Clients.{AppName}` (multi-app)
2. Create the client `.csproj` with `Microsoft.Kiota.Bundle` and `Microsoft.Extensions.Http` (see `references/kiota.md`)
3. Create the `Extensions/ServiceCollectionExtensions.cs` for DI registration with `IAuthenticationProvider` + `HttpClientRequestAdapter` (see `references/kiota.md`)
4. Commit all client source files: `Api/`, `Models/`, `ApiClient.cs`, `kiota-lock.json`
5. The API project's MSBuild `OpenAPI` target regenerates the client on every build

### Security Hardening

1. **Validation**: Data Annotations on request models + `.WithValidation<T>()` filter on every POST/PUT route (see `references/security.md`)
2. **Rate limiting**: `AddRateLimiter` with fixed window (1000 req/min) + `UseRateLimiter` middleware
3. **CORS**: Explicit origins (never `AllowAnyOrigin` in production), call `UseCors` before `UseAuthorization`
4. **HTTPS**: `UseHttpsRedirection` for all environments
5. **Auth**: Group-level `RequireAuthorization` with `RequireAuthenticatedUser()` and authentication schemes

## Key Rules (non-negotiable)

- Named static methods — never inline lambdas
- `TypedResults.*` — not `Results.*`
- Explicit `.Produces<T>()` on every route (success + errors)
- `.WithValidation<T>()` + `.ProducesValidationProblem()` on POST/PUT
- `[FromServices]`, `[FromBody]`, `[FromRoute]`, `[FromQuery]` on all parameters
- `CancellationToken cancellationToken = default` on all async methods
- Individual command parameters — never pass model objects to commands
- Document + schema transformers in a single `AddOpenApi()` call — no document name parameter
- `Microsoft.Kiota.Bundle` — not individual Kiota packages
- API `.csproj` OpenAPI target runs on every build, not just Debug
- Rate limiting, CORS, HTTPS middleware must be present
- Middleware order: HTTPS → CORS → RateLimiter → Auth

## References

- `references/endpoints.md` — Complete endpoint code patterns, route metadata, TypedResults
- `references/openapi.md` — .csproj configuration, document/schema transformers, build-time generation
- `references/kiota.md` — Client naming, project setup, DI registration, MSBuild target
- `references/security.md` — Validation filter, rate limiting, CORS, HTTPS, auth patterns

## Templates

- `.ai/reference/templates/endpoint.cs.txt` — Endpoint class template
- `.ai/reference/templates/command-handler.cs.txt` — Command handler template
- `.ai/reference/templates/query-handler.cs.txt` — Query handler template

## Patterns

- `.ai/patterns/api-patterns.md` — Full API endpoint patterns reference
- `.ai/reference/critical-rules.md` — Non-negotiable critical rules (rules 16-21 for API)
