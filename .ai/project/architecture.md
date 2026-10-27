# Project Architecture — I-Synergy Framework

> **Note:** This repository is a reusable framework library (`ISynergy.Framework.*`), not an application with domain projects. The documented `{ApplicationName}.Domain.{Domain}` project structure with CQRS Features/Commands/Queries/Models applies to consuming applications built on top of this framework, not to this repo itself. The src/ directory contains framework projects providing CQRS abstractions, EntityFramework helpers, UI components, and cross-cutting utilities.

## Architecture Style

**Primary Architecture:** Modular Framework Library

**Why:** The framework provides reusable building blocks for Clean Architecture / CQRS / Event Sourcing applications. It does not prescribe one application topology — instead it ships composable packages that consuming applications wire together.

## Architectural Patterns (as provided by the framework)

| Pattern | Provided By | Mechanism |
|---------|-------------|-----------|
| **CQRS** | `ISynergy.Framework.CQRS` | `ICommand<T>`, `IQuery<T>`, handlers, dispatcher |
| **Event Sourcing** | `ISynergy.Framework.EventSourcing.*` | Event store, aggregate replay, snapshots |
| **MVVM** | `ISynergy.Framework.Mvvm` + `ISynergy.Framework.UI.*` | ViewModel base classes, navigation, dialog, messaging |
| **Message Bus** | `ISynergy.Framework.MessageBus.*` | Pub/sub (Azure Service Bus, RabbitMQ) |
| **Multi-Tenancy** | `ISynergy.Framework.AspNetCore.MultiTenancy` | Tenant resolution, per-tenant data isolation |
| **Automations** | `ISynergy.Framework.Automations` | Rule-based triggers, actions, conditions |
| **Monitoring** | `ISynergy.Framework.Monitoring` + `ISynergy.Framework.OpenTelemetry.*` | Real-time monitoring via SignalR + OpenTelemetry |
| **Synchronization** | `ISynergy.Framework.Synchronization` | Offline/online data sync via Dotmim.Sync |

## Clean Architecture Layers (how the framework supports them)

The framework provides building blocks for each Clean Architecture layer:

```
Presentation Layer (UI, API)
    ↓ depends on
Application Layer (Use Cases, CQRS)
    ↓ depends on
Domain Layer (Core Abstractions)
    ↑ implements
Infrastructure Layer (Data, External Services)
```

| Layer | Framework Projects | Purpose |
|-------|-------------------|---------|
| **Presentation** | `UI.*`, `UI.Maui`, `UI.WinUI`, `UI.WPF`, `UI.Blazor`, `AspNetCore.Blazor` | ViewModels, pages, controls, navigation, dialogs |
| **Application** | `CQRS`, `Automations`, `Monitoring` | Command/Query handling, automation rules, monitoring hubs |
| **Domain / Core** | `Core`, `Mvvm` | Base classes, abstractions, messaging, MVVM primitives |
| **Infrastructure** | `EntityFramework`, `EventSourcing.*`, `Storage.*`, `Mail.*`, `MessageBus.*`, `KeyVault.*`, `OpenTelemetry.*` | Data access, external services, telemetry |

## Project Structure

