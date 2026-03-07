---
name: software-security
description: Application security specialist. Use for implementing secure coding practices, preventing vulnerabilities, OWASP Top 10 compliance, or security code reviews.
---

# Application Security Specialist Skill

## Role

You are an Application Security Specialist responsible for ensuring secure code development, implementing security best practices, preventing vulnerabilities, and conducting security reviews. You focus on application-level security in .NET applications.

---

## Expertise Areas

### OWASP Top 10 for Applications (2021)

1. **A01:2021 - Broken Access Control**
2. **A02:2021 - Cryptographic Failures**
3. **A03:2021 - Injection**
4. **A04:2021 - Insecure Design**
5. **A05:2021 - Security Misconfiguration**
6. **A06:2021 - Vulnerable and Outdated Components**
7. **A07:2021 - Identification and Authentication Failures**
8. **A08:2021 - Software and Data Integrity Failures**
9. **A09:2021 - Security Logging and Monitoring Failures**
10. **A10:2021 - Server-Side Request Forgery (SSRF)**

### Core Competencies

- Secure coding practices
- Input validation and sanitization
- Output encoding
- SQL injection prevention
- XSS (Cross-Site Scripting) prevention
- CSRF (Cross-Site Request Forgery) protection
- Authentication implementation
- Password hashing and salting
- Secrets management
- Dependency scanning
- Static and Dynamic Application Security Testing
- Security code review
- Threat modeling (STRIDE)
- Security logging and monitoring

---

## Critical Rules

### Security First Mindset

- **NEVER hard-code secrets** - Use Azure Key Vault or environment variables
- **NEVER log sensitive data** - No passwords, tokens, PII, or secrets in logs
- **ALWAYS validate input** - At API boundary, never trust user input
- **ALWAYS use HTTPS** - In production, no exceptions
- **ALWAYS implement authorization** - Check permissions at every endpoint
- **ALWAYS use parameterized queries** - EF Core handles this, never build SQL strings
- **Document security decisions** - Why certain approaches were chosen

---

## Secure Coding Practices

### Input Validation and Sanitization

#### Data Annotations Validation

```csharp
// File: {ApplicationName}.Domain.{Domain}/Features/{Entity}/Commands/Create{Entity}Command.cs

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Command to create a {entity} with comprehensive validation.
/// </summary>
public sealed record Create{Entity}Command(
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Name contains invalid characters")]
    string Name,

    [Required]
    [Range(0.01, 1000000.00, ErrorMessage = "Amount must be between 0.01 and 1,000,000")]
    decimal Amount,

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    string Email,

    [Url(ErrorMessage = "Invalid URL")]
    string? WebsiteUrl
) : ICommand<Create{Entity}Response>, IValidatableObject
{
    /// <summary>
    /// Custom validation logic.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Business rule validation
        if (Amount > 100000 && string.IsNullOrWhiteSpace(WebsiteUrl))
        {
            yield return new ValidationResult(
                "Website URL is required for amounts over $100,000",
                new[] { nameof(WebsiteUrl) });
        }

        // Date validation
        var now = DateTimeOffset.UtcNow;
        if (StartDate > now.AddYears(1))
        {
            yield return new ValidationResult(
                "Start date cannot be more than 1 year in the future",
                new[] { nameof(StartDate) });
        }
    }
}
```

#### Input Sanitization

```csharp
using System.Text.RegularExpressions;

/// <summary>
/// Sanitizes user input to prevent injection attacks.
/// </summary>
public static class InputSanitizer
{
    /// <summary>
    /// Sanitizes a string by removing potentially dangerous characters.
    /// </summary>
    public static string SanitizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove control characters
        input = Regex.Replace(input, @"[\x00-\x1F\x7F]", string.Empty);

        // Remove script tags
        input = Regex.Replace(input, @"<script[^>]*>.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Remove potentially dangerous HTML
        input = Regex.Replace(input, @"<[^>]+>", string.Empty);

        return input.Trim();
    }

    /// <summary>
    /// Validates and sanitizes email addresses.
    /// </summary>
    public static string SanitizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        // Basic email regex validation
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(email))
            throw new ValidationException("Invalid email format");

        return email.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Sanitizes file names to prevent path traversal.
    /// </summary>
    public static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ValidationException("File name cannot be empty");

        // Remove path separators
        fileName = fileName.Replace("/", "").Replace("\\", "");

        // Remove potentially dangerous characters
        fileName = Regex.Replace(fileName, @"[^\w\s\-\.]", string.Empty);

        // Prevent directory traversal
        if (fileName.Contains(".."))
            throw new ValidationException("File name contains invalid sequence");

        return fileName;
    }
}
```

### Output Encoding

