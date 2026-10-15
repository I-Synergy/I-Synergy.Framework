# Plan — Authenticate `Sample.EventSourcing.Web` to the API with a real JWT

Status: **PROPOSED — awaiting approval** (2026-10-09)
Scope: `samples/Sample.EventSourcing.Api` + `samples/Sample.EventSourcing.Web`. No framework (`src/`) change.

## Goal

Make the Blazor front-end (`Sample.EventSourcing.Web`, https://localhost:7201) able to call the
EventSourcing API successfully, instead of failing with `401 (Unauthorized)`, by having it obtain and
send a real bearer token whose `tenant`/`username` claims the API already trusts.

## Problem (evidence)

- The Web calls the API with **no token**. `OrdersApiClient.BuildRequest`
  (`samples/Sample.EventSourcing.Web/Services/OrdersApiClient.cs:102-107`) adds only an `X-Tenant-Id`
  header; a grep for `Bearer`/`Authorization`/`token` across the Web project finds nothing.
- The API's authorization fallback policy requires an authenticated user for every endpoint except
  `/health` (`samples/Sample.EventSourcing.Api/Program.cs:49-52`), so an unauthenticated call gets `401`
  and `EnsureSuccessStatusCode()` throws the message the user sees.
- Root cause of the drift: commit `bc6aa648` ("sample tenant/auth … hardening") rewrote the API
  `Program.cs` to JWT bearer auth but its diff **did not touch `Sample.EventSourcing.Web`** — the front-end
  was left on the old spoofable `X-Tenant-Id` protocol.
- The API resolves tenant/user purely from claims: `AddMultiTenancyIntegration` sets
  `TenantIdClaimType = "tenant"`, `UsernameClaimType = "username"`
  (`Program.cs:57-61`); `TenantResolutionMiddleware` reads those claims
  (`src/ISynergy.Framework.AspNetCore.MultiTenancy/Middleware/TenantResolutionMiddleware.cs:33-34,65-83`).
- The API already has the "expose an endpoint only in Development" pattern:
  `/api/archive/run` is gated on `env.IsDevelopment()`
  (`samples/Sample.EventSourcing.Api/Endpoints/ArchiveEndpoints.cs:15-35`).

## Design

A dev-only token issuer on the API (acting as the local identity provider) + a token provider in the Web
that fetches a bearer token and attaches it. The signing key stays only in the API.

### 1. API — Development-only token endpoint

New file `samples/Sample.EventSourcing.Api/Endpoints/DevTokenEndpoints.cs`:

- `POST /dev/token` — registered **only when `env.IsDevelopment()`** (same guard as `/api/archive/run`),
  marked `.AllowAnonymous()` (it must bypass the fallback policy) and with full `.Produces<T>(200)` /
  `.ProducesValidationProblem()` metadata.
- Request body `{ tenantId: Guid, username: string? }`; response `{ accessToken, tokenType, expiresIn }`.
- Mints an HS256 JWT signed with the configured `Jwt:SymmetricKeySecret`, with `Issuer`/`Audience` from
  config and claims `tenant` (= `tenantId`) and `username` (default `"demo-user"`), plus `sub` and an
  expiry (~1h). Uses `Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler` (already available
  transitively via `Microsoft.AspNetCore.Authentication.JwtBearer`) — **no new package**.
- A tiny `DevTokenIssuer` service (new file `samples/Sample.EventSourcing.Api/Services/DevTokenIssuer.cs`)
  holds issuer/audience/key read from `IConfiguration`, so the signing key is not re-read ad hoc in the
  endpoint.

`Program.cs`: register `DevTokenIssuer` and call `app.MapDevTokenEndpoints()` (which self-gates on
Development) alongside the existing endpoint mappings.

### 2. Web — fetch and attach a bearer token

- New file `samples/Sample.EventSourcing.Web/Services/DevTokenProvider.cs`: a typed `HttpClient` client that
  POSTs to `/dev/token` (base address `http+https://eventsourcing-api`, resolved by Aspire service
  discovery), caches the token per tenant id until near expiry, and returns the raw access token. Registered
  `AddHttpClient<DevTokenProvider>(...)` with the same base address as `OrdersApiClient`.
- `OrdersApiClient.BuildRequest` (`Services/OrdersApiClient.cs:102-107`): replace the `X-Tenant-Id` header
  with `Authorization: Bearer <token>` obtained from `DevTokenProvider` for the current
  `TenantContext.TenantId`. Because `DevTokenProvider` is async, `BuildRequest` becomes
  `BuildRequestAsync` (or the token is fetched before building the request); the methods already await
  I/O.

### 3. Docs

Update `samples/Sample.EventSourcing.Api/readme.md` and `samples/Sample.EventSourcing.Web/readme.md` to
describe the dev token flow and that the Web no longer relies on `X-Tenant-Id`.

## Files

| File | Change |
|---|---|
| `samples/Sample.EventSourcing.Api/Endpoints/DevTokenEndpoints.cs` | new — Development-only `POST /dev/token` |
| `samples/Sample.EventSourcing.Api/Services/DevTokenIssuer.cs` | new — issues HS256 JWTs from config |
| `samples/Sample.EventSourcing.Api/Program.cs` | register issuer + map dev endpoints |
| `samples/Sample.EventSourcing.Web/Services/DevTokenProvider.cs` | new — fetches/caches token |
| `samples/Sample.EventSourcing.Web/Services/OrdersApiClient.cs` | send `Authorization: Bearer`, drop `X-Tenant-Id` |
| `samples/Sample.EventSourcing.Web/Program.cs` | register `DevTokenProvider` HttpClient |
| `samples/…Api/readme.md`, `samples/…Web/readme.md` | document the flow |

## Security note / council check

- The token endpoint **does not exist in production** — it is gated on `env.IsDevelopment()`, exactly like
  the existing `/api/archive/run`. The shipped (non-Development) exposure surface is unchanged, so the
  change applies the documented JWT-bearer pattern and the existing dev-only-endpoint pattern rather than
  altering production security posture. **No council convened.**
- Within Development the endpoint can mint a token for any tenant by design — this is the local stand-in
  for an identity provider and is documented as such in the readme.
- The signing key remains only in the API; the Web never sees it.

## Verification plan

1. `dotnet build` the solution (with `MSBUILDDISABLENODEREUSE=1` and `-nodeReuse:false`).
2. Run the AppHost (needs Docker — not available in this environment, so this step is the user's to run):
   load https://localhost:7201, confirm the Orders page loads and "Place Order" succeeds instead of 401.
3. Confirm `dotnet user-secrets list` is unchanged / no secret is committed.
4. `git diff --check` clean.

## Out of scope

- The `Sample.EventSourcing.Web`→API call currently has no real login; a username is defaulted for the
  demo. Real user authentication (Keycloak) is a separate task.
- The stale `Sample.EventSourcing.AppHost` name in the connection-string error message
  (`Program.cs:65-68`) — cosmetic, not touched here.
