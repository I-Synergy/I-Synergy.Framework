using ISynergy.Framework.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Sample.TokenService.Business;
using Sample.TokenService.Contracts;
using Sample.TokenService.Models;
using System.Security.Claims;

namespace Sample.TokenService.Controllers;

/// <summary>
/// Authentication controller that issues and inspects <see cref="Token"/> objects.
/// </summary>
[Route("auth/[controller]")]
[ApiController]
public class TokenController : ControllerBase, ITokenServiceContract
{
    private readonly ITokenManager _tokenManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    public TokenController(ITokenManager manager)
    {
        _tokenManager = manager;
    }

    /// <summary>
    /// Generates a HmacSha256-signed JWT from the supplied <see cref="TokenRequest"/>.
    /// </summary>
    [HttpPost("jwt")]
    public Task<Token> GenerateJwtTokenAsync([FromBody] TokenRequest request) =>
        _tokenManager.GenerateJwtTokenAsync(request);

    /// <summary>
    /// Creates a WOPI token from the supplied <see cref="WopiTokenInput"/>.
    /// </summary>
    [HttpPost("wopi")]
    public Task<WopiToken> GenerateWopiTokenAsync([FromBody] WopiTokenInput input) =>
        _tokenManager.GenerateWopiTokenAsync(input);

    /// <summary>
    /// Returns all claims contained in the given <see cref="Token"/>.
    /// </summary>
    [HttpPost("wopi/list")]
    public Task<List<Claim>> GetClaimsAsync([FromBody] Token token) =>
        _tokenManager.GetClaimsAsync(token);

    /// <summary>
    /// Returns all claims of the given type contained in the <see cref="Token"/>.
    /// </summary>
    [HttpPost("wopi/{claimType}/list")]
    public Task<List<string>> GetClaimsAsync([FromBody] Token token, string claimType) =>
        _tokenManager.GetClaimsAsync(token, claimType);

    /// <summary>
    /// Returns the single claim of the given type from the <see cref="Token"/>.
    /// Throws if there is not exactly one matching claim.
    /// </summary>
    [HttpPost("wopi/{claimType}/single")]
    public Task<string> GetSingleClaimAsync([FromBody] Token token, string claimType) =>
        _tokenManager.GetSingleClaimAsync(token, claimType);
}
