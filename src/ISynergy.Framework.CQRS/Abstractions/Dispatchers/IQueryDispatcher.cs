using ISynergy.Framework.CQRS.Queries;

namespace ISynergy.Framework.CQRS.Abstractions.Dispatchers;

/// <summary>
/// Query dispatcher interface.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query. Requires a <see cref="ISynergy.Framework.CQRS.Dispatch.QueryDispatchTable"/>
    /// registered in DI for AOT compatibility; falls back to reflection when the table is absent.
    /// </summary>
    /// <typeparam name="TResult">Type of result.</typeparam>
    /// <param name="query">Query to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query result.</returns>
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a query using statically-known types. AOT-safe — no reflection required.
    /// Prefer this overload in AOT-published applications.
    /// </summary>
    /// <typeparam name="TQuery">Concrete type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    /// <param name="query">Query to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query result.</returns>
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}
