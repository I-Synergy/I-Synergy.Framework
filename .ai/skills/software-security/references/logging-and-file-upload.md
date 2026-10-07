# Security Logging & File Upload Security
What to log and never log, and validation for uploaded files. Load this when adding security logging or accepting file uploads.

## Security Logging and Monitoring

### Security Event Logging

```csharp
public sealed class SecurityAuditLogger
{
    private readonly ILogger<SecurityAuditLogger> _logger;

    public SecurityAuditLogger(ILogger<SecurityAuditLogger> logger)
    {
        _logger = logger;
    }

    public void LogAuthenticationSuccess(Guid userId, string ipAddress)
    {
        _logger.LogInformation(
            "Authentication successful. UserId: {UserId}, IP: {IpAddress}",
            userId, ipAddress);
    }

    public void LogAuthenticationFailure(string email, string ipAddress, string reason)
    {
        _logger.LogWarning(
            "Authentication failed. Email: {Email}, IP: {IpAddress}, Reason: {Reason}",
            email, ipAddress, reason);
    }

    public void LogAuthorizationFailure(Guid userId, string resource, string action)
    {
        _logger.LogWarning(
            "Authorization denied. UserId: {UserId}, Resource: {Resource}, Action: {Action}",
            userId, resource, action);
    }

    public void LogPasswordChange(Guid userId)
    {
        _logger.LogInformation(
            "Password changed. UserId: {UserId}",
            userId);
    }

    public void LogSuspiciousActivity(Guid userId, string activity, string details)
    {
        _logger.LogWarning(
            "Suspicious activity detected. UserId: {UserId}, Activity: {Activity}, Details: {Details}",
            userId, activity, details);
    }
}
```

---

## File Upload Security

```csharp
/// <summary>
/// Securely handles file uploads.
/// </summary>
public sealed class SecureFileUploadService
{
    private readonly ILogger<SecureFileUploadService> _logger;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public SecureFileUploadService(ILogger<SecureFileUploadService> logger)
    {
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        // Validate file exists
        if (file == null || file.Length == 0)
            throw new ValidationException("File is required");

        // Validate file size
        if (file.Length > MaxFileSize)
            throw new ValidationException($"File size exceeds maximum of {MaxFileSize / 1024 / 1024} MB");

        // Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new ValidationException($"File type {extension} is not allowed");

        // Sanitize file name
        var sanitizedFileName = InputSanitizer.SanitizeFileName(file.FileName);

        // Generate unique file name to prevent overwriting
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        // Validate file content (magic bytes)
        using var stream = file.OpenReadStream();
        if (!IsValidFileContent(stream, extension))
            throw new ValidationException("File content does not match extension");

        // Save file to secure location
        var uploadPath = Path.Combine("uploads", uniqueFileName);
        var fullPath = Path.GetFullPath(uploadPath);

        // Prevent path traversal
        if (!fullPath.StartsWith(Path.GetFullPath("uploads")))
            throw new SecurityException("Invalid file path");

        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(fileStream, cancellationToken);

        _logger.LogInformation(
            "File uploaded successfully: {FileName} ({Size} bytes)",
            uniqueFileName, file.Length);

        return uniqueFileName;
    }

    private static bool IsValidFileContent(Stream stream, string extension)
    {
        // Check magic bytes (file signature)
        var buffer = new byte[8];
        stream.Read(buffer, 0, 8);
        stream.Position = 0;

        return extension switch
        {
            ".jpg" or ".jpeg" => buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF,
            ".png" => buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47,
            ".pdf" => buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46,
            _ => false
        };
    }
}
```

---
