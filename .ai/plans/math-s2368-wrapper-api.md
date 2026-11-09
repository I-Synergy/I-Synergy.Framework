# Mathematics S2368 — wrapper-type public API (design + migration plan)

Status: PROPOSED (not started)
Relates to: `.ai/plans/build-warnings-and-sonar.md`

## Why this is a separate project

`S2368` reports 582 public declarations (152 method groups; 43 overload sets differ only by
array rank/shape). Sonar's compliant fixes are "make it non-public" or "do not expose raw
multidimensional/jagged arrays".

A wrapper type cannot be a drop-in change:

- Extension methods do **not** bind through user-defined implicit conversions
  (`double[,]` does not satisfy a `this Matrix<double>` receiver — CS1929).
- Generic inference does not use user-defined conversions (`T` cannot be inferred
  from `T[,]` to `Matrix<T>` — CS0411).
- 43 overload sets differ only by shape, so `System.Array` substitution would produce
  duplicate signatures.

Consequence: every call site must construct the wrapper explicitly. `S2368` is therefore a
public API redesign plus a repository-wide client migration, not a lint fix.

## Proposed types (Mathematics namespace)

| Type | Wraps | Notes |
|---|---|---|
| `Vector<T>` | `T[]` | explicit ctor, `Span<T>` accessor, `Length` |
| `Matrix<T>` | `T[,]` | `Rows`, `Columns`, indexer, `AsSpan()` only where safe |
| `Jagged<T>` | `T[][]` | `Rows`, row accessor |
| `Tensor3<T>` | `T[,,]` | `GetLength(i)` |

All wrappers are `readonly struct` where possible, hold the array without copying, and expose an
`internal T[,] Array { get; }` (or a public `ToArray()` for consumers). Non-generic aliases
(`Matrix`, `Jagged`, `Vector`) are not needed if callers use `new Matrix<double>(array)`.

## Migration stages (each leaves the build and tests green)

1. **Introduce the wrappers** (`src/ISynergy.Framework.Mathematics/Abstractions/`), with unit
   tests for construction, accessors and no-copy semantics. No API changes yet.
2. **Dual API**: add wrapper-typed overloads next to the existing array ones for one vertical
   slice (e.g. `Matrix.Common`: `Rows`, `Columns`, `Transpose`, `Sum`, `Mean`). Mark the array
   overloads `[Obsolete]` (S1133 would then need their removal later). Migrate the internal
   callers and tests for that slice.
3. Repeat stage 2 by file group (Matrices, Statistics, Decompositions, Optimization,
   Integration, IO, Random, Environments). 152 method groups; 43 shape-colliding sets are the
   tricky ones because both shapes become distinct wrappers, which is exactly what S2368 wants.
4. **Remove the array overloads** and the now-unused `S2368` findings. The project's `NoWarn`
   and pragmas stay empty.
5. Migrate `src/ISynergy.Framework.Core`, `Geography`, `samples/*` and `tests/*` (the compiler
   drives the list). `InternalsVisibleTo` is not an option: the public API is the product.

## Acceptance criteria

- `dotnet build` with the Sonar analyzer injected: zero `S2368` and zero suppressions.
- All test projects green; Mathematics suite unchanged (954 tests).
- Package consumers migrate by wrapping their arrays once, e.g.
  `new Matrix<double>(array).Mean()` instead of `array.Mean()`.

## Recommendation

Do this as a dedicated branch/PR, one stage at a time, with the design agreed first. It touches
the public contract of a published NuGet package, so semantic-versioning and release notes must
be decided with the API design.
