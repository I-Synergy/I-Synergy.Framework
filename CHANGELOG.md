# Changelog

All notable changes to the I-Synergy Framework will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

#### AOT Compatibility — New Projects
- **ISynergy.Framework.CQRS.SourceGenerator**: New Roslyn incremental source generator that emits `AddCqrsHandlers()` and `AddQueryDispatchTable()` extension methods at compile time, enabling fully AOT-safe CQRS handler registration without runtime reflection
- **ISynergy.Framework.UI.SourceGenerator**: New Roslyn incremental source generator that emits `AddUITypes()` and a `ViewModelViewMap` at compile time, replacing `AppDomain.GetAssemblies()` scanning for view/view-model discovery
- **ISynergy.Framework.AspNetCore.Blazor.SourceGenerator**: New Roslyn incremental source generator that emits `AddBlazorRegistrations()` for AOT-safe Blazor component and service registration

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

### Changed

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

### Fixed
- **Sample.WinUI**: Changed `<DefaultLanguage>` from `en` to `en-US` to eliminate PRI257 resource packaging warning; `makepri.exe` requires the default language tag to match the actual resource qualifiers present (`en-us`, `de-de`, etc.)
- **Sample.Maui (Android 16)**: Resolved XA0141 by pinning `SQLitePCLRaw.lib.e_sqlite3.android` to 2.1.11 in central package management and adding a direct Android-conditional `PackageReference`; the transitively-pulled version 2.1.6 (via `Dotmim.Sync.Sqlite`) used 4KB ELF segment alignment which Android 16 rejects
- **Compiler warnings — MAUI samples**: Replaced all deprecated `NamedSize` string values (`Title`, `Body`, `Subtitle`, `Caption`, `Large`) in MAUI XAML files with explicit numeric pixel values; removed obsolete `Frame` and `ListView` XAML styles
- **Compiler warnings — library projects**: Removed unused private fields (`CS0169`, `CS0414`), removed unused local variables (`CS0219`), fixed nullable return on `SignUpViewModel.Mail` getter (`CS8603`), and resolved ambiguous XML `<see cref>` in `MauiAppBuilderExtensions` (`CS0419`)

### Deprecated
- **ISynergy.Framework.Core** `ObjectExtensions.Clone<T>()` (reflection-based overload): Use `Clone<T>(JsonSerializerContext)` in AOT/trimmed deployments. The parameterless overload is annotated `[RequiresUnreferencedCode]` and will be removed in a future major version.
- **ISynergy.Framework.CQRS** reflection-based query dispatch: Use the source-generated `QueryDispatchTable` registered via `AddQueryDispatchTable()`. The dynamic dispatch path is annotated `[RequiresDynamicCode]`.
- **ISynergy.Framework.MessageBus.Azure** and **ISynergy.Framework.MessageBus.RabbitMQ** `JsonSerializerOptions`-based constructors: Use `JsonTypeInfo<T>`-based overloads in AOT/trimmed deployments. The `JsonSerializerOptions` constructors are annotated `[RequiresDynamicCode]`.
- **ISynergy.Framework.Mvvm** `AppDomain`-based view/view-model scanning: Use `ViewModelExtensions.RegisterViewTypes()` with the source-generated map. The scanning path is annotated `[RequiresUnreferencedCode]`.

### Documentation
- Updated [src/ISynergy.Framework.UI.UWP/readme.md](src/ISynergy.Framework.UI.UWP/readme.md) with breaking change notice, migration guide, and technical justification

---

## Previous Releases

See individual package changelogs and release notes for version history prior to this centralized changelog.
