using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Models;
using Microsoft.Extensions.Logging;
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
    private readonly IContext _context;
    private readonly IMessengerService _messengerService;
    private readonly ILogger _logger;

    public bool IsAuthenticated => _context.IsAuthenticated;
    public IProfile? CurrentProfile => _context.Profile;

    public AuthenticationService(
        IContext context,
        IMessengerService messengerService,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
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
            UserId = Guid.NewGuid(),
            TenantId = Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
            Name = $"Demo User - {username}",
            TimeZoneId = "Europe/Amsterdam",
            CountryCode = "NL",
            CultureCode = "nl-NL",
            Email = $"{username}@demo.com",
            Roles = new List<string> { "Administrator", "User" },
            Features = new List<string> { "Dashboard", "Settings", "Admin" },
            Expiration = DateTimeOffset.Now.AddHours(24),
            Organizations = new List<Organization>
            {
                new Organization
                {
                    OrganizationId = Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
                    Name = "Demo Organization"
                }
            }
        };

        _context.Profile = profile;
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
        _context.Profile = null;
        _messengerService.Send(new UserSignedOutMessage());
        return Task.CompletedTask;
    }

    public Task SwitchOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        if (_context.Profile is not { } profile)
            return Task.CompletedTask;

        // Validate: must be either switching to personal context (UserId) or to an org in the list
        var isPersonalContext = organizationId == profile.UserId;
        var isKnownOrg = profile.Organizations.Any(o => o.OrganizationId == organizationId);

        if (!isPersonalContext && !isKnownOrg)
            return Task.CompletedTask;

        profile.TenantId = organizationId;
        _messengerService.Send(new TenantSwitchedMessage(organizationId));

        _logger.LogTrace("Demo tenant switched to: {TenantId}", organizationId);
        return Task.CompletedTask;
    }

    private static string GenerateToken() =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray()) +
        Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}
