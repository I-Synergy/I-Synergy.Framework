using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.UI.Extensions;

public static class CredentialExtensions
{
    public static string GetNormalizedCredentials(this string username)
    {
        Argument.IsNotNullOrEmpty(username);

        var context = ServiceLocator.Default.GetInstance<IContext>();

        // if username starts with "test:" or "local:"
        // remove this prefix and set environment to test.
        if (username.StartsWith(GenericConstants.UsernamePrefixTest, StringComparison.InvariantCultureIgnoreCase))
        {
            context.Environment = SoftwareEnvironments.Test;
            return username.Replace(GenericConstants.UsernamePrefixTest, "");
        }
        // remove this prefix and set environment to local.
        else if (username.StartsWith(GenericConstants.UsernamePrefixLocal, StringComparison.InvariantCultureIgnoreCase))
        {
            context.Environment = SoftwareEnvironments.Local;
            return username.Replace(GenericConstants.UsernamePrefixLocal, "");
        }
        else
        {
            context.Environment = SoftwareEnvironments.Production;
            return username;
        }
    }
}
