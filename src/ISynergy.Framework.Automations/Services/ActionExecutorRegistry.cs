using ISynergy.Framework.Automations.Abstractions;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Compile-time dispatch table that maps concrete <see cref="IAction"/> types to their
/// corresponding executor factory delegates.
/// </summary>
/// <remarks>
/// <para>
/// This registry replaces the reflection-based <c>MakeGenericType</c> / <c>GetInterfaces()</c>
/// dispatch that was previously used in <see cref="ActionExecutorFactory.GetExecutor(IAction)"/>.
/// All action types are registered at application start-up (inside
/// <c>ServiceCollectionExtensions.AddAutomationServices</c>) using <c>typeof()</c> literals,
/// which are statically known at compile time and therefore fully AOT-safe.
/// </para>
/// <para>
/// <strong>Extending the registry with custom action types:</strong>
/// If you define custom <see cref="IAction"/> and <see cref="IActionExecutor{TAction}"/>
/// implementations, register them both in the DI container and in this registry:
/// <code>
/// services.TryAddScoped&lt;IActionExecutor&lt;MyAction&gt;, MyActionExecutor&gt;();
/// services.PostConfigure&lt;ActionExecutorRegistry&gt;(registry =&gt;
///     registry.Register&lt;MyAction&gt;(sp =&gt; sp.GetRequiredService&lt;IActionExecutor&lt;MyAction&gt;&gt;()));
/// </code>
/// </para>
/// </remarks>
public sealed class ActionExecutorRegistry
{
    private readonly Dictionary<Type, Func<IServiceProvider, IActionExecutor>> _factories = new();

    /// <summary>
    /// Registers a typed executor factory for the specified <typeparamref name="TAction"/> type.
    /// </summary>
    /// <typeparam name="TAction">The concrete action type to register an executor for.</typeparam>
    /// <param name="factory">
    /// A delegate that accepts an <see cref="IServiceProvider"/> and returns an
    /// <see cref="IActionExecutor{TAction}"/> for <typeparamref name="TAction"/>.
    /// </param>
    public void Register<TAction>(Func<IServiceProvider, IActionExecutor<TAction>> factory)
        where TAction : IAction
        => _factories[typeof(TAction)] = sp => factory(sp);

    /// <summary>
    /// Resolves an executor for the given <paramref name="action"/> instance using the
    /// pre-registered factory delegates, or returns <see langword="null"/> if no factory
    /// was registered for the action's concrete type.
    /// </summary>
    /// <param name="action">The action instance whose executor should be resolved.</param>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> passed to the registered factory delegate.
    /// </param>
    /// <returns>
    /// An <see cref="IActionExecutor"/> for <paramref name="action"/>, or
    /// <see langword="null"/> if no executor is registered for the action's runtime type.
    /// </returns>
    public IActionExecutor? Resolve(IAction action, IServiceProvider serviceProvider)
    {
        if (_factories.TryGetValue(action.GetType(), out var factory))
            return factory(serviceProvider);

        return null;
    }
}
