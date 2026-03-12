using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.CQRS.Dispatch;

/// <summary>
/// A compile-time-populated dispatch table that maps concrete query types to
/// pre-compiled, AOT-safe handler invocations. Populate via the source-generated
/// <c>AddQueryDispatchTable()</c> extension method.
/// </summary>
public sealed class QueryDispatchTable
{
    private readonly Dictionary<Type, Func<IServiceProvider, object, CancellationToken, Task<object?>>> _dispatchers
        = new();

    /// <summary>
    /// Registers an AOT-safe dispatcher for <typeparamref name="TQuery"/> → <typeparamref name="TResult"/>.
    /// Call this from generated startup code only.
    /// </summary>
    /// <typeparam name="TQuery">Concrete query type.</typeparam>
    /// <typeparam name="TResult">Result type.</typeparam>
    public void Register<TQuery, TResult>()
        where TQuery : IQuery<TResult>
    {
        _dispatchers[typeof(TQuery)] = async (sp, query, ct) =>
        {
            var handler = sp.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return (object?)await handler.HandleAsync((TQuery)query, ct).ConfigureAwait(false);
        };
    }

    /// <summary>
    /// Attempts to retrieve a pre-compiled dispatcher for the given query type.
    /// </summary>
    /// <param name="queryType">Runtime type of the query.</param>
    /// <param name="dispatcher">The dispatcher delegate if found.</param>
    /// <returns><c>true</c> if a dispatcher was registered for <paramref name="queryType"/>.</returns>
    public bool TryGetDispatcher(
        Type queryType,
        out Func<IServiceProvider, object, CancellationToken, Task<object?>> dispatcher)
        => _dispatchers.TryGetValue(queryType, out dispatcher!);
}
