# ISynergy.Framework.UI.SourceGenerator

Roslyn incremental source generator for I-Synergy UI AOT-compatible view, window, and viewmodel type registration.

This package is automatically bundled with `ISynergy.Framework.UI` and provides:

- `AddUITypes()` — compile-time registration of all `IView`, `IWindow`, and `IViewModel` implementors (replaces reflection-based `RegisterAssemblies()`)
- `ViewModelViewMap.Map` — AOT-safe compile-time dictionary mapping ViewModel names to view types
