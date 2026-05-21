namespace Sample.Abstractions.Services;

/// <summary>
/// Sample-specific authentication contract.
/// Extends the framework base with a credential-based sign-in method for the demo flow.
/// </summary>
public interface IAuthenticationService : ISynergy.Framework.Core.Abstractions.Services.IAuthenticationService
{
    Task AuthenticateWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);
}