```
src/
├── ISynergy.Framework.Core/              # Core abstractions, base classes, utilities
├── ISynergy.Framework.CQRS/              # CQRS abstractions (ICommand, IQuery, handlers)
├── ISynergy.Framework.Mvvm/              # MVVM base classes, ViewModel primitives
├── ISynergy.Framework.EntityFramework/    # EF Core helpers, DataContext base, repositories
├── ISynergy.Framework.EventSourcing/     # Event store abstractions, aggregate roots
│   ├── ISynergy.Framework.EventSourcing.EntityFramework/
│   ├── ISynergy.Framework.EventSourcing.Storage/
│   ├── ISynergy.Framework.EventSourcing.Storage.Azure/
│   └── ISynergy.Framework.EventSourcing.Storage.S3/
├── ISynergy.Framework.AspNetCore/        # ASP.NET Core middleware, filters, extensions
│   ├── ISynergy.Framework.AspNetCore.Authentication/
│   ├── ISynergy.Framework.AspNetCore.Blazor/
│   ├── ISynergy.Framework.AspNetCore.Globalization/
│   ├── ISynergy.Framework.AspNetCore.Monitoring/
│   └── ISynergy.Framework.AspNetCore.MultiTenancy/
├── ISynergy.Framework.UI/               # Shared UI base (dialogs, navigation, theming)
│   ├── ISynergy.Framework.UI.Maui/       # MAUI controls, services
│   ├── ISynergy.Framework.UI.WinUI/      # WinUI 3 controls, services
│   ├── ISynergy.Framework.UI.WPF/        # WPF controls, services
│   └── ISynergy.Framework.UI.Blazor/     # Blazor components
├── ISynergy.Framework.MessageBus/       # Message bus abstractions
│   ├── ISynergy.Framework.MessageBus.Azure/
│   └── ISynergy.Framework.MessageBus.RabbitMQ/
├── ISynergy.Framework.Storage/          # Storage abstractions
│   ├── ISynergy.Framework.Storage.Azure/
│   └── ISynergy.Framework.Storage.S3/
├── ISynergy.Framework.Mail/             # Mail abstractions
│   ├── ISynergy.Framework.Mail.Microsoft365/
│   └── ISynergy.Framework.Mail.SendGrid/
├── ISynergy.Framework.KeyVault/         # Secrets management abstractions
│   ├── ISynergy.Framework.KeyVault.Azure/
│   └── ISynergy.Framework.KeyVault.OpenBao/
├── ISynergy.Framework.OpenTelemetry/    # Observability abstractions
│   ├── ISynergy.Framework.OpenTelemetry.ApplicationInsights/
│   └── ISynergy.Framework.OpenTelemetry.Sentry/
├── ISynergy.Framework.Documents/        # Document processing
│   ├── ISynergy.Framework.Documents.Syncfusion/
├── ISynergy.Framework.Financial/        # Financial/mathematical libraries
├── ISynergy.Framework.Geography/        # Geography types & calculations
├── ISynergy.Framework.Mathematics/      # Math utilities
├── ISynergy.Framework.Physics/          # Physics types & calculations
├── ISynergy.Framework.IO/              # I/O utilities
├── ISynergy.Framework.Printer.Label/   # Label printing
│   └── ISynergy.Framework.Printer.Label.Dymo/
├── ISynergy.Framework.Synchronization/ # Data synchronization
├── ISynergy.Framework.Automations/     # Automation engine
├── ISynergy.Framework.Monitoring/      # Real-time monitoring
│   └── ISynergy.Framework.Monitoring.Client/
└── ISynergy.Framework.Automations/     # Rule-based automation
```

## Data Access Pattern (for consuming applications)

**NO explicit Repository interfaces** — Consuming apps use EF Core primitives directly on DataContext via named DbSet properties. The framework provides `ISynergy.Framework.EntityFramework` with base `DataContext` class and audit/soft-delete support.

## Key Framework Patterns

### Dependency Injection Registration
Projects that have services to wire expose an extension method on `IServiceCollection` (or the platform builder) for simple consumer wiring:
```csharp
builder.Services.AddCQRS();
builder.Services.AddAutomationServices();
builder.Services.AddStorage(/* config */);
```

Registration helpers are **not** uniform and **not** universal. Packages such as `AspNetCore.*`,
`Automations`, `CQRS`, `Documents.Syncfusion`, `EventSourcing.EntityFramework`, `EventSourcing.Storage*`,
`KeyVault.Azure`, `KeyVault.OpenBao`, `Mail.*`, `MessageBus.*`, `Monitoring.Client`, `Printer.Label.Dymo`,
`Storage.Azure`, `Storage.S3`, and the UI platform packages expose `Add*`/`Configure*`/`Register*`
extensions; the `OpenTelemetry.*` packages extend `TelemetryBuilder` rather than `IServiceCollection`.
Pure abstraction/base libraries — `Core`, `Globalization`, `EntityFramework`, `EventSourcing`,
`Financial`, `Geography`, `IO`, `Mathematics`, `Monitoring`, `Mvvm`, `Physics`, `Storage` — expose none
and are consumed as types by the projects that reference them.

### AOT / Trim Readiness
Reflection-heavy public APIs carry `[RequiresUnreferencedCode]`, `[RequiresDynamicCode]`, and
`[DynamicallyAccessedMembers]` annotations, and source generators replace some reflection-based
registration. **Trim analysis is enabled only for the platform UI packages** (`UI.WPF`, `UI.WinUI`,
`UI.Maui`, via `<IsTrimmable>`/`<EnableTrimAnalyzer>`); **no project declares Native AOT
compatibility** — `IsAotCompatible`/`PublishAot` are unset everywhere. Treat AOT/trim readiness as a
direction the code is being annotated toward, not a guarantee for every package.

