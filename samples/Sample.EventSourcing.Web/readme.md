# I-Synergy Framework

## Sample.EventSourcing.Web

Blazor Server front-end for the Event Sourcing API.

### Authentication

The Web obtains a JWT from the API's Development-only `POST /dev/token` endpoint for the
active tenant (the same `TenantContext` that drives tenant switching in the UI). It sends
that token to the API as an `Authorization: Bearer <token>` header on every request.

`DevTokenProvider` caches the token per tenant and refreshes it when the active tenant
changes or shortly before expiry, so the endpoint is not called on every request.

The previous spoofable `X-Tenant-Id` header is no longer used; the tenant is derived from
the token's `tenant` claim by the API.
