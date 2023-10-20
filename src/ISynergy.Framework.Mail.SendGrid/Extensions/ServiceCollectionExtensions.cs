using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mail.Abstractions.Services;
using ISynergy.Framework.Mail.Options;
using ISynergy.Framework.Mail.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Mail.Extensions
{
    /// <summary>
    /// Service collection extensions for Sendgrid
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Sendgrid Mail integration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMailSendGridIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<MailOptions>(configuration.GetSection(nameof(MailOptions)).BindWithReload);
            services.TryAddSingleton<IMailService, MailService>();
            return services;
        }
    }
}
