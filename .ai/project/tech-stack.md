# Technology Stack — I-Synergy Framework

## Runtime & Language

- **.NET SDK:** 10.0.300 (`global.json`)
- **Language:** C# 14
- **Nullable Reference Types:** Enabled (global in `Directory.Build.props`)
- **Implicit Usings:** Enabled (global in `Directory.Build.props`)

## Frameworks & Libraries

| Category | Technology | Version | Notes |
|----------|-----------|---------|-------|
| **CQRS** | I-Synergy.Framework.CQRS | In-repo | Custom CQRS with ICommand/IQuery, source-generator friendly |
| **ORM** | Entity Framework Core 10 | 10.0.8 | Code-first; no explicit Repository pattern — DataContext primitives |
| **Mapping** | Manual | — | Explicit projections; no AutoMapper |
| **Logging** | Microsoft.Extensions.Logging (ILogger) | 10.0.8 | Structured logging with correlation IDs |
| **Testing** | MSTest | 4.2.3 | MSTest.TestAdapter + MSTest.TestFramework |
| **Mocking** | Moq | 4.20.72 | For unit test mocking |
| **BDD** | Reqnroll | 3.3.4 | Open-source SpecFlow fork |
| **API Docs** | NSwag.AspNetCore | 14.7.1 | OpenAPI doc generation + Swagger UI |
| **Resilience** | Microsoft.Extensions.Http.Resilience | 10.6.0 | Standard resilience handler (retry, circuit breaker) |
| **Auth** | JwtBearer + Microsoft.Identity.Client | 10.0.5 / 4.84.1 | OAuth2/OpenID with token management |
| **Validation** | Data Annotations + IValidatableObject | Built-in | No FluentValidation |
| **Serialization** | System.Text.Json + MessagePack | 3.1.6 (MsgPack) | AOT-compatible serialization; MessagePack for performance |
| **Source Gen** | Microsoft.CodeAnalysis.CSharp | 5.3.0 | Source generators for zero-reflection registration |

## UI Layer

| Platform | Technology | Version | Notes |
|----------|-----------|---------|-------|
| **Web UI** | Blazor (Server + WebAssembly) + FluentUI | 10.0.8 / 4.14.2 | Microsoft.FluentUI.AspNetCore.Components |
| **Desktop (Windows)** | WPF + WinUI 3 | — | WindowsAppSDK 2.1.3 |
| **Mobile** | MAUI | 10.0.70 | CommunityToolkit.Maui.MediaElement |
| **MVVM** | I-Synergy.Framework.Mvvm | In-repo | Custom MVVM framework (base classes, ViewModels, services) |

## Data Layer

| Technology | Version | Notes |
|-----------|---------|-------|
| **SQL Server (EF Core)** | 10.0.5 | Primary relational database |
| **PostgreSQL (EF Core)** | 10.0.2 | Via Npgsql.EntityFrameworkCore.PostgreSQL |
| **SQLite** | — | Via Dotmim.Sync.Sqlite for offline/sync scenarios |
| **Azure Blob Storage** | 12.28.0 | File/document storage |
| **AWS S3** | 4.0.23.4 | Alternative cloud storage |
| **Data Sync** | 1.3.0 | Dotmim.Sync — bidirectional sync (PostgreSQL, SQLite, Web) |

## Authentication & Authorization

| Technology | Version | Notes |
|-----------|---------|-------|
| **OAuth2/OpenID** | — | JwtBearer + Microsoft.Identity.Client |
| **Multi-Tenancy** | In-repo | ISynergy.Framework.AspNetCore.MultiTenancy |
| **Key Vault (Azure)** | 4.11.0 | Azure.Security.KeyVault.Secrets |
| **Key Vault (OpenBao)** | 1.17.5.1 | VaultSharp-based alternative |

## Messaging & Events

| Technology | Version | Notes |
|-----------|---------|-------|
| **Azure Service Bus** | 7.20.1 | Cloud message bus |
| **RabbitMQ** | 7.2.1 | Self-hosted message bus |
| **SignalR** | 10.0.8 | Real-time monitoring & client comms |
| **Event Sourcing** | In-repo | Custom event store with EF Core, Azure, S3 backends |

## Observability

| Technology | Version | Notes |
|-----------|---------|-------|
| **OpenTelemetry** | 1.15.3 | Tracing + metrics (Console, OTLP exporters) |
| **Application Insights** | 1.8.1 | Azure Monitor exporter + Microsoft.ApplicationInsights 3.1.1 |
| **Sentry** | 6.6.0 | Error monitoring via Sentry.OpenTelemetry |
| **Health Checks** | Built-in | ASP.NET Core health checks |

## Package Management

- **Version control:** Central Package Management (`Directory.Packages.props`)
- **Package source:** nuget.org (private publish for framework packages)
- **Version strategy:** Pinned versions (all versions verified on nuget.org)

## Cloud Services

| Service | Technology | Notes |
|---------|-----------|-------|
| **Cloud Provider** | Azure (primary) + AWS (secondary) | Multi-cloud for storage/messaging |
| **Compute** | Azure App Service / Container Apps | — |
| **Storage** | Azure Blob Storage / AWS S3 | Abstracted via ISynergy.Framework.Storage |
| **Messaging** | Azure Service Bus / RabbitMQ | Abstracted via ISynergy.Framework.MessageBus |
| **Key Vault** | Azure Key Vault / OpenBao | Abstracted via ISynergy.Framework.KeyVault |
| **Monitoring** | Azure Monitor + Sentry | OpenTelemetry-based |

## Development Tools

| Tool | Details |
|------|---------|
| **IDE** | Visual Studio 2022+ / JetBrains Rider |
| **Version Control** | Git (GitHub: I-Synergy/I-Synergy.Framework) |
| **CI/CD** | Azure Pipelines (`azure-pipelines.yml`) |
| **Performance** | BenchmarkDotNet (0.15.8) — in `performance/` projects |
| **AOT/Trim** | All libraries annotated for Native AOT + trimmed deployments |

## Forbidden Technologies

| ❌ DO NOT USE | ✅ USE INSTEAD | Reason |
|---------------|---------------|--------|
| MediatR | I-Synergy.Framework.CQRS | In-repo CQRS framework, better perf, source-gen friendly |
| xUnit / NUnit | MSTest | Project standard, consistent framework |
| Swashbuckle | NSwag.AspNetCore | Modern Microsoft-ecosystem approach |
| FluentValidation | Data Annotations + IValidatableObject | Built-in, no external dependency |
| Standalone Polly | Microsoft.Extensions.Http.Resilience | Microsoft-integrated resilience library |
| Serilog | ILogger + OpenTelemetry | Project logging abstraction |
| SpecFlow | Reqnroll | SpecFlow requires license; Reqnroll is OSS fork |
| AutoMapper | Manual mapping | Explicit, debuggable, no reflection overhead |

---

**Remember:** This file documents the actual technology stack. Update it when technologies change. For architecture, see [architecture.md](architecture.md). For workflow preferences, see [preferences.md](preferences.md).
