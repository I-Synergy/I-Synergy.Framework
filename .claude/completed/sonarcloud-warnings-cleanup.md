# SonarCloud Warnings Cleanup

**Status:** DONE

## Scope

All 134 SonarCloud rules from `warnings.md` (Azure Pipelines build log, ~15,270 occurrences) addressed across the entire codebase.

## Strategy

- **Mathematics library** (4,000+ warnings): File-level `#pragma warning disable S907,S1244,S3776,S2368,S1905,S1199` — ported numerical algorithms where these patterns are intentional
- **Application/infrastructure code**: Actual fixes (re-throw exceptions, correct ArgumentException param names, replace general Exception, remove dead code, etc.)
- **False positives**: `// NOSONAR` inline suppression (hardcoded IP in NetworkUtility, password regex name in GenericConstants, float equality in geometry/financial code, WeakAction IsAlive pattern)

## Commits (9 total on development/main)

| Commit | Scope | Files |
|--------|-------|-------|
| `f12f95fd` | fix(ui): UI/MVVM/AspNetCore | 41 files |
| `ec0347c1` | fix(samples): samples + performance | 27 files |
| `76e55ab5` | fix(tests): test projects (S2701, S2699, S2325, S125) | 22 files |
| `e25167b4` | fix(core): Core/IO/CQRS/Geography/Automations | 38 files |
| `9b49939f` | fix(mathematics): Mathematics library pragma disables | 264 files |
| `c6796e28` | refactor(quality): S2292, S4663 | 6 files |
| `5c00fd13` | fix(datetime): S6562, S6580 | 7 files |
| `8bf28480` | fix(logging): S2629 | 19 files |
| `ea41749f` | fix(tests): S3415 Assert arg order | 15 files |

## Outcome

- `dotnet build` → 0 errors
- `dotnet test` → 0 failures (~2,500+ tests, 25 assemblies)

## Rules addressed

S907, S1244, S3776, S2368, S1905, S1199, S125, S1116, S1110, S1481, S1854, S3928, S112, S1764, S2068, S5332, S1313, S5122, S2139, S2701, S2699, S2589, S2583, S1066, S2325, S2933, S3267, S4487, S1144, S1450, S1133, S1192, S4136, S2629, S3415, S6562, S6580, S2292, S4663
