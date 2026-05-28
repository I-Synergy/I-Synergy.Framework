namespace ISynergy.Framework.CQRS.Queries;

/// <summary>
/// Query handler interface
/// </summary>
/// <typeparam name="TQuery">Type of query</typeparam>
/// <typeparam name="TResult">Type of result</typeparam>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query result</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}