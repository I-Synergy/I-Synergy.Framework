using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.UI.Extensions;

public static class CredentialExtensions
{
    public static string GetNormalizedCredentials(this string username, IContext context)
    {
        Argument.IsNotNullOrEmpty(username);

        if (username.StartsWith("test?", StringComparison.InvariantCultureIgnoreCase))
        {
            context.Environment = SoftwareEnvironments.Test;
            return username.Replace("test?", "");
        }

        if (username.StartsWith("local?", StringComparison.InvariantCultureIgnoreCase))
        {
            context.Environment = SoftwareEnvironments.Local;
            return username.Replace("local?", "");
        }

        context.Environment = SoftwareEnvironments.Production;
        return username;
    }
}
