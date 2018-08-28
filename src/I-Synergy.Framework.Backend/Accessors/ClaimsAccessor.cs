using ISynergy.Exceptions;
using ISynergy.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISynergy.Accessors
{
    public class ClaimsAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ClaimsAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid GetUserId() => GetSingleClaimAsGuid(ClaimTypes.UserIdType);

        public string GetUserName() => GetSingleClaim(ClaimTypes.UserNameType);

        public Guid GetAccountId() => GetSingleClaimAsGuid(ClaimTypes.AccountIdType);

        public string GetSecurityStamp() => GetSingleClaim(ClaimTypes.SecurityStampType);

        public Guid GetClientId() => GetSingleClaimAsGuid(ClaimTypes.ClientIdType);

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

        private bool HasClaim(string claimType) =>
            httpContextAccessor.HttpContext?
                .User.Claims?
                .FindSingleValue(claimType) != null;

        private string GetSingleClaim(string claimType)
        {
            var claims = GetClaims(claimType);

            if (claims.Count > 1)
                throw new DuplicateClaimException(claimType);

            if (claims == null || claims.Count == 0)
                throw new ClaimNotFoundException(claimType);

            return claims.Single();
        }

        private List<int> GetClaimsAsInt(string claimType)
            => GetClaimsAs<int>(claimType, int.TryParse);

        private List<T> GetClaimsAsEnum<T>(string claimType) where T : struct
        => GetClaimsAs<T>(claimType, Enum.TryParse);

        private int GetSingleClaimAsInt(string claimType)
            => GetSingleClaimAs<int>(claimType, int.TryParse);

        private Guid GetSingleClaimAsGuid(string claimType)
            => GetSingleClaimAs<Guid>(claimType, Guid.TryParse);

        private T GetSingleClaimAsEnum<T>(string claimType) where T : struct
        => GetSingleClaimAs<T>(claimType, Enum.TryParse);

        private List<T> GetClaimsAs<T>(string claimType, TryFunc<string, T> transformFunc)
            where T : struct
        {
            var claimValues = GetClaims(claimType);
            var transformedClaimValues = new List<T>();
            foreach (var claimValue in claimValues)
                transformedClaimValues.Add(GetClaimAs(claimType, claimValue, transformFunc));
            return transformedClaimValues;
        }

        private T GetSingleClaimAs<T>(string claimType, TryFunc<string, T> transformFunc)
            where T : struct
        {
            var claimValue = GetSingleClaim(claimType);
            return GetClaimAs(claimType, claimValue, transformFunc);
        }

        private T GetClaimAs<T>(string claimType, string claimValue, TryFunc<string, T> transformFunc)
            where T : struct
        {
            if (!transformFunc(claimValue, out var result)) throw new InvalidClaimValueException(claimType);
            return result;
        }

        private delegate bool TryFunc<in T, TResult>(T arg, out TResult result);
    }
}