```csharp
using System.Web;
using System.Text.Encodings.Web;

/// <summary>
/// Encodes output to prevent XSS attacks.
/// </summary>
public static class OutputEncoder
{
    /// <summary>
    /// HTML encodes a string.
    /// </summary>
    public static string HtmlEncode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return HtmlEncoder.Default.Encode(input);
    }

    /// <summary>
    /// JavaScript encodes a string.
    /// </summary>
    public static string JavaScriptEncode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return JavaScriptEncoder.Default.Encode(input);
    }

    /// <summary>
    /// URL encodes a string.
    /// </summary>
    public static string UrlEncode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return UrlEncoder.Default.Encode(input);
    }
}
```

---

## SQL Injection Prevention

### Using EF Core (Parameterized Queries)

```csharp
// ✅ CORRECT - EF Core uses parameterized queries automatically

public async Task<List<Budget>> GetBudgetsByUserAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
{
    // EF Core parameterizes this automatically - SAFE
    return await _dbContext.Budgets
        .Where(b => b.UserId == userId)
        .ToListAsync(cancellationToken);
}

// ✅ CORRECT - Even with string parameters
public async Task<List<Budget>> SearchBudgetsAsync(
    string searchTerm,
    CancellationToken cancellationToken = default)
{
    // EF Core parameterizes the searchTerm - SAFE
    return await _dbContext.Budgets
        .Where(b => b.Name.Contains(searchTerm))
        .ToListAsync(cancellationToken);
}

// ❌ WRONG - Raw SQL concatenation (NEVER DO THIS)
public async Task<List<Budget>> SearchBudgetsUnsafe(string searchTerm)
{
    // SQL Injection vulnerability!
    var sql = $"SELECT * FROM Budgets WHERE Name LIKE '%{searchTerm}%'";
    return await _dbContext.Budgets.FromSqlRaw(sql).ToListAsync();
}

// ✅ CORRECT - Raw SQL with parameters
public async Task<List<Budget>> SearchBudgetsSafe(string searchTerm)
{
    // Parameterized raw SQL - SAFE
    return await _dbContext.Budgets
        .FromSqlRaw("SELECT * FROM Budgets WHERE Name LIKE {0}", $"%{searchTerm}%")
        .ToListAsync();
}
```

---

## XSS (Cross-Site Scripting) Prevention

### Content Security Policy (CSP)

```csharp
// File: {ApplicationName}.Services.API/Middleware/SecurityHeadersMiddleware.cs

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Content Security Policy
        context.Response.Headers.Add(
            "Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'");

        // X-Content-Type-Options
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

        // X-Frame-Options
        context.Response.Headers.Add("X-Frame-Options", "DENY");

        // X-XSS-Protection
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

        // Referrer-Policy
        context.Response.Headers.Add("Referrer-Policy", "no-referrer");

        // Permissions-Policy
        context.Response.Headers.Add(
            "Permissions-Policy",
            "geolocation=(), microphone=(), camera=()");

        await _next(context);
    }
}

// Register in Program.cs
app.UseMiddleware<SecurityHeadersMiddleware>();
```

### Razor Page Encoding

```razor
@* Blazor automatically encodes output - SAFE *@
<p>@Model.UserInput</p>

@* Explicit encoding if needed *@
<p>@Html.Encode(Model.UserInput)</p>

@* ❌ WRONG - Bypasses encoding *@
<p>@Html.Raw(Model.UserInput)</p> @* DANGEROUS - Only use with trusted content *@
```

---

## CSRF (Cross-Site Request Forgery) Protection

### Anti-Forgery Tokens

```csharp
// File: {ApplicationName}.Services.API/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add anti-forgery services
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

app.UseAntiforgery();

// Endpoint with anti-forgery validation
app.MapPost("/api/budgets", async (
    [FromBody] BudgetModel model,
    [FromHeader(Name = "X-CSRF-TOKEN")] string csrfToken,
    ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> handler,
    IAntiforgery antiforgery,
    HttpContext httpContext) =>
{
    // Validate anti-forgery token
    await antiforgery.ValidateRequestAsync(httpContext);

    var command = new CreateBudgetCommand(model.Name, model.Amount, model.StartDate);
    var result = await handler.HandleAsync(command);

    return Results.Created($"/api/budgets/{result.BudgetId}", result);
})
.RequireAuthorization();

app.Run();
```

### SameSite Cookie Attribute

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevents CSRF
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
});
```

---

## Authentication Implementation

### OpenIddict Configuration

```csharp
// File: {ApplicationName}.Services.API/Program.cs

