using ISynergy.Framework.AspNetCore.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Utilities
{
    /// <summary>
    /// Class AzureUtilities.
    /// </summary>
    public class AzureUtilities
    {
        //this is an optional property to hold the secret after it is retrieved
        /// <summary>
        /// Gets or sets the encrypt secret.
        /// </summary>
        /// <value>The encrypt secret.</value>
        public string EncryptSecret { get; set; } = string.Empty;

        /// <summary>
        /// The vault settings
        /// </summary>
        private readonly IOptions<AzureKeyVaultOptions> _vaultSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureUtilities"/> class.
        /// </summary>
        /// <param name="vaultSettings">The vault settings.</param>
        /// <exception cref="ArgumentNullException">vaultSettings</exception>
        public AzureUtilities(IOptions<AzureKeyVaultOptions> vaultSettings)
        {
            _vaultSettings = vaultSettings ?? throw new ArgumentNullException(nameof(vaultSettings));
        }

        //the method that will be provided to the KeyVaultClient
        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="resource">The resource.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="InvalidOperationException">Failed to obtain the JWT token</exception>
        public async Task<string> GetToken(string authority, string resource)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(
                _vaultSettings.Value.ClientId,
                _vaultSettings.Value.ClientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred).ConfigureAwait(false);

            if (result is null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
