# Sample.EventSourcing.Web JWT auth

**Status:** DONE
Started: 2026-10-09

Plan: `.ai/plans/eventsourcing-web-jwt-auth.md`

## Steps
- [x] API: Development-only `/dev/token` issuer (DevTokenIssuer.cs, DevTokenEndpoints.cs, Program.cs, readme)
- [x] Web: DevTokenProvider + `Authorization: Bearer` in OrdersApiClient (Program.cs, readme)
- [x] Build solution
- [x] Review final diff (`git diff --check`)

## Notes
- Dev-only token endpoint mirrors the `/api/archive/run` Development gate; production exposure unchanged.
- Contract: `POST /dev/token` `{ tenantId, username }` -> `{ accessToken, tokenType, expiresIn }`; claims `tenant`, `username`, `sub`.
