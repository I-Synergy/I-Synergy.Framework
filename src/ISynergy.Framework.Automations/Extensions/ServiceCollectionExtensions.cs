using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.BackgroundServices;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Services.Executors;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Automations.Extensions;

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
    /// <param name="prefix"></param>
    public static void AddAutomationServices(this IServiceCollection services, IConfiguration configuration, string prefix = "")
    {
        services.Configure<AutomationOptions>(configuration.GetSection($"{prefix}{nameof(AutomationOptions)}").BindWithReload);

        // Core services
        services.TryAddSingleton<IActionService, ActionService>();
        services.TryAddSingleton<IStateTypeResolver, StateTypeResolver>();
        services.TryAddSingleton<IOperatorStrategyFactory, OperatorStrategyFactory>();
        services.TryAddSingleton<IAutomationConditionValidator, AutomationConditionValidator>();
        services.TryAddSingleton<IActionExecutorFactory, ActionExecutorFactory>();
        services.TryAddSingleton<IActionQueueBuilder, ActionQueueBuilder>();
        services.TryAddSingleton<IAutomationService, AutomationService>();

        // Operator strategies
        services.TryAddSingleton<Services.Operators.AndOperatorStrategy>();
        services.TryAddSingleton<Services.Operators.OrOperatorStrategy>();

        // Action executors - register each executor for its specific action type
        services.TryAddScoped<IActionExecutor<Actions.CommandAction>, CommandActionExecutor>();
        services.TryAddScoped<IActionExecutor<Actions.DelayAction>, DelayActionExecutor>();
        services.TryAddScoped<IActionExecutor<Actions.AutomationAction>, AutomationActionExecutor>();

        services.AddHostedService<ActionQueuingBackgroundService>();
        services.AddHostedService<AutomationBackgroundService>();
    }
}
