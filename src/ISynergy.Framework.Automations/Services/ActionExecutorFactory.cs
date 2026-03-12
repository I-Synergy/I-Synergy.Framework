using ISynergy.Framework.Automations.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Factory for resolving action executors from the DI container.
/// </summary>
/// <remarks>
/// <para>
/// The generic overload <see cref="GetExecutor{TAction}"/> is fully AOT-safe because it uses
/// the statically-typed <see cref="IServiceProvider.GetService"/> overload.
/// </para>
/// <para>
/// The non-generic overload <see cref="GetExecutor(IAction)"/> is also AOT-safe: it delegates
/// to an <see cref="ActionExecutorRegistry"/> that was populated with <c>typeof()</c> literals
/// at start-up, avoiding <c>MakeGenericType</c> and <c>GetInterfaces()</c> reflection.
/// </para>
/// </remarks>
public class ActionExecutorFactory : IActionExecutorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ActionExecutorRegistry _registry;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExecutorFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving typed executors.</param>
    /// <param name="registry">
    /// The compile-time dispatch table mapping concrete <see cref="IAction"/> types to
    /// their executor factory delegates.
    /// </param>
    public ActionExecutorFactory(IServiceProvider serviceProvider, ActionExecutorRegistry registry)
    {
        _serviceProvider = serviceProvider;
        _registry = registry;
    }

    /// <summary>
    /// Gets an executor for the specified action type using the statically-typed DI resolution.
    /// </summary>
    /// <typeparam name="TAction">The type of action.</typeparam>
    /// <returns>An executor for <typeparamref name="TAction"/>, or <see langword="null"/> if none is registered.</returns>
    public IActionExecutor<TAction>? GetExecutor<TAction>() where TAction : IAction
    {
        return _serviceProvider.GetService<IActionExecutor<TAction>>();
    }

    /// <summary>
    /// Gets an executor for the specified action instance using the compile-time
    /// <see cref="ActionExecutorRegistry"/> dispatch table.
    /// </summary>
    /// <param name="action">The action instance.</param>
    /// <returns>
    /// An <see cref="IActionExecutor"/> for <paramref name="action"/>, or <see langword="null"/>
    /// if no executor is registered for the action's runtime type.
    /// </returns>
    public object? GetExecutor(IAction action)
    {
        return _registry.Resolve(action, _serviceProvider);
    }
}
