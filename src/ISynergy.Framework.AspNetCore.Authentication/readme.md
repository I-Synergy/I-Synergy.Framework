# I-Synergy Framework AspNetCore Authentication

Authentication and identity management extensions for ASP.NET Core applications. This package provides JWT token handling, claims-based authorization utilities, custom password validation, and integration with OpenIddict for OAuth 2.0/OpenID Connect workflows.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.AspNetCore.Authentication.svg)](https://www.nuget.org/packages/I-Synergy.Framework.AspNetCore.Authentication/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **JWT token configuration** with symmetric key support for secure authentication
- **Claims-based authorization** with rich extension methods for ClaimsPrincipal
- **Custom password validation** with regex pattern support
- **OpenIddict integration** for OAuth 2.0 and OpenID Connect
- **Type-safe claim retrieval** with automatic type conversion
- **Exception handling** for authentication failures via filter attributes
- **Identity options** with enhanced password policy enforcement
- **Extension methods** for retrieving user identity, account, tenant, and client information

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.AspNetCore.Authentication
```

## Quick Start

### 1. Configure JWT Authentication

In your `Program.cs`:

```csharp
using ISynergy.Framework.AspNetCore.Authentication.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT options
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(nameof(JwtOptions)));

var jwtOptions = builder.Configuration
    .GetSection(nameof(JwtOptions))
    .Get<JwtOptions>();

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SymmetricKeySecret))
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

### 2. Configure JWT Options in appsettings.json

```json
{
  "JwtOptions": {
    "SymmetricKeySecret": "your-secret-key-minimum-32-characters-long",
    "Issuer": "https://your-api.com",
    "Audience": "https://your-app.com"
  }
}
```

### 3. Using Claims Extensions

Retrieve user information from ClaimsPrincipal:

```csharp
using ISynergy.Framework.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        // Get user ID from claims
        var userId = User.GetUserId();

        // Get username
        var username = User.GetUserName();

        // Get account ID (tenant identifier)
        var accountId = User.GetAccountId();

        // Get client ID
        var clientId = User.GetClientId();

        return Ok(new
        {
            UserId = userId,
            Username = username,
            AccountId = accountId,
            ClientId = clientId
        });
    }

    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        // Get single claim value
        var email = User.GetSingleClaim("email");

        // Get multiple claims
        var roles = User.GetClaims("role");

        // Get claim as specific type
        var age = User.GetSingleClaimAsInt("age");

        // Get claim as enum
        var status = User.GetSingleClaimAsEnum<UserStatus>("status");

        // Check if claim exists
        bool hasEmail = User.HasClaim("email");

        return Ok(new
        {
            Email = email,
            Roles = roles,
            Age = age,
            Status = status,
            HasEmail = hasEmail
        });
    }
}
```

### 4. Custom Password Validation

Configure enhanced password validation with regex patterns:

```csharp
using ISynergy.Framework.AspNetCore.Authentication.Options;
using ISynergy.Framework.AspNetCore.Authentication.Validators;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Configure password options
builder.Services.Configure<IdentityPasswordOptions>(options =>
{
    options.RequiredLength = 8;
    options.RequireDigit = true;
    options.RequireLowercase = true;
    options.RequireUppercase = true;
    options.RequireNonAlphanumeric = true;
    options.RequiredUniqueChars = 4;

    // Custom regex pattern for additional validation
    // Example: Require at least one special character from a specific set
    options.RequiredRegexMatch = new Regex(@"^(?=.*[!@#$%^&*])");
});

// Add Identity with custom password validator
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddPasswordValidator<IdentityPasswordValidator<ApplicationUser>>();
```

### 5. OpenIddict Integration

Using claims with OpenIddict authentication:

```csharp
using ISynergy.Framework.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using static OpenIddict.Abstractions.OpenIddictConstants;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "OpenIddict.Validation.AspNetCore")]
public class SecureController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult GetUserInfo()
    {
        // Claims are automatically extracted from OpenIddict tokens
        var userId = User.GetUserId();        // Gets Claims.Subject
        var username = User.GetUserName();    // Gets Claims.Username
        var accountId = User.GetAccountId();  // Gets Claims.KeyId
        var clientId = User.GetClientId();    // Gets Claims.ClientId

        return Ok(new
        {
            UserId = userId,
            Username = username,
            AccountId = accountId,
            ClientId = clientId
        });
    }
}
```

## Core Components

### Options

```
ISynergy.Framework.AspNetCore.Authentication.Options/
├── JwtOptions                      # JWT configuration (issuer, audience, secret)
└── IdentityPasswordOptions         # Enhanced password validation with regex
```

