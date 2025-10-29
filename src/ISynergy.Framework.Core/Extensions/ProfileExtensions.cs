using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Core.Extensions;

public static class ProfileExtensions
{
    public static bool IsInRole(this IProfile? profile, string role)
    {
        if (profile is null)
            return false;

        return !string.IsNullOrWhiteSpace(role) && profile.Roles.Exists(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }
}
