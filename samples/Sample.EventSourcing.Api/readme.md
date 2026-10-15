# I-Synergy Framework

## Authentication and tenant isolation

This sample API **requires an authenticated caller** and resolves the tenant and user from the
authenticated JWT's claims (`tenant` and `username`) via
`ISynergy.Framework.AspNetCore.MultiTenancy` — it no longer trusts the `X-Tenant-Id`/`X-User-Name`
request headers, which a client could spoof. Every endpoint requires authentication by default
(authorization fallback policy); only `/health` is anonymous.

Configure the signing key before running (never commit it):

```bash
dotnet user-secrets set "Jwt:SymmetricKeySecret" "<a 32+ character key>"
# or: set the Jwt__SymmetricKeySecret environment variable
```

`Jwt:Issuer` / `Jwt:Audience` default to `i-synergy-sample` / `i-synergy-sample-api`. CORS allows
only the origins listed in `Cors:AllowedOrigins` (no wildcard).

## Development-only token endpoint

In **Development** the API exposes `POST /dev/token` — an anonymous endpoint that mints a JWT for a
given `{ tenantId, username }`:

```bash
curl -X POST http://localhost:5201/dev/token \
  -H "Content-Type: application/json" \
  -d '{ "tenantId": "11111111-1111-1111-1111-111111111111", "username": "demo-user" }'
# => { "accessToken": "<jwt>", "tokenType": "Bearer", "expiresIn": 3600 }
```

It signs with the same `Jwt:SymmetricKeySecret` the API validates against, so the returned token
authenticates on the protected endpoints. This is a local stand-in for an identity provider only:
it mints tokens for **any** tenant on purpose and is gated on `IsDevelopment()`, so production
builds do not expose it.
