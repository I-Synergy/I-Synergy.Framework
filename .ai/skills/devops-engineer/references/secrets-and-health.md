# Pipeline Secrets & Health Checks
Secret handling in pipelines and health-check endpoints for liveness and readiness probes. Load this when wiring secrets into CI/CD or adding a health endpoint.

## Secrets Management in Pipelines

### Azure Pipelines Variable Groups
```yaml
# Reference variable group
variables:
  - group: '{ApplicationName}-Secrets'

# Use secrets
steps:
  - task: AzureCLI@2
    inputs:
      scriptType: 'bash'
      scriptLocation: 'inlineScript'
      inlineScript: |
        echo "Connection String: $(DbConnectionString)"
    env:
      DB_CONNECTION: $(DbConnectionString)
```

### GitHub Actions Secrets
```yaml
steps:
  - name: Deploy
    env:
      DB_CONNECTION: ${{ secrets.DB_CONNECTION_STRING }}
      API_KEY: ${{ secrets.EXTERNAL_API_KEY }}
    run: |
      echo "Deploying with secrets"
```

## Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!)
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

