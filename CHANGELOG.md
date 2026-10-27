# Changelog

All notable changes to the I-Synergy Framework will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

#### System theme across every head
- **ISynergy.Framework.UI.WPF / UI.WinUI / UI.Maui / UI.Blazor**: `IThemeService` now treats `Themes.Default` as **System**. `Theme` and `IsLightThemeEnabled` report the resolved Light/Dark theme, `SetTheme(Themes.Default)` persists the preference and applies each platform's follow-the-OS mode, and an OS theme change while the app runs re-applies the palette and raises `ThemeChanged`.
- **WPF**: reads `HKCU\…\Themes\Personalize\AppsUseLightTheme` and watches `Microsoft.Win32.SystemEvents.UserPreferenceChanged`.
- **WinUI**: maps System to `ElementTheme.Default` and watches the window root's `ActualThemeChanged`.
- **MAUI**: maps System to `AppTheme.Unspecified` and watches `Application.RequestedThemeChanged`.
- **Blazor**: the interop module resolves System with `matchMedia('(prefers-color-scheme: dark)')`, watches the media query, and calls back through a `[JSInvokable]` method so `ThemeChanged` fires.
- **Samples**: `LocalSettings.Theme` defaults to `Themes.Default`, so a fresh install follows the OS; WPF, WinUI, MAUI and Blazor settings surfaces offer **System** in their theme pickers. `Themes.Default`'s localized description is now "System".
- **ISynergy.Framework.UI.WPF**: new `EnumToDescriptionConverter` (parity with the WinUI/MAUI converters).

#### .NET 11 (RC) Multi-Targeting
- **All 48 `src/` libraries** now ship `net10.0` (LTS) **and** `net11.0` (preview) assets in the same NuGet package; the WPF, WinUI and MAUI packages twin their platform variants (`net11.0-windows*`, `net11.0-android` / `-ios` / `-maccatalyst`), so WPF ships six target frameworks and MAUI eight
- **Directory.Packages.props**: framework-aligned packages are split into per-TFM groups — `10.0.13` for `net10.0`, `11.0.0-rc.2.26504.105` for `net11.0` (plus `Npgsql.EntityFrameworkCore.PostgreSQL` `11.0.0-rc.2` and `Microsoft.Maui.Controls` `11.0.0-rc.2.26505.5`); third-party packages remain unconditional
- **global.json**: pinned to the .NET 11 RC SDK with `allowPrerelease: true`
- **Toolchain**: `net11.0` targets are only appended when `$(BundledNETCoreAppPackageVersion)` is ≥ 11, so machines and pipelines still on SDK 10 keep building the `net10.0` assets
- **tests/ISynergy.Framework.UI.WinUI.Tests**: `WindowsSdkPackageVersion` raised to `10.0.26100.87`; the projection conflict that SDK 10 reported as a warning is an error (`NETSDK1148`) on SDK 11

#### MAUI Clipboard — Platform Image Support
- **ISynergy.Framework.UI.Maui**: dropped the `SkiaSharp` dependency; `ClipboardService.GetImageFromClipboardAsync()` now uses each platform's native imaging APIs — WinRT `BitmapDecoder`/`BitmapEncoder` on Windows (BMP → PNG), `ClipboardManager` + `BitmapFactory` on Android (content URI), `UIPasteboard` on iOS and MacCatalyst
- Image clipboard support now exists on Android, iOS and MacCatalyst, which previously always returned `null`
- Also clears the `MSB4018`/`MissingMethodException` in `Microsoft.Maui.Resizetizer` 11.0 RC (`SKTextBlobBuilder.AddPositionedRun`) that broke MAUI 11 builds, caused by the project's `SkiaSharp` 4.153.1 being loaded instead of the version the Resizetizer task ships with

#### AOT Compatibility — New Projects
- **ISynergy.Framework.CQRS.SourceGenerator**: New Roslyn incremental source generator that emits `AddCqrsHandlers()` and `AddQueryDispatchTable()` extension methods at compile time, enabling fully AOT-safe CQRS handler registration without runtime reflection
- **ISynergy.Framework.UI.SourceGenerator**: New Roslyn incremental source generator that emits `AddUITypes()` and a `ViewModelViewMap` at compile time, replacing `AppDomain.GetAssemblies()` scanning for view/view-model discovery
- **ISynergy.Framework.AspNetCore.Blazor.SourceGenerator**: New Roslyn incremental source generator that emits `AddBlazorRegistrations()` for AOT-safe Blazor component and service registration

#### Live Localization and Theming — New APIs
- **ISynergy.Framework.Globalization**: `LocalizedStrings` — singleton service providing indexed access to localized strings with `INotifyPropertyChanged` and `Changed` event support for automatic UI refresh on culture change
- **ISynergy.Framework.Globalization**: `CultureInfoExtensions.ApplyCulture()` — hook that calls `LocalizedStrings.Default.Refresh()` automatically when culture changes, enabling live language switching
- **ISynergy.Framework.UI.Maui**: `LocalizedStringsExtension` — XAML markup extension for trim-friendly, compiled binding to `LocalizedStrings.Default` indexer
- **ISynergy.Framework.Mvvm**: `IThemeService` extended with: `Theme` property, `AccentColor` property, `ThemeChanged` event, `SetTheme(Themes)` method, `SetAccentColor(string)` method — enables live theme switching with automatic persistence
- **ISynergy.Framework.UI.Blazor**: `LocalizedComponentBase` — base component class that automatically re-renders when language changes via `LocalizedStrings.Changed` event
- **ISynergy.Framework.UI.Blazor**: `ThemeService` (Blazor-specific) — applies theme and accent via JavaScript interop (`theme_service_interop.js`) with `SetTheme` and `SetAccentColor` support

