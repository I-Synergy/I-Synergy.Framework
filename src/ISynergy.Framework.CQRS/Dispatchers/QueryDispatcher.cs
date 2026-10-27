using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Dispatch;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;

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
    /// Caches the reflected <c>HandleAsync</c> method per constructed handler interface type, so the
    /// reflection fallback reflects each handler shape only once.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo> _handleAsyncCache = new();

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
    [RequiresDynamicCode("Falls back to MakeGenericType-based reflection dispatch when no QueryDispatchTable entry exists for the query's runtime type; not supported under NativeAOT.")]
    [RequiresUnreferencedCode("The reflection dispatch fallback is not trim-compatible.")]
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

    [RequiresDynamicCode("Uses MakeGenericType to resolve IQueryHandler<,> for a runtime-only-known query type; not supported under NativeAOT.")]
    [RequiresUnreferencedCode("Not trim-compatible.")]
    private async Task<TResult> DispatchViaReflection<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);

        // Invoke HandleAsync through a cached MethodInfo rather than `dynamic`: this avoids the
        // runtime (DLR) binder entirely and reflects each handler shape only once.
        var handleAsync = _handleAsyncCache.GetOrAdd(handlerType, static type =>
            type.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"Type '{type}' has no HandleAsync method."));

        try
        {
            var task = handleAsync.Invoke(handler, [query, cancellationToken]) as Task<TResult>
                ?? throw new InvalidOperationException(
                    $"HandleAsync on '{handlerType}' did not return a Task<{typeof(TResult).Name}>.");

            return await task.ConfigureAwait(false);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            // Surface the handler's own exception rather than the reflection wrapper.
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw; // unreachable - Throw() always throws; present to satisfy the compiler
        }
    }
}