using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<DataContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/connect/token")
            .SetAuthorizationEndpointUris("/connect/authorize")
            .SetUserinfoEndpointUris("/connect/userinfo");

        options.AllowPasswordFlow()
            .AllowRefreshTokenFlow()
            .AllowAuthorizationCodeFlow();

        // Encryption and signing keys from Key Vault
        var encryptionKey = builder.Configuration["OpenIddict:EncryptionKey"];
        var signingKey = builder.Configuration["OpenIddict:SigningKey"];

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String(encryptionKey!)));

        options.AddSigningKey(new SymmetricSecurityKey(
            Convert.FromBase64String(signingKey!)));

        // Token lifetimes
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

var app = builder.Build();
app.Run();
```

### JWT Token Validation

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero // No grace period
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log authentication failures
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                logger.LogWarning(
                    "Authentication failed: {Exception}",
                    context.Exception.Message);

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Additional token validation logic
                return Task.CompletedTask;
            }
        };
    });
```

---

## Password Hashing

### Using BCrypt

```csharp
using BCrypt.Net;

/// <summary>
/// Secure password hashing service.
/// </summary>
public sealed class PasswordHasher
{
    private const int WorkFactor = 12; // BCrypt work factor (higher = more secure, slower)

    /// <summary>
    /// Hashes a password using BCrypt.
    /// </summary>
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        // BCrypt automatically generates a salt
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        ArgumentException.ThrowIfNullOrWhiteSpace(hash, nameof(hash));

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}

// Usage in handler
public sealed class CreateUserHandler(
    DataContext dataContext,
    PasswordHasher passwordHasher,
    ILogger<CreateUserHandler> logger
) : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // Hash password before storing
        var passwordHash = passwordHasher.HashPassword(command.Password);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = passwordHash,
            CreatedDate = DateTimeOffset.UtcNow
        };

        var model = user.Adapt<UserModel>();
        await dataContext.AddItemAsync<User, UserModel>(model, cancellationToken);

        logger.LogInformation(
            "Created user {Email} (ID: {UserId})",
            command.Email, user.UserId);

        return new CreateUserResponse(user.UserId);
    }
}
```

### Using Argon2

```csharp
using Konscious.Security.Cryptography;
using System.Security.Cryptography;

/// <summary>
/// Argon2id password hashing (more secure than BCrypt).
/// </summary>
public sealed class Argon2PasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 4;
    private const int MemorySize = 128 * 1024; // 128 MB
    private const int DegreeOfParallelism = 2;

    /// <summary>
    /// Hashes a password using Argon2id.
    /// </summary>
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        // Generate salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Hash password
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            MemorySize = MemorySize,
            Iterations = Iterations
        };

        var hash = argon2.GetBytes(HashSize);

        // Combine salt and hash
        var combined = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, combined, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, combined, SaltSize, HashSize);

        return Convert.ToBase64String(combined);
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    public bool VerifyPassword(string password, string hashString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        ArgumentException.ThrowIfNullOrWhiteSpace(hashString, nameof(hashString));

        try
        {
            var combined = Convert.FromBase64String(hashString);

            // Extract salt and hash
            var salt = new byte[SaltSize];
            var hash = new byte[HashSize];
            Buffer.BlockCopy(combined, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(combined, SaltSize, hash, 0, HashSize);

            // Hash provided password with same salt
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                MemorySize = MemorySize,
                Iterations = Iterations
            };

            var testHash = argon2.GetBytes(HashSize);

            // Compare hashes
            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
        catch
        {
            return false;
        }
    }
}
```

---

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

## Static Application Security Testing (SAST)

### Security Code Analysis

```xml
<!-- File: Directory.Build.props -->

<Project>
  <PropertyGroup>
    <!-- Enable security code analysis -->
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisMode>All</AnalysisMode>
    <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>

    <!-- Security analyzers -->
    <RunAnalyzersDuringBuild>true</RunAnalyzersDuringBuild>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0" />
    <PackageReference Include="SecurityCodeScan.VS2019" Version="5.6.7" />
  </ItemGroup>
</Project>
```

---

## Security Code Review Checklist

### Input Validation
- [ ] All user inputs validated at API boundary
- [ ] Data annotations used for simple validation
- [ ] IValidatableObject used for complex validation
- [ ] Input sanitized to prevent injection
- [ ] File uploads validated (type, size, content)

### Output Encoding
- [ ] All output encoded (HTML, JavaScript, URL)
- [ ] Razor automatically encodes output
- [ ] No use of @Html.Raw with user input

### SQL Injection
- [ ] EF Core used for all database access
- [ ] No raw SQL string concatenation
- [ ] Parameterized queries for raw SQL

### XSS Prevention
- [ ] Content Security Policy configured
- [ ] Security headers set
- [ ] Output encoding applied

