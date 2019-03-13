using ISynergy.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace ISynergy.Utilities
{
    public class AzureUtilities
    {
        //this is an optional property to hold the secret after it is retrieved
        public string EncryptSecret { get; set; }

        private readonly IOptions<AzureKeyVaultOptions> _vaultSettings;

        public AzureUtilities(IOptions<AzureKeyVaultOptions> vaultSettings)
        {
            _vaultSettings = vaultSettings ?? throw new ArgumentNullException(nameof(vaultSettings));
        }

        //the method that will be provided to the KeyVaultClient
        public async Task<string> GetToken(string authority, string resource)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(
                _vaultSettings.Value.ClientId,
                _vaultSettings.Value.ClientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result is null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
