using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.BackgroundServices;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Services.Executors;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Automations.Extensions;

/// <summary>
/// ServiceCollection extensions for Automation.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds automation services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration used to bind <see cref="AutomationOptions"/>.</param>
    /// <param name="prefix">Optional configuration section prefix prepended before <c>AutomationOptions</c>.</param>
    /// <remarks>
    /// <para>
    /// This method registers a compile-time <see cref="ActionExecutorRegistry"/> that maps concrete
    /// <see cref="IAction"/> types to their executor factories. The built-in action types
    /// (<see cref="Actions.CommandAction"/>, <see cref="Actions.DelayAction"/>, and
    /// <see cref="Actions.AutomationAction"/>) are pre-registered.
    /// </para>
    /// <para>
    /// <strong>Custom action executors:</strong> If you add custom <see cref="IAction"/> types,
    /// register both the DI service and an entry in the registry:
    /// <code>
    /// services.TryAddScoped&lt;IActionExecutor&lt;MyAction&gt;, MyActionExecutor&gt;();
    /// services.PostConfigure&lt;ActionExecutorRegistry&gt;(registry =&gt;
    ///     registry.Register&lt;MyAction&gt;(sp =&gt; sp.GetRequiredService&lt;IActionExecutor&lt;MyAction&gt;&gt;()));
    /// </code>
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("Configuration binding via BindWithReload uses reflection and is not AOT-safe.")]
    [RequiresDynamicCode("Configuration binding via BindWithReload requires dynamic code generation.")]
    public static void AddAutomationServices(this IServiceCollection services, IConfiguration configuration, string prefix = "")
    {
        services.Configure<AutomationOptions>(configuration.GetSection($"{prefix}{nameof(AutomationOptions)}").BindWithReload);

        // Register compile-time action executor dispatch table (AOT-safe alternative to MakeGenericType).
        services.TryAddSingleton(sp =>
        {
            var registry = new ActionExecutorRegistry();
            registry.Register<Actions.CommandAction>(sp2 => sp2.GetRequiredService<IActionExecutor<Actions.CommandAction>>());
            registry.Register<Actions.DelayAction>(sp2 => sp2.GetRequiredService<IActionExecutor<Actions.DelayAction>>());
            registry.Register<Actions.AutomationAction>(sp2 => sp2.GetRequiredService<IActionExecutor<Actions.AutomationAction>>());
            return registry;
        });

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
