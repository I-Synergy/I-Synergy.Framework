# Library Modules Catalog

> **Note:** This repository is a reusable framework library (`ISynergy.Framework.*`), not an application with business domains. This file catalogs the framework's library modules (src/ projects) instead of business domains.

## Module Overview

**Total Modules:** 50+ NuGet-packageable libraries under `src/`

**Organization Strategy:** Modular monorepo organized by capability — each `src/ISynergy.Framework.{Module}/` project is a self-contained NuGet package with minimal dependencies.

## Module List

### Core & Architecture

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **Core** | `I-Synergy.Framework.Core` | Base classes, abstractions, utilities, messaging, extension methods | Active |
| **CQRS** | `I-Synergy.Framework.CQRS` | Command Query Responsibility Segregation — ICommand, IQuery, dispatcher, handlers | Active |
| **Mvvm** | `I-Synergy.Framework.Mvvm` | MVVM pattern — ViewModel base classes, messaging, selection, wizard patterns | Active |
| **Automations** | `I-Synergy.Framework.Automations` | Rule-based automation engine — triggers, actions, conditions, schedules | Active |
| **Synchronization** | `I-Synergy.Framework.Synchronization` | Data synchronization abstractions for offline/online scenarios | Active |

### Data & Persistence

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **EntityFramework** | `I-Synergy.Framework.EntityFramework` | EF Core DataContext base, audit fields, soft delete, query helpers | Active |
| **EventSourcing** | `I-Synergy.Framework.EventSourcing` | Event store abstractions, aggregate roots, event replay | Active |
| **EventSourcing.EntityFramework** | `I-Synergy.Framework.EventSourcing.EntityFramework` | EF Core-backed event store | Active |
| **EventSourcing.Storage** | `I-Synergy.Framework.EventSourcing.Storage` | Storage-backed event store abstractions | Active |
| **EventSourcing.Storage.Azure** | `I-Synergy.Framework.EventSourcing.Storage.Azure` | Azure Blob-backed event store | Active |
| **EventSourcing.Storage.S3** | `I-Synergy.Framework.EventSourcing.Storage.S3` | AWS S3-backed event store | Active |
| **Storage** | `I-Synergy.Framework.Storage` | Cloud storage abstractions (upload, download, CRUD) | Active |
| **Storage.Azure** | `I-Synergy.Framework.Storage.Azure` | Azure Blob Storage implementation | Active |
| **Storage.S3** | `I-Synergy.Framework.Storage.S3` | AWS S3 implementation | Active |

### Communication & Messaging

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **MessageBus** | `I-Synergy.Framework.MessageBus` | Pub/sub message bus abstractions | Active |
| **MessageBus.Azure** | `I-Synergy.Framework.MessageBus.Azure` | Azure Service Bus implementation | Active |
| **MessageBus.RabbitMQ** | `I-Synergy.Framework.MessageBus.RabbitMQ` | RabbitMQ implementation | Active |
| **Mail** | `I-Synergy.Framework.Mail` | Email sending abstractions | Active |
| **Mail.Microsoft365** | `I-Synergy.Framework.Mail.Microsoft365` | Microsoft Graph / Microsoft 365 mail implementation | Active |
| **Mail.SendGrid** | `I-Synergy.Framework.Mail.SendGrid` | SendGrid mail implementation | Active |
| **Monitoring** | `I-Synergy.Framework.Monitoring` | Real-time monitoring via SignalR hubs | Active |
| **Monitoring.Client** | `I-Synergy.Framework.Monitoring.Client` | SignalR client for monitoring consumers | Active |

### Web & API Development

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **AspNetCore** | `I-Synergy.Framework.AspNetCore` | ASP.NET Core middleware, filters, extensions, health checks | Active |
| **AspNetCore.Authentication** | `I-Synergy.Framework.AspNetCore.Authentication` | JWT/OAuth2 authentication middleware | Active |
| **AspNetCore.Blazor** | `I-Synergy.Framework.AspNetCore.Blazor` | Blazor Server/WebAssembly integration components | Active |
| **AspNetCore.Globalization** | `I-Synergy.Framework.AspNetCore.Globalization` | Multi-language localization middleware | Active |
| **AspNetCore.Monitoring** | `I-Synergy.Framework.AspNetCore.Monitoring` | ASP.NET monitoring integration (metrics, health) | Active |
| **AspNetCore.MultiTenancy** | `I-Synergy.Framework.AspNetCore.MultiTenancy` | Multi-tenant resolution and data isolation | Active |

### UI & Client Frameworks

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **UI** | `I-Synergy.Framework.UI` | Shared UI base — dialogs, navigation, theming, converters | Active |
| **UI.Maui** | `I-Synergy.Framework.UI.Maui` | MAUI controls, services, platform integration | Active |
| **UI.WinUI** | `I-Synergy.Framework.UI.WinUI` | WinUI 3 controls, services, platform integration | Active |
| **UI.WPF** | `I-Synergy.Framework.UI.WPF` | WPF controls, services, platform integration | Active |
| **UI.Blazor** | `I-Synergy.Framework.UI.Blazor` | Blazor component library | Active |

