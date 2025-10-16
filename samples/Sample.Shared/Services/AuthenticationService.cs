using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Models;

namespace Sample.Services;

/// <summary>
/// Class AuthenticationService.
/// Implements the <see cref="IAuthenticationService" />
/// </summary>
/// <seealso cref="IAuthenticationService" />
public class AuthenticationService : IAuthenticationService
{
    private readonly IScopedContextService _scopedContextService;
    private readonly ILogger _logger;

    public AuthenticationService(IScopedContextService scopedContextService, ILogger<AuthenticationService> logger)
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

    public Task AuthenticateWithUsernamePasswordAsync(string username, string password, bool remember, CancellationToken cancellationToken = default)
    {
        _scopedContextService.GetRequiredService<IContext>().Environment = SoftwareEnvironments.Test;
        _scopedContextService.GetRequiredService<IContext>().Profile = new Profile(
            new Token(),
            Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
            "Test",
            "Europe/Amsterdam",
            "NL",
            Guid.NewGuid(),
            username,
            "admin@demo.com",
            ["Administrator"],
            [],
            DateTimeOffset.Now.AddDays(7),
            1,
            DateTime.Now.AddHours(24));

        if (remember)
        {
            if (_scopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser != username)
            {
                _scopedContextService.GetRequiredService<ISettingsService>().LocalSettings.IsAutoLogin = true;
                _scopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser = username;
                _scopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings();
            }
        }

        ValidateToken(new Token());

        return Task.CompletedTask;
    }

    public Task SignOutAsync()
    {
        _scopedContextService.CreateNewScope();
        ValidateToken(null);
        return Task.CompletedTask;
    }

    private void ValidateToken(Token? token)
    {
        if (token is not null)
        {
            _scopedContextService.GetRequiredService<IContext>().Profile = new Profile(
                token,
                Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
                "Test",
                "Europe/Amsterdam",
                "NL",
                Guid.NewGuid(),
                "admin",
                "admin@demo.com",
                ["Administrator"],
                [],
                DateTimeOffset.Now.AddDays(7),
                1,
                DateTime.Now.AddHours(24));

            MessengerService.Default.Send(new AuthenticationChangedMessage(true));
        }
        else
        {
            _scopedContextService.GetRequiredService<IContext>().Profile = null;

            MessengerService.Default.Send(new AuthenticationChangedMessage(false));
        }
    }
}
