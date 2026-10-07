# DevSecOps & Security Monitoring (SIEM)
Security gates in the delivery pipeline and SIEM detection, alerting and triage. Load this when embedding security into CI/CD or wiring monitoring.

## DevSecOps Implementation

### Security in CI/CD Pipeline

```yaml
# File: .github/workflows/security-scan.yml

name: Security Scan

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  schedule:
    - cron: '0 2 * * *' # Daily at 2 AM

jobs:
  dependency-scan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2

      - name: Setup .NET
        uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '10.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: NuGet Package Vulnerability Scan
        run: dotnet list package --vulnerable --include-transitive

      - name: Dependency Review
        uses: actions/dependency-review-action@v2

  sast-scan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2

      - name: Run Security Code Scan
        run: |
          dotnet tool install --global security-scan
          security-scan analyze .

      - name: Upload SARIF results
        uses: github/codeql-action/upload-sarif@v1
        with:
          sarif_file: results.sarif

  container-scan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2

      - name: Build Docker image
        run: docker build -t myapp:${{ github.sha }} .

      - name: Run Trivy vulnerability scanner
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: 'myapp:${{ github.sha }}'
          format: 'sarif'
          output: 'trivy-results.sarif'

      - name: Upload Trivy results
        uses: github/codeql-action/upload-sarif@v1
        with:
          sarif_file: 'trivy-results.sarif'

  secret-scan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
        with:
          fetch-depth: 0

      - name: TruffleHog Secret Scan
        uses: trufflesecurity/trufflehog@main
        with:
          path: ./
          base: main
          head: HEAD
```

---

## Security Monitoring and SIEM

### Azure Sentinel Configuration

```csharp
// File: infrastructure/sentinel.bicep

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: 'application-logs'
}

resource sentinel 'Microsoft.SecurityInsights/onboardingStates@2023-02-01' = {
  name: 'default'
  scope: logAnalytics
}

// Alert rules
resource suspiciousLoginAlert 'Microsoft.SecurityInsights/alertRules@2023-02-01' = {
  scope: logAnalytics
  name: 'SuspiciousLoginAttempts'
  kind: 'Scheduled'
  properties: {
    displayName: 'Multiple Failed Login Attempts'
    description: 'Detects multiple failed login attempts from same IP'
    severity: 'High'
    enabled: true
    query: '''
      SigninLogs
      | where ResultType != 0
      | summarize FailedAttempts = count() by IPAddress, bin(TimeGenerated, 5m)
      | where FailedAttempts >= 5
    '''
    queryFrequency: 'PT5M'
    queryPeriod: 'PT5M'
    triggerOperator: 'GreaterThan'
    triggerThreshold: 0
    suppressionDuration: 'PT1H'
    suppressionEnabled: false
  }
}
```

### Application-Level Security Monitoring

```csharp
/// <summary>
/// Monitors security events and anomalies.
/// </summary>
public sealed class SecurityMonitoringService
{
    private readonly ILogger<SecurityMonitoringService> _logger;
    private readonly SecurityIncidentLogger _incidentLogger;

    public async Task MonitorAuthenticationAsync(
        string email,
        string ipAddress,
        bool success,
        CancellationToken cancellationToken = default)
    {
        if (!success)
        {
            // Check for brute force attack
            var recentFailures = await GetRecentFailedAttemptsAsync(
                ipAddress,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            if (recentFailures >= 5)
            {
                await _incidentLogger.LogIncidentAsync(
                    SecurityIncidentType.UnauthorizedAccess,
                    $"Multiple failed login attempts from IP: {ipAddress}",
                    SecurityIncidentSeverity.High,
                    cancellationToken);

                // Block IP address
                await BlockIpAddressAsync(ipAddress, cancellationToken);
            }
        }
    }

    public async Task MonitorDataAccessAsync(
        Guid userId,
        string resourceType,
        int recordsAccessed,
        CancellationToken cancellationToken = default)
    {
        // Detect unusual data access patterns
        if (recordsAccessed > 1000)
        {
            await _incidentLogger.LogIncidentAsync(
                SecurityIncidentType.SuspiciousActivity,
                $"User {userId} accessed {recordsAccessed} {resourceType} records",
                SecurityIncidentSeverity.Medium,
                cancellationToken);
        }
    }
}
```

---