#### AOT Compatibility — New APIs
- **ISynergy.Framework.CQRS**: `QueryDispatchTable` — compile-time dispatch table replacing `MakeGenericType`-based dynamic dispatch in the query dispatcher
- **ISynergy.Framework.Automations**: `ActionExecutorRegistry` — compile-time action executor registry replacing `MakeGenericType`-based dispatch for automation actions
- **ISynergy.Framework.Mvvm**: `ICommandProvider` interface — AOT-safe command discovery contract, replacing reflection-based command scanning in ViewModels
- **ISynergy.Framework.Mvvm**: `ViewTypeRegistry` and `ViewModelExtensions.RegisterViewTypes()` — explicit view/view-model type registration replacing `AppDomain.GetAssemblies()` scanning
- **ISynergy.Framework.EntityFramework**: `ModelBuilderExtensions.ApplyModelBuilderConfigurations(IReadOnlyList<Type>)` overload — AOT-safe EF Core model configuration applying a pre-resolved list of configuration types instead of assembly scanning
- **ISynergy.Framework.Core**: `ObjectExtensions.Clone<T>(JsonSerializerContext)` overload — AOT-safe deep-clone using a caller-supplied `JsonSerializerContext`, complementing the existing reflection-based overload
- **ISynergy.Framework.UI.Maui**: `IThemeResourceDictionary` marker interface — enables AOT-safe theme resource dictionary discovery without type scanning
- **ISynergy.Framework.MessageBus.Azure**: `JsonTypeInfo<T>`-based constructors and DI registration overloads for all message publishers and consumers — fully AOT-safe serialization path
- **ISynergy.Framework.MessageBus.RabbitMQ**: `JsonTypeInfo<T>`-based constructors and DI registration overloads for all message publishers and consumers — fully AOT-safe serialization path

#### AOT Compatibility — Trim & Dynamic-Code Annotations
- Added `[RequiresUnreferencedCode]`, `[RequiresDynamicCode]`, and `[DynamicallyAccessedMembers]` attributes to all reflection-heavy public APIs across: `ISynergy.Framework.Core`, `ISynergy.Framework.CQRS`, `ISynergy.Framework.IO`, `ISynergy.Framework.Mathematics`, `ISynergy.Framework.Physics`, `ISynergy.Framework.Synchronization`, `ISynergy.Framework.Automations`, `ISynergy.Framework.Mvvm`, `ISynergy.Framework.EntityFramework`, `ISynergy.Framework.AspNetCore` (all five libraries), `ISynergy.Framework.MessageBus.*`, `ISynergy.Framework.OpenTelemetry.*`, and `ISynergy.Framework.Monitoring`
- Set `<IsTrimmable>true</IsTrimmable>` and `<EnableTrimAnalyzer>true</EnableTrimAnalyzer>` in all platform UI project files: `ISynergy.Framework.UI.WPF`, `ISynergy.Framework.UI.WinUI`, `ISynergy.Framework.UI.Maui`, `ISynergy.Framework.UI.Blazor`, and `ISynergy.Framework.UI.UWP`

#### AOT Compatibility — EventSourcing
- **ISynergy.Framework.EventSourcing.EntityFramework**: `IEventSerializer` abstraction — pluggable serialization contract for domain events, separating JSON strategy from `EventStore` and `AggregateRepository`
- **ISynergy.Framework.EventSourcing.EntityFramework**: `JsonReflectionEventSerializer` — reflection-based `IEventSerializer` implementation; annotated `[RequiresUnreferencedCode]`/`[RequiresDynamicCode]` for use in non-trimmed deployments
- **ISynergy.Framework.EventSourcing.EntityFramework**: `JsonSourceGeneratedEventSerializer` — AOT-safe `IEventSerializer` backed by a consumer-supplied `JsonSerializerContext`; requires `[JsonSerializable]` attributes on all event types
- **ISynergy.Framework.EventSourcing.EntityFramework**: `DictionaryEventTypeResolver` — AOT/trim-safe `IEventTypeResolver` using an explicit compile-time type map; use instead of `DefaultEventTypeResolver` in trimmed deployments
- **ISynergy.Framework.EventSourcing.EntityFramework**: AOT-safe `AddEventSourcingEntityFramework` overload accepting a `JsonSerializerContext` and explicit event-type map

