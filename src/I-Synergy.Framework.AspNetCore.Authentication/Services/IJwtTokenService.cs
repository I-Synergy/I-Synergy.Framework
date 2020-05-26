using System.Collections.Generic;
using System.Security.Claims;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.AspNetCore.Authentication.Services
{
    public interface IJwtTokenService
    {
        Token GenerateJwtToken(TokenRequest request);
        List<Claim> GetClaims(Token token);
        List<string> GetClaims(Token token, string claimType);
        string GetSingleClaim(Token token, string claimType);
    }
}
