using ISynergy.Framework.AspNetCore.Authentication.Exceptions;
using ISynergy.Framework.AspNetCore.Authentication.Extensions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Monitoring.Accessors
{
    /// <summary>
    /// Claims accessors.
    /// </summary>
    internal class HubClaimsAccessor : BaseClaimsAccessor
    {
        /// <summary>
        /// The hub context
        /// </summary>
        private readonly HubCallerContext _hubContext;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        public HubClaimsAccessor(HubCallerContext hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        /// <exception cref="ClaimNotFoundException"></exception>
        protected override List<string> GetClaims(string claimType)
        {
            var claimSet = _hubContext?.User.Claims;
            if (claimSet is not null)
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
        protected override bool HasClaim(string claimType) => _hubContext?.User.Claims?.FindSingleValue(claimType) is not null;
    }
}
