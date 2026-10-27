# Blazor component analyzer warnings (BL0012 / BL0013 / BL0016)

**Status:** DONE
Started: 2026-10-27
Finished: 2026-10-27

## Steps
- [x] Remove the unnecessary `StateHasChanged()` calls (`Counter`, `MainLayout.ToggleTheme`)
- [x] Read the authentication state of `NavigationMenuControl` through `CascadingAuthenticationState`
- [x] Guard the JS interop calls in `JsRuntimeExtensions`, `AntiforgeryHttpClientFactory` and `AntiForgeryTokenControl` (both Blazor projects)
- [x] Release rebuild of `Sample.Blazor` and its referenced `src/` projects with analyzers on
- [x] Run the `Sample.Blazor.UI.Tests` suite
- [x] Record the outcome (CHANGELOG + session handoff)

## Outcome
Nine Blazor analyzer warnings are gone; `Sample.Blazor` (and the `ISynergy.Framework.AspNetCore.Blazor` /
`ISynergy.Framework.UI.Blazor` projects it references) build with **0 warnings** in a forced Release
rebuild, and `Sample.Blazor.UI.Tests` is **36/36**.

## Notes
The warnings come from the .NET 11 `Microsoft.AspNetCore.Components.Analyzers` rules
(`UnnecessaryStateHasChangedCall` → BL0012, `AuthenticationStateProviderCachedWithoutSubscription` →
BL0013, `UnguardedJSInteropCall` → BL0016). Analyzers only run in `Release` (`Directory.Build.props`),
so a Debug build never shows them. A throwaway probe project confirmed which shapes satisfy BL0016:
a `try` needs a matching `catch` (a `finally` alone is not a guard), and `JSDisconnectedException`
does **not** derive from `JSException`, so both have to be caught.
