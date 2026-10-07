# Secure Coding Practices, Injection & CSRF Prevention
Baseline secure-coding rules plus SQL injection, XSS and CSRF prevention patterns. Load this when writing code that handles input, SQL, HTML or state-changing requests.

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