### Extensions

```
ISynergy.Framework.Core.Extensions/
└── ClaimsPrincipalExtensions       # Claims retrieval and conversion utilities
```

### Validators

```
ISynergy.Framework.AspNetCore.Authentication.Validators/
└── IdentityPasswordValidator<T>    # Custom password validator with regex support
```

### Exception Filters

```
ISynergy.Framework.AspNetCore.Authentication.Exceptions/
└── ClaimNotFoundExceptionFilterAttribute  # Handle missing claims gracefully
```

## Advanced Features

### Type-Safe Claim Retrieval

```csharp
using ISynergy.Framework.Core.Extensions;

public class ClaimsExample
{
    public void ProcessUserClaims(ClaimsPrincipal user)
    {
        // Get single claim as string
        var email = user.GetSingleClaim("email");

        // Get single claim as int
        var userId = user.GetSingleClaimAsInt("user_id");

        // Get single claim as Guid
        var tenantId = user.GetSingleClaimAsGuid("tenant_id");

        // Get single claim as enum
        var role = user.GetSingleClaimAsEnum<UserRole>("role");

        // Get multiple claims as list
        var permissions = user.GetClaims("permission");

        // Get multiple claims as int list
        var groupIds = user.GetClaimsAsInt("group_id");

        // Get multiple claims as enum list
        var scopes = user.GetClaimsAsEnum<AccessScope>("scope");
    }
}

public enum UserRole
{
    User,
    Admin,
    SuperAdmin
}

public enum AccessScope
{
    Read,
    Write,
    Delete
}
```

### Exception Handling for Missing Claims

```csharp
using ISynergy.Framework.Core.Exceptions;
using ISynergy.Framework.Core.Extensions;

public class SecureService
{
    public string GetUserEmail(ClaimsPrincipal user)
    {
        try
        {
            // Throws ClaimNotFoundException if claim doesn't exist
            return user.GetSingleClaim("email");
        }
        catch (ClaimNotFoundException ex)
        {
            // Handle missing claim
            throw new UnauthorizedAccessException($"Missing required claim: {ex.Message}");
        }
        catch (DuplicateClaimException ex)
        {
            // Handle duplicate claims
            throw new InvalidOperationException($"Duplicate claim found: {ex.Message}");
        }
        catch (InvalidClaimValueException ex)
        {
            // Handle invalid claim value (type conversion failed)
            throw new ArgumentException($"Invalid claim value: {ex.Message}");
        }
    }

    public bool TryGetUserEmail(ClaimsPrincipal user, out string email)
    {
        email = string.Empty;

        // Safe check without throwing
        if (!user.HasClaim("email"))
            return false;

        try
        {
            email = user.GetSingleClaim("email");
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

### Custom Password Validation Patterns

```csharp
using ISynergy.Framework.AspNetCore.Authentication.Options;
using System.Text.RegularExpressions;

// Example 1: Require at least one special character
var options1 = new IdentityPasswordOptions
{
    RequiredLength = 8,
    RequireDigit = true,
    RequiredRegexMatch = new Regex(@"^(?=.*[!@#$%^&*(),.?""{}|<>])")
};

// Example 2: Prevent common password patterns
var options2 = new IdentityPasswordOptions
{
    RequiredLength = 10,
    RequiredRegexMatch = new Regex(@"^(?!.*(?:password|123456|qwerty))",
        RegexOptions.IgnoreCase)
};

// Example 3: Require specific character sets
var options3 = new IdentityPasswordOptions
{
    RequiredLength = 12,
    RequiredRegexMatch = new Regex(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]")
};

// Example 4: Maximum length restriction
var options4 = new IdentityPasswordOptions
{
    RequiredLength = 8,
    RequiredRegexMatch = new Regex(@"^.{8,50}$")
};
```

## Usage Examples

### Building a Secure API with JWT

Complete example of a secure Web API:

```csharp
using ISynergy.Framework.AspNetCore.Authentication.Options;
using ISynergy.Framework.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT
var jwtOptions = builder.Configuration
    .GetSection(nameof(JwtOptions))
    .Get<JwtOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SymmetricKeySecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;

    public AuthController(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validate credentials (implement your own logic)
        if (!ValidateCredentials(request.Username, request.Password))
            return Unauthorized();

        // Create claims
        var claims = new[]
        {
            new Claim(Claims.Subject, request.UserId),
            new Claim(Claims.Username, request.Username),
            new Claim(Claims.KeyId, request.AccountId.ToString()),
            new Claim(Claims.ClientId, "web-app"),
            new Claim("email", request.Email),
            new Claim("role", "User")
        };

        // Generate token
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SymmetricKeySecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { Token = tokenString });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            UserId = User.GetUserId(),
            Username = User.GetUserName(),
            AccountId = User.GetAccountId(),
            Email = User.GetSingleClaim("email"),
            Roles = User.GetClaims("role")
        });
    }
}

