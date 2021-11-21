using ISynergy.Framework.AspNetCore.Authentication.Extensions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.AspNetCore.Authentication.Accessors
{
    /// <summary>
    /// Class ClaimsAccessor.
    /// </summary>
    public class ClaimsAccessor : BaseClaimsAccessor
    {
        /// <summary>
        /// The HTTP context accessor
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsAccessor"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <exception cref="ArgumentNullException">httpContextAccessor</exception>
        public ClaimsAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        /// <exception cref="ClaimNotFoundException"></exception>
        protected override List<string> GetClaims(string claimType)
        {
            var claimSet = httpContextAccessor.HttpContext?.User.Claims;
            if (claimSet != null)
            {
                var claims = claimSet.FindValues(claimType).ToList();
                if (claims.Any()) return claims;
            }
            throw new ClaimNotFoundException(claimType);
        }

        /// <summary>
        /// Determines whether the specified claim type has claim.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns><c>true</c> if the specified claim type has claim; otherwise, <c>false</c>.</returns>
        protected override bool HasClaim(string claimType) =>
            httpContextAccessor.HttpContext?
                .User.Claims?
                .FindSingleValue(claimType) != null;
    }
}
