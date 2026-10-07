# Security Compliance
Compliance frameworks and control mapping (GDPR, SOC 2, ISO 27001, HIPAA). Load this when a feature must satisfy a regulatory control.

### GDPR (General Data Protection Regulation)

**Requirements:**
- Right to access
- Right to erasure (right to be forgotten)
- Data portability
- Consent management
- Data breach notification

**Implementation:**
```csharp
// File: {ApplicationName}.Services.API/Controllers/PrivacyController.cs

/// <summary>
/// GDPR compliance endpoints.
/// </summary>
[ApiController]
[Route("api/privacy")]
public sealed class PrivacyController : ControllerBase
{
    private readonly ICommandHandler<ExportUserDataCommand, ExportUserDataResponse> _exportHandler;
    private readonly ICommandHandler<DeleteUserDataCommand, DeleteUserDataResponse> _deleteHandler;

    /// <summary>
    /// Right to access - export user data.
    /// </summary>
    [HttpGet("export")]
    [Authorize]
    public async Task<IActionResult> ExportUserData(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var command = new ExportUserDataCommand(Guid.Parse(userId));
        var result = await _exportHandler.HandleAsync(command, cancellationToken);

        return File(
            result.Data,
            "application/json",
            $"user-data-{userId}.json");
    }

    /// <summary>
    /// Right to be forgotten - delete user data.
    /// </summary>
    [HttpDelete("delete-account")]
    [Authorize]
    public async Task<IActionResult> DeleteUserData(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var command = new DeleteUserDataCommand(Guid.Parse(userId));
        await _deleteHandler.HandleAsync(command, cancellationToken);

        return NoContent();
    }
}
```

### SOC 2 (Service Organization Control 2)

**Trust Service Criteria:**
- Security
- Availability
- Processing Integrity
- Confidentiality
- Privacy

**Implementation:**
```csharp
// Audit logging for SOC 2 compliance

public sealed class AuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public void LogDataAccess(Guid userId, string resourceType, Guid resourceId, string action)
    {
        _logger.LogInformation(
            "[AUDIT] User {UserId} performed {Action} on {ResourceType} {ResourceId}",
            userId, action, resourceType, resourceId);
    }

    public void LogConfigurationChange(Guid userId, string configKey, string oldValue, string newValue)
    {
        _logger.LogWarning(
            "[AUDIT] User {UserId} changed configuration {ConfigKey} from {OldValue} to {NewValue}",
            userId, configKey, MaskSensitive(oldValue), MaskSensitive(newValue));
    }

    public void LogSecurityEvent(string eventType, string details)
    {
        _logger.LogWarning(
            "[AUDIT] Security event: {EventType} - {Details}",
            eventType, details);
    }

    private static string MaskSensitive(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length <= 4)
            return "****";

        return $"{value[..2]}****{value[^2..]}";
    }
}
```

### ISO 27001 (Information Security Management)

**Requirements:**
- Information security policy
- Risk assessment
- Asset management
- Access control
- Cryptography
- Physical security
- Operations security
- Communications security
- System acquisition and maintenance
- Supplier relationships
- Incident management
- Business continuity

### HIPAA (Health Insurance Portability and Accountability Act)

**For healthcare applications:**

```csharp
// PHI (Protected Health Information) encryption

public sealed class PHIEncryptionService
{
    private readonly byte[] _key;

    public PHIEncryptionService(IConfiguration configuration)
    {
        _key = Convert.FromBase64String(configuration["Encryption:PHIKey"]!);
    }

    public string EncryptPHI(string plaintext)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plaintext);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string DecryptPHI(string ciphertext)
    {
        var buffer = Convert.FromBase64String(ciphertext);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[aes.IV.Length];
        Buffer.BlockCopy(buffer, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
```

---