#### Blade Panes — WPF and MAUI Parity + RTL Anchoring
- **ISynergy.Framework.UI**: `BladePaneAnchor` (`Auto`/`Start`/`End`) and `BladePlacement` — one platform-agnostic rule that turns a *logical* anchor into a physical edge (`IsStartSide`) and a slide/shadow direction (`HorizontalSign`), so WPF, MAUI and WinUI agree and the rule is testable without a UI thread
- **ISynergy.Framework.UI.WPF**: new `Controls/BladeView` host — a scrim plus a horizontal stack of blades; `ItemsSource`, `Anchor`, `DisabledOpacity`, `BladeSpacing`, `BladeMaxWidth` and `InnerPadding` are dependency properties, the newest blade is the only interactive one, and the pane collapses when `IViewModelBladeView.IsPaneVisible` is `false` (the WinUI control did not consult `IsPaneVisible`). A WPF `Page` cannot join a panel, so each blade is hosted in a `Frame` (`JournalOwnership = OwnsJournal`, navigation UI hidden)
- **ISynergy.Framework.UI.WPF**: `Services/BladeService` now resolves `ISingleSelectionViewModel`/`IMultipleSelectionViewModel` to the selection views instead of relying on the view-name convention; new `Views/SingleSelectionView` and `Views/MultipleSelectionView`
- **ISynergy.Framework.UI.Maui**: new `Controls/BladePane` with the same bindable properties, rendering each blade's `ContentPage.Content` because a page cannot be a child of a layout; new `Services/IBladePaneHost` + `BladePaneHostRegistry` (weak references, singleton) so a blade service can discover an attached pane; new `Views/SingleSelectionView` and `Views/MultipleSelectionView`
- **ISynergy.Framework.UI.Maui**: new `Services/BladeService` — **adaptive**: an overlay pane on tablet/desktop when a pane host is attached and the idiom is not `Phone`; otherwise it pushes the blade page with `PushModalAsync` and pops it on the blade's `Closed` (only while that page is on top of the modal stack), falling back to the overlay with a logged warning when neither path is possible. Registered as `IBladeService`/`IBladePaneHost` in `MauiAppBuilderExtensions`
- **ISynergy.Framework.UI.WinUI**: `BladeView` gained `Anchor` (default `Auto`) and the read-only, already-mirrored `BladeAlignment`; `HorizontalBladeAlignment` is kept as the left-to-right intent and mirrored for right-to-left, the `ListBox` is pinned to `FlowDirection="LeftToRight"` so the alignment is physical, and each card's content mirrors through `FlowDirection`
- **ISynergy.Framework.UI.WinUI**: XAML `FlowDirection` is now applied to the window root in `Application.OnLaunched` (from `LocalSettings.Language`) and re-applied to window-level pages in `NavigationService.SetWindowContent` (from `CultureInfo.CurrentUICulture`); the native `WS_EX_LAYOUTRTL` style is now only responsible for the caption/non-client chrome, because XAML content is composed outside GDI and was never mirrored by it
- **ISynergy.Framework.UI.WinUI**: `FrameworkElementExtensions.ApplyFlowDirection` now accepts `string?` — `null` or whitespace means left-to-right
- **samples**: `Sample.WPF` gained a Blades page (`DetailsView`/`DetailView`/`DetailAView` with their view models and a navigation entry); `Sample.Maui` converted its Details/Detail flow to blades with the pane inside `DetailsView`

#### Settings as a Blade — WPF, WinUI and MAUI
- **ISynergy.Framework.UI**: new `ViewModels/BladeHostViewModel` — a sealed `ViewModelBladeView<object>` shell-level blade owner that carries no items (no-op `Add`/`Edit`/`Remove`/`Search`); registered as a singleton by `WPFAppBuilderExtensions` and `MauiAppBuilderExtensions`, so a head can own a pane without changing the base class of its shell view model
- **ISynergy.Framework.UI.WPF**: new `Controls/BladeHostView` — resolves `BladeHostViewModel` from `Application.Services`, binds a `BladeView` to its `Blades`, and stays collapsed with a trace when the view model is not resolvable (design time)
- **ISynergy.Framework.UI.Maui**: new `Controls/BladeHostView` — a `ContentView` that resolves `BladeHostViewModel` lazily (constructor, `Loaded`, `OnParentSet`) and attaches a `BladePane` with `ItemsSource = Blades`; it stays hidden with a trace when unresolvable, and is declared as the last child so it overlays the page
- **Sample.WPF**: Settings is now a blade — `SettingsViewModel` is a `ViewModelBlade<object>` (no navigation service, no cancel override), `ShellViewModel.OpenSettingsAsync` opens it through `IBladeService` against the shared host, `ShellView` hosts `BladeHostView` above the content frame, and the Close button closes the blade through its cancel command
- **Sample.Maui**: Settings is now a blade — the settings UI moved into `Views/SettingsView.xaml`, `SettingsViewModel` is a `ViewModelBlade<object>` with a `SaveCommand` (saves local settings, asks the restart question, saves global settings, then closes) plus the inherited cancel command; the shell's `RootView` hosts `BladeHostView` as its last child (the per-page hosts were removed when the sample dropped `Shell`), and the shell opens the blade through `IBladeService`, first run included. The `SettingsDrawer` control, `SettingsDrawerState` and their `MauiProgram` registration were removed; on phone idiom the blade presents as a modal page
- **Sample.WinUI**: verified against the new host model and unchanged in behaviour — Settings was already a `ViewModelBlade<object>` opened through `IBladeService` into the shell's `BladeView` and closed by its `CloseCommand`

