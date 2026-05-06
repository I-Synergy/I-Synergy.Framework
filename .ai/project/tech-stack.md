# Technology Stack (CUSTOMIZE THIS)

**Instructions:** Replace placeholders with your actual technology choices.

**Purpose:** This file defines WHAT technologies you use - concrete versions, frameworks, and libraries.

## Runtime & Language

- **.NET SDK:** [Version - e.g., 10.0.101]
- **Language:** [C# version - e.g., C# 14]
- **Nullable Reference Types:** [Enabled / Disabled]
- **Implicit Usings:** [Enabled / Disabled]

## Frameworks & Libraries

| Category | Technology | Version | Notes |
|----------|-----------|---------|-------|
| **Orchestration** | [e.g., .NET Aspire / Docker Compose] | [Version] | [Purpose/notes] |
| **Database** | [e.g., PostgreSQL / SQL Server] | [Version] | [Cloud/local details] |
| **ORM** | [e.g., EF Core / Dapper] | [Version] | [Migration approach] |
| **Caching** | [e.g., Redis / Memory Cache] | [Version] | [Distributed/local] |
| **CQRS** | [e.g., I-Synergy.Framework.CQRS / MediatR] | [Version] | [Why chosen] |
| **Mapping** | [Your choice] | [Version] | [Approach] |
| **Logging** | [e.g., ILogger / Serilog] | [Version] | [Sink configuration] |
| **Testing** | [e.g., MSTest / xUnit] | [Version] | [Framework choice] |
| **Mocking** | [e.g., Moq / NSubstitute] | [Version] | [Mocking approach] |
| **BDD** | [e.g., Reqnroll / SpecFlow] | [Version] | [Gherkin usage] |
| **API Docs** | [e.g., Microsoft.AspNetCore.OpenApi / Swashbuckle] | [Version] | [Documentation approach] |
| **Resilience** | [e.g., Microsoft.Extensions.Resilience / Polly] | [Version] | [Retry/circuit breaker] |
| **Auth** | [e.g., OpenIddict / IdentityServer] | [Version] | [OAuth2/OpenID] |
| **Validation** | [e.g., Data Annotations / FluentValidation] | [Version] | [Validation approach] |

## UI Layer (if applicable)

- **Web UI:** [e.g., Blazor Server / Blazor WebAssembly / React]
- **Desktop/Mobile:** [e.g., MAUI / WPF / None]
- **MVVM Framework:** [e.g., I-Synergy.Framework.Mvvm / CommunityToolkit.Mvvm]
- **Component Library:** [e.g., Microsoft FluentUI / MudBlazor / Custom]

## Data Layer

- **Primary Database:** [e.g., PostgreSQL 15+ / SQL Server 2022]
- **ORM:** [e.g., Entity Framework Core 10 with Npgsql]
- **Caching:** [e.g., Azure Managed Redis / Local Redis]
- **Secrets:** [e.g., Azure Key Vault / AWS Secrets Manager / Environment Variables]
- **Data Sync:** [e.g., Dotmim.Sync / Custom] (if applicable)
- **Offline Storage:** [e.g., SQLite / None] (if applicable)

## Authentication & Authorization

- **OAuth2/OpenID:** [e.g., OpenIddict 7.x / Auth0 / Okta]
- **JWT:** [e.g., Microsoft.AspNetCore.Authentication.JwtBearer]
- **Token Management:** [e.g., Microsoft.IdentityModel.JsonWebTokens]
- **User Store:** [e.g., EF Core Identity / Custom / External]

## Observability

- **Monitoring:** [e.g., Application Insights / Prometheus / Datadog]
- **Distributed Tracing:** [e.g., OpenTelemetry / Jaeger]
- **Logging:** [e.g., I-Synergy.Framework.OpenTelemetry / Serilog + Seq]
- **Health Checks:** [e.g., Microsoft.Extensions.Diagnostics.HealthChecks]

## Package Management

- **Version Control:** [Central Package Management / Individual project versions]
- **Package Source:** [nuget.org / Private feed / Both]
- **Version Strategy:** [Pinned versions / Latest stable / Floating]

## Forbidden Technologies (Customize for Your Project)

| ❌ DO NOT USE | ✅ USE INSTEAD | Reason |
|---------------|---------------|---------|
| [Technology] | [Replacement] | [Why forbidden] |
| [Technology] | [Replacement] | [Why forbidden] |

**Example:**
| ❌ DO NOT USE | ✅ USE INSTEAD | Reason |
|---------------|---------------|---------|
| MediatR | I-Synergy.Framework.CQRS | Project standard, better performance |
| xUnit | MSTest | Project standard, team familiarity |

## Cloud Services (if applicable)

- **Cloud Provider:** [Azure / AWS / GCP / On-premises]
- **Hosting:** [Azure App Service / AWS ECS / Kubernetes]
- **Database:** [Azure Flexible Server / AWS RDS / Self-hosted]
- **Storage:** [Azure Blob Storage / AWS S3 / File System]
- **Key Vault:** [Azure Key Vault / AWS Secrets Manager / HashiCorp Vault]

## Development Tools

- **IDE:** [Visual Studio / Rider / VS Code]
- **Version Control:** [Git / Other]
- **CI/CD:** [Azure DevOps / GitHub Actions / GitLab CI]
- **Container Runtime:** [Docker / Podman]
- **Local Development:** [.NET Aspire / Docker Compose / None]

---

**Remember:** This file documents your actual technology stack. Update it when technologies change. For architectural patterns, see [architecture.md](architecture.md). For workflow preferences, see [preferences.md](preferences.md).
