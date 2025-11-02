using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Models;

namespace Sample.Services;

/// <summary>
/// Fake/Demo Authentication Service for sample applications.
/// Implements the <see cref="IAuthenticationService" />
/// Provides event-driven authentication state management.
/// 
/// Security Note: This is a DEMO service only. It accepts ANY username/password combination
/// for testing purposes. Never use this in production.
/// </summary>
/// <seealso cref="IAuthenticationService" />
public class AuthenticationService : IAuthenticationService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger _logger;

    /// <summary>
    /// Raised when authentication succeeds (user logged in).
    /// The authentication state has been updated in the context.
    /// </summary>
    public event EventHandler<AuthenticationSuccessEventArgs>? AuthenticationSucceeded;

    /// <summary>
    /// Raised when authentication fails or user logs out.
    /// The context has been cleared.
    /// </summary>
    public event EventHandler<EventArgs>? AuthenticationFailed;

    /// <summary>
    /// Determines if a user is currently authenticated.
    /// </summary>
    public bool IsAuthenticated { get; private set; }

    public AuthenticationService(
            IScopedContextService scopedContextService,
         ILogger<AuthenticationService> logger)
    {
        _scopedContextService = scopedContextService;
        _logger = logger;
        _logger.LogTrace($"AuthenticationService instance created with ID: {Guid.NewGuid()}");
    }

    public Task AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AuthenticateWithRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Demo authentication that accepts any username/password combination.
    /// 
    /// Security: DEMO ONLY - Never use in production!
    /// This method accepts any credentials for testing purposes and creates
    /// a profile with realistic demo data.
    /// </summary>
    /// <param name="username">Any username (for demo purposes)</param>
    /// <param name="password">Any password (for demo purposes)</param>
    /// <param name="remember">Unused in demo service (for interface compatibility)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when authentication is processed</returns>
    public Task AuthenticateWithUsernamePasswordAsync(
    string username,
 string password,
        bool remember,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Demo authentication attempt for user: {Username}", username);

        // Set environment
        _scopedContextService.GetRequiredService<IContext>().Environment = SoftwareEnvironments.Test;

        // Create profile with demo data
        var profile = new Profile
        {
            Token = new Token
            {
                AccessToken = GenerateToken(),
                IdToken = GenerateToken(),
                RefreshToken = GenerateToken(),
                ExpiresIn = 3600,
                TokenType = "Bearer"
            },
            AccountId = Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
            Description = $"Demo Account - {username}",
            TimeZoneId = "Europe/Amsterdam",
            CountryCode = "NL",
            CultureCode = "nl-NL",
            UserId = Guid.NewGuid(),
            Username = username,
            Email = $"{username}@demo.com",
            Roles = new List<string> { "Administrator", "User" },
            Modules = new List<string> { "Dashboard", "Settings", "Admin" },
            Expiration = DateTimeOffset.Now.AddHours(24)
        };

        // Set profile in context
        _scopedContextService.GetRequiredService<IContext>().Profile = profile;

        // Validate and raise events
        ValidateToken(profile.Token);

        return Task.CompletedTask;
    }

    public Task SignOutAsync()
    {
        _logger.LogTrace("Demo sign out");

        _scopedContextService.CreateNewScope();
        ValidateToken(null);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Validates the token and raises appropriate authentication events.
    /// </summary>
    /// <param name="token">The authentication token (null if authentication failed/user logged out).</param>
    private void ValidateToken(Token? token)
    {
        if (token is not null)
        {
            _logger.LogTrace("Authentication succeeded, raising AuthenticationSucceeded event");
            IsAuthenticated = true;
            RaiseAuthenticationSucceeded();
        }
        else
        {
            _logger.LogTrace("Authentication failed, raising AuthenticationFailed event");
            IsAuthenticated = false;
            RaiseAuthenticationFailed();
        }
    }

    /// <summary>
    /// Raises the AuthenticationSucceeded event and handles any exceptions from subscribers.
    /// </summary>
    private void RaiseAuthenticationSucceeded()
    {
        try
        {
            AuthenticationSucceeded?.Invoke(this, new AuthenticationSuccessEventArgs(shouldRemember: false));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AuthenticationSucceeded event handlers");
        }
    }

    /// <summary>
    /// Raises the AuthenticationFailed event and handles any exceptions from subscribers.
    /// </summary>
    private void RaiseAuthenticationFailed()
    {
        try
        {
            AuthenticationFailed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AuthenticationFailed event handlers");
        }
    }

    /// <summary>
    /// Generates a demo token for testing purposes.
    /// This is NOT a real JWT token, just a random string for demo use.
    /// </summary>
    private string GenerateToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) +
          Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