public record LoginRequest(
    string UserId,
    string Username,
    string Password,
    string Email,
    Guid AccountId);
```

### Multi-Tenant Authentication

```csharp
using ISynergy.Framework.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tenants/{tenantId}/[controller]")]
[Authorize]
public class TenantDataController : ControllerBase
{
    [HttpGet]
    public IActionResult GetData([FromRoute] Guid tenantId)
    {
        // Verify user belongs to this tenant
        var userTenantId = User.GetAccountId();

        if (userTenantId != tenantId)
            return Forbid();

        // Retrieve tenant-specific data
        var data = GetTenantData(tenantId);

        return Ok(data);
    }

    [HttpPost]
    public IActionResult CreateData([FromRoute] Guid tenantId, [FromBody] DataModel data)
    {
        var userTenantId = User.GetAccountId();

        if (userTenantId != tenantId)
            return Forbid();

        // Create tenant-specific data
        data.TenantId = tenantId;
        data.CreatedBy = User.GetUserId();

        SaveData(data);

        return CreatedAtAction(nameof(GetData), new { tenantId }, data);
    }
}
```

## Best Practices

> [!TIP]
> Store JWT secrets securely using **Azure Key Vault** or **environment variables** instead of hardcoding them in configuration files.

> [!IMPORTANT]
> Use **HTTPS** in production to protect JWT tokens from interception during transmission.

> [!NOTE]
> Set appropriate **token expiration** times based on your security requirements. Shorter lifetimes are more secure but may impact user experience.

### JWT Configuration

- Use strong symmetric keys (minimum 32 characters, 256 bits)
- Set appropriate token expiration times (1-24 hours typical)
- Use refresh tokens for long-lived sessions
- Validate issuer and audience to prevent token reuse
- Set ClockSkew to minimize timing vulnerabilities
- Rotate signing keys periodically

### Claims Management

- Use standard OpenID Connect claim types when possible
- Keep claim payloads minimal to reduce token size
- Don't store sensitive data in claims (they're not encrypted)
- Validate claim values before using them
- Use type-safe claim retrieval methods
- Handle missing or invalid claims gracefully

### Password Validation

- Combine standard password options with regex validation
- Test regex patterns thoroughly before deployment
- Provide clear error messages for validation failures
- Consider using passphrases instead of complex passwords
- Implement password history to prevent reuse
- Use password strength meters in UI

### Security Considerations

- Never log or expose JWT tokens in error messages
- Implement token revocation for logout scenarios
- Use HTTPS everywhere to protect tokens in transit
- Implement rate limiting on authentication endpoints
- Monitor for suspicious authentication patterns
- Use role-based and claim-based authorization together

## Testing

Example unit tests for authentication components:

```csharp
using ISynergy.Framework.Core.Extensions;
using System.Security.Claims;
using Xunit;

public class ClaimsExtensionsTests
{
    [Fact]
    public void GetUserId_WithValidClaim_ReturnsUserId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(Claims.Subject, "user-123")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var userId = principal.GetUserId();

        // Assert
        Assert.Equal("user-123", userId);
    }

    [Fact]
    public void GetAccountId_WithValidClaim_ReturnsGuid()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim(Claims.KeyId, accountId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetAccountId();

        // Assert
        Assert.Equal(accountId, result);
    }

    [Fact]
    public void HasClaim_WithExistingClaim_ReturnsTrue()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("email", "test@example.com")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.HasClaim("email");

        // Assert
        Assert.True(result);
    }
}
```

## Dependencies

- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication support
- **Microsoft.AspNetCore.Identity** - Identity framework integration
- **OpenIddict.Abstractions** - OAuth 2.0/OpenID Connect abstractions
- **ISynergy.Framework.Core** - Core framework utilities

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.AspNetCore** - Base ASP.NET Core integration
- **I-Synergy.Framework.AspNetCore.MultiTenancy** - Multi-tenant support
- **I-Synergy.Framework.AspNetCore.Monitoring** - SignalR monitoring
- **I-Synergy.Framework.EntityFramework** - Data persistence

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).
