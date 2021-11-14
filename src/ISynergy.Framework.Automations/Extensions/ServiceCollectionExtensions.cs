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
