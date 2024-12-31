using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Extensions;

public static class ContextExtensions
{
    public static string ToEnvironmentalRefreshToken(this IContext context)
    {
        var token = context.Profile.Token.RefreshToken;
        return context.Environment switch
        {
            SoftwareEnvironments.Test => $"{GenericConstants.UsernamePrefixTest}{token}",
            SoftwareEnvironments.Local => $"{GenericConstants.UsernamePrefixLocal}{token}",
            _ => token,
        };
    }
}
