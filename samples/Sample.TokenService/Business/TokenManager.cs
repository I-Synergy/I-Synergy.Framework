using ISynergy.Framework.AspNetCore.Authentication.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Validation;
using Sample.TokenService.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Sample.TokenService.Business
{
    /// <summary>
    /// Token manager to be used in the business layer
    /// </summary>
    public class TokenManager : ITokenManager
    {
        private readonly IJwtTokenService jwtTokenService;

        /// <summary>
        /// Constructor with dependency injection for the JwtTokenService
        /// </summary>
        /// <param name="tokenService"></param>
        public TokenManager(IJwtTokenService tokenService)
        {
            jwtTokenService = tokenService;
        }

        /// <summary>
        /// Generates a HmacSha256 encoded <see cref="Token"/> from a <see cref="GenericPrincipal"/> and an expiration <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Token> GenerateJwtTokenAsync(TokenRequest request)
        {
            return Task.FromResult(jwtTokenService.GenerateJwtToken(request));
        }

        /// <summary>
        /// Generates a <see cref="WopiToken"/> from <see cref="WopiTokenInput"/> 
        /// </summary>
        /// <param name="input"></param>
        /// <returns><see cref="WopiToken"/></returns>
        public Task<WopiToken> GenerateWopiTokenAsync(WopiTokenInput input)
        {
            Argument.IsNotNull(nameof(input), input);

            var claims = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CustomClaimTypes.ApplicationIdType, input.ApplicationId),
                new KeyValuePair<string, string>(CustomClaimTypes.DocumentIdType, input.DocumentId.ToString()),
                new KeyValuePair<string, string>(ISynergy.Framework.Core.Constants.ClaimTypes.AccountIdType, input.TenantId.ToString()),
                new KeyValuePair<string, string>(ISynergy.Framework.Core.Constants.ClaimTypes.UserIdType, input.UserId.ToString())
            };

            var token = jwtTokenService.GenerateJwtToken(new TokenRequest(
                input.Username,
                claims,
                input.Roles,
                input.Expiration
            ));

            if (token is Token)
            {
                return Task.FromResult(
                    new WopiToken(
                        input.ApplicationId,
                        input.TenantId,
                        input.UserId,
                        input.Username,
                        input.DocumentId,
                        input.BrandName,
                        input.BrandUrl,
                        input.FriendlyName,
                        input.Roles,
                        input.Expiration,
                        token.AccessToken));
            }

            return null;
        }

        /// <summary>
        /// Validates token and gets profile from token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<Profile> GetProfileAsync(Token token) =>
            Task.FromResult(jwtTokenService.GetProfile(token));

        /// <summary>
        /// Validates token and get all claims from token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<Claim>> GetClaimsAsync(Token token) =>
            Task.FromResult(jwtTokenService.GetClaims(token));

        /// <summary>
        /// Validates token and get all claims with specified ClaimType.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public Task<List<string>> GetClaimsAsync(Token token, string claimType) =>
            Task.FromResult(jwtTokenService.GetClaims(token, claimType));

        /// <summary>
        /// Validates token and get single claim with specified ClaimType.
        /// this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public Task<string> GetSingleClaimAsync(Token token, string claimType) =>
            Task.FromResult(jwtTokenService.GetSingleClaim(token, claimType));
    }
}