### Multi-Platform UI
The MVVM layer (`ISynergy.Framework.Mvvm`) is platform-agnostic. Platform-specific UI projects (`UI.WinUI`, `UI.WPF`, `UI.Maui`, `UI.Blazor`) provide controls and services that bind to the same ViewModel base classes.

## Package Dependencies

```
Core (no framework dependencies)
├── Globalization → Core
├── CQRS → Core
├── Mvvm → Core
├── EntityFramework → Core
├── EventSourcing → Core
│   └── EventSourcing.EntityFramework → Core, EventSourcing
│       └── EventSourcing.Storage → EventSourcing, EventSourcing.EntityFramework
│           ├── EventSourcing.Storage.Azure → EventSourcing.Storage
│           └── EventSourcing.Storage.S3 → EventSourcing.Storage
├── OpenTelemetry → Core
│   ├── OpenTelemetry.ApplicationInsights → OpenTelemetry
│   └── OpenTelemetry.Sentry → OpenTelemetry
├── MessageBus → Core
│   ├── MessageBus.Azure → MessageBus
│   └── MessageBus.RabbitMQ → MessageBus
├── Storage → Core
│   ├── Storage.Azure → Storage
│   └── Storage.S3 → Storage
├── Mail → Core
│   ├── Mail.Microsoft365 → Mail
│   └── Mail.SendGrid → Mail
├── KeyVault → Core
│   ├── KeyVault.Azure → KeyVault
│   └── KeyVault.OpenBao → KeyVault
├── AspNetCore → Core, OpenTelemetry, Storage
│   ├── AspNetCore.Authentication → AspNetCore
│   ├── AspNetCore.Blazor → AspNetCore, Mvvm, Globalization
│   ├── AspNetCore.Globalization → AspNetCore, Core, Globalization
│   ├── AspNetCore.Monitoring → AspNetCore.Authentication, MessageBus, Monitoring
│   └── AspNetCore.MultiTenancy → AspNetCore.Authentication
├── UI → Mvvm, OpenTelemetry, Globalization
│   ├── UI.Maui → UI, Mvvm
│   ├── UI.WinUI → UI
│   ├── UI.WPF → UI
│   └── UI.Blazor → UI
├── Monitoring (no framework refs)
│   └── Monitoring.Client → Monitoring, Mvvm
├── Documents (no framework refs)
│   └── Documents.Syncfusion → Core, Documents, Globalization
├── Printer.Label (no framework refs)
│   └── Printer.Label.Dymo → Printer.Label
└── Leaf modules → Core
    Automations, Financial, IO, Mathematics, Physics → Core;  Geography → Core, Globalization
```

## Deployment Architecture

- **Library packaging:** NuGet packages built via `dotnet pack`, published to nuget.org
- **CI/CD:** GitHub Actions (`.github/workflows/ci.yml`) — build, test, pack, sign, publish to NuGet.org, tag
- **Versioning:** Date-based (`yyyy.1MMdd.1HHmm`, computed by `scripts/Set-VersionVariables.ps1`; PR builds append `-preview`)
- **Strong naming:** All assemblies strong-named with SNK file
- **Signing:** Release builds run analyzers; Debug builds skip them for build speed

## Known Limitations

1. **Framework, not an application** — The `.ai/project/domains.md` and `.ai/reference/critical-rules.md` entity/domain patterns are designed for consuming applications, not this repo itself
2. **Multi-platform UI divergence** — WinUI uses StoreContext for updates, WPF uses HTTP; these are genuinely different mechanisms that cannot be unified (documented in session-context.md)
3. **Shared Globalization types physically live in Core** — to keep `Core` dependency-free, the shared types `ILanguageService` (namespace `ISynergy.Framework.Globalization.Abstractions.Services`), `GlobalizationConstants`, and the `DateTimeConverter`/`DateTimeOffsetConverter`/`TimeOnlyConverter` JSON converters were moved into the `ISynergy.Framework.Core` assembly while keeping their `ISynergy.Framework.Globalization.*` namespaces. `ISynergy.Framework.Globalization` now references Core and type-forwards them (`TypeForwarding.cs`) for binary compatibility; the concrete `LanguageService` (which owns the resource satellite assemblies) stays in Globalization and registers itself with Core's `LanguageServiceProvider` when constructed. Consumers that previously received Globalization transitively through Core must now reference Globalization explicitly.

---

**Remember:** This file documents the framework architecture. For technology choices, see [tech-stack.md](tech-stack.md). For project preferences, see [preferences.md](preferences.md).
