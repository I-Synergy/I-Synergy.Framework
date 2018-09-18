using ISynergy.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Utilities
{
    public class AzureUtilities
    {
        //this is an optional property to hold the secret after it is retrieved
        public string EncryptSecret { get; set; }

        private readonly IOptions<AzureKeyVault> _vaultSettings;

        public AzureUtilities(IOptions<AzureKeyVault> vaultSettings)
        {
            _vaultSettings = vaultSettings ?? throw new ArgumentNullException(nameof(vaultSettings));
        }

        //the method that will be provided to the KeyVaultClient
        public async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(
                _vaultSettings.Value.ClientId,
                _vaultSettings.Value.ClientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result is null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
