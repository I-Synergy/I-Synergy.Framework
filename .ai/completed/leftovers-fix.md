# Fixing the review leftovers

**Status:** DONE
**Started:** 2026-10-07
**Client:** Reasonix Code
**Report:** `docs/FRAMEWORK_REVIEW_2026-10-07.md`

## Scope (user, via ask)
The three remaining items: the two fragile seams and the `MessageBox*` enum naming (renamed to plural,
per the user's choice of the breaking option).

## Steps
- [x] L1: Replace the `dynamic` dispatch fallback in `QueryDispatcher`
- [x] L2: Harden `EventArchiver.CreateAggregate` type resolution
- [x] L3: Rename the `MessageBox*` enums to the plural form
- [x] L4: Verify
- [x] (bonus) Fix the 9 `CS0618` obsolete `FileResult.File` access warnings introduced by the earlier High-issue `FileResult` deprecation

## Result
Full solution build: **0 non-`Sample.Maui` errors, 0 CS0618 warnings**. Test sweep: green (the one
Playwright dialog E2E failure did not reproduce on re-run: 36/36). `git diff --check`: clean.

## Details
- **L1 `QueryDispatcher`** — the reflection fallback no longer uses `dynamic`. It constructs the handler
  interface type, reflects `HandleAsync` **once** per handler shape (a `ConcurrentDictionary` cache), and
  invokes it via `MethodInfo.Invoke`; a synchronous handler throw wrapped in `TargetInvocationException`
  is unwrapped so the handler's own exception surfaces. The `RequiresDynamicCode`/`RequiresUnreferencedCode`
  annotations remain.
- **L2 `EventArchiver.CreateAggregate`** — the resolved aggregate type is cached (`ConcurrentDictionary`,
  including a not-found `null`) so the loaded-assembly scan runs once per stream type name; matches are
  ordered by full name for a deterministic pick rather than depending on assembly load order, with
  ambiguity surfaced via `Trace`; per-assembly enumeration was extracted into `GetLoadableTypes`
  (tolerating `ReflectionTypeLoadException`).
- **L3 `MessageBox*` enums** — `MessageBoxImage`→`MessageBoxImages` and `MessageBoxResult`→`MessageBoxResults`
  across the three defining projects (`Mvvm`, `UI.Blazor`, `AspNetCore.Blazor`) and every reference
  (`IDialogService`, `MessageBoxRequest`, the platform `DialogService` implementations, the WinUI/Maui
  `ImageBrowser` controls, and the samples); the six enum files were renamed to match. WPF's own
  `System.Windows.MessageBoxImage`/`MessageBoxResult` were left untouched (the WPF dialog service's local
  alias was repointed and the redundant alias removed).
- **CS0618 cleanup** — nine `FileResult.File` reads were switched to `await GetFileAsync()` in
  `UI.WPF`/`UI.Maui`/`UI.WinUI` `FileService`/`CameraService`/`ImageBrowser`.

## Notes
- **L3 is a breaking public rename** (the user chose this explicitly). Consumers referencing
  `…Enumerations.MessageBoxImage`/`MessageBoxResult` must switch to the plural names.
- Build rule honoured: node reuse disabled. No committing was performed.
