using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.Extensions;
public static class EnvironmentExtensions
{
    public static bool IsDocker(this IHostEnvironment environment)
    {
        if (environment.IsEnvironment("Docker"))
            return true;
        return false;
    }
}
