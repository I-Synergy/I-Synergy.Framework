using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Models;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Models;

namespace Sample.Services;

/// <summary>
/// Demo authentication service for sample applications.
/// Accepts any username/password combination for testing purposes.
/// On sign-in, sets the profile on the current context and sends <see cref="UserSignedInMessage"/>.
/// On sign-out, clears the profile and sends <see cref="UserSignedOutMessage"/>.
///
/// Security Note: DEMO ONLY — never use in production.
/// </summary>
public class AuthenticationService : Sample.Abstractions.Services.IAuthenticationService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly IMessengerService _messengerService;
    private readonly ILogger _logger;

    public bool IsAuthenticated => _scopedContextService.GetRequiredService<IContext>().IsAuthenticated;
    public IProfile? CurrentProfile => _scopedContextService.GetRequiredService<IContext>().Profile;

    public AuthenticationService(
        IScopedContextService scopedContextService,
        IMessengerService messengerService,
        ILogger<AuthenticationService> logger)
    {
        _scopedContextService = scopedContextService;
        _messengerService = messengerService;
        _logger = logger;
    }

    public Task AuthenticateWithUsernamePasswordAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Demo authentication attempt for user: {Username}", username);

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
            Features = new List<string> { "Dashboard", "Settings", "Admin" },
            Expiration = DateTimeOffset.Now.AddHours(24)
        };

        _scopedContextService.GetRequiredService<IContext>().Profile = profile;
        _messengerService.Send(new UserSignedInMessage(profile));

        return Task.CompletedTask;
    }

    public Task<bool> TryRestoreSessionAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task SignInAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Demo sign out");
        _scopedContextService.GetRequiredService<IContext>().Profile = null;
        _messengerService.Send(new UserSignedOutMessage());
        return Task.CompletedTask;
    }

    private static string GenerateToken() =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray()) +
        Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}