#### MAUI Shell — Content-Host Navigation Without `Shell`
- **ISynergy.Framework.UI.Maui**: new `Services/INavigationHost` + `INavigationHostRegistry`/`NavigationHostRegistry` (weak reference, singleton) and the `Controls/NavigationHostView` control — the MAUI equivalent of the content `Frame` a WinUI or WPF shell exposes. `NavigationService` resolves the registered host as its navigation frame (`GetNavigationFrame`/`GetFrameContent`/`SetFrameContent`) and leaves the backstack to `BaseNavigationService`, so `NavigateAsync` replaces the host's content like it replaces a frame's content on the other heads; the MAUI-specific push/pop overrides are gone. MAUI pages cannot be children of a layout, so the host detaches and caches each page's `Content` and reparents it, the same mechanism `BladePane` uses. `NavigateModalAsync` still uses MAUI's native modal stack, but only pushes on top of a root that owns a navigation host (otherwise it replaces the window root, as before)
- **ISynergy.Framework.Mvvm**: `INavigationService.CurrentViewModelType` — the type of the view model the host currently shows, implemented by `BaseNavigationService`, so a shell can restore its breadcrumb, selection or title after a back navigation. The MAUI `NavigationService` now always raises `BackStackChanged` after a pop (the base only raised when the stack became empty), so back-button state stays correct
- **samples/Sample.Maui**: the sample no longer uses `Microsoft.Maui.Controls.Shell`. `Views/RootView` is one always-alive page holding the header, the sidebar, the navigation host and the shell-level `BladeHostView` (last child, so a blade dims and covers all of it), and it is set as the window root at startup. `ShellViewModel` navigates its sections with `INavigationService.NavigateAsync<TViewModel>()` exactly like `Sample.WinUI` navigates its content frame; `CurrentRoute`/`CrumbCurrent` follow the shell navigation service, including back. Phones get a slide-in sidebar drawer (header menu button, scrim, closes on selection) instead of a sidebar column; desktop keeps the fixed sidebar. Back is the header button bound to `IsBackEnabled`, and Android's hardware back routes to `INavigationService.GoBackAsync()` when it can go back

### Changed

#### CI/CD moved to GitHub Actions
- **.github/workflows/ci.yml** replaces `azure-pipelines.yml`: version stamping, SDK and MAUI workload installation, build, vulnerable-package audit, packing, the net10/net11 package-content guard, tests with OpenCover coverage, Key Vault signing, NuGet.org publishing and release tagging now run on GitHub-hosted Windows runners. PR builds keep the ACC/PROD split: they sign and push `-preview` packages through the `acceptance` environment, while merges to `main` publish release packages through `production`, which carries the approval gate and is the only lane that tags. Key Vault signing uses OIDC (`azure/login@v2` + `WorkloadIdentityCredential`) with one federated credential per environment, and the NuGet.org push uses trusted publishing (`NuGet/login@v1`).
- **scripts/Set-VersionVariables.ps1 and scripts/New-GitTag.ps1** write `GITHUB_OUTPUT` and `::error::` annotations under GitHub Actions while keeping the `##vso[...]` protocol for Azure Pipelines. The version scheme (`yyyy.1MMdd.1HHmm`) and `v<version>` tags are unchanged.
- **azure-pipelines.yml** is deprecated and will be deleted after the first successful GitHub release.

#### Unified Sample Styling
- **Samples (WPF / WinUI / MAUI / Blazor)**: Applied the `Sample.Maui` design language to every sample — Figtree typography, shared surface/border/text/muted/accent tokens, the four-color accent palette (teal / blue / green / orange) and the same settings surface layout. Only the platform entry point differs (WPF page, WinUI blade, MAUI and Blazor drawers); menu items stay platform-specific.
- **Samples**: The default accent colour is now the MAUI teal `#0B5D55` (`Sample.Shared/Models/LocalSettings.Color`).
- **Sample.WPF / Sample.WinUI**: Bundle the Figtree font family; **Sample.Blazor** serves it from `wwwroot/fonts`.
- **Sample.WPF**: Removed the `Syncfusion.SfNavigationDrawer.WPF` control/package from the shell. The shell is now plain WPF (300px sidebar, brand tile, navigation rows, breadcrumb header with theme/settings buttons, `Frame` content host) styled with the shared tokens.
- **Sample.WinUI**: Added the shared breadcrumb header (back/theme/settings), a `PaneHeader` brand tile and the shared surface/font tokens to the shell; Theme/Settings moved out of the footer menu.
- **Samples (WPF / WinUI / MAUI)**: The demo user auto-authenticates at startup, so the samples go straight to the shell instead of showing the login screen. WinUI no longer forces the settings blade on first run.
- **Sample shells (WPF / WinUI / Blazor)**: Help is now a footer action styled like the navigation rows, followed by the product version. Logout/Login was removed from the shell navigation.
- **Sample fixes**: WPF validation now runs on input and the WPF theme switcher applies the dark/light palettes. WinUI settings-blade close is guarded, the Help footer uses the navigation icon tile, and the breadcrumb follows the selected navigation item. Blazor's shell was rebuilt without FluentLayout to match the MAUI sidebar/header.
- **Sample titles**: Each head shows its own name — WPF Sample, WinUI Sample, Blazor Sample, Maui Sample.
- **WPF chrome**: Caption buttons sit flush against the window edge; corners are rounded only when restored (4px) and square when maximized/minimized; the maximize button uses the work area so the taskbar stays visible, while kiosk/full-screen remains available.
- **WinUI chrome**: System caption buttons now follow the light/dark palette instead of a fixed light scheme.
- **Sample shells (WPF / WinUI / MAUI)**: Removed the separate OS title bar. WPF draws its own title row with minimize/maximize/close buttons (borderless `WindowChrome`); WinUI and MAUI Windows extend the header into the native title bar and keep the native caption buttons; macOS hides the native title text and keeps the traffic lights.

