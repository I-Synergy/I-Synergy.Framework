using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.EventSourcing.Abstractions.Aggregates;
using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.Aggregates;
using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

/// <summary>
/// EF Core implementation of <see cref="IAggregateRepository{TAggregate, TId}"/> that loads
/// and saves aggregates via <see cref="IEventStore"/>.
/// </summary>
/// <typeparam name="TAggregate">
/// The aggregate root type. Must have a public parameterless constructor so it can be
/// instantiated before history is replayed.
/// </typeparam>
/// <typeparam name="TId">
/// The aggregate identifier type. Must be a <see cref="Guid"/> at runtime because
/// <see cref="IEventStore"/> uses <c>Guid</c> as the aggregate identifier.
/// </typeparam>
/// <remarks>
/// Event deserialization relies on <see cref="IEventTypeResolver"/> to map the stored type
/// name back to a CLR type and on <see cref="IEventSerializer"/> to perform the actual
/// JSON deserialization. Events whose type cannot be resolved are skipped with a warning
/// rather than throwing, so that unknown event types introduced by future schema migrations
/// do not prevent existing aggregates from loading.
/// </remarks>
public sealed class AggregateRepository<TAggregate, TId> : IAggregateRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>, new()
    where TId : struct
{
    private readonly IEventStore _eventStore;
    private readonly ITenantService _tenantService;
    private readonly IEventTypeResolver _typeResolver;
    private readonly IEventSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of <see cref="AggregateRepository{TAggregate, TId}"/>.
    /// </summary>
    public AggregateRepository(
        IEventStore eventStore,
        ITenantService tenantService,
        IEventTypeResolver typeResolver,
        IEventSerializer serializer)
    {
        _eventStore = eventStore;
        _tenantService = tenantService;
        _typeResolver = typeResolver;
        _serializer = serializer;
    }

    /// <inheritdoc />
    /// <returns>
    /// A hydrated aggregate if events exist, or <c>null</c> when no events are found
    /// (meaning the aggregate has never been created).
    /// </returns>
    public async Task<TAggregate?> LoadAsync(TId id, CancellationToken cancellationToken = default)
    {
        var aggregateId = ToGuid(id);
        var aggregateType = typeof(TAggregate).Name;

        var records = await _eventStore
            .GetEventsForAggregateAsync(aggregateType, aggregateId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (records.Count == 0)
            return null;

        var events = records
            .Select(r => DeserializeEvent(r))
            .Where(e => e is not null)
            .Cast<IDomainEvent>()
            .ToList();

        var aggregate = new TAggregate();
        aggregate.LoadFromHistory(events);
        return aggregate;
    }

    /// <inheritdoc />
    public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        var uncommitted = aggregate.GetUncommittedEvents();
        if (uncommitted.Count == 0)
            return;

        var tenantId = _tenantService.TenantId;
        var aggregateType = typeof(TAggregate).Name;
        var aggregateId = ToGuid(aggregate.Id);
        var expectedVersion = aggregate.Version;

        foreach (var @event in uncommitted)
        {
            expectedVersion = await _eventStore.AppendEventAsync(
                tenantId,
                aggregateType,
                aggregateId,
                expectedVersion,
                @event,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        aggregate.MarkEventsAsCommitted();
    }

    private IDomainEvent? DeserializeEvent(ISynergy.Framework.EventSourcing.Models.EventRecord record)
    {
        var type = _typeResolver.Resolve(record.EventType);
        if (type is null)
            return null;

        return _serializer.Deserialize(record.Data, type);
    }

    private static Guid ToGuid(TId id) =>
        id is Guid g ? g : (Guid)Convert.ChangeType(id, typeof(Guid));
}