### Key Vault & Secrets

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **KeyVault** | `I-Synergy.Framework.KeyVault` | Secrets management abstractions | Active |
| **KeyVault.Azure** | `I-Synergy.Framework.KeyVault.Azure` | Azure Key Vault implementation | Active |
| **KeyVault.OpenBao** | `I-Synergy.Framework.KeyVault.OpenBao` | OpenBao (Vault) implementation | Active |

### Monitoring & Observability

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **OpenTelemetry** | `I-Synergy.Framework.OpenTelemetry` | OpenTelemetry configuration, tracing, metrics | Active |
| **OpenTelemetry.ApplicationInsights** | `I-Synergy.Framework.OpenTelemetry.ApplicationInsights` | Azure Application Insights exporter | Active |
| **OpenTelemetry.Sentry** | `I-Synergy.Framework.OpenTelemetry.Sentry` | Sentry error monitoring exporter | Active |

### Specialized Libraries

| Module | Package ID | Purpose | Status |
|--------|-----------|---------|--------|
| **Documents** | `I-Synergy.Framework.Documents` | Document processing abstractions | Active |
| **Documents.Syncfusion** | `I-Synergy.Framework.Documents.Syncfusion` | Syncfusion document processing (PDF, Excel, Word) | Active |
| **Financial** | `I-Synergy.Framework.Financial` | Financial calculations, currency, VAT | Active |
| **Geography** | `I-Synergy.Framework.Geography` | Geography types, coordinates, distance calculations | Active |
| **Mathematics** | `I-Synergy.Framework.Mathematics` | Mathematical utilities and extensions | Active |
| **Physics** | `I-Synergy.Framework.Physics` | Physics types, units, conversions | Active |
| **IO** | `I-Synergy.Framework.IO` | I/O utilities, watchers, file system helpers | Active |
| **Printer.Label** | `I-Synergy.Framework.Printer.Label` | Label printing abstractions | Active |
| **Printer.Label.Dymo** | `I-Synergy.Framework.Printer.Label.Dymo` | DYMO LabelWriter implementation | Active |

## Module Dependencies

All modules depend on `ISynergy.Framework.Core` either directly or transitively:

```
Core (zero NuGet deps — only Microsoft.Extensions.*)
├── CQRS, Mvvm, Automations, Synchronization
├── EntityFramework → Core
├── EventSourcing → Core, EntityFramework
├── MessageBus → Core
├── Storage → Core
├── Mail → Core
├── KeyVault → Core
├── OpenTelemetry → Core
├── UI → Core, Mvvm
└── AspNetCore → Core
    └── AspNetCore.* → AspNetCore
```

Leaf modules (Financial, Geography, Mathematics, Physics, IO, Printer.Label) depend only on Core.

## Cross-Module Concepts

| Concept | Modules | Approach | Location |
|---------|---------|----------|----------|
| **DataContext** | EntityFramework, EventSourcing.EntityFramework | Shared base class | `ISynergy.Framework.EntityFramework` |
| **CQRS ICommand/IQuery** | CQRS, Mvvm | ICommand/IQuery interfaces | `ISynergy.Framework.CQRS` |
| **ViewModel** | Mvvm, UI.* | ViewModel base classes | `ISynergy.Framework.Mvvm` |
| **Storage abstractions** | Storage, Storage.Azure, Storage.S3 | Strategy pattern | `ISynergy.Framework.Storage` |
| **Message bus** | MessageBus, MessageBus.Azure, MessageBus.RabbitMQ | Pub/sub abstractions | `ISynergy.Framework.MessageBus` |
| **Event store** | EventSourcing.* | Event sourcing with multiple backends | `ISynergy.Framework.EventSourcing` |

## Reference Implementation

**Primary Reference:** `ISynergy.Framework.Core` — The foundational module that all others build on. Demonstrates:
- Base class patterns (ObservableClass, ObservableValidatedClass)
- Service abstraction patterns (IBusyService, IMessageBus)
- Extension method patterns (IServiceCollection extensions)
- AOT/trim compatibility annotations
- Source generator integration

## Module Maturity

| Module | Maturity | Test Coverage | Notes |
|--------|----------|---------------|-------|
| Core | Mature | High | Most mature module, 30+ test files |
| CQRS | Mature | High | Well-tested with full test project |
| Mvvm | Mature | High | Extensive ViewModel hierarchy tested |
| EntityFramework | Mature | High | Core data access layer |
| EventSourcing | Growing | Medium | Multiple backend implementations |
| UI.* | Growing | Medium | Platform-specific split, ongoing refactoring |
| AspNetCore.* | Growing | Medium | Authentication & multi-tenancy tested |
| MessageBus | Mature | High | Azure Service Bus + RabbitMQ |
| OpenTelemetry | Growing | Medium | Multiple exporters |

## Ubiquitous Language

| Term | Definition | Usage |
|------|------------|-------|
| Module | A NuGet-packageable project under `src/` | Each `src/ISynergy.Framework.{Module}/` directory |
| Framework | The entire monorepo of modules | `I-Synergy Framework` on NuGet |
| Consumer | An application that uses framework NuGet packages | Sample projects, external customers |
| DataContext | EF Core DbContext subclass provided by EntityFramework module | Base class for consumer data access |

---

**Remember:** This is a framework library catalog, not a business domain list. For consuming-application domain patterns, see `critical-rules.md` and `cqrs-patterns.md`. For module architecture details, see [architecture.md](architecture.md).
