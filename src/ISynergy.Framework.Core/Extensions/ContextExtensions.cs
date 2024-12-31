using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Extensions;

public static class ContextExtensions
{
    public static string ToEnvironmentalRefreshToken(this IContext context)
    {
        var token = string.Empty;

        if (context.Profile is not null && context.Profile.Token is not null)
            token = context.Profile.Token.RefreshToken;

        return context.Environment switch
        {
            SoftwareEnvironments.Test => $"{GenericConstants.UsernamePrefixTest}{token}",
            SoftwareEnvironments.Local => $"{GenericConstants.UsernamePrefixLocal}{token}",
            _ => token,
        };
    }
}
