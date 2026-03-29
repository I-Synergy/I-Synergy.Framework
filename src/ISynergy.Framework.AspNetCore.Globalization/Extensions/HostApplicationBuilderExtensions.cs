using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions;

public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds globalization integration.
    /// Don't use AddLocalization() in your Startup.cs, use this extension method instead.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Calls AddGlobalization which uses ConfigurationBinder.Bind with reflection.")]
    [RequiresDynamicCode("Calls AddGlobalization which requires dynamic code generation at runtime.")]
    public static IHostApplicationBuilder AddGlobalization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddGlobalization(builder.Configuration);
        return builder;
    }
}
