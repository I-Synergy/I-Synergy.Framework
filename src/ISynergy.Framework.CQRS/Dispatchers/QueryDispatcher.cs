using System.Diagnostics.CodeAnalysis;
using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Dispatch;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.CQRS.Dispatchers;

/// <summary>
/// Default query dispatcher implementation.
/// Prefers the AOT-safe <see cref="QueryDispatchTable"/> when registered;
/// falls back to reflection-based dispatch for non-AOT scenarios.
/// </summary>
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger? _logger;
    private readonly QueryDispatchTable? _dispatchTable;

    /// <summary>
    /// Initializes a new instance of <see cref="QueryDispatcher"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="dispatchTable">
    /// Optional AOT-safe dispatch table. When provided, reflection-based dispatch is bypassed.
    /// Populated by the source-generated <c>AddQueryDispatchTable()</c> call.
    /// </param>
    public QueryDispatcher(
        IServiceProvider serviceProvider,
        ILogger<QueryDispatcher> logger,
        QueryDispatchTable? dispatchTable = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _dispatchTable = dispatchTable;
    }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Reflection fallback is only reached when QueryDispatchTable is absent. AOT builds must register a dispatch table via AddQueryDispatchTable().")]
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Reflection fallback is only reached when QueryDispatchTable is absent. AOT builds must register a dispatch table via AddQueryDispatchTable().")]
    public async Task<TResult> DispatchAsync<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        try
        {
            // AOT-safe path: use pre-compiled dispatch table when available.
            if (_dispatchTable is not null &&
                _dispatchTable.TryGetDispatcher(query.GetType(), out var dispatch))
            {
                return (TResult)(await dispatch(_serviceProvider, query, cancellationToken).ConfigureAwait(false))!;
            }

            // Reflection fallback — not AOT-safe. Triggers trimmer warnings when table is absent.
            return await DispatchViaReflection<TResult>(query, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger?.LogError(ex, "Error dispatching query of type {QueryType}", query.GetType().Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<TResult> DispatchAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        ArgumentNullException.ThrowIfNull(query);

        try
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger?.LogError(ex, "Error dispatching query of type {QueryType}", typeof(TQuery).Name);
            throw;
        }
    }

    [RequiresDynamicCode("Reflection-based query dispatch is not AOT-compatible. Register a QueryDispatchTable via AddQueryDispatchTable() or use DispatchAsync<TQuery, TResult> instead.")]
    [RequiresUnreferencedCode("Reflection-based query dispatch may not work after trimming. Register a QueryDispatchTable via AddQueryDispatchTable() or use DispatchAsync<TQuery, TResult> instead.")]
    private async Task<TResult> DispatchViaReflection<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }
}
