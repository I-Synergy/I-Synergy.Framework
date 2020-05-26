using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IAuthenticationService
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Checks for expired token.
        /// </summary>
        /// <returns>Task.</returns>
        Task CheckForExpiredToken();
        /// <summary>
        /// Determines whether the specified e is transient.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> IsTransient(Exception e);
        /// <summary>
        /// Authenticates the with username password asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Task.</returns>
        Task AuthenticateWithUsernamePasswordAsync(string username, string password);
        /// <summary>
        /// Authenticates the with client credentials asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task AuthenticateWithClientCredentialsAsync();
        /// <summary>
        /// Authenticates the with refresh token asynchronous.
        /// </summary>
        /// <param name="refreshtoken">The refreshtoken.</param>
        /// <returns>Task.</returns>
        Task AuthenticateWithRefreshTokenAsync(string refreshtoken);
    }
}
