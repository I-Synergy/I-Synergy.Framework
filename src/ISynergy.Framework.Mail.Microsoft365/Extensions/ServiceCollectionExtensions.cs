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
/// Service collection extensions for Microsoft 365 mail integration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Microsoft 365 mail integration using the Microsoft Graph API.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> This method registers <see cref="MailService"/>, which depends on
    /// <c>GraphServiceClient</c> from the Microsoft.Graph SDK. That SDK uses reflection-based JSON serialization
    /// internally. Applications targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should suppress
    /// <c>IL2026</c> warnings at this call site or verify that the installed Microsoft.Graph version ships with
    /// full AOT annotations.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("Registers MailService which depends on Microsoft.Graph SDK using reflection-based JSON serialization. Not compatible with AOT publishing in this configuration.")]
    public static IServiceCollection AddMicrosoft365MailIntegration(this IServiceCollection services, IConfiguration configuration, string prefix = "")
    {
        services.AddOptions();
        services.Configure<MailOptions>(configuration.GetSection($"{prefix}{nameof(MailOptions)}").BindWithReload);
        services.TryAddSingleton<IMailService, MailService>();
        return services;
    }
}
