using ISynergy.Framework.AspNetCore.Authentication.Models.General;
using Microsoft.AspNetCore.Mvc;
using Sample.TokenService.Business;
using Sample.TokenService.Contracts;
using Sample.TokenService.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sample.TokenService.Controllers
{
    /// <summary>
    /// Authentication controller that generates a <see cref="Token"/>
    /// </summary>
    [Route("auth/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase, ITokenServiceContract
    {
        private readonly ITokenManager tokenManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manager"></param>
        public TokenController(ITokenManager manager)
        {
            tokenManager = manager;
        }

        /// <summary>
        /// Generates a HmacSha256 encoded <see cref="Token"/> from a <see cref="TokenRequest"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("jwt")]
        public Task<Token> GenerateJwtTokenAsync([FromBody]TokenRequest request) =>
            tokenManager.GenerateJwtTokenAsync(request);

        /// <summary>
        /// Validates given <see cref="Token"/> and returns the <see cref="Profile"/> of user
        /// </summary>
        /// <param name="token"></param>
        /// <returns><see cref="Profile"/></returns>
        [HttpPost("profile")]
        public Task<Profile> GetProfileAsync([FromBody]Token token) =>
            tokenManager.GetProfileAsync(token);

        /// <summary>
        /// Creates a <see cref="WopiToken"/> from <see cref="WopiTokenInput"/> 
        /// </summary>
        /// <param name="input"></param>
        /// <returns><see cref="WopiToken"/></returns>
        [HttpPost("wopi")]
        public Task<WopiToken> GenerateWopiTokenAsync([FromBody]WopiTokenInput input) =>
            tokenManager.GenerateWopiTokenAsync(input);

        /// <summary>
        /// Retrieves a list of <see cref="Claim"/> from a <see cref="Token"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("wopi/list")]
        public Task<List<Claim>> GetClaimsAsync([FromBody]Token token) =>
            tokenManager.GetClaimsAsync(token);

        /// <summary>
        /// Retrieves a list of string from a <see cref="Token"/> and <see cref="CustomClaimTypes"/>
        /// </summary>
        /// <param name="token"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        [HttpPost("wopi/{claimType}/list")]
        public Task<List<string>> GetClaimsAsync([FromBody]Token token, string claimType) =>
            tokenManager.GetClaimsAsync(token, claimType);

        /// <summary>
        /// Retrieves a single of string from a <see cref="Token"/> and <see cref="CustomClaimTypes"/>
        /// </summary>
        /// <param name="token"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        [HttpPost("wopi/{claimType}/single")]
        public Task<string> GetSingleClaimAsync([FromBody]Token token, string claimType) =>
            tokenManager.GetSingleClaimAsync(token, claimType);
    }
}
