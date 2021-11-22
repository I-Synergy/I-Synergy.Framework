using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mail.Abstractions.Services;
using ISynergy.Framework.Mail.Options;
using ISynergy.Framework.Mail.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.Configure<SendGridOptions>(configuration.GetSection(nameof(SendGridOptions)).BindWithReload);
            services.Configure<MailOptions>(configuration.GetSection(nameof(MailOptions)).BindWithReload);

            services.AddSingleton<IMailService, MailService>();

            return services;
        }
    }
}
