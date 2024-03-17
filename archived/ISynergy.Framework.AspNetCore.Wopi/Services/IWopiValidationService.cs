using System.Threading.Tasks;
using ISynergy.Framework.Wopi.Models;

namespace ISynergy.Framework.Wopi.Services
{
    public interface IWopiValidationService
    {
        Task<bool> ValidateAsync(WopiRequest wopiRequest);
        bool VerifyProof(byte[] expectedProof, string proofFromRequest, string proofFromDiscovery);
    }
}