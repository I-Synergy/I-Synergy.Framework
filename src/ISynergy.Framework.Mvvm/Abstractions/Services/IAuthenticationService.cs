namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IAuthenticationService
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// authenticate with username password as an asynchronous operation.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        Task AuthenticateWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// authenticate with client credentials as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        Task AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// authenticate with refresh token as an asynchronous operation.
        /// </summary>
        /// <param name="refreshtoken">The refreshtoken.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        /// In the previous betas, OpenIddict used a non-configurable mode codenamed "rolling tokens": every time a refresh token was sent as part of a grant_type=refresh_token
        /// request, it was automatically revoked and a new single-use refresh token was generated and returned to the client application.
        /// This approach was great from a security perspective but had a few downsides.For instance, it didn't play well with heavily distributed client applications
        /// like MVC apps implementing transparent access token renewal (e.g using Microsoft's OIDC client middleware). In such scenario, if two refresh tokens requests
        /// were simultaneously sent with the same refresh token, one of them would be automatically rejected as the refresh token would be already marked as "redeemed"
        /// when handling the second request.
        /// The previous default behavior is still supported but is now an opt-in option. To enable it, call options.UseRollingTokens() from the OpenIddict configuration
        /// delegate, in ConfigureServices().
        Task AuthenticateWithRefreshTokenAsync(string refreshtoken, CancellationToken cancellationToken = default);

        /// <summary>
        /// authenticate with API key as an asynchronous operation.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        Task AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
        void SignOut();
        string GetEnvironmentalAuthToken(string token);
    }
}
