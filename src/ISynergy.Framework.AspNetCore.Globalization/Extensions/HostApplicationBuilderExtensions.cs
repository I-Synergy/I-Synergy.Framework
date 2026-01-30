using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions;

public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds globalization integration.
    /// Don't use AddLocalization() in your Startup.cs, use this extension method instead.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddGlobalization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddGlobalization(builder.Configuration);
        return builder;
    }
}