#### AOT Compatibility — Wave 1 (Core infrastructure)
- **ISynergy.Framework.Core**: Reflection-based utilities annotated for trim safety; AOT-safe `Clone<T>` overload added alongside existing API
- **ISynergy.Framework.CQRS**: Dynamic handler resolution replaced with source-generated `QueryDispatchTable`; existing reflection path retained with `[RequiresDynamicCode]` annotation for non-AOT consumers
- **ISynergy.Framework.IO**, **ISynergy.Framework.Mathematics**, **ISynergy.Framework.Physics**, **ISynergy.Framework.Synchronization**, **ISynergy.Framework.Automations**: All reflection-heavy code paths annotated; compile-time alternatives provided where necessary

#### AOT Compatibility — Wave 2 (Application layer)
- **ISynergy.Framework.Mvvm**: Command and view discovery migrated to `ICommandProvider` / `ViewTypeRegistry` model; legacy `AppDomain`-based scan path annotated `[RequiresUnreferencedCode]`
- **ISynergy.Framework.EntityFramework**: Model builder configuration discovery now supports explicit type-list overload alongside the existing assembly-scan path
- **ISynergy.Framework.AspNetCore** (Authentication, Globalization, MultiTenancy, Monitoring, Middleware): All dynamic middleware and service resolution paths annotated; AOT-friendly registration overloads added
- **ISynergy.Framework.MessageBus** (abstractions): Serialization contracts updated to support `JsonTypeInfo<T>` alongside existing `JsonSerializerOptions`-based paths
- **ISynergy.Framework.OpenTelemetry.\***, **ISynergy.Framework.Monitoring**: Exporter and instrumentation registration paths annotated for trim safety

#### AOT Compatibility — Wave 3 (UI platforms)
- **ISynergy.Framework.UI.WPF**: Trim analysis enabled; dynamic resource and converter discovery replaced with source-generated registrations
- **ISynergy.Framework.UI.WinUI**: Trim analysis enabled; WinUI-specific activation factory calls annotated
- **ISynergy.Framework.UI.Maui**: Trim analysis enabled; handler registration and theme discovery migrated to `IThemeResourceDictionary` marker interface
- **ISynergy.Framework.UI.Blazor**: Trim analysis enabled; component scanning replaced with source-generated `AddBlazorRegistrations()`
- **ISynergy.Framework.UI.UWP**: Trim analysis enabled; activation and navigation registration paths annotated

#### AOT Compatibility — Wave 4 (Messaging)
- **ISynergy.Framework.MessageBus.Azure**: All publisher/consumer classes now accept `JsonTypeInfo<T>` in constructors and DI registration; `JsonSerializerOptions` paths retained with `[RequiresDynamicCode]` for backwards compatibility
- **ISynergy.Framework.MessageBus.RabbitMQ**: All publisher/consumer classes now accept `JsonTypeInfo<T>` in constructors and DI registration; `JsonSerializerOptions` paths retained with `[RequiresDynamicCode]` for backwards compatibility

#### AOT Compatibility — Wave 5 (EventSourcing)
- **ISynergy.Framework.EventSourcing.EntityFramework**: `EventStore` and `AggregateRepository` refactored to accept `IEventSerializer` — inline `JsonSerializer` calls removed from both classes; serialization strategy is now fully pluggable
- **ISynergy.Framework.EventSourcing.EntityFramework**: `ServiceCollectionExtensions.AddEventSourcingEntityFramework` — existing reflection-based overload annotated `[RequiresUnreferencedCode]`/`[RequiresDynamicCode]` and now registers `JsonReflectionEventSerializer` automatically
- **ISynergy.Framework.EventSourcing.EntityFramework**: `<IsAotCompatible>true</IsAotCompatible>` added to project file; the trim analyzer now validates AOT-safe code paths at build time

- **[BREAKING] ISynergy.Framework.EventSourcing**: `IEventStore.AppendEventAsync` — `object? metadata` parameter renamed to `string? metadataJson`; callers must pre-serialize metadata to JSON. No existing callers passed this parameter (all call sites used the default `null`), so the impact is limited to custom implementations of `IEventStore`.

