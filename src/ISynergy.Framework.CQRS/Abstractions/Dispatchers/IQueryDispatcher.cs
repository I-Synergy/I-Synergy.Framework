using ISynergy.Framework.CQRS.Queries;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.CQRS.Abstractions.Dispatchers;

/// <summary>
/// Query dispatcher interface.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query by its runtime type. AOT-safe only if a
    /// <see cref="ISynergy.Framework.CQRS.Dispatch.QueryDispatchTable"/> populated via
    /// <see cref="ISynergy.Framework.CQRS.Dispatch.QueryDispatchTable.Register{TQuery, TResult}"/>
    /// is registered in DI and has an entry for this query's type; otherwise falls back to a
    /// MakeGenericType-based reflection path that is not supported under NativeAOT. There is
    /// currently no source generator in this repo that populates the table automatically --
    /// callers must call Register manually, or use the AOT-safe overload below instead.
    /// </summary>
    /// <typeparam name="TResult">Type of result.</typeparam>
    /// <param name="query">Query to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query result.</returns>
    [RequiresDynamicCode("Falls back to MakeGenericType-based reflection dispatch when no QueryDispatchTable entry exists for the query's runtime type; not supported under NativeAOT. Prefer DispatchAsync<TQuery, TResult> or register the query type on a QueryDispatchTable.")]
    [RequiresUnreferencedCode("The reflection dispatch fallback is not trim-compatible.")]
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
