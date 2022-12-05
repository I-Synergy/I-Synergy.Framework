using System;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace Sample.Services
{
    /// <summary>
    /// Class AuthenticationService.
    /// Implements the <see cref="IBaseAuthenticationService" />
    /// </summary>
    /// <seealso cref="IBaseAuthenticationService" />
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Authenticates the with API key asynchronous.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IProfile> AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Authenticates the with client credentials asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IProfile> AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Authenticates the with refresh token asynchronous.
        /// </summary>
        /// <param name="refreshtoken">The refreshtoken.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IProfile> AuthenticateWithRefreshTokenAsync(string refreshtoken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Authenticates the with username password asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IProfile> AuthenticateWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks for expired token.
        /// </summary>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task CheckForExpiredTokenAsync(IProfile profile, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified e is transient.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> IsTransient(Exception e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logouts the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
