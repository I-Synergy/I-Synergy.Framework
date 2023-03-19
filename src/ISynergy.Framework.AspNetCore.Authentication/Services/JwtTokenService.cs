using ISynergy.Framework.AspNetCore.Abstractions.Services;
using ISynergy.Framework.AspNetCore.Authentication.Options;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace ISynergy.Framework.AspNetCore.Authentication.Services
{
    /// <summary>
    /// JwtTokenService that can be injected in controllers for basic token handling
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        /// <summary>
        /// The JWT options
        /// </summary>
        private readonly JwtOptions jwtOptions;

        /// <summary>
        /// Constructor with dependency injection to <see cref="JwtOptions" />
        /// </summary>
        /// <param name="options">The options.</param>
        public JwtTokenService(IOptions<JwtOptions> options)
        {
            jwtOptions = options.Value;
        }

        /// <summary>
        /// Generates a HmacSha256 encoded <see cref="Token" /> from a <see cref="TokenRequest" />
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Token.</returns>
        public Token GenerateJwtToken(TokenRequest request)
        {
            // Use code below to generate symmetric secret key.
            // var hmac = new HMACSHA256();
            // var key = Convert.ToBase64String(hmac.Key);

            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SymmetricKeySecret));
            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);
            var currentTimeStamp = DateTime.UtcNow;

            var identity = new GenericIdentity(request.Username, AuthenticationTypes.ClientCredentials);
            var userClaims = new ClaimsIdentity(identity);

            foreach (var item in request.Claims)
            {
                userClaims.AddClaim(new Claim(item.Key, item.Value));
            }

            foreach (var role in request.Roles)
            {
                userClaims.AddClaim(new Claim(Core.Constants.ClaimTypes.RoleType, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = userClaims,
                NotBefore = currentTimeStamp,
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Expires = currentTimeStamp.Add(request.Expiration),
                SigningCredentials = credentials
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new Token { AccessToken = token };
        }

        /// <summary>
        /// Validates a <see cref="Token" /> and returns a <see cref="ClaimsPrincipal" /> when successful
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Nullable&lt;ClaimsPrincipal&gt;.</returns>
        /// <exception cref="SecurityTokenExpiredException"></exception>
        private ClaimsPrincipal ValidateToken(Token token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token.AccessToken);

            if (jwtToken is null)
                return null;

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SymmetricKeySecret));
            var validationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricKey
            };

            var principal = tokenHandler.ValidateToken(token.AccessToken, validationParameters, out var securityToken);
            var currentUtc = DateTime.UtcNow;

            if (currentUtc >= securityToken.ValidFrom && currentUtc <= securityToken.ValidTo)
            {
                return principal;
            }
            else
            {
                throw new SecurityTokenExpiredException();
            }
        }

        /// <summary>
        /// Retrieves a list of <see cref="Claim" /> from a <see cref="Token" />
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>List&lt;Claim&gt;.</returns>
        public List<Claim> GetClaims(Token token)
        {
            var principal = ValidateToken(token);

            if (principal is not null)
            {
                return principal.Claims.ToList();
            }

            return new List<Claim>();
        }

        /// <summary>
        /// Retrieves a list of string from a <see cref="Token" /> and <see cref="Core.Constants.ClaimTypes" />
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> GetClaims(Token token, string claimType) =>
            GetClaims(token).FindValues(claimType).ToList();

        /// <summary>
        /// Retrieves a single of string from a <see cref="Token" /> and <see cref="Core.Constants.ClaimTypes" />
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>System.String.</returns>
        public string GetSingleClaim(Token token, string claimType) =>
            GetClaims(token).FindSingleValue(claimType);
    }
}
