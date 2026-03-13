# ISynergy.Framework.CQRS.SourceGenerator

Roslyn incremental source generator for I-Synergy CQRS AOT-compatible handler registration and query dispatch table generation.

This package is automatically bundled with `ISynergy.Framework.CQRS` and provides:

- `AddCQRSHandlers()` — compile-time handler registration (replaces reflection-based `AddHandlers()`)
- `AddQueryDispatchTable()` — AOT-safe query dispatch table population
