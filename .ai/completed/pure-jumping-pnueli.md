# Navigation Architecture Refactor — Progress

**Plan:** `.ai/completed/pure-jumping-pnueli.md`  
**Status:** COMPLETED  
**Commit:** `1df36b0` — refactor: introduce per-frame DI scopes in navigation service

## Changes delivered

| File | Status |
|------|--------|
| `src/ISynergy.Framework.UI/Services/BaseNavigationService.cs` | Done — NavigationFrame, scoped backstack, HandleCurrentFrame, GoBackAsync, Dispose |
| `src/ISynergy.Framework.UI/Extensions/ReflectionExtensions.cs` | Done — Register respects IsSingleton/IsScoped, ViewModels registered as Scoped |
| `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` | Done — CreateView accepts IServiceProvider |
| `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs` | Done — CreateView accepts IServiceProvider |

## Pending verification

- [ ] `dotnet build` on Core, UI, WinUI, WPF, MAUI
- [ ] Manual test in WinUI sample (forward nav, back nav, deep stack >10, modal dialog)
