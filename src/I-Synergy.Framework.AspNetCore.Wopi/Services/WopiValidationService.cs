using ISynergy.Framework.Wopi.Models;
using ISynergy.Framework.Wopi.Security;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Wopi.Services
{
    public class WopiValidationService : IWopiValidationService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IWopiDiscoveryService wopiDiscoveryService;

        public WopiValidationService(IMemoryCache memoryCache, IWopiDiscoveryService wopiDiscoveryService)
        {
            this.memoryCache = memoryCache;
            this.wopiDiscoveryService = wopiDiscoveryService;
        }

        /// <summary>
        /// Validates the WOPI Proof on an incoming WOPI request
        /// </summary>
        public async Task<bool> ValidateAsync(WopiRequest wopiRequest)
        {
            var hostUrl = wopiRequest.RequestUri.OriginalString.Replace(":44300", "").Replace(":443", "");

            // Make sure the request has the correct headers
            if (wopiRequest.Proof == null ||
                wopiRequest.Timestamp == null)
                return false;

            // Set the requested proof values
            var requestProof = wopiRequest.Proof;
            var requestProofOld = string.Empty;
            if (wopiRequest.ProofOld != null)
                requestProofOld = wopiRequest.ProofOld;

            // Get the WOPI proof info from discovery
            var discoProof = await GetWopiProofAsync();

            // Encode the values into bytes
            var accessTokenBytes = Encoding.UTF8.GetBytes(wopiRequest.AccessToken);
            var hostUrlBytes = Encoding.UTF8.GetBytes(hostUrl.ToUpperInvariant());
            var timeStampBytes = BitConverter.GetBytes(Convert.ToInt64(wopiRequest.Timestamp)).Reverse().ToArray();

            // Build expected proof
            List<byte> expected = new List<byte>(
                4 + accessTokenBytes.Length +
                4 + hostUrlBytes.Length +
                4 + timeStampBytes.Length);

            // Add the values to the expected variable
            expected.AddRange(BitConverter.GetBytes(accessTokenBytes.Length).Reverse().ToArray());
            expected.AddRange(accessTokenBytes);
            expected.AddRange(BitConverter.GetBytes(hostUrlBytes.Length).Reverse().ToArray());
            expected.AddRange(hostUrlBytes);
            expected.AddRange(BitConverter.GetBytes(timeStampBytes.Length).Reverse().ToArray());
            expected.AddRange(timeStampBytes);
            byte[] expectedBytes = expected.ToArray();

            return (VerifyProof(expectedBytes, requestProof, discoProof.value) ||
                VerifyProof(expectedBytes, requestProof, discoProof.oldvalue) ||
                VerifyProof(expectedBytes, requestProofOld, discoProof.value));
        }

        /// <summary>
        /// Verifies the proof against a specified key
        /// </summary>
        public bool VerifyProof(byte[] expectedProof, string proofFromRequest, string proofFromDiscovery)
        {
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider())
            {
                try
                {
                    rsaProvider.ImportCspBlob(Convert.FromBase64String(proofFromDiscovery));
                    return rsaProvider.VerifyData(expectedProof, "SHA256", Convert.FromBase64String(proofFromRequest));
                }
                catch (FormatException)
                {
                    return false;
                }
                catch (CryptographicException)
                {
                    return false;
                }
            }
        }

        private async Task<WopiProof> GetWopiProofAsync()
        {
            // Check cache for this data
            if (!memoryCache.TryGetValue(Constants.WopiProof, out _))
            {
                await wopiDiscoveryService.RefreshAsync();
            }

            return memoryCache.Get<WopiProof>(Constants.WopiProof);
        }
    }
}
