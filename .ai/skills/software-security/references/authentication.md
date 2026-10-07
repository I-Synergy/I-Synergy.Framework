# Authentication Implementation & Password Hashing
Authentication flows, token handling and password hashing (bcrypt/Argon2). Load this when implementing login, sessions or credential storage.

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
public sealed class CreateUserCommandHandler(
    DataContext dataContext,
    PasswordHasher passwordHasher,
    ILogger<CreateUserCommandHandler> logger
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

        dataContext.Users.Add(user);
        var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);

        if (rowsAffected == 0)
            throw new InvalidOperationException("Failed to create user");

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

