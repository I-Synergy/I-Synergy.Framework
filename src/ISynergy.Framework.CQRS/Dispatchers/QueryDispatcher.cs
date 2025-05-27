using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.CQRS.Dispatchers;

/// <summary>
/// Default query dispatcher implementation
/// </summary>
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }
}