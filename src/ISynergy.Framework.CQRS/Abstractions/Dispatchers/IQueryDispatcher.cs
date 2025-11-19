using ISynergy.Framework.CQRS.Queries;

namespace ISynergy.Framework.CQRS.Abstractions.Dispatchers;

/// <summary>
/// Query dispatcher interface
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="query">Query to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Query result</returns>
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}