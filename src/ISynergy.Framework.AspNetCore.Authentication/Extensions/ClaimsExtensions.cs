namespace ISynergy.Framework.AspNetCore.Authentication.Extensions
{
    /// <summary>
    /// Class ClaimsExtensions.
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Finds the first value.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>System.String.</returns>
        public static string FindFirstValue(this IEnumerable<Claim> claims, string claimType)
            => claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .SingleOrDefault();

        /// <summary>
        /// Finds the single value.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>System.String.</returns>
        public static string FindSingleValue(this IEnumerable<Claim> claims, string claimType)
            => claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .SingleOrDefault();

        /// <summary>
        /// Finds the values.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> FindValues(this IEnumerable<Claim> claims, string claimType)
            => claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .ToList();
    }
}
