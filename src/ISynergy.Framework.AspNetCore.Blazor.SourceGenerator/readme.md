# ISynergy.Framework.AspNetCore.Blazor.SourceGenerator

Roslyn incremental source generator for I-Synergy Blazor AOT-compatible view, window, and viewmodel type registration.

This package is automatically bundled with `ISynergy.Framework.AspNetCore.Blazor` and provides:

- `AddBlazorRegistrations()` — compile-time registration of all `IView`, `IWindow`, and `IViewModel` implementors (replaces reflection-based `RegisterAssemblies()`)
