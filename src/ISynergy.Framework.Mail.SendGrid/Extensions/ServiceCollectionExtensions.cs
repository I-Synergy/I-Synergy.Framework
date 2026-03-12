using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mail.Abstractions.Services;
using ISynergy.Framework.Mail.Options;
using ISynergy.Framework.Mail.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Mail.Extensions;

/// <summary>
/// Service collection extensions for SendGrid mail integration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SendGrid mail integration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> SendGrid SDK v9.x uses <c>Newtonsoft.Json</c> for serialization,
    /// which is not AOT-compatible. Applications targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c>
    /// should use <c>ISynergy.Framework.Mail.Microsoft365</c> as an alternative. For non-AOT applications,
    /// suppress <c>IL2026</c> and <c>IL3050</c> warnings at this call site with a justification comment.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("SendGrid SDK uses Newtonsoft.Json for serialization, which is not AOT-compatible. Use a different mail provider for AOT publishing.")]
    [RequiresDynamicCode("Newtonsoft.Json requires dynamic code generation.")]
    public static IServiceCollection AddMailSendGridIntegration(this IServiceCollection services, IConfiguration configuration, string prefix = "")
    {
        services.AddOptions();
        services.Configure<MailOptions>(configuration.GetSection($"{prefix}{nameof(MailOptions)}").BindWithReload);
        services.TryAddSingleton<IMailService, MailService>();
        return services;
    }
}
