using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.BackgroundServices;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Automations.Extensions
{
    /// <summary>
    /// ServiceCollection extensions for Automation.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds automation services to ServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddAutomationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AutomationOptions>(configuration.GetSection(nameof(AutomationOptions)).BindWithReload);

            services.AddSingleton<IActionService, ActionService>();
            services.AddSingleton<IAutomationService, AutomationService>();

            services.AddHostedService<ActionQueuingBackgroundService>();
            services.AddHostedService<AutomationBackgroundService>();
        }
    }
}
