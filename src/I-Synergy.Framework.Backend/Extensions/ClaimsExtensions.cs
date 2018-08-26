using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ISynergy.Extensions
{
    public static class ClaimsExtensions
    {
        public static string FindFirstValue(this IEnumerable<Claim> claims, string claimType)
            => claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .SingleOrDefault();

        public static string FindSingleValue(this IEnumerable<Claim> claims, string claimType)
            => claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .SingleOrDefault();

        public static IEnumerable<string> FindValues(this IEnumerable<Claim> claims, string claimType)
            => claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .ToList();
    }
}