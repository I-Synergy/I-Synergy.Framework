# Secrets Management & Dependency Scanning
Secret storage and rotation, plus scanning and updating vulnerable dependencies. Load this when handling credentials or auditing packages.

## Secrets Management

### Azure Key Vault Integration

```csharp
// File: {ApplicationName}.Services.API/Program.cs

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUrl = builder.Configuration["KeyVault:Url"];
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl!),
        new DefaultAzureCredential());
}

// Access secrets
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
var apiKey = builder.Configuration["ExternalService:ApiKey"];

var app = builder.Build();
app.Run();
```

### Secret Rotation

```csharp
/// <summary>
/// Service for rotating secrets.
/// </summary>
public sealed class SecretRotationService
{
    private readonly SecretClient _secretClient;
    private readonly ILogger<SecretRotationService> _logger;

    public SecretRotationService(
        SecretClient secretClient,
        ILogger<SecretRotationService> logger)
    {
        _secretClient = secretClient;
        _logger = logger;
    }

    /// <summary>
    /// Rotates a secret in Azure Key Vault.
    /// </summary>
    public async Task RotateSecretAsync(
        string secretName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rotating secret: {SecretName}", secretName);

        // Generate new secret value
        var newSecretValue = GenerateSecretValue();

        // Store new secret in Key Vault
        await _secretClient.SetSecretAsync(
            secretName,
            newSecretValue,
            cancellationToken);

        _logger.LogInformation(
            "Secret rotated successfully: {SecretName}",
            secretName);
    }

    private static string GenerateSecretValue()
    {
        // Generate cryptographically secure random value
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
```

---

## Dependency Scanning

### NuGet Package Vulnerability Scanning

```xml
<!-- File: Directory.Build.props -->

<Project>
  <PropertyGroup>
    <!-- Enable NuGet audit -->
    <NuGetAudit>true</NuGetAudit>
    <NuGetAuditMode>all</NuGetAuditMode>
    <NuGetAuditLevel>low</NuGetAuditLevel>
  </PropertyGroup>
</Project>
```

### GitHub Dependabot Configuration

```yaml
# File: .github/dependabot.yml

version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 10
    reviewers:
      - "security-team"
    labels:
      - "dependencies"
      - "security"
```

---