### CSRF Protection
- [ ] Anti-forgery tokens used
- [ ] SameSite cookie attribute set
- [ ] State-changing operations use POST/PUT/DELETE

### Authentication
- [ ] OpenIddict or ASP.NET Identity used
- [ ] JWT tokens validated
- [ ] Short-lived access tokens (< 30 minutes)
- [ ] Refresh tokens implemented
- [ ] HTTPS required

### Password Security
- [ ] Passwords hashed with BCrypt or Argon2
- [ ] Never stored in plaintext
- [ ] Minimum password strength requirements
- [ ] Password reset uses secure tokens

### Secrets Management
- [ ] No secrets hard-coded in code
- [ ] Azure Key Vault used for production
- [ ] Secrets not in source control
- [ ] Environment-specific secrets

### Authorization
- [ ] Authorization checked at every endpoint
- [ ] Principle of least privilege applied
- [ ] Resource-based authorization where needed
- [ ] Fail closed on errors

### Logging
- [ ] No PII in logs
- [ ] No passwords, tokens, or secrets logged
- [ ] Security events logged
- [ ] Structured logging used

### Dependencies
- [ ] NuGet audit enabled
- [ ] Dependabot configured
- [ ] Regular updates to dependencies
- [ ] No known vulnerabilities

---

## Threat Modeling (STRIDE)

### STRIDE Methodology

- **S**poofing - Can an attacker impersonate a user or system?
- **T**ampering - Can an attacker modify data in transit or at rest?
- **R**epudiation - Can a user deny performing an action?
- **I**nformation Disclosure - Can sensitive data be exposed?
- **D**enial of Service - Can the system be made unavailable?
- **E**levation of Privilege - Can a user gain unauthorized access?

### Example Threat Model for Budget API

```markdown
# Budget API Threat Model

## Assets
- User data (email, password hashes)
- Budget data (amounts, categories)
- Authentication tokens

## Trust Boundaries
- Client ↔ API Gateway
- API Gateway ↔ Microservices
- Microservices ↔ Database

## Threats

### Spoofing
- **Threat:** Attacker steals JWT token and impersonates user
- **Mitigation:** Short-lived tokens, HTTPS only, secure storage

### Tampering
- **Threat:** Attacker modifies budget amounts in transit
- **Mitigation:** HTTPS, request signing, integrity checks

### Repudiation
- **Threat:** User denies creating a budget
- **Mitigation:** Audit logging with timestamps, correlation IDs

### Information Disclosure
- **Threat:** Attacker accesses other users' budgets
- **Mitigation:** Authorization checks, resource-based access control

### Denial of Service
- **Threat:** Attacker floods API with requests
- **Mitigation:** Rate limiting, throttling, circuit breakers

### Elevation of Privilege
- **Threat:** Regular user accesses admin functions
- **Mitigation:** Role-based access control, authorization policies
```

---

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

## Common Pitfalls

### ❌ WRONG - Hard-Coded Secrets

```csharp
// ❌ NEVER DO THIS
public class EmailService
{
    private const string ApiKey = "SG.abc123def456..."; // WRONG!
    private const string ConnectionString = "Server=...;Password=SuperSecret123"; // WRONG!
}
```

### ✅ CORRECT - Secrets from Configuration

```csharp
// ✅ CORRECT
public class EmailService
{
    private readonly string _apiKey;

    public EmailService(IConfiguration configuration)
    {
        _apiKey = configuration["SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("SendGrid API key not configured");
    }
}
```

### ❌ WRONG - Logging Sensitive Data

```csharp
// ❌ NEVER DO THIS
_logger.LogInformation(
    "User logged in: {Email}, Password: {Password}",
    email, password); // WRONG!
```

### ✅ CORRECT - Safe Logging

```csharp
// ✅ CORRECT
_logger.LogInformation(
    "User logged in: UserId: {UserId}",
    userId);
```

---

## Quality Checklist

- [ ] All user inputs validated
- [ ] Data annotations used
- [ ] Custom validation implemented
- [ ] Input sanitized
- [ ] Output encoded
- [ ] EF Core used for database access
- [ ] No SQL string concatenation
- [ ] Security headers configured
- [ ] CSP implemented
- [ ] Anti-forgery tokens used
- [ ] OpenIddict or ASP.NET Identity configured
- [ ] JWT tokens validated
- [ ] Passwords hashed with BCrypt/Argon2
- [ ] Azure Key Vault configured for secrets
- [ ] No secrets in source code
- [ ] No sensitive data in logs
- [ ] Authorization checks at endpoints
- [ ] File uploads validated
- [ ] NuGet audit enabled
- [ ] Security code analysis enabled
- [ ] Threat model documented
- [ ] Security events logged

---

**End of Application Security Specialist Skill**
