# I-Synergy Framework

## Telemetry configuration (samples)

The samples (`Sample.Api`, `Sample.WPF`, `Sample.WinUI`, `Sample.Maui`) expose a `Telemetry`
section in their `appsettings.json`. The `Dsn` (Sentry) and `ConnectionString` (Azure Application
Insights) values are committed **blank** on purpose — never commit real telemetry credentials.

Supply your own at runtime by overriding configuration with environment variables or user-secrets,
for example:

```bash
# Environment variables (double underscore = section separator)
set Telemetry__Dsn=https://<key>@o<org>.ingest.sentry.io/<project>
set Telemetry__ConnectionString=InstrumentationKey=<key>;IngestionEndpoint=https://<region>.in.applicationinsights.azure.com/

# Or user-secrets, from the sample directory
dotnet user-secrets set "Telemetry:Dsn" "https://<key>@o<org>.ingest.sentry.io/<project>"
dotnet user-secrets set "Telemetry:ConnectionString" "InstrumentationKey=<key>;IngestionEndpoint=https://<region>.in.applicationinsights.azure.com/"
```

With empty values the samples run with console/OTLP telemetry only and do not export to Sentry or
Application Insights.
