using ISynergy.Framework.Automations.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Factory for resolving action executors.
/// </summary>
public class ActionExecutorFactory : IActionExecutorFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExecutorFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving executors.</param>
    public ActionExecutorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets an executor for the specified action type.
    /// </summary>
    /// <typeparam name="TAction">The type of action.</typeparam>
    /// <returns>An executor for the action type, or null if no executor is registered.</returns>
    public IActionExecutor<TAction>? GetExecutor<TAction>() where TAction : IAction
    {
        return _serviceProvider.GetService<IActionExecutor<TAction>>();
    }

    /// <summary>
    /// Gets an executor for the specified action instance.
    /// </summary>
    /// <param name="action">The action instance.</param>
    /// <returns>An executor for the action, or null if no executor is registered.</returns>
    public object? GetExecutor(IAction action)
    {
        // Try to get executor for the specific action type
        var actionType = action.GetType();
        var executorType = typeof(IActionExecutor<>).MakeGenericType(actionType);
        var executor = _serviceProvider.GetService(executorType);
        
        if (executor is not null)
            return executor;

        // Fallback: get all executors and find one that can handle this action type
        var allExecutors = _serviceProvider.GetServices<IActionExecutor>();
        return allExecutors.FirstOrDefault(e => e.GetType().GetInterfaces()
            .Any(i => i.IsGenericType && 
                      i.GetGenericTypeDefinition() == typeof(IActionExecutor<>) && 
                      i.GetGenericArguments()[0].IsAssignableFrom(actionType)));
    }
}

