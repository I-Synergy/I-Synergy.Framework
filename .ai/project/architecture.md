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
Every framework project exposes an extension method on `IServiceCollection` for simple consumer wiring:
```csharp
builder.Services.AddCQRS();
builder.Services.AddOpenTelemetry();
builder.Services.AddStorage(/* config */);
```

### AOT / Trim Readiness
All libraries are annotated for Native AOT and trimmed deployments. Source generators replace reflection-based registration where possible.

### Multi-Platform UI
The MVVM layer (`ISynergy.Framework.Mvvm`) is platform-agnostic. Platform-specific UI projects (`UI.WinUI`, `UI.WPF`, `UI.Maui`, `UI.Blazor`) provide controls and services that bind to the same ViewModel base classes.

## Package Dependencies

```
Core (no dependencies)
├── CQRS → Core
├── Mvvm → Core
├── EntityFramework → Core
├── EventSourcing → Core
├── OpenTelemetry → Core
├── UI → Core, Mvvm
│   ├── UI.Maui → UI
│   ├── UI.WinUI → UI
│   ├── UI.WPF → UI
│   └── UI.Blazor → UI
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
├── AspNetCore → Core
│   ├── AspNetCore.Authentication → AspNetCore
│   ├── AspNetCore.Blazor → AspNetCore
│   ├── AspNetCore.Globalization → AspNetCore
│   ├── AspNetCore.Monitoring → AspNetCore
│   └── AspNetCore.MultiTenancy → AspNetCore
└── Various leaf modules → Core
```

## Deployment Architecture

- **Library packaging:** NuGet packages built via `dotnet pack`, published to nuget.org
- **CI/CD:** Azure Pipelines (`azure-pipelines.yml`) — build, test, pack, publish
- **Versioning:** Date-based (`$(Date:yyyy).1$(Date:MMdd).1$(Date:HHmm)`)
- **Strong naming:** All assemblies strong-named with SNK file
- **Signing:** Release builds run analyzers; Debug builds skip them for build speed

## Known Limitations

1. **Framework, not an application** — The `.ai/project/domains.md` and `.ai/reference/critical-rules.md` entity/domain patterns are designed for consuming applications, not this repo itself
2. **Multi-platform UI divergence** — WinUI uses StoreContext for updates, WPF uses HTTP; these are genuinely different mechanisms that cannot be unified (documented in session-context.md)

---

**Remember:** This file documents the framework architecture. For technology choices, see [tech-stack.md](tech-stack.md). For project preferences, see [preferences.md](preferences.md).
