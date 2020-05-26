using ISynergy.Framework.AspNetCore.Authentication.Exceptions;
using ISynergy.Framework.AspNetCore.Authentication.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.AspNetCore.Authentication.Accessors
{
    /// <summary>
    /// Class ClaimsAccessor.
    /// </summary>
    public class ClaimsAccessor
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
        /// Gets the user identifier.
        /// </summary>
        /// <returns>Guid.</returns>
        public Guid GetUserId() => GetSingleClaimAsGuid(Core.Constants.ClaimTypes.UserIdType);

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetUserName() => GetSingleClaim(Core.Constants.ClaimTypes.UserNameType);

        /// <summary>
        /// Gets the account identifier.
        /// </summary>
        /// <returns>Guid.</returns>
        public Guid GetAccountId() => GetSingleClaimAsGuid(Core.Constants.ClaimTypes.AccountIdType);

        /// <summary>
        /// Gets the security stamp.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetSecurityStamp() => GetSingleClaim(Core.Constants.ClaimTypes.SecurityStampType);

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <returns>Guid.</returns>
        public Guid GetClientId() => GetSingleClaimAsGuid(Core.Constants.ClaimTypes.ClientIdType);

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        /// <exception cref="ClaimNotFoundException"></exception>
        private List<string> GetClaims(string claimType)
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
        private bool HasClaim(string claimType) =>
            httpContextAccessor.HttpContext?
                .User.Claims?
                .FindSingleValue(claimType) != null;

        /// <summary>
        /// Gets the single claim.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="DuplicateClaimException"></exception>
        /// <exception cref="ClaimNotFoundException"></exception>
        private string GetSingleClaim(string claimType)
        {
            var claims = GetClaims(claimType);

            if (claims.Count > 1)
                throw new DuplicateClaimException(claimType);

            if (claims is null || claims.Count == 0)
                throw new ClaimNotFoundException(claimType);

            return claims.Single();
        }

        /// <summary>
        /// Gets the claims as int.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>List&lt;System.Int32&gt;.</returns>
        private List<int> GetClaimsAsInt(string claimType)
            => GetClaimsAs<int>(claimType, int.TryParse);

        /// <summary>
        /// Gets the claims as enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>List&lt;T&gt;.</returns>
        private List<T> GetClaimsAsEnum<T>(string claimType) where T : struct
        => GetClaimsAs<T>(claimType, Enum.TryParse);

        /// <summary>
        /// Gets the single claim as int.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>System.Int32.</returns>
        private int GetSingleClaimAsInt(string claimType)
            => GetSingleClaimAs<int>(claimType, int.TryParse);

        /// <summary>
        /// Gets the single claim as unique identifier.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>Guid.</returns>
        private Guid GetSingleClaimAsGuid(string claimType)
            => GetSingleClaimAs<Guid>(claimType, Guid.TryParse);

        /// <summary>
        /// Gets the single claim as enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>T.</returns>
        private T GetSingleClaimAsEnum<T>(string claimType) where T : struct
        => GetSingleClaimAs<T>(claimType, Enum.TryParse);

        /// <summary>
        /// Gets the claims as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claimType">Type of the claim.</param>
        /// <param name="transformFunc">The transform function.</param>
        /// <returns>List&lt;T&gt;.</returns>
        private List<T> GetClaimsAs<T>(string claimType, TryFunc<string, T> transformFunc)
            where T : struct
        {
            var claimValues = GetClaims(claimType);
            var transformedClaimValues = new List<T>();
            foreach (var claimValue in claimValues)
                transformedClaimValues.Add(GetClaimAs(claimType, claimValue, transformFunc));
            return transformedClaimValues;
        }

        /// <summary>
        /// Gets the single claim as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claimType">Type of the claim.</param>
        /// <param name="transformFunc">The transform function.</param>
        /// <returns>T.</returns>
        private T GetSingleClaimAs<T>(string claimType, TryFunc<string, T> transformFunc)
            where T : struct
        {
            var claimValue = GetSingleClaim(claimType);
            return GetClaimAs(claimType, claimValue, transformFunc);
        }

        /// <summary>
        /// Gets the claim as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claimType">Type of the claim.</param>
        /// <param name="claimValue">The claim value.</param>
        /// <param name="transformFunc">The transform function.</param>
        /// <returns>T.</returns>
        /// <exception cref="InvalidClaimValueException"></exception>
        private static T GetClaimAs<T>(string claimType, string claimValue, TryFunc<string, T> transformFunc)
            where T : struct
        {
            if (!transformFunc(claimValue, out var result)) throw new InvalidClaimValueException(claimType);
            return result;
        }

        /// <summary>
        /// Delegate TryFunc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="arg">The argument.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private delegate bool TryFunc<in T, TResult>(T arg, out TResult result);
    }
}
