namespace Sample.Abstractions.Services;
public interface IAuthenticationService
{
    Task AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default);
    Task AuthenticateWithRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task AuthenticateWithUsernamePasswordAsync(string username, string password, bool remember, CancellationToken cancellationToken = default);
    Task SignOutAsync();
}
