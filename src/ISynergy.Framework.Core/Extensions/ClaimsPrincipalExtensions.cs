using ISynergy.Framework.Core.Exceptions;
using System.Security.Claims;
using ClaimTypes = ISynergy.Framework.Core.Constants.ClaimTypes;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Extension for retriving or quering claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    /// <exception cref="ClaimNotFoundException"></exception>
    public static List<string> GetClaims(this ClaimsPrincipal principal, string claimType)
    {
        var claimSet = principal.Claims;
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
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns><c>true</c> if the specified claim type has claim; otherwise, <c>false</c>.</returns>
    public static bool HasClaim(this ClaimsPrincipal principal, string claimType) =>
        principal.Claims.FindSingleValue(claimType) is not null;

    /// <summary>
    /// Get UserId from claims.
    /// </summary>
    /// <returns>Guid.</returns>
    public static string GetUserId(this ClaimsPrincipal principal) => principal.GetSingleClaim(ClaimTypes.UserIdType);

    /// <summary>
    /// Get Username from claims.
    /// </summary>
    /// <returns>System.String.</returns>
    public static string GetUserName(this ClaimsPrincipal principal) => principal.GetSingleClaim(ClaimTypes.UserNameType);

    /// <summary>
    /// Get AccountId from claims.
    /// </summary>
    /// <returns>Guid.</returns>
    public static Guid GetAccountId(this ClaimsPrincipal principal) => principal.GetSingleClaimAsGuid(ClaimTypes.AccountIdType);

    /// <summary>
    /// Get SecurityStamp from claims.
    /// </summary>
    /// <returns>System.String.</returns>
    public static string GetSecurityStamp(this ClaimsPrincipal principal) => principal.GetSingleClaim(ClaimTypes.SecurityStampType);

    /// <summary>
    /// Get clientId from claims.
    /// </summary>
    /// <returns>Guid.</returns>
    public static string GetClientId(this ClaimsPrincipal principal) => principal.GetSingleClaim(ClaimTypes.ClientIdType);

    /// <summary>
    /// Gets the single claim.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="DuplicateClaimException"></exception>
    /// <exception cref="ClaimNotFoundException"></exception>
    public static string GetSingleClaim(this ClaimsPrincipal principal, string claimType)
    {
        var claims = principal.GetClaims(claimType);

        if (claims is null || claims.Count == 0)
            throw new ClaimNotFoundException(claimType);

        if (claims.Count > 1)
            throw new DuplicateClaimException(claimType);

        return claims.Single();
    }

    /// <summary>
    /// Gets the claims as int.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>List&lt;System.Int32&gt;.</returns>
    public static List<int> GetClaimsAsInt(this ClaimsPrincipal principal, string claimType) =>
        principal.GetClaimsAs<int>(claimType, int.TryParse);

    /// <summary>
    /// Gets the claims as enum.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public static List<T> GetClaimsAsEnum<T>(this ClaimsPrincipal principal, string claimType) where T : struct =>
        principal.GetClaimsAs<T>(claimType, Enum.TryParse);

    /// <summary>
    /// Gets the single claim as int.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>System.Int32.</returns>
    public static int GetSingleClaimAsInt(this ClaimsPrincipal principal, string claimType) =>
        principal.GetSingleClaimAs<int>(claimType, int.TryParse);

    /// <summary>
    /// Gets the single claim as unique identifier.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>Guid.</returns>
    public static Guid GetSingleClaimAsGuid(this ClaimsPrincipal principal, string claimType) =>
        principal.GetSingleClaimAs<Guid>(claimType, Guid.TryParse);

    /// <summary>
    /// Gets the single claim as enum.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>T.</returns>
    public static T GetSingleClaimAsEnum<T>(this ClaimsPrincipal principal, string claimType) where T : struct =>
        principal.GetSingleClaimAs<T>(claimType, Enum.TryParse);

    /// <summary>
    /// Gets the claims as.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <param name="transformFunc">The transform function.</param>
    /// <returns>List&lt;T&gt;.</returns>
    private static List<T> GetClaimsAs<T>(this ClaimsPrincipal principal, string claimType, TryFunc<string, T> transformFunc)
        where T : struct
    {
        var claimValues = principal.GetClaims(claimType);
        var transformedClaimValues = new List<T>();
        foreach (var claimValue in claimValues.EnsureNotNull())
            transformedClaimValues.Add(principal.GetClaimAs(claimType, claimValue, transformFunc));
        return transformedClaimValues;
    }

    /// <summary>
    /// Gets the single claim as.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <param name="transformFunc">The transform function.</param>
    /// <returns>T.</returns>
    private static T GetSingleClaimAs<T>(this ClaimsPrincipal principal, string claimType, TryFunc<string, T> transformFunc)
        where T : struct
    {
        var claimValue = principal.GetSingleClaim(claimType);
        return principal.GetClaimAs(claimType, claimValue, transformFunc);
    }

    /// <summary>
    /// Gets the claim as.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="principal"></param>
    /// <param name="claimType">Type of the claim.</param>
    /// <param name="claimValue">The claim value.</param>
    /// <param name="transformFunc">The transform function.</param>
    /// <returns>T.</returns>
    /// <exception cref="InvalidClaimValueException"></exception>
    private static T GetClaimAs<T>(this ClaimsPrincipal principal, string claimType, string claimValue, TryFunc<string, T> transformFunc)
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
