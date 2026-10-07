# I-Synergy Framework Sample for Maui

> **Known build issue (environmental).** Building this sample requires the MAUI workload, and
> under .NET SDK 10.0.401 the `Microsoft.Maui.Resizetizer` 10.0.110 `GenerateSplashAssets` task
> fails with `MSB4018` / `System.MissingMethodException: HarfBuzzSharp.Font.SetVariations(...)` — a
> Resizetizer ↔ SkiaSharp/HarfBuzzSharp toolchain mismatch, not a defect in the sample's or the
> framework's source. It breaks a solution-level `dotnet build`/`dotnet test`; build the other
> projects or the `src/` tree directly until the workload/tooling is aligned. See
> `docs/FRAMEWORK_REVIEW_2026-10-07.md` §2.