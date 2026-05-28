using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds globalization integration.
    /// Don't use AddLocalization() in your Startup.cs, use this extension method instead.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddMultiTenancyIntegration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMultiTenancyIntegration();
        return builder;
    }
}