- **[BREAKING] ISynergy.Framework.UI.UWP**: Minimum Windows version raised from 10.0.17763.0 (Version 1809) to 10.0.19041.0 (Version 2004)
  - **Location**: `src/ISynergy.Framework.UI.UWP/ISynergy.Framework.UI.UWP.csproj` line 4
  - **Old Value**: `<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>`
  - **New Value**: `<TargetPlatformMinVersion>10.0.19041.0</TargetPlatformMinVersion>`
  - **Affected Scenarios**: Any UWP application targeting Windows 10 Version 1809 must upgrade to Version 2004 or later
  - **Technical Justification**: Windows 10 Version 2004 provides access to modern Windows SDK APIs and tooling features required for optimal compatibility with .NET 10, including enhanced platform capabilities and improved build toolchain support
  - **Migration Path**: See [UWP Migration Guide](src/ISynergy.Framework.UI.UWP/readme.md#migration-guide) in the package README

#### Framework review — layering, async, robustness and audit (2026-10-07)
- **[BREAKING] ISynergy.Framework.Core / ISynergy.Framework.Globalization**: Removed the `ISynergy.Framework.Core → ISynergy.Framework.Globalization` project reference so `Core` has no framework-project dependency. The shared types `ILanguageService`, `GlobalizationConstants` and the `DateTimeConverter` / `DateTimeOffsetConverter` / `TimeOnlyConverter` JSON converters now physically live in the `ISynergy.Framework.Core` assembly — their `ISynergy.Framework.Globalization.*` namespaces are unchanged and `Globalization` type-forwards them (`TypeForwarding.cs`). `Globalization` now references `Core`.
  - **Migration**: projects that previously received `ISynergy.Framework.Globalization` transitively through `Core` must now reference it explicitly (or transitively through `UI`/`AspNetCore.Blazor`/`AspNetCore.Globalization`/`Documents.Syncfusion`/`Geography`, which were updated).
- **ISynergy.Framework.OpenTelemetry.Sentry**: Migrated off the obsolete `Sentry.OpenTelemetry` API (`AddSentry` / `UseOpenTelemetry`) to `Sentry.OpenTelemetry.Exporter` (`AddSentryOtlpExporter` / `UseOtlp`); the package reference is now `Sentry.OpenTelemetry.Exporter`.
- **ISynergy.Framework.Automations / ISynergy.Framework.Core**: `IActionService` / `IAutomationService` async methods and `NetworkUtility`'s async methods now accept a `CancellationToken`; `BaseAsyncRelayCommand.LogIfFailed` is no longer `async void`; the execution timer no longer leaks on refresh; previously swallowed exceptions are traced.
- **ISynergy.Framework.CQRS**: `QueryDispatcher`'s reflection fallback no longer uses `dynamic` — it reflects `HandleAsync` once per handler shape and invokes it through `MethodInfo`.
- **Build / CI**: `NuGetAuditMode` raised from `direct` to `all` (transitive packages are audited on every restore/build) and the Azure Pipeline now runs `dotnet list … --vulnerable --include-transitive`.
- **Internals (no public API change)**: `BaseAsyncRelayCommand`, `MessengerService` and `RedBlackTree<T>` were decomposed — the pools/timeout-cache into `CommandExecutionResourcePool`, recipient storage into `MessengerRecipientRegistry`, and the red-black balancing fix-ups into a nested `Balancing` collaborator.

### Removed

- **[BREAKING] ISynergy.Framework.UI / UI.WPF / UI.WinUI / UI.Maui**: Removed the dialog-based theme and language selection — `ThemeViewModel`, `LanguageViewModel`, `IThemeWindow`, `ILanguageWindow`, `ILanguageViewModel`, the WPF/WinUI/MAUI `ThemeWindow` and `LanguageWindow` views, and the MAUI `RadioButtonToThemeConverter`. `IShellViewModel.LanguageCommand` / `BaseShellViewModel.LanguageCommand` and the `OpenLanguageAsync`/`OpenThemesAsync` hooks are gone from all heads.
  - The shell's `ThemeCommand` now toggles between `Themes.Light` and `Themes.Dark` through `IThemeService`; accent-color and language selection belong in the application's settings surface.
  - `BaseShellViewModel` constructors in WPF, WinUI and MAUI now take an `IThemeService`.
  - **Migration**: put language/theme/accent controls in your settings view (`ISupportedCulturesService.SupportedCultures` + `CultureInfoExtensions.ApplyCulture()` for language, `IThemeService.SetTheme`/`SetAccentColor` for theme) and pass `IThemeService` to the base constructor.

- **[BREAKING] ISynergy.Framework.EntityFramework**: Removed the `DbContextExtensions` class (`ExistsAsync`, `GetItemByIdAsync`, `RemoveItemAsync`) and its unit tests. These repository-style helpers contradicted the framework's documented data-access rule — use EF Core `DbSet<T>` primitives directly on the `DataContext` — were not AOT-safe (runtime `Expression` building), used `context.Update`, and auto-included every navigation. Migrate to `_context.Set<TEntity>()` or a named `DbSet` property with `AnyAsync` / `FirstOrDefaultAsync` / `Add` / `Remove` / `SaveChangesAsync`. See `src/ISynergy.Framework.EntityFramework/readme.md`.

- **[BREAKING] ISynergy.Framework.Mvvm / ISynergy.Framework.UI.Blazor / ISynergy.Framework.AspNetCore.Blazor**: The `MessageBoxImage` and `MessageBoxResult` enums were renamed to `MessageBoxImages` and `MessageBoxResults` (plural), consistent with the already-plural `MessageBoxButtons` and the framework's plural-enum convention. The enum files were renamed to match; the platform-specific `System.Windows.MessageBoxImage` / `System.Windows.MessageBoxResult` (WPF) are unaffected.
  - **Migration**: replace references to `…Enumerations.MessageBoxImage` / `MessageBoxResult` with `MessageBoxImages` / `MessageBoxResults`; `MessageBoxRequest.MessageBoxImage` is now `MessageBoxRequest.MessageBoxImages`.

### Fixed
- **Sample.WPF / Sample.WinUI / Sample.Blazor**: the sidebar footer now shows the copyright line (`© <year> I-Synergy. All rights reserved.`) under the version, matching the MAUI sample's footer.
- **ISynergy.Framework.UI.Maui**: `Themes.Default` was mapped to `AppTheme.Dark` in `ThemeService.ApplyTheme`, so "follow the system" actually forced dark mode. It now maps to `AppTheme.Unspecified`.
- **ISynergy.Framework.UI.WPF**: `ThemeService` resolves `Themes.Default` to the operating system's application theme instead of letting each consumer interpret it, and `Theme` / `IsLightThemeEnabled` now report that resolved theme. `Themes.Default` used to paint the light palette (`Sample.WPF` compared it with `Themes.Dark`) while the shell's toggle treated it as dark, so the WPF sample started in light mode with a dark-mode icon and needed two clicks to reach the dark theme; one click now switches correctly.
- **ISynergy.Framework.UI.WPF**: `DynamicThemeResourceDictionary` takes the theme and derives the semantic `Tinted*` brushes from the matching end of the gray ramp (dark: 950/900/600/500, light: 000/100/200/300) instead of always using the dark values, so framework controls follow light/dark switches.
- **Sample.WPF**: The theme icon, the design tokens and the settings blade's theme picker are derived from the resolved `IThemeService` theme, so the whole shell starts in the theme it reports and the picker never opens with an empty selection.
- **Sample.Maui title bar polish**: the native caption area is `Tall` (48px) and the shell header row is 48px, so the minimise/maximise/close buttons line up with the header's icon buttons as in the WinUI sample; the system backdrop (Mica) is cleared and the title-bar strip plus the caption-button backgrounds/hover are painted with the shell colours (`AppBg*`/`AppSurface2*`) after the title bar is realized, so the chrome matches the page and follows runtime theme changes; `AppVerseActionButton` binds `TextColor` to the explicit light/dark muted pair instead of the single-key `AppMuted` token. The hamburger is the first header control and is padded so its icon sits on the sidebar's icon column, and the WinUI system focus rectangle is suppressed on the header buttons (it read as a white border around the initially focused hamburger). Sidebar rows, the breadcrumb link and buttons get a hand cursor on Windows, and the rows gained a hover fill.
- **Sample.Maui shell polish**: the blade pane's card background, stroke and separator now follow the app theme (`AppSurface*`/`AppBorder*` with the framework `TintedGray*` palette as fallback) instead of the fixed light tints that produced a white outline in dark mode, the settings card's own `Stroke` is theme-aware, the sidebar Help row is a full-size row (52px, 36px icon tile) aligned with the menu items, and the header icon buttons paint on hover only — `Pressed`/`Focused` no longer leave a "selected" fill after a blade opens over the pointer. The hamburger is available on every idiom: it collapses the sidebar into a 0-width column on desktop and opens/closes the drawer on phones.
- **Sample.Maui shell polish**: the blade pane's card background, stroke and separator now follow the app theme (`AppSurface*`/`AppBorder*` with the framework `TintedGray*` palette as fallback) instead of the fixed light tints that produced a white outline in dark mode, the settings card's own `Stroke` is theme-aware, the sidebar Help row is a full-size row (52px, 36px icon tile) aligned with the menu items, and the header icon buttons paint on hover only — `Pressed`/`Focused` no longer leave a "selected" fill after a blade opens over the pointer. The hamburger is available on every idiom: it collapses the sidebar into a 0-width column on desktop and opens/closes the drawer on phones.
- **Sample.Maui title bar**: the shell header is now the window title bar (WinUI look) — `App.SetTitleBar` builds a `TitleBar` whose `Content` is the header, `RootView` binds it to the shell view model and collapses the in-page header row, so the breadcrumb, header commands and native caption buttons share one row. `Application.SetTitleBar` is nullable so a head can opt out.
- **ISynergy.Framework.UI.Maui**: `BladeHostView` is now only visible while its `BladePane` has blades to show. The host used to set `IsVisible = true` as soon as it resolved its view model, so an empty (invisible) pane with a visible, full-window host sat on top of the shell and swallowed every click aimed at the header, the sidebar and the page underneath. Shells that host the pane at the root (as `Sample.Maui` now does) were completely unclickable.
- **Sample.Maui**: The breadcrumb and the sidebar selection no longer fall back to the previous section after a forward navigation. The navigation service raises `BackStackChanged` before the new frame becomes current, so the route is only re-read from `INavigationService.CurrentViewModelType` when no shell navigation is in flight (that is, on back).
- **Sample.WinUI**: Added the missing `BladeView` host to `ShellView.xaml`, so blades opened from the shell (including the Settings blade used by `SettingsCommand`) are rendered.
- **Sample.Blazor**: Registered the missing `IThemeService` (scoped `ISynergy.Framework.UI.Services.ThemeService`) in `Program.cs`; the sample previously failed at startup when `MainLayout` requested it.
- **ISynergy.Framework.UI.Blazor**: `ThemeService` now re-applies the theme on every `SetTheme`/`SetAccentColor` call (the `_isInitialized` guard blocked live updates) and imports the interop module from its `_content/I-Synergy.Framework.UI.Blazor/js/` package path instead of the non-existent `./js/` path.
- **Sample.Blazor**: Replaced the header language toggle with a settings drawer exposing language, theme and accent colour with live application.
- **Sample.WinUI**: Changed `<DefaultLanguage>` from `en` to `en-US` to eliminate PRI257 resource packaging warning; `makepri.exe` requires the default language tag to match the actual resource qualifiers present (`en-us`, `de-de`, etc.)
- **Sample.Maui (Android 16)**: Resolved XA0141 by pinning `SQLitePCLRaw.lib.e_sqlite3.android` to 2.1.11 in central package management and adding a direct Android-conditional `PackageReference`; the transitively-pulled version 2.1.6 (via `Dotmim.Sync.Sqlite`) used 4KB ELF segment alignment which Android 16 rejects
- **Compiler warnings — MAUI samples**: Replaced all deprecated `NamedSize` string values (`Title`, `Body`, `Subtitle`, `Caption`, `Large`) in MAUI XAML files with explicit numeric pixel values; removed obsolete `Frame` and `ListView` XAML styles
- **Compiler warnings — library projects**: Removed unused private fields (`CS0169`, `CS0414`), removed unused local variables (`CS0219`), fixed nullable return on `SignUpViewModel.Mail` getter (`CS8603`), and resolved ambiguous XML `<see cref>` in `MauiAppBuilderExtensions` (`CS0419`)
- **Compiler warnings — WinUI/MAUI**: the English-only `SupportedCulturesService` fallback now passes a concrete array instead of a collection expression, removing `CsWinRT1032` ("Collection expressions targeting non mutable interface type … are not supported for trimming and AOT compatibility")
- **Compiler warnings — `FileResult` deprecation**: Migrated the WPF/WinUI/MAUI `FileService` / `CameraService` / `ImageBrowser` call sites from the obsolete `FileResult.File` to `await GetFileAsync()`, eliminating the `CS0618` warnings introduced by the deprecation below.
- **ISynergy.Framework.EventSourcing.Storage**: `EventArchiver.CreateAggregate` now caches aggregate-type resolution and selects deterministically (ordinal full-name order, ambiguity traced) instead of depending on assembly load order.
- **ISynergy.Framework.Storage.S3**: `S3StorageService.GetObjectKey` normalises separators and drops `.` / `..` segments, closing a path-traversal hole in the object key.
- **Samples**: Documented the `Sample.Maui` MAUI-workload build failure (`MSB4018` in the Resizetizer `GenerateSplashAssets` task) in its readme.

### Deprecated
- **ISynergy.Framework.Core** `ObjectExtensions.Clone<T>()` (reflection-based overload): Use `Clone<T>(JsonSerializerContext)` in AOT/trimmed deployments. The parameterless overload is annotated `[RequiresUnreferencedCode]` and will be removed in a future major version.
- **ISynergy.Framework.CQRS** reflection-based query dispatch: Use the source-generated `QueryDispatchTable` registered via `AddQueryDispatchTable()`. The dynamic dispatch path is annotated `[RequiresDynamicCode]`.
- **ISynergy.Framework.MessageBus.Azure** and **ISynergy.Framework.MessageBus.RabbitMQ** `JsonSerializerOptions`-based constructors: Use `JsonTypeInfo<T>`-based overloads in AOT/trimmed deployments. The `JsonSerializerOptions` constructors are annotated `[RequiresDynamicCode]`.
- **ISynergy.Framework.Mvvm** `AppDomain`-based view/view-model scanning: Use `ViewModelExtensions.RegisterViewTypes()` with the source-generated map. The scanning path is annotated `[RequiresUnreferencedCode]`.
- **ISynergy.Framework.Core** `FileResult.File` and `FileResult.GetStream()`: Use `FileResult.GetFileAsync()` and `FileResult.GetStreamAsync()`. The synchronous accessors block the calling thread when the file was supplied through an asynchronous getter and can deadlock.

### Security
- **Samples**: Removed the committed Sentry DSN and Azure Application Insights connection string from the sample `appsettings.json` files; telemetry credentials must now be supplied via environment variables or user-secrets (see `samples/Sample.Shared/readme.md`). **Rotate the previously committed values — they remain in git history.**
- **Sample.EventSourcing.Api**: No longer trusts the spoofable `X-Tenant-Id` / `X-User-Name` request headers; it requires an authenticated JWT and resolves the tenant and user from claims via `AddMultiTenancyIntegration`, with CORS restricted to the configured origins.
- **ISynergy.Framework.OpenTelemetry**: `UserContextEnrichingLogProcessor` no longer attaches user/PII attributes unless `IncludeUserContext` is enabled; the samples ship `SendDefaultPii: false`.

### Documentation
- Updated [src/ISynergy.Framework.UI.UWP/readme.md](src/ISynergy.Framework.UI.UWP/readme.md) with breaking change notice, migration guide, and technical justification

---

## Previous Releases

See individual package changelogs and release notes for version history prior to this centralized changelog.
