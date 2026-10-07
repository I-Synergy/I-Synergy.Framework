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
