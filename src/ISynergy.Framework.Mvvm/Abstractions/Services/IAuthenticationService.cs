using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Interface IAuthenticationService
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Occurs when authentication changed.
    /// </summary>
    event EventHandler<ReturnEventArgs<bool>> AuthenticationChanged;

    /// <summary>
    /// Handles the <see cref="E:AuthenticationChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="ReturnEventArgs{Profile}"/> instance containing the event data.</param>
    void OnAuthenticationChanged(ReturnEventArgs<bool> e);

    /// <summary>
    /// authenticate with username password as an asynchronous operation.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="remember"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
    Task AuthenticateWithUsernamePasswordAsync(string username, string password, bool remember, CancellationToken cancellationToken = default);

    /// <summary>
    /// authenticate with client credentials as an asynchronous operation.
    /// </summary>
    /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
    Task AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// authenticate with refresh token as an asynchronous operation.
    /// </summary>
    /// <param name="refreshToken">The refreshToken.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
    /// In the previous betas, OpenIddict used a non-configurable mode codenamed "rolling tokens": every time a refresh token was sent as part of a grant_type=refresh_token
    /// request, it was automatically revoked and a new single-use refresh token was generated and returned to the client application.
    /// This approach was great from a security perspective but had a few downsides.For instance, it didn't play well with heavily distributed client applications
    /// like MVC apps implementing transparent access token renewal (e.g using Microsoft's OIDC client middleware). In such scenario, if two refresh tokens requests
    /// were simultaneously sent with the same refresh token, one of them would be automatically rejected as the refresh token would be already marked as "redeemed"
    /// when handling the second request.
    /// The previous default behavior is still supported but is now an opt-in option. To enable it, call _options.UseRollingTokens() from the OpenIddict configuration
    /// delegate, in ConfigureServices().
    Task AuthenticateWithRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// authenticate with API key as an asynchronous operation.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
    Task AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);

    Task SignOutAsync();

    string GetEnvironmentalAuthToken(string token);

    /// <summary>
    /// Gets the modules asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;Module&gt;&gt;.</returns>
    Task<List<Module>> GetModulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all countries from masterdata.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Country>> GetCountriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if license name is available.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> CheckRegistrationNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if email address is available.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> CheckRegistrationEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new account.
    /// </summary>
    /// <param name="registration">The e.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> RegisterNewAccountAsync(RegistrationData registration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a password reset.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> ResetPasswordAsync(string email, CancellationToken cancellationToken = default);
}
